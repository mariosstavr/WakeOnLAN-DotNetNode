Wake Up On Lan is a lightweight .NET C# desktop application that communicates with a standalone Node.js or HTTP service to send Wake-on-LAN (WOL) packets and check the power status of devices over the local network.
This project is designed to provide a simple and reliable way to remotely wake devices and monitor their current state on your LAN.

How It Works
Wake Up On Lan Request
The C# app sends a WOL request to the backend service (Node.js or HTTP server).

Backend Service
The service formats and sends the "magic packet" to the target device’s MAC address over the LAN.

Device Response
The target device powers on if WOL is enabled in BIOS/network settings.
The app can also query device status via a dedicated endpoint to verify whether the device is online.

Features
C# Desktop App:
User-friendly interface with cards for each device, status indicators, and wake buttons.
Wake Device:
Trigger a WOL packet to remotely power on devices over the LAN.
Check Status:
Query the backend service for device states to see which devices are online, offline, or unknown.
LAN Communication:
Sends WOL "magic packets" and queries over the local network.
Status Indicators:
Visual feedback with colored circles: green for online, red for offline, gray for unknown.
Logging:
Real-time log of actions and responses displayed in the app.
Standalone Backend:
The server can run independently, enabling flexible deployment and separation of responsibilities.
Installer Ready:
Can be packaged with Inno Setup for easy installation and desktop/start menu shortcuts.

Requirements
.NET 6 or later – for the C# desktop app
Node.js 18 or later – if using Node.js backend (optional if HTTP service exists)
Local network access – to send WOL packets and query device statuses

Setup
1. Backend Service (Node.js or HTTP)
Install dependencies (if Node.js):
npm install
Start the service:
node Wake_Up_On_Lan.js
By default, listens on http://localhost:5000 (C# app communicates via this URL).

2. C# Desktop App
Open project in Visual Studio (or preferred IDE)
Build and run the app
The app communicates with the backend service to send WOL packets and check statuses.

3. Installation via Inno Setup
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
Name: "{group}\Wake Up On Lan"; Filename: "{app}\WolClientApp.exe"; Desktop shortcut
Name: "{commondesktop}\Wake Up On Lan"; Filename: "{app}\WolClientApp.exe"; IconFilename: "{app}\app.ico"
