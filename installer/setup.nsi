Unicode true
RequestExecutionLevel user
SetCompressor zlib
CRCCheck on
ManifestSupportedOS all
!define MUI_ICON "${SETUP_ICON}"
!define MUI_UNICON "${SETUP_ICON}"
!include MUI2.nsh
!include LogicLib.nsh
!include FileFunc.nsh
!include x64.nsh
!include WinVer.nsh
!include WinMessages.nsh
!include StrFunc.nsh
${StrStr}

!define PRODUCT_ID "RecRoomStudioSourceLauncher"
!define PRODUCT_NAME "RecFlare Launcher"
!ifndef INSTALL_KEY
!define INSTALL_KEY "Software\${PRODUCT_ID}"
!endif
!ifndef DEFAULT_INSTALL_DIR
!define DEFAULT_INSTALL_DIR "$LOCALAPPDATA\Programs\recflare-launcher"
!endif
!define UNINSTALL_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_ID}"
Name "${PRODUCT_NAME}"
Caption "RecFlare Launcher Setup"
ShowInstDetails nevershow
AutoCloseWindow true
BrandingText "RecFlare Launcher"
OutFile "${SETUP_OUTPUT}"
InstallDir "${DEFAULT_INSTALL_DIR}"
InstallDirRegKey HKCU "${INSTALL_KEY}" "InstallLocation"
Icon "${SETUP_ICON}"
UninstallIcon "${SETUP_ICON}"
VIProductVersion "3.0.0.0"
VIAddVersionKey /LANG=1033 "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey /LANG=1033 "FileDescription" "RecFlare Launcher setup"
VIAddVersionKey /LANG=1033 "FileVersion" "3.0.0.0"
VIAddVersionKey /LANG=1033 "ProductVersion" "3.0.0"
VIAddVersionKey /LANG=1033 "Comments" "Local unsigned test build"
VIAddVersionKey /LANG=1033 "LegalCopyright" "RecFlare Launcher. Third-party components are listed in THIRD_PARTY_NOTICES.md."
Var ExtractOnly
Var NoIntegration
Var Parameters
Var ExplicitInstallDir
Var PatchedUpgrade
Var DirFallback

!define MUI_PAGE_CUSTOMFUNCTION_SHOW CompactInstallShow
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE English

; One-click presentation matching the original compact installer. Keep the
; ordinary instfiles page underneath so section execution/error handling remain
; NSIS-owned; only its layout is changed. /D= still selects a custom directory.
!macro HideCompactControl ID
  GetDlgItem $4 $HWNDPARENT ${ID}
  ShowWindow $4 ${SW_HIDE}
!macroend
Function CompactInstallShow
  System::Call 'user32::GetDpiForWindow(p $HWNDPARENT)i.r0'
  ${If} $0 = 0
    StrCpy $0 96
  ${EndIf}
  IntOp $1 $0 * 400
  IntOp $1 $1 / 96
  IntOp $2 $0 * 160
  IntOp $2 $2 / 96
  System::Call 'user32::SetWindowPos(p $HWNDPARENT, p 0, i 0, i 0, i r1, i r2, i 0x16)'
  ; Hide the wizard header, branding and navigation controls.
  !insertmacro HideCompactControl 1037
  !insertmacro HideCompactControl 1038
  !insertmacro HideCompactControl 1034
  !insertmacro HideCompactControl 1039
  !insertmacro HideCompactControl 1028
  !insertmacro HideCompactControl 1256
  !insertmacro HideCompactControl 1035
  !insertmacro HideCompactControl 1045
  !insertmacro HideCompactControl 1
  !insertmacro HideCompactControl 2
  !insertmacro HideCompactControl 3
  FindWindow $3 "#32770" "" $HWNDPARENT
  System::Call 'user32::SetWindowPos(p r3, p 0, i 0, i 0, i r1, i r2, i 0x14)'
  GetDlgItem $4 $3 1027
  ShowWindow $4 ${SW_HIDE}
  GetDlgItem $4 $3 1016
  ShowWindow $4 ${SW_HIDE}
  GetDlgItem $4 $3 1006
  SendMessage $4 ${WM_GETFONT} 0 0 $9
  ShowWindow $4 ${SW_HIDE}
  System::Call 'user32::CreateWindowExW(i 0, w "STATIC", w "Installing, please wait...", i 0x50000000, i 0, i 0, i 0, i 0, p r3, p 0, p 0, p 0)p.r4'
  SendMessage $4 ${WM_SETFONT} $9 1
  IntOp $5 $0 * 64
  IntOp $5 $5 / 96
  IntOp $6 $0 * 20
  IntOp $6 $6 / 96
  IntOp $7 $1 - $5
  System::Call 'user32::SetWindowPos(p r4, p 0, i r5, i r6, i r7, i 24, i 0x14)'
  GetDlgItem $4 $3 1004
  IntOp $5 $0 * 16
  IntOp $5 $5 / 96
  IntOp $6 $0 * 88
  IntOp $6 $6 / 96
  IntOp $7 $5 * 2
  IntOp $7 $1 - $7
  IntOp $8 $0 * 18
  IntOp $8 $8 / 96
  System::Call 'user32::SetWindowPos(p r4, p 0, i r5, i r6, i r7, i r8, i 0x14)'
  IntOp $6 $0 * 16
  IntOp $6 $6 / 96
  IntOp $8 $0 * 32
  IntOp $8 $8 / 96
  System::Call 'user32::CreateWindowExW(i 0, w "STATIC", w "", i 0x50000003, i r5, i r6, i r8, i r8, p r3, p 0, p 0, p 0)p.r4'
  System::Call 'shell32::ExtractIconExW(w "$EXEPATH", i 0, *p .r9, p 0, i 1)i.r7'
  SendMessage $4 ${STM_SETICON} $9 0
FunctionEnd

Function .onInstSuccess
  IfSilent no_launch
  StrCmp $ExtractOnly 1 no_launch
  StrCmp $NoIntegration 1 no_launch
  SetOutPath "$INSTDIR"
  Exec '$\"$INSTDIR\RecFlare Launcher.exe$\"'
no_launch:
FunctionEnd

; Every path component is checked before file writes/deletes. Payload paths come
; from the measured build manifest, never from an installation-controlled list.
!macro PATH_FUNCTIONS PREFIX
Function ${PREFIX}NormalizePath
  Exch $0
  Push $1
  Push $2
  ; NSIS GetFullPathName can fail for a fresh destination. The Windows API
  ; canonicalizes it without creating directories or following reparse points.
  System::Call 'kernel32::GetFullPathNameW(w r0, i ${NSIS_MAX_STRLEN}, w .r1, p 0)i.r2'
  IntCmp $2 0 normalize_bad normalize_bad
  IntCmp $2 ${NSIS_MAX_STRLEN} normalize_bad normalize_ok normalize_bad
normalize_bad:
  SetErrorLevel 87
  Abort
normalize_ok:
  StrCpy $0 $1
  Pop $2
  Pop $1
  Exch $0
FunctionEnd

Function ${PREFIX}CheckPath
  Exch $0
  Push $1
  Push $2
  Push "$0"
  Call ${PREFIX}NormalizePath
  Pop $0
  StrCpy $1 $0 2
  StrCmp $1 "\\" invalid_path
check_parent:
  System::Call 'kernel32::GetFileAttributesW(w r0)i.r1'
  StrCmp $1 -1 next_parent
  IntOp $2 $1 & 0x400
  IntCmp $2 0 next_parent invalid_path invalid_path
next_parent:
  ${GetParent} "$0" $1
  StrCmp $1 "" path_ok
  StrCmp $1 $0 path_ok
  StrCpy $0 $1
  Goto check_parent
invalid_path:
  MessageBox MB_OK|MB_ICONSTOP "Setup cannot use a network, linked or redirected installation path." /SD IDOK
  SetErrorLevel 87
  Abort
path_ok:
  Pop $2
  Pop $1
  Pop $0
FunctionEnd

Function ${PREFIX}CheckUnlocked
  Exch $0
  Push $1
  IfFileExists "$0" 0 unlocked
  System::Call 'kernel32::CreateFileW(w r0, i 0x40000000, i 7, p 0, i 3, i 0x80, p 0)p.r1'
  StrCmp $1 -1 locked
  System::Call 'kernel32::CloseHandle(p r1)'
  Goto unlocked
locked:
  MessageBox MB_OK|MB_ICONSTOP "Close the launcher and its updater before installing or removing this build." /SD IDOK
  SetErrorLevel 32
  Abort
unlocked:
  Pop $1
  Pop $0
FunctionEnd
!macroend
!insertmacro PATH_FUNCTIONS ""
!insertmacro PATH_FUNCTIONS "un."

Function .onInit
  ${IfNot} ${RunningX64}
    MessageBox MB_OK|MB_ICONSTOP "64-bit Windows is required." /SD IDOK
    SetErrorLevel 1633
    Abort
  ${EndIf}
  ${IfNot} ${AtLeastWin10}
    MessageBox MB_OK|MB_ICONSTOP "Windows 10 or later is required." /SD IDOK
    SetErrorLevel 1633
    Abort
  ${EndIf}
  SetShellVarContext current
  SetRegView 64
  StrCpy $ExplicitInstallDir 0
  ClearErrors
  ${GetOptions} "$CMDLINE" "/D=" $0
  ${IfNot} ${Errors}
    StrCpy $ExplicitInstallDir 1
  ${EndIf}
  StrCpy $ExtractOnly 0
  StrCpy $NoIntegration 0
  ${GetParameters} $Parameters
  ClearErrors
  ${GetOptions} $Parameters "/EXTRACTONLY" $0
  ${IfNot} ${Errors}
    StrCpy $ExtractOnly 1
    StrCpy $NoIntegration 1
  ${EndIf}
  ClearErrors
  ${GetOptions} $Parameters "/NOINTEGRATION" $0
  ${IfNot} ${Errors}
    StrCpy $NoIntegration 1
  ${EndIf}
FunctionEnd

Section "Launcher"
  StrCpy $PatchedUpgrade 0
  StrCpy $DirFallback 0
recheck_dir:
  Push "$INSTDIR"
  Call NormalizePath
  Pop $INSTDIR
  ${GetParent} "$INSTDIR" $0
  StrCmp $0 "" invalid_target
  StrCmp $INSTDIR $WINDIR invalid_target
  StrCmp $INSTDIR $PROFILE invalid_target
  Push "$INSTDIR"
  Call CheckPath
  ; The standard launcher folder is reserved for Studio. Older releases may
  ; have no recognizable receipt or config: upgrade them without blocking.
  StrCmp $INSTDIR "${DEFAULT_INSTALL_DIR}" standard_studio_directory
  ReadINIStr $0 "$INSTDIR\source-launcher-install.ini" "Setup" "Product"
  StrCmp $0 "${PRODUCT_ID}" owned_directory
  ; Recognize original/patched launchers and portable source builds even when
  ; no private migration receipt was distributed with them. Parse JSON instead
  ; of matching a version substring in the first line of the config file.
  IfFileExists "$INSTDIR\RecFlare Launcher.exe" launcher_exe_found
  IfFileExists "$INSTDIR\Rec Room Studio Launcher.exe" 0 check_fresh_directory
launcher_exe_found:
  IfFileExists "$INSTDIR\resources\app.asar" 0 check_fresh_directory
  IfFileExists "$INSTDIR\resources\config.json" 0 check_fresh_directory
  Push "$INSTDIR\resources\config.json"
  Call CheckPath
  Push "$INSTDIR\build-manifest.json"
  Call CheckPath
  InitPluginsDir
  SetOutPath "$PLUGINSDIR"
  File /oname=recognize-install.ps1 "${SETUP_RECOGNIZER}"
  nsExec::ExecToStack /TIMEOUT=30000 '"$SYSDIR\WindowsPowerShell\v1.0\powershell.exe" -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -File "$PLUGINSDIR\recognize-install.ps1" -InstallDirectory "$INSTDIR"'
  Pop $0
  Pop $1
  StrCmp $0 "0" 0 invalid_target
  StrCpy $PatchedUpgrade 1
  Goto owned_directory
standard_studio_directory:
  StrCpy $PatchedUpgrade 1
  Goto owned_directory
check_fresh_directory:
  FindFirst $1 $0 "$INSTDIR\*"
check_empty:
  StrCmp $0 "" empty_directory
  StrCmp $0 "." next_entry
  StrCmp $0 ".." next_entry
  FindClose $1
  ; Non-empty, unrecognized folder. If it was auto-selected from a stale
  ; registry InstallLocation (not chosen with /D=), fall back to the default
  ; folder once so an old data-only folder does not block setup.
  StrCmp $ExplicitInstallDir 1 invalid_target
  StrCmp $DirFallback 1 invalid_target
  StrCpy $DirFallback 1
  StrCpy $INSTDIR "${DEFAULT_INSTALL_DIR}"
  Goto recheck_dir
next_entry:
  FindNext $1 $0
  Goto check_empty
empty_directory:
  FindClose $1
owned_directory:
  !include "${SETUP_PREFLIGHT}"
  Push "$INSTDIR\resources\updater\Recovered.Updater.pdb"
  Call CheckPath
  Push "$INSTDIR\resources\updater\Recovered.Updater.pdb"
  Call CheckUnlocked
  !include "${SETUP_LEGACY_PREFLIGHT}"
  SetOverwrite on
  ClearErrors
  !include "${SETUP_PAYLOAD}"
  IfErrors install_failed
  ; Retire the known symbol file shipped by older source-built test packages.
  IfFileExists "$INSTDIR\resources\updater\Recovered.Updater.pdb" 0 debug_cleanup_done
  ClearErrors
  Delete "$INSTDIR\resources\updater\Recovered.Updater.pdb"
  IfErrors install_failed
debug_cleanup_done:
  StrCmp $PatchedUpgrade 1 0 keep_unowned_files
  !include "${SETUP_OBSOLETE}"
keep_unowned_files:
  WriteINIStr "$INSTDIR\source-launcher-install.ini" "Setup" "Product" "${PRODUCT_ID}"
  WriteINIStr "$INSTDIR\source-launcher-install.ini" "Setup" "Version" "3.0.0"
  WriteINIStr "$INSTDIR\source-launcher-install.ini" "Setup" "NoIntegration" "$NoIntegration"
  WriteINIStr "$INSTDIR\source-launcher-install.ini" "Setup" "PayloadSha256" "${SETUP_PAYLOAD_HASH}"
  WriteUninstaller "$INSTDIR\Uninstall Source Launcher.exe"
  IfErrors install_failed
  StrCmp $NoIntegration 1 install_done
  ; A fresh Recflare installation needs only its nameserver setting. Preserve
  ; explicit existing profiles; do not reuse launcher preferences or template data.
  IfFileExists "$LOCALAPPDATA\RecRoomStudioPatch\services.ini" keep_service_profile
  CreateDirectory "$LOCALAPPDATA\RecRoomStudioPatch"
  WriteINIStr "$LOCALAPPDATA\RecRoomStudioPatch\services.ini" "Network" "ServerUrl" "https://ns.recflare.net"
keep_service_profile:
  ; Retire the old Apps entry only when it belongs to this exact destination.
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\b2ae587a-b913-5ae0-a9fa-017e52aabe01" "UninstallString"
  StrCpy $2 '$\"$INSTDIR\Uninstall Rec Room Studio Launcher.exe$\"'
  StrLen $3 $2
  StrCpy $1 $0 $3
  StrCmp $1 $2 0 keep_legacy_registration
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\b2ae587a-b913-5ae0-a9fa-017e52aabe01"
keep_legacy_registration:
  WriteRegStr HKCU "${INSTALL_KEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "DisplayName" "${PRODUCT_NAME}"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "DisplayVersion" "3.0.0"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "UninstallString" '$\"$INSTDIR\Uninstall Source Launcher.exe$\"'
  WriteRegStr HKCU "${UNINSTALL_KEY}" "QuietUninstallString" '$\"$INSTDIR\Uninstall Source Launcher.exe$\" /S'
  WriteRegStr HKCU "${UNINSTALL_KEY}" "DisplayIcon" "$INSTDIR\resources\launcher.ico,0"
  WriteRegDWORD HKCU "${UNINSTALL_KEY}" "NoModify" 1
  WriteRegDWORD HKCU "${UNINSTALL_KEY}" "NoRepair" 1
  CreateDirectory "$SMPROGRAMS\RecFlare"
  CreateShortCut "$SMPROGRAMS\RecFlare\${PRODUCT_NAME}.lnk" "$INSTDIR\Launch.cmd" "" "$INSTDIR\resources\launcher.ico"
  CreateShortCut "$DESKTOP\${PRODUCT_NAME}.lnk" "$INSTDIR\Launch.cmd" "" "$INSTDIR\resources\launcher.ico"
  ; Retire the previous display name when upgrading.
  Delete "$SMPROGRAMS\Rec Room Inc\RecFlare Studio Launcher (test build).lnk"
  Delete "$DESKTOP\RecFlare Studio Launcher (test build).lnk"
  Delete "$SMPROGRAMS\Rec Room Inc\RecFlare Studio (test build).lnk"
  Delete "$DESKTOP\RecFlare Studio (test build).lnk"
  ; Rename links owned by prior versions without touching their targets.
  Delete "$SMPROGRAMS\Rec Room Inc\Rec Room Studio Launcher (test build).lnk"
  Delete "$DESKTOP\Rec Room Studio Launcher (test build).lnk"
  Delete "$SMPROGRAMS\Rec Room Inc\Rec Room Studio Launcher.lnk"
  Delete "$DESKTOP\Rec Room Studio Launcher.lnk"
  Delete "$SMPROGRAMS\Rec Room Inc\RecFlare Studio Launcher (Test Build 3).lnk"
  Delete "$DESKTOP\RecFlare Studio Launcher (Test Build 3).lnk"
  RMDir "$SMPROGRAMS\Rec Room Inc"
  System::Call 'shell32::SHChangeNotify(i 0x08000000, i 0, p 0, p 0)'
install_done:
  SetErrorLevel 0
  Goto finish_install
invalid_target:
  MessageBox MB_OK|MB_ICONSTOP "Setup could not recognize an existing launcher in:$\r$\n$INSTDIR$\r$\n$\r$\nInstall into an empty folder or a Rec Room/RecFlare Studio launcher installation. To choose another location, run setup with /D= followed by the full folder path." /SD IDOK
  SetErrorLevel 87
  Abort
install_failed:
  MessageBox MB_OK|MB_ICONSTOP "Setup could not write all files. Close the launcher and retry setup." /SD IDOK
  SetErrorLevel 5
  Abort
finish_install:
SectionEnd

Function un.onInit
  SetShellVarContext current
  SetRegView 64
  Push "$INSTDIR"
  Call un.NormalizePath
  Pop $INSTDIR
  ReadINIStr $0 "$INSTDIR\source-launcher-install.ini" "Setup" "Product"
  StrCmp $0 "${PRODUCT_ID}" +4
  SetErrorLevel 87
  MessageBox MB_OK|MB_ICONSTOP "This directory is not owned by the source launcher installer." /SD IDOK
  Abort
  Push "$INSTDIR"
  Call un.CheckPath
FunctionEnd

Section "Uninstall"
  !include "${SETUP_UNINSTALL_PREFLIGHT}"
  ReadINIStr $NoIntegration "$INSTDIR\source-launcher-install.ini" "Setup" "NoIntegration"
  SetOutPath "$TEMP"
  !include "${SETUP_UNINSTALL}"
  StrCmp $NoIntegration 1 skip_registration
  ReadRegStr $0 HKCU "${INSTALL_KEY}" "InstallLocation"
  StrCmp $0 $INSTDIR 0 skip_registration
  Delete "$SMPROGRAMS\RecFlare\${PRODUCT_NAME}.lnk"
  RMDir "$SMPROGRAMS\RecFlare"
  Delete "$SMPROGRAMS\Rec Room Inc\RecFlare Studio Launcher (Test Build 3).lnk"
  Delete "$SMPROGRAMS\Rec Room Inc\RecFlare Studio Launcher (test build).lnk"
  Delete "$DESKTOP\RecFlare Studio Launcher (test build).lnk"
  Delete "$SMPROGRAMS\Rec Room Inc\Rec Room Studio Launcher.lnk"
  Delete "$SMPROGRAMS\Rec Room Inc\Rec Room Studio Launcher (test build).lnk"
  Delete "$SMPROGRAMS\Rec Room Inc\RecFlare Studio (test build).lnk"
  Delete "$DESKTOP\RecFlare Studio (test build).lnk"
  RMDir "$SMPROGRAMS\Rec Room Inc"
  Delete "$DESKTOP\${PRODUCT_NAME}.lnk"
  Delete "$DESKTOP\Rec Room Studio Launcher.lnk"
  Delete "$DESKTOP\Rec Room Studio Launcher (test build).lnk"
  DeleteRegKey HKCU "${UNINSTALL_KEY}"
  DeleteRegKey HKCU "${INSTALL_KEY}"
skip_registration:
  Delete "$INSTDIR\source-launcher-install.ini"
  Delete "$INSTDIR\Uninstall Source Launcher.exe"
  ; Only empty directories are removed. Data, profiles and unknown files survive.
  RMDir "$INSTDIR"
  SetErrorLevel 0
SectionEnd
