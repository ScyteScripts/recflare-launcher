using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class GetTempDir
{
	private class TempDir : IDisposable, ITempDirDisposable, ITempDir, IBaseDisposable
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "TempDir";

		private CreateDirDelegate CreateDir { get; }

		private DeleteFileIfExistsDelegate DeleteFileIfExists { get; }

		private DeleteDirRecursivelyIfExistsDelegate DeleteDirRecursivelyIfExists { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public Path Path { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
			DeleteDirRecursivelyIfExists(Path, (UnauthorizedAccess e) =>
			{
				LogWarning("Unable to delete directory because of unauthorized access. Leaving temporary directory.", e);
			});
		}

		public TempDir(Path path, CreateDirDelegate createDir, DeleteFileIfExistsDelegate deleteFileIfExists, DeleteDirRecursivelyIfExistsDelegate deleteDirRecursivelyIfExists, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(path, "path");
			Arg.IsNotNull(createDir, "createDir");
			Arg.IsNotNull(deleteFileIfExists, "deleteFileIfExists");
			Arg.IsNotNull(deleteDirRecursivelyIfExists, "deleteDirRecursivelyIfExists");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Path = path;
			CreateDir = createDir;
			DeleteFileIfExists = deleteFileIfExists;
			DeleteDirRecursivelyIfExists = deleteDirRecursivelyIfExists;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~TempDir()
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

	public CreateDirDelegate CreateDir { get; }

	public DeleteFileIfExistsDelegate DeleteFileIfExists { get; }

	public DeleteDirRecursivelyIfExistsDelegate DeleteDirRecursivelyIfExists { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ITempDirDisposable Execute()
	{
		string tempFileName = System.IO.Path.GetTempFileName();
		tempFileName = tempFileName.Replace('\\', '/');
		tempFileName = tempFileName.TrimEnd(new char[1] { '/' });
		Path path = new Path(tempFileName);
		DeleteFileIfExists(path, null);
		CreateDir(path, null, (ConflictingFile e) =>
		{
			throw new UnpredictedException(e);
		});
		return NewTempDir(path);
	}

	private TempDir NewTempDir(Path path)
	{
		return new TempDir(path, CreateDir, DeleteFileIfExists, DeleteDirRecursivelyIfExists, Log, BeginLogRegion, EndLogRegion);
	}

	public GetTempDir(CreateDirDelegate createDir, DeleteFileIfExistsDelegate deleteFileIfExists, DeleteDirRecursivelyIfExistsDelegate deleteDirRecursivelyIfExists, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createDir);
		Assert.IsNotNull(deleteFileIfExists);
		Assert.IsNotNull(deleteDirRecursivelyIfExists);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateDir = createDir;
		DeleteFileIfExists = deleteFileIfExists;
		DeleteDirRecursivelyIfExists = deleteDirRecursivelyIfExists;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ITempDirDisposable DelegateExecute(HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		LogInfo("Executing GetTempDir...");
		try
		{
			BeginLogRegion(enabled: true);
			ITempDirDisposable tempDirDisposable = Execute();
			EndLogRegion(enabled: true);
			LogTrace("result = " + tempDirDisposable);
			LogInfo("GetTempDir finished: success.");
			return tempDirDisposable;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetTempDir finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (ITempDirDisposable)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetTempDir has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetTempDir failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetTempDir finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetTempDirDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetTempDir(context.Resolve<CreateDirDelegate>(), context.Resolve<DeleteFileIfExistsDelegate>(), context.Resolve<DeleteDirRecursivelyIfExistsDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
