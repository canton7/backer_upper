;NSIS Modern User Interface

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

;--------------------------------
;General

  !define VERSION "0.9.2"

  ;Name and file
  Name "Backer Upper"
  OutFile "BackerUpper_v${VERSION}.exe"
  Icon "..\icon.ico"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\BackerUpper"

  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\BackerUpper" ""

  RequestExecutionLevel admin
  ;Request application privileges for Windows Vista

  ;Stuff for the uninstaller
  !include "FileFunc.nsh"

  !include "FileAssociation.nsh"

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "..\LICENSE.txt"
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
  File "..\LICENSE.txt"
  File "remove_tasks.bat"

  ;Store installation folder
  WriteRegStr HKCU "BackerUpper" "" $INSTDIR

  ;Create uninstaller
  WriteUninstaller  "$INSTDIR\uninstall.exe"

  ;Register to add/remove programs
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                   "DisplayName" "Backer Upper"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                   "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                   "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                   "DisplayIcon" "$\"$INSTDIR\icon.ico$\""
  WriteRegDWord HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                     "NoModify" 1
  WriteRegDWord HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                     "NoModifyRepair" 1
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                   "InstallLocation" "$\"$INSTDIR$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                   "DisplayVersion" "${VERSION}"

 ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
 IntFmt $0 "0x%08X" $0
 WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper" \
                    "EstimatedSize" "$0"

!insertmacro APP_ASSOCIATE "baup" "BAUP File" "Backer Upper Profile" "$INSTDIR\icon.ico" "Open with Backer Upper" "$INSTDIR\BackerUpper.exe $\"%1$\""
!insertmacro UPDATEFILEASSOC

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

  ${If} ${Cmd} 'MessageBox MB_YESNO "Do you want to delete your backup databases?" /SD IDNO IDYES'
    RMDir /r "$APPDATA\BackerUpper\Backups"
  ${EndIf}

  Delete "$INSTDIR\BackerUpper.exe"
  Delete "$INSTDIR\AWSSDK.dll"
  Delete "$INSTDIR\Microsoft.Win32.TaskScheduler.dll"
  Delete "$INSTDIR\Microsoft.Win32.TaskScheduler.xml"
  Delete "$INSTDIR\System.Data.SQLite.dll"
  Delete "$INSTDIR\System.Data.SQLite.Linq.dll"
  Delete "$INSTDIR\icon.ico"
  Delete "$INSTDIR\LICENSE.txt"

  Delete "$SMPROGRAMS\Backer Upper.lnk"
  Delete "$DESKTOP\Backer Upper.lnk"

  RMDir /r "$APPDATA\BackerUpper\Logs"
  RMDir "$APPDATA\BackerUpper"

  nsExec::ExecToLog "$INSTDIR\remove_tasks.bat"
  Delete "$INSTDIR\remove_tasks.bat"

  Delete "$INSTDIR\uninstall.exe"

  RMDir "$INSTDIR"

  !insertmacro APP_UNASSOCIATE "baup" "BAUP File"
  !insertmacro UPDATEFILEASSOC

  DeleteRegKey /ifempty HKCU "Software\BackerUpper"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BackerUpper"

SectionEnd
