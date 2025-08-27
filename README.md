Wake Up On Lan

Wake Up On Lan is a lightweight .NET C# application that communicates with a standalone Node.js service to send Wake-on-LAN (WOL) packets over the local network. 
This project is designed to provide a simple and reliable way to remotely wake up devices on your LAN.

How It Works

The C# application sends a WOL request to the Node.js service.

The Node.js service formats and sends the "magic packet" to the target device's MAC address over the LAN.

The target device wakes up if WOL is enabled in its BIOS/network settings.
Features

C# Desktop App: User-friendly interface to trigger WOL commands.

Node.js Service: Runs independently, handling network communication for WOL packets.

LAN Communication: Sends magic packets over the local network to wake devices.

Standalone Node.js: Node server can be run separately, allowing flexible deployment.

Installer Ready: Comes with an Inno Setup script for easy installation and shortcuts.

Requirements

.NET 6 or later (for the C# app)

Node.js 18 or later (for the backend service)

Local network access to target devices for WOL

Setup
1. Node.js Service

Install dependencies:
npm install
Start the Node service:
node Wake_Up_On_Lan.js
By default, the service listens on http://localhost:3000.

2. C# Desktop App

Build the project using Visual Studio or your preferred IDE.

Run the app; it will communicate with the Node service to send WOL packets.

3. Installation via Inno Setup

You can package the app with the following Inno Setup Compiler script:

[Setup]
AppName=Wake Up On Lan
AppVersion=1.0
DefaultDirName={pf}\WakeUpOnLan
DefaultGroupName=Wake Up On Lan
OutputBaseFilename=SetupWakeUpOnLan
Compression=lzma
SolidCompression=yes

[Files]
; Copy EXE and icon to installation folder
Source: "WolClientApp.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "app.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
; Start Menu shortcut
Name: "{group}\Wake Up On Lan"; Filename: "{app}\WolClientApp.exe"; IconFilename: "{app}\app.ico"
; Desktop shortcut
Name: "{commondesktop}\Wake Up On Lan"; Filename: "{app}\WolClientApp.exe"; IconFilename: "{app}\app.ico"
