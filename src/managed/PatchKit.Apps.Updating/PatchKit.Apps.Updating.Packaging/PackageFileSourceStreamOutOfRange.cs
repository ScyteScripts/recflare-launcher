using System;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class PackageFileSourceStreamOutOfRange : FunctionException
{
	public BytesCount StreamCurrentPos { get; }

	public BytesCount StreamLength { get; }

	public BytesCount FileStreamStartPos { get; }

	public BytesCount FileStreamLength { get; }

	public PackageFileSourceStreamOutOfRange(BytesCount streamCurrentPos, BytesCount streamLength, BytesCount fileStreamStartPos, BytesCount fileStreamLength, Exception inner = null)
		: base("", inner)
	{
		StreamCurrentPos = streamCurrentPos;
		StreamLength = streamLength;
		FileStreamStartPos = fileStreamStartPos;
		FileStreamLength = fileStreamLength;
	}
}
