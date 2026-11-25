using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class CreateFile
{
	private class FileWriteStream : IDisposable, IFileWriteStreamDisposable, IFileWriteStream, IBaseDisposable
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "FileWriteStream";

		private CreateParentDirDelegate CreateParentDir { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private Stream Stream { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		private void ExecuteWrite(Buffer<byte> buffer)
		{
			try
			{
				Stream.Write(buffer.Array, buffer.Offset, buffer.Length);
			}
			catch (IOException inner)
			{
				throw new OutOfFreeDiskSpace(inner);
			}
		}

		public void Write(Buffer<byte> buffer, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace)
		{
			_disposalHandler.ThrowIfDisposed();
			Arg.IsValid(buffer, "buffer");
			try
			{
				ExecuteWrite(buffer);
			}
			catch (UnauthorizedAccess e)
			{
				if (onUnauthorizedAccess == null)
				{
					throw;
				}
				onUnauthorizedAccess(e);
			}
			catch (OutOfFreeDiskSpace e2)
			{
				if (onOutOfFreeDiskSpace == null)
				{
					throw;
				}
				onOutOfFreeDiskSpace(e2);
			}
			catch (OperationCanceledException)
			{
				LogInfo("FileWriteStream::Write has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("FileWriteStream::Write failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("FileWriteStream::Write finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public FileWriteStream(Stream stream, CreateParentDirDelegate createParentDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsNotNull(stream, "stream");
			Arg.IsNotNull(createParentDir, "createParentDir");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Stream = stream;
			CreateParentDir = createParentDir;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~FileWriteStream()
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

	public CreateParentDirDelegate CreateParentDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IFileWriteStreamDisposable Execute(Path path)
	{
		try
		{
			if (new FileInfo(path).Exists)
			{
				throw new ConflictingFile(path);
			}
			if (new DirectoryInfo(path).Exists)
			{
				throw new ConflictingDir(path);
			}
			CreateParentDir(path, null, (ConflictingFile e) =>
			{
				throw new UnpredictedException(e);
			});
			FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
			FileWriteStream fileWriteStream = NewFileWriteStream(fileStream);
			fileWriteStream.AttachDisposable(fileStream);
			return fileWriteStream;
		}
		catch (UnauthorizedAccessException inner)
		{
			throw new UnauthorizedAccess(inner);
		}
	}

	private FileWriteStream NewFileWriteStream(Stream stream)
	{
		return new FileWriteStream(stream, CreateParentDir, Log, BeginLogRegion, EndLogRegion);
	}

	public CreateFile(CreateParentDirDelegate createParentDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createParentDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateParentDir = createParentDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IFileWriteStreamDisposable DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<ConflictingFile> onConflictingFile, HandleExceptionAndReturnDelegate<ConflictingDir> onConflictingDir)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing CreateFile...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			IFileWriteStreamDisposable fileWriteStreamDisposable = Execute(path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + fileWriteStreamDisposable);
			LogInfo("CreateFile finished: success.");
			return fileWriteStreamDisposable;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (IFileWriteStreamDisposable)obj;
		}
		catch (ConflictingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateFile finished: ConflictingFile.");
			if (onConflictingFile == null)
			{
				throw;
			}
			object obj2 = onConflictingFile(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (IFileWriteStreamDisposable)obj2;
		}
		catch (ConflictingDir e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateFile finished: ConflictingDir.");
			if (onConflictingDir == null)
			{
				throw;
			}
			object obj3 = onConflictingDir(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (IFileWriteStreamDisposable)obj3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("CreateFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("CreateFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public CreateFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new CreateFile(context.Resolve<CreateParentDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
