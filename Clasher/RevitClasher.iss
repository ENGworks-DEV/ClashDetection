; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{D76DF5A5-CDF4-4A15-804A-95C3D95A2F42}
AppName=ENG Revit Tab tools 2018 Setup    
AppVersion=1.0.1
;AppVerName=Synchro Setup 1.01
AppPublisher=PRD
AppPublisherURL=-
AppSupportURL=-
AppUpdatesURL=-
DefaultDirName={pf}\ENG RevitTools 2018
DisableDirPage=yes
DefaultGroupName=ENG Revit Tab tools 2018 Setup
DisableProgramGroupPage=yes
OutputBaseFilename=ENG Revit Tab tools 2018 Setup 0.0.1
Compression=lzma
SolidCompression=yes
OutputManifestFile=Setup-Manifest.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Dirs]
Name: "{userappdata}\Autodesk\Revit\Addins\2018\"
[InstallDelete]
Type: filesandordirs; Name: "{userappdata}\Autodesk\Revit\Addins\2018\ENG_ClashDetection"
    

[Files]
Source: "C:\Users\pderendinger\AppData\Roaming\Autodesk\Revit\Addins\2018\ENG_ClashDetection\*"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2018\ENG_ClashDetection\"; Flags: ignoreversion recursesubdirs createallsubdirs
[Files]
Source: "C:\Users\pderendinger\AppData\Roaming\Autodesk\Revit\Addins\2018\RVT_AutomateClash.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2018\"; Flags: ignoreversion recursesubdirs createallsubdirs

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

