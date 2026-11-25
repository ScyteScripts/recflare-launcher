using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating;

public class UpdateAppFailure : FunctionException
{
	public UpdateAppFailure(Exception inner = null)
		: base("", inner)
	{
	}
}
