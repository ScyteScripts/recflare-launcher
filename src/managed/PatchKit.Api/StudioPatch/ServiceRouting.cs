using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StudioPatch;

internal static class ServiceRouting
{
	public static bool UseInstalledTemplate()
	{
		return ReadValue("Launcher", "UseInstalledTemplate") == "1";
	}

	public static Task InstalledTemplateUpdateCheck(CancellationToken cancellation)
	{
		if (!cancellation.IsCancellationRequested)
		{
			return Task.CompletedTask;
		}
		return Task.FromCanceled(cancellation);
	}

	public static string ReadValue(string section, string key)
	{
		string text = ReadFile(Path.Combine(Environment.CurrentDirectory, "ProjectSettings", "RecRoomStudioPatch.ini"), section, key);
		if (!string.IsNullOrWhiteSpace(text))
		{
			return text;
		}
		string text2 = Environment.GetEnvironmentVariable("RECROOM_STUDIO_PATCH_CONFIG");
		if (string.IsNullOrWhiteSpace(text2))
		{
			text2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RecRoomStudioPatch", "services.ini");
		}
		return ReadFile(text2, section, key);
	}

	private static string ReadFile(string path, string section, string key)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		string a = "";
		string result = null;
		string[] array = File.ReadAllLines(path);
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Trim();
			if (text.Length == 0 || text.StartsWith(";") || text.StartsWith("#"))
			{
				continue;
			}
			if (text.StartsWith("[") && text.EndsWith("]"))
			{
				a = text.Substring(1, text.Length - 2).Trim();
				continue;
			}
			int num = text.IndexOf('=');
			if (num > 0 && string.Equals(a, section, StringComparison.OrdinalIgnoreCase) && string.Equals(text.Substring(0, num).Trim(), key, StringComparison.OrdinalIgnoreCase))
			{
				result = text.Substring(num + 1).Trim();
			}
		}
		return result;
	}

	private static string Origin(string key)
	{
		string value = ReadValue("Network", key);
		if (string.IsNullOrWhiteSpace(value))
		{
			value = key switch {
				"ServerUrl" => "https://ns.recflare.net",
				"StudioCdnUrl" => "https://studio.recflare.net",
				"WebUrl" => "https://recflare.net",
				"PatchKitApiUrl" or "PatchKitCacheUrl" => "https://studio.recflare.net",
				_ => null
			};
		}
		return ValidateOrigin(value, key);
	}

	private static string RewriteRecRoomUrl(string original)
	{
		if (!Uri.TryCreate(original, UriKind.Absolute, out Uri uri) || (uri.Scheme != "https" && uri.Scheme != "http")) return original;
		foreach (string domain in new[] { "rec.net", "recroom.com", "recroom.net", "recroom.cloud" })
		{
			if (uri.Host != domain && !uri.Host.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase)) continue;
			string prefix = uri.Host == domain ? "" : uri.Host.Substring(0, uri.Host.Length - domain.Length);
			return new UriBuilder(uri) { Host = prefix + "recflare.net", Scheme = "https", Port = -1 }.Uri.AbsoluteUri;
		}
		return original;
	}

	private static string ValidateOrigin(string value, string key)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}
		if (!Uri.TryCreate(value, UriKind.Absolute, out Uri result) || (result.Scheme != "https" && result.Scheme != "http") || result.UserInfo.Length != 0 || result.Query.Length != 0 || result.Fragment.Length != 0 || result.AbsolutePath != "/")
		{
			throw new InvalidDataException("RecRoomStudioPatch: " + key + " must be an HTTP(S) origin without a path, credentials, query or fragment.");
		}
		return result.GetLeftPart(UriPartial.Authority);
	}

	public static string ResolveUrl(string original)
	{
		if (!Uri.TryCreate(original, UriKind.Absolute, out Uri result))
		{
			return original;
		}
		string text = null;
		if (result.Host == "ns.rec.net")
		{
			text = "ServerUrl";
		}
		else if (result.Host == "studiocdn.rec.net")
		{
			text = "StudioCdnUrl";
		}
		else if (result.Host == "rec.net")
		{
			text = "WebUrl";
		}
		else if (result.Host == "api2.patchkit.net")
		{
			text = "PatchKitApiUrl";
		}
		else if (result.Host == "api-cache.patchkit.net")
		{
			text = "PatchKitCacheUrl";
		}
		else if (result.Host == "keys2.patchkit.net")
		{
			text = "PatchKitKeysUrl";
		}
		if (text == null)
		{
			return RewriteRecRoomUrl(original);
		}
		if (text == "PatchKitApiUrl" || text == "PatchKitCacheUrl")
		{
			string text2 = ReadValue("Network", "PatchKitAppSecretSha256");
			if (string.IsNullOrWhiteSpace(text2)) text2 = "f31e174f7cd5991d0e45db176e6283bfe04a74d91a95929c465e2edace7770e6";
			if (!string.IsNullOrWhiteSpace(text2))
			{
				string[] array = result.AbsolutePath.Split(new char[1] { '/' });
				if (array.Length < 4 || array[1] != "1" || array[2] != "apps")
				{
					return original;
				}
				if (array[3].Length != 0)
				{
					using SHA256 sHA = SHA256.Create();
					string b = BitConverter.ToString(sHA.ComputeHash(Encoding.UTF8.GetBytes(Uri.UnescapeDataString(array[3])))).Replace("-", "");
					if (!string.Equals(text2, b, StringComparison.OrdinalIgnoreCase))
					{
						return original;
					}
				}
			}
		}
		string text3 = Origin(text);
		if (text3 == null)
		{
			return original;
		}
		string text4 = result.PathAndQuery + result.Fragment;
		if (result.AbsolutePath == "/" && result.Query.Length == 0 && result.Fragment.Length == 0 && !original.EndsWith("/"))
		{
			text4 = "";
		}
		return RewriteRecRoomUrl(text3 + text4);
	}

	public static Uri ResolveUri(Uri original)
	{
		string text = ResolveUrl(original.AbsoluteUri);
		if (!(text == original.AbsoluteUri))
		{
			return new Uri(text);
		}
		return original;
	}

	public static string ResolveService(string original, string service)
	{
		return RewriteRecRoomUrl(ValidateOrigin(ReadValue("Services", service), "Services." + service) ?? ResolveUrl(original));
	}
}
