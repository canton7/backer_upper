;NSIS Modern User Interface

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

;--------------------------------
;General

  ;Name and file
  Name "Backer Upper"
  OutFile "BackerUpper.exe"
  Icon "..\icon.ico"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\BackerUpper"

  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\BackerUpper" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel admin

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES

  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section
  SetOutPath "$INSTDIR"

  File "..\bin\Release\BackerUpper.exe"

  File "..\bin\Release\AWSSDK.dll"
  File "..\bin\Release\Microsoft.Win32.TaskScheduler.dll"
  File "..\bin\Release\Microsoft.Win32.TaskScheduler.xml"
  File "..\bin\Release\System.Data.SQLite.dll"
  File "..\bin\Release\System.Data.SQLite.Linq.dll"
  File "..\icon.ico"
  File "remove_tasks.bat"

  ;Store installation folder
  WriteRegStr HKCU "BackerUpper" "" $INSTDIR

  ;Create uninstaller
  WriteUninstaller  "$INSTDIR\uninstall.exe"

SectionEnd

Section "Start Menu Shortcut" secStartMenu
  CreateShortCut "$SMPROGRAMS\Backer Upper.lnk" "$INSTDIR\BackerUpper.exe" "" "$INSTDIR\icon.ico"
SectionEnd

Section "Desktop Shortcut" secDesktop
  CreateShortCut "$DESKTOP\Backer Upper.lnk" "$INSTDIR\BackerUpper.exe" "" "$INSTDIR\icon.ico"
SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  ;LangString DESC_secStartMenu ${LANG_ENGLISH} "Add a link in the start menu"

  ;Assign language strings to sections
  ;!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  ;  !insertmacro MUI_DESCRIPTION_TEXT ${SecDummy} $(DESC_SecDummy)
  ;!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ${If} ${Cmd} 'MessageBox MB_YESNO "Do you want to delete your backup databases?" IDYES'
    RMDir /r "$APPDATA\BackerUpper\Backups"
  ${EndIf}

  Delete "$INSTDIR\BackerUpper.exe"
  Delete "$INSTDIR\AWSSDK.dll"
  Delete "$INSTDIR\Microsoft.Win32.TaskScheduler.dll"
  Delete "$INSTDIR\Microsoft.Win32.TaskScheduler.xml"
  Delete "$INSTDIR\System.Data.SQLite.dll"
  Delete "$INSTDIR\System.Data.SQLite.Linq.dll"
  Delete "$INSTDIR\icon.ico"

  Delete "$SMPROGRAMS\Backer Upper.lnk"
  Delete "$DESKTOP\Backer Upper.lnk"

  RMDir /r "$APPDATA\BackerUpper\Logs"
  RMDir "$APPDATA\BackerUpper"

  nsExec::ExecToLog "$INSTDIR\remove_tasks.bat"
  Delete "$INSTDIR\remove_tasks.bat"

  Delete "$INSTDIR\uninstall.exe"

  RMDir "$INSTDIR"

  DeleteRegKey /ifempty HKCU "Software\BackerUpper"

SectionEnd
