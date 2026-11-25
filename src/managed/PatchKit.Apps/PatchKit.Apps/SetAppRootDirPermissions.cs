using System;
using System.Runtime.InteropServices;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class SetAppRootDirPermissions
{
	private enum ACCESS_MODE
	{
		NOT_USED_ACCESS,
		GRANT_ACCESS,
		SET_ACCESS,
		DENY_ACCESS,
		REVOKE_ACCESS,
		SET_AUDIT_SUCCESS,
		SET_AUDIT_FAILURE
	}

	private enum SE_OBJECT_TYPE
	{
		SE_UNKNOWN_OBJECT_TYPE,
		SE_FILE_OBJECT,
		SE_SERVICE,
		SE_PRINTER,
		SE_REGISTRY_KEY,
		SE_LMSHARE,
		SE_KERNEL_OBJECT,
		SE_WINDOW_OBJECT,
		SE_DS_OBJECT,
		SE_DS_OBJECT_ALL,
		SE_PROVIDER_DEFINED_OBJECT,
		SE_WMIGUID_OBJECT,
		SE_REGISTRY_WOW64_32KEY
	}

	[Flags]
	private enum SECURITY_INFORMATION : uint
	{
		OWNER_SECURITY_INFORMATION = 1u,
		GROUP_SECURITY_INFORMATION = 2u,
		DACL_SECURITY_INFORMATION = 4u,
		SACL_SECURITY_INFORMATION = 8u,
		UNPROTECTED_SACL_SECURITY_INFORMATION = 0x10000000u,
		UNPROTECTED_DACL_SECURITY_INFORMATION = 0x20000000u,
		PROTECTED_SACL_SECURITY_INFORMATION = 0x40000000u,
		PROTECTED_DACL_SECURITY_INFORMATION = 0x80000000u
	}

	private enum MULTIPLE_TRUSTEE_OPERATION
	{
		NO_MULTIPLE_TRUSTEE,
		TRUSTEE_IS_IMPERSONATE
	}

	private enum TRUSTEE_FORM
	{
		TRUSTEE_IS_SID,
		TRUSTEE_IS_NAME,
		TRUSTEE_BAD_FORM,
		TRUSTEE_IS_OBJECTS_AND_SID,
		TRUSTEE_IS_OBJECTS_AND_NAME
	}

	private enum TRUSTEE_TYPE
	{
		TRUSTEE_IS_UNKNOWN,
		TRUSTEE_IS_USER,
		TRUSTEE_IS_GROUP,
		TRUSTEE_IS_DOMAIN,
		TRUSTEE_IS_ALIAS,
		TRUSTEE_IS_WELL_KNOWN_GROUP,
		TRUSTEE_IS_DELETED,
		TRUSTEE_IS_INVALID,
		TRUSTEE_IS_COMPUTER
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct TRUSTEE
	{
		public IntPtr pMultipleTrustee;

		public MULTIPLE_TRUSTEE_OPERATION MultipleTrusteeOperation;

		public TRUSTEE_FORM TrusteeForm;

		public TRUSTEE_TYPE TrusteeType;

		public IntPtr ptstrName;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct EXPLICIT_ACCESS
	{
		public uint grfAccessPermissions;

		public uint grfAccessMode;

		public uint grfInheritance;

		public TRUSTEE Trustee;
	}

	private const int STANDARD_RIGHTS_REQUIRED = 983040;

	private const int SYNCHRONIZE = 1048576;

	private const int FILE_ALL_ACCESS = 2032127;

	private const int OBJECT_INHERIT_ACE = 1;

	private const int CONTAINER_INHERIT_ACE = 2;

	public GetCurrentPlatformTypeDelegate GetCurrentPlatformType { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern bool ConvertStringSidToSid(string StringSid, out IntPtr ptrSid);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
	private static extern uint GetNamedSecurityInfo(string pObjectName, SE_OBJECT_TYPE ObjectType, SECURITY_INFORMATION SecurityInfo, out IntPtr pSidOwner, out IntPtr pSidGroup, out IntPtr pDacl, out IntPtr pSacl, out IntPtr pSecurityDescriptor);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetSecurityDescriptorDacl(IntPtr pSecurityDescriptor, [MarshalAs(UnmanagedType.Bool)] out bool bDaclPresent, ref IntPtr pDacl, [MarshalAs(UnmanagedType.Bool)] out bool bDaclDefaulted);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern void BuildExplicitAccessWithName(ref EXPLICIT_ACCESS pExplicitAccess, string pTrusteeName, uint AccessPermissions, uint AccessMode, uint Inheritance);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern int SetEntriesInAcl(int cCountOfExplicitEntries, ref EXPLICIT_ACCESS pListOfExplicitEntries, IntPtr OldAcl, out IntPtr NewAcl);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
	private static extern uint SetNamedSecurityInfo(string pObjectName, SE_OBJECT_TYPE ObjectType, SECURITY_INFORMATION SecurityInfo, IntPtr psidOwner, IntPtr psidGroup, IntPtr pDacl, IntPtr pSacl);

	private void GrantOnDir(App app, string stringSid)
	{
		ConvertStringSidToSid(stringSid, out var ptrSid);
		GetNamedSecurityInfo(app.Path.Value, SE_OBJECT_TYPE.SE_FILE_OBJECT, SECURITY_INFORMATION.DACL_SECURITY_INFORMATION, out var _, out var _, out var _, out var _, out var pSecurityDescriptor);
		IntPtr pDacl2 = IntPtr.Zero;
		GetSecurityDescriptorDacl(pSecurityDescriptor, out var _, ref pDacl2, out var _);
		EXPLICIT_ACCESS pExplicitAccess = default;
		BuildExplicitAccessWithName(ref pExplicitAccess, "", 2032127u, 1u, 3u);
		pExplicitAccess.Trustee.TrusteeForm = TRUSTEE_FORM.TRUSTEE_IS_SID;
		pExplicitAccess.Trustee.ptstrName = ptrSid;
		SetEntriesInAcl(1, ref pExplicitAccess, pDacl2, out var NewAcl);
		SetNamedSecurityInfo(app.Path.Value, SE_OBJECT_TYPE.SE_FILE_OBJECT, SECURITY_INFORMATION.DACL_SECURITY_INFORMATION, IntPtr.Zero, IntPtr.Zero, NewAcl, IntPtr.Zero);
	}

	private void Execute(App app)
	{
		PlatformType platformType = GetCurrentPlatformType();
		if (platformType == PlatformType.Win32 || platformType == PlatformType.Win64)
		{
			GrantOnDir(app, "S-1-5-32-545");
			GrantOnDir(app, "S-1-5-18");
		}
	}

	public SetAppRootDirPermissions(GetCurrentPlatformTypeDelegate getCurrentPlatformType, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getCurrentPlatformType);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetCurrentPlatformType = getCurrentPlatformType;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing SetAppRootDirPermissions...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app);
			EndLogRegion(enabled: true);
			LogInfo("SetAppRootDirPermissions finished: success.");
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir e)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppRootDirPermissions finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			onUnauthorizedAccessToAppRootDir(e);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppRootDirPermissions has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppRootDirPermissions failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppRootDirPermissions finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public SetAppRootDirPermissionsDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new SetAppRootDirPermissions(context.Resolve<GetCurrentPlatformTypeDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
