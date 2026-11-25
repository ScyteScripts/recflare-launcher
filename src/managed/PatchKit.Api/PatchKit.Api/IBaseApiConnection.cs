using System.Threading;
using PatchKit.Core;

namespace PatchKit.Api;

public interface IBaseApiConnection
{
	ApiResponse SendRequest(ApiGetRequest request, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	ApiResponse SendRequest(ApiPostRequest request, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);
}
