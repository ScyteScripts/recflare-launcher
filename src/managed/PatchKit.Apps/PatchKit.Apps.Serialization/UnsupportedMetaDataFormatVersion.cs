using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class UnsupportedMetaDataFormatVersion : FunctionException
{
	public MetaDataFormatVersion Version { get; }

	public UnsupportedMetaDataFormatVersion(MetaDataFormatVersion version, Exception inner = null)
		: base($"Unsupported meta data version - '{version}'.", inner)
	{
		Version = version;
	}
}
