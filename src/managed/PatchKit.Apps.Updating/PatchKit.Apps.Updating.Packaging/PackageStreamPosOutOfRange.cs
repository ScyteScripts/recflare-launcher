using System;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class PackageStreamPosOutOfRange : FunctionException
{
	public BytesCount CurrentPos { get; }

	public BytesCount TargetPos { get; }

	public BytesCount Length { get; }

	public PackageStreamPosOutOfRange(BytesCount currentPos, BytesCount targetPos, BytesCount length, Exception inner = null)
		: base("", inner)
	{
		CurrentPos = currentPos;
		TargetPos = targetPos;
		Length = length;
	}
}
