using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public delegate AppMetaData DeserializeMetaDataFrom2_x_xDelegate(Text value, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure);
