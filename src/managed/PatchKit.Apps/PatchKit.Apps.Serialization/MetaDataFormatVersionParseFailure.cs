using System;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class MetaDataFormatVersionParseFailure : FunctionException
{
	public Text Value { get; }

	public MetaDataFormatVersionParseFailure(Text value, Exception inner = null)
		: base($"Invalid meta data format version - '{value}'.", inner)
	{
		Value = value;
	}
}
