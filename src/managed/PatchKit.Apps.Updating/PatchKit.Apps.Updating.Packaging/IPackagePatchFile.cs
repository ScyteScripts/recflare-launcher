using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackagePatchFile : IPackageFile, IBaseDisposable
{
	AppPath Path { get; }

	AppVersionId SourceVersionId { get; }

	AppVersionId TargetVersionId { get; }

	bool IsPatchEmpty { get; }

	IPackageFileStream Stream { get; }
}
