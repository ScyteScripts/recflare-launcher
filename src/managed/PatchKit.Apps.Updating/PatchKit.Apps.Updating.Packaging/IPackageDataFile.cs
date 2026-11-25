using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackageDataFile : IPackageFile, IBaseDisposable
{
	AppPath Path { get; }

	AppVersionId VersionId { get; }

	IPackageFileStream Stream { get; }
}
