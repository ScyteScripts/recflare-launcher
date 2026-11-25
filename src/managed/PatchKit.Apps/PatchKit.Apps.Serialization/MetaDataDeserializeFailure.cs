using System;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class MetaDataDeserializeFailure : FunctionException
{
	public Text Value { get; }

	public MetaDataDeserializeFailure(Text value, Exception inner = null)
		: base($"Failed to deserialize meta data:\n{value}", inner)
	{
		Value = value;
	}
}
