using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public delegate MetaDataFormatVersion ParseMetaDataFormatVersionDelegate(Text value, HandleExceptionAndReturnDelegate<MetaDataFormatVersionParseFailure> onMetaDataFormatVersionParseFailure);
