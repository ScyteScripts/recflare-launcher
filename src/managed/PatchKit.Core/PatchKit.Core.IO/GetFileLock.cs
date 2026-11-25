using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class GetFileLock
{
	private class FileLock : IDisposable, IFileLockDisposable, IFileLock, IBaseDisposable
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "FileLock";

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		public FileLock(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~FileLock()
		{
			Dispose(isDisposing: false);
		}

		public void Dispose()
		{
			Dispose(isDisposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool isDisposing)
		{
			if (_disposalHandler.IsDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				DisposeManaged();
				if (_attachedDisposables != null)
				{
					foreach (IDisposable attachedDisposable in _attachedDisposables)
					{
						attachedDisposable.Dispose();
					}
				}
			}
			DisposeUnmanaged();
			_disposalHandler.MarkAsDisposed();
		}

		public void AttachDisposable(IDisposable disposable)
		{
			Assert.IsNotNull(disposable);
			if (_attachedDisposables == null)
			{
				_attachedDisposables = new List<IDisposable>();
			}
			_attachedDisposables.Add(disposable);
		}

		private void LogTrace(string message, Exception exception = null)
		{
			Log(new LogMessage(message, LogMessageType.Trace, exception));
		}

		private void LogInfo(string message, Exception exception = null)
		{
			Log(new LogMessage(message, LogMessageType.Info, exception));
		}

		private void LogWarning(string message, Exception exception = null)
		{
			Log(new LogMessage(message, LogMessageType.Warning, exception));
		}

		private void LogError(string message, Exception exception = null)
		{
			Log(new LogMessage(message, LogMessageType.Error, exception));
		}
	}

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IFileLockDisposable Execute(Path path)
	{
		IDisposable disposable;
		try
		{
			disposable = File.Open(path, FileMode.Open);
		}
		catch (FileNotFoundException inner)
		{
			throw new NotExistingFile(path, inner);
		}
		catch (IOException inner2)
		{
			throw new FileAlreadyInUse(path, inner2);
		}
		catch (UnauthorizedAccessException inner3)
		{
			throw new UnauthorizedAccess(inner3);
		}
		FileLock fileLock = NewFileLock();
		fileLock.AttachDisposable(disposable);
		return fileLock;
	}

	private FileLock NewFileLock()
	{
		return new FileLock(Log, BeginLogRegion, EndLogRegion);
	}

	public GetFileLock(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IFileLockDisposable DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile, HandleExceptionAndReturnDelegate<FileAlreadyInUse> onFileAlreadyInUse)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing GetFileLock...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			IFileLockDisposable fileLockDisposable = Execute(path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + fileLockDisposable);
			LogInfo("GetFileLock finished: success.");
			return fileLockDisposable;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileLock finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (IFileLockDisposable)obj;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileLock finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingFile(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (IFileLockDisposable)obj2;
		}
		catch (FileAlreadyInUse e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileLock finished: FileAlreadyInUse.");
			if (onFileAlreadyInUse == null)
			{
				throw;
			}
			object obj3 = onFileAlreadyInUse(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (IFileLockDisposable)obj3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileLock has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFileLock failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFileLock finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFileLockDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFileLock(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
	}

	private void LogTrace(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Trace, exception));
	}

	private void LogInfo(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Info, exception));
	}

	private void LogWarning(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Warning, exception));
	}

	private void LogError(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Error, exception));
	}
}
