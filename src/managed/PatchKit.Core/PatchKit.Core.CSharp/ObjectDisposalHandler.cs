namespace PatchKit.Core.CSharp;

public sealed class ObjectDisposalHandler
{
	private readonly object _obj;

	public bool IsDisposed { get; private set; }

	public ObjectDisposalHandler(object obj)
	{
		Assert.IsNotNull(obj);
		_obj = obj;
	}

	public void ThrowIfDisposed()
	{
		if (IsDisposed)
		{
			throw new UsageOfDisposedObject();
		}
	}

	public void MarkAsDisposed()
	{
		IsDisposed = true;
	}
}
