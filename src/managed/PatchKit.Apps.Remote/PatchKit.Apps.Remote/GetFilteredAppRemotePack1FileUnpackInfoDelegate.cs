using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public delegate FilteredAppRemotePack1FileUnpackInfo GetFilteredAppRemotePack1FileUnpackInfoDelegate(AppRemotePack1FileUnpackInfo fileUnpackInfo, BytesCount chunkSize);
