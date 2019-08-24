; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!


[Setup]
AppName=CanyonShooter
AppVerName=CanyonShooter
AppPublisher=CanyonShooter
AppPublisherURL=canyonshooter.tu-bs.de
AppSupportURL=canyonshooter.tu-bs.de
AppUpdatesURL=canyonshooter.tu-bs.de
DefaultDirName={pf}\CanyonShooter
DefaultGroupName=CanyonShooter
OutputBaseFilename=CanyonShooterSetup
Compression=lzma
SolidCompression=yes

[Languages]
Name: english; MessagesFile: compiler:Default.isl

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked

[Files]
Source: C:\Program Files\Microsoft XNA\XNA Game Studio\v2.0\Redist\DX Redist\*; DestDir: {tmp}
Source: C:\Program Files\Microsoft XNA\XNA Game Studio\v2.0\Redist\XNA FX Redist\xnafx20_redist.msi; DestDir: {tmp}
Source: bin\x86\Release\CanyonShooter.exe; DestDir: {app}; Flags: ignoreversion
Source: bin\x86\Release\Content\*; DestDir: {app}\Content; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: {group}\CanyonShooter; Filename: {app}\CanyonShooter.exe
Name: {group}\{cm:UninstallProgram,CanyonShooter}; Filename: {uninstallexe}
Name: {commondesktop}\CanyonShooter; Filename: {app}\CanyonShooter.exe; Tasks: desktopicon

[Run]
Filename: {tmp}\dotnetfx.exe; Flags: skipifdoesntexist; Parameters: /Q
Filename: {tmp}\dxsetup.exe; Parameters: /silent
Filename: msiexec.exe; Parameters: "/quiet /i ""{tmp}\xnafx_redist.msi"""
Filename: {app}\CanyonShooter.exe; Description: {cm:LaunchProgram,CanyonShooter}; Flags: nowait postinstall skipifsilent

[_ISToolDownload]
Source: http://www.microsoft.com/downloads/info.aspx?na=90&p=&SrcDisplayLang=en&SrcCategoryId=&SrcFamilyId=0856eacb-4362-4b0d-8edd-aab15c5e04f5&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2f5%2f6%2f7%2f567758a3-759e-473e-bf8f-52154438565a%2fdotnetfx.exe; DestDir: {tmp}; DestName: dotnetfx.exe

[Code]
var
	hasDotnet2: Boolean;

function InitializeSetup(): Boolean;
begin
	hasDotnet2 := RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\.NETFramework\policy\v2.0');
	Result := True;
end;

// Function generated by ISTool.
function NextButtonClick(CurPage: Integer): Boolean;
begin
		Result := True;
end;