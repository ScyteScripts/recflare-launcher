using System;

namespace PatchKit.Core.CSharp;

public interface IBaseDisposable
{
	void AttachDisposable(IDisposable disposable);
}
