using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public delegate ImmutableArray<AppExtraEntryInfo> DeserializeExtraEntriesInfoDelegate(Text value, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure);
