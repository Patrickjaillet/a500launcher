; A500 Launcher installer (Inno Setup 6+ / Inno Setup 7).
; Build with: iscc /DAppVersion=x.y.z /DPublishDir=<path> installer\A500Launcher.iss

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif

#ifndef PublishDir
  #define PublishDir "..\bin\Release\net8.0-windows10.0.22000.0\publish"
#endif

#define AppName "A500 Launcher"
#define AppExe "A500Launcher.exe"
#define AppPublisher "Patrick JAILLET"
#define AppUrl "https://patrickjaillet.github.io/a500launcher"

[Setup]
AppId={{9A1B2C3D-4E5F-4A6B-8C7D-A500A500A500}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppUrl}
AppSupportURL={#AppUrl}
VersionInfoVersion={#AppVersion}
DefaultDirName={autopf}\A500 Launcher
DefaultGroupName=A500 Launcher
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#AppExe}
OutputDir=..\dist
OutputBaseFilename=A500Launcher-Setup-{#AppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=classic
WizardImageFile=..\assets\installer\wizard-large.bmp
WizardSmallImageFile=..\assets\installer\wizard-small.bmp
SetupIconFile=..\assets\icons\installer.ico
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.22000
LicenseFile=..\LICENSE

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl,A500Launcher.en.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Components]
Name: "app"; Description: "A500 Launcher"; Types: full compact custom; Flags: fixed
Name: "winuae"; Description: "Bundled WinUAE 6.0.3"; Types: full compact custom; Flags: fixed
Name: "i18n"; Description: "Translations"; Types: full custom
Name: "sounds"; Description: "Drive sound"; Types: full custom

[Files]
Source: "{#PublishDir}\{#AppExe}"; DestDir: "{app}"; Components: app; Flags: ignoreversion
Source: "{#PublishDir}\*.dll"; DestDir: "{app}"; Components: app; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#PublishDir}\*.json"; DestDir: "{app}"; Components: app; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#PublishDir}\assets\i18n\en.json"; DestDir: "{app}\assets\i18n"; Components: app; Flags: ignoreversion
Source: "{#PublishDir}\assets\i18n\*.json"; DestDir: "{app}\assets\i18n"; Excludes: "en.json"; Components: i18n; Flags: ignoreversion
Source: "{#PublishDir}\assets\sounds\*"; DestDir: "{app}\assets\sounds"; Components: sounds; Flags: ignoreversion recursesubdirs
Source: "{#PublishDir}\assets\fonts\*"; DestDir: "{app}\assets\fonts"; Components: app; Flags: ignoreversion recursesubdirs skipifsourcedoesntexist
Source: "{#PublishDir}\winuae\*"; DestDir: "{app}\winuae"; Components: winuae; Flags: ignoreversion recursesubdirs
Source: "..\bios\README.txt"; DestDir: "{app}\bios"; Flags: ignoreversion
Source: "..\roms\README.txt"; DestDir: "{app}\roms"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\CHANGELOG.md"; DestDir: "{app}"; Flags: ignoreversion

[Dirs]
Name: "{app}\bios"
Name: "{app}\roms"

[Icons]
Name: "{group}\A500 Launcher"; Filename: "{app}\{#AppExe}"
Name: "{group}\{cm:UninstallProgram,A500 Launcher}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\A500 Launcher"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "{cm:LaunchProgram,A500 Launcher}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}\assets"
Type: filesandordirs; Name: "{app}\winuae"
