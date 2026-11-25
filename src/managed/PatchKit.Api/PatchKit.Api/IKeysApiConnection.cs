using System.Threading;
using PatchKit.Api.Models.Keys;
using PatchKit.Core;

namespace PatchKit.Api;

public interface IKeysApiConnection
{
	Job GetJobInfo(string guid, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	LicenseKeysCollection GetCollectionKeys(int collectionId, string token, int size, int offset, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	LicenseKey GetKeyInfo(string key, string appSecret, string keySecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);
}
