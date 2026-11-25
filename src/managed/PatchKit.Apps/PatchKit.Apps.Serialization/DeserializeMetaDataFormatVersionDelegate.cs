using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public delegate MetaDataFormatVersion? DeserializeMetaDataFormatVersionDelegate(Text value, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure);
