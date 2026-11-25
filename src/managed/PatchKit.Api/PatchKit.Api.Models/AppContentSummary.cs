using Newtonsoft.Json;
using PatchKit.Core.CSharp;

namespace PatchKit.Api.Models;

public struct AppContentSummary
{
	[JsonProperty("version")]
	public string Version { get; }

	[JsonProperty("size")]
	public long Size { get; }

	[JsonProperty("uncompressed_size")]
	public long UncompressedSize { get; }

	[JsonProperty("encryption_method")]
	public string EncryptionMethod { get; }

	[JsonProperty("compression_method")]
	public string CompressionMethod { get; }

	[JsonProperty("files")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<AppContentSummaryFile>))]
	public ImmutableArray<AppContentSummaryFile> Files { get; }

	[JsonProperty("hash_code")]
	public string HashCode { get; }

	[JsonProperty("chunks")]
	public Chunks Chunks { get; }

	[JsonConstructor]
	public AppContentSummary(string version, long size, long uncompressedSize, string encryptionMethod, string compressionMethod, ImmutableArray<AppContentSummaryFile> files, string hashCode, Chunks chunks)
	{
		this = default;
		Version = version;
		Size = size;
		UncompressedSize = uncompressedSize;
		EncryptionMethod = encryptionMethod;
		CompressionMethod = compressionMethod;
		Files = files;
		HashCode = hashCode;
		Chunks = chunks;
	}
}
