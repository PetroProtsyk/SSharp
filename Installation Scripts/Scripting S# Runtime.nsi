; Require admin execution level in order to write to registry
; without VS registration installer can use "user" level
RequestExecutionLevel admin

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "S#"
!define PRODUCT_VERSION "3.0"
!define PRODUCT_PUBLISHER "Scripting"
!define PRODUCT_WEB_SITE "http://www.protsyk.com/scriptdotnet"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

!define REG_NET_ASSEMBLYFOLDER "SOFTWARE\Microsoft\.NETFramework\AssemblyFolders"
!define REG_SL3_ASSEMBLYFOLDER "SOFTWARE\Microsoft\Microsoft SDKs\Silverlight\v3.0\AssemblyFoldersEx"
!define REG_CF_POCKETPC_ASSEMBLYFOLDER "SOFTWARE\Microsoft\.NETCompactFramework\v3.5.0.0\PocketPC\AssemblyFoldersEx"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; Sets the theme path
!define OMUI_THEME_PATH "Theme"


; MUI Settings
!define MUI_ABORTWARNING
; In the moment of writing this, NSIS don't support well Vista icons with PNG compression.
; We provide both, compressed and uncompressed (-nopng) icons.
!define MUI_ICON "${OMUI_THEME_PATH}\installer-nopng.ico"
!define MUI_UNICON "${OMUI_THEME_PATH}\uninstaller-nopng.ico"

; MUI Settings / Header
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_HEADERIMAGE_BITMAP "${OMUI_THEME_PATH}\header-r.bmp"
!define MUI_HEADERIMAGE_UNBITMAP "${OMUI_THEME_PATH}\header-r-un.bmp"

; MUI Settings / Wizard
!define MUI_WELCOMEFINISHPAGE_BITMAP "${OMUI_THEME_PATH}\wizard.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${OMUI_THEME_PATH}\wizard-un.bmp"


; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!define MUI_LICENSEPAGE_RADIOBUTTONS
!insertmacro MUI_PAGE_LICENSE "..\SSharp.Net\License.txt"
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "Scripting.SSharp.Runtime.exe"
InstallDir "$PROGRAMFILES\Scripting\Scripting S#\"
ShowInstDetails show
ShowUninstDetails show

; Installation Paths
!define OUTPATH_SL "$INSTDIR\Silverlight 3.0"
!define OUTPATH_NET "$INSTDIR\NET 3.5"
!define OUTPATH_CF "$INSTDIR\CF 3.5"

Section "MainSection" SEC01
	SetOutPath "${OUTPATH_SL}"
	SetOverwrite ifnewer
	File "..\Public Assemblies\Silverlight 3.0\Scripting.SSharp.dll"
SectionEnd

Section "MainSection" SEC02
	SetOutPath "${OUTPATH_NET}"
	SetOverwrite ifnewer
	File "..\Public Assemblies\NET 3.5\Scripting.SSharp.dll"
SectionEnd

Section "MainSection" SEC03
	SetOutPath "${OUTPATH_SL}\Samples"
	SetOverwrite ifnewer
	File "Samples\Silverlight3\ProcessingSharp.zip"
SectionEnd

Section "MainSection" SEC04
	SetOutPath "${OUTPATH_NET}\Samples"
	SetOverwrite ifnewer
	File "Samples\NET 3.5\AutoSharp.zip"
SectionEnd

Section "MainSection" SEC05
	SetOutPath "${OUTPATH_CF}"
	SetOverwrite ifnewer
	File "..\Public Assemblies\CF 3.5\Scripting.SSharp.dll"
SectionEnd

Section -AdditionalIcons
	SetShellVarContext all
	WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
	CreateDirectory "$SMPROGRAMS\Scripting S#"
	CreateDirectory "$SMPROGRAMS\Scripting S#\Samples"
	
	CreateShortCut "$SMPROGRAMS\Scripting S#\Samples\Silverlight 3.0.lnk" "$INSTDIR\Silverlight 3.0\Samples\"
	CreateShortCut "$SMPROGRAMS\Scripting S#\Samples\NET 3.5.lnk" "$INSTDIR\NET 3.5\Samples\"
	CreateShortCut "$SMPROGRAMS\Scripting S#\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
	CreateShortCut "$SMPROGRAMS\Scripting S#\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
	; Register SL runtime for Visual Studio
	WriteRegStr HKLM "${REG_SL3_ASSEMBLYFOLDER}\${PRODUCT_NAME} for Silverlight 3.0" "" "${OUTPATH_SL}"
	WriteRegStr HKLM "${REG_NET_ASSEMBLYFOLDER}\${PRODUCT_NAME} for Silverlight 3.0" "" "${OUTPATH_SL}"
	; Register .NET runtime for Visual Studio
	WriteRegStr HKLM "${REG_NET_ASSEMBLYFOLDER}\${PRODUCT_NAME} for .NET 3.5" "" "${OUTPATH_NET}"
	; Register CF (Pocket PC) runtime for Visual Studio
	WriteRegStr HKLM "${REG_CF_POCKETPC_ASSEMBLYFOLDER}\${PRODUCT_NAME} for .NETCF 3.5" "" "${OUTPATH_CF}"
	WriteUninstaller "$INSTDIR\uninst.exe"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
	HideWindow
	MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
	MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
	Abort
FunctionEnd

Section Uninstall
	SetShellVarContext all
	Delete "$INSTDIR\${PRODUCT_NAME}.url"
	Delete "$INSTDIR\uninst.exe"
	Delete "${OUTPATH_SL}\Scripting.SSharp.dll"
	Delete "${OUTPATH_NET}\Scripting.SSharp.dll"

	Delete "$SMPROGRAMS\Scripting S#\Uninstall.lnk"
	Delete "$SMPROGRAMS\Scripting S#\Website.lnk"	
	Delete "$SMPROGRAMS\Scripting S#\Samples\Silverlight 3.0.lnk"
	Delete "$SMPROGRAMS\Scripting S#\Samples\NET 3.5.lnk"

	RMDir "$SMPROGRAMS\Scripting S#\Samples"
	RMDir "$SMPROGRAMS\Scripting S#"
	RMDir /r "$INSTDIR"

	DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
	DeleteRegKey HKLM "${REG_SL3_ASSEMBLYFOLDER}\${PRODUCT_NAME} for Silverlight 3.0"
	DeleteRegKey HKLM "${REG_NET_ASSEMBLYFOLDER}\${PRODUCT_NAME} for Silverlight 3.0"
	DeleteRegKey HKLM "${REG_NET_ASSEMBLYFOLDER}\${PRODUCT_NAME} for .NET 3.5"
	DeleteRegKey HKLM "${REG_CF_POCKETPC_ASSEMBLYFOLDER}\${PRODUCT_NAME} for .NETCF 3.5"

	SetAutoClose true
SectionEnd