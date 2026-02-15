# Connectivity Tracker

Connectivity Tracker is a WPF (.NET 8) app that measures and logs network connectivity, latency and traffic. This README explains how to set up the project locally and how to build Windows installers (.msi) for both **x86** and **ARM64**.

---

## Quick start ✅

Prerequisites
- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022/2023 (optional) with **.NET Desktop Development** workload
- (For MSI) WiX Toolset v4 (or later) — https://wixtoolset.org/

Build & run (CLI)
- Restore & build: `dotnet build "Connectivity Tracker/Connectivity Tracker.csproj" -c Release`
- Run from source: `dotnet run --project "Connectivity Tracker/Connectivity Tracker.csproj" -c Debug`
- Open the solution in Visual Studio and press F5 to run the app.

Project layout (high level)
- `Controllers/`, `Models/`, `Views/`, `ViewModels/`, `Services/` — MVC pattern used
- Main entry: `Connectivity Tracker/` (WPF project)

---

## Publish app (produce files for the installer) 🔧

You will publish platform-specific outputs and then create an MSI that contains those published files.

Recommended RIDs:
- x86: `win-x86`
- ARM64: `win-arm64`

Framework-dependent (smaller installer; target machine must have .NET Desktop Runtime):

```
# x86
dotnet publish "Connectivity Tracker/Connectivity Tracker.csproj" -c Release -r win-x86 --self-contained false -o ./publish/win-x86

# ARM64
dotnet publish "Connectivity Tracker/Connectivity Tracker.csproj" -c Release -r win-arm64 --self-contained false -o ./publish/win-arm64
```

Self-contained (bundles runtime — larger):

```
# x86 self-contained single-folder
dotnet publish "Connectivity Tracker/Connectivity Tracker.csproj" -c Release -r win-x86 --self-contained true -p:PublishSingleFile=false -o ./publish/selfcontained/win-x86

# ARM64 self-contained
dotnet publish "Connectivity Tracker/Connectivity Tracker.csproj" -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=false -o ./publish/selfcontained/win-arm64
```

Test the content of `publish/<RID>` before packaging (you should find `Connectivity Tracker.exe` and supporting files).

---

## Create an MSI installer (recommended: WiX Toolset) 🧩

High-level steps
1. Publish the app for desired RID (see previous section).
2. Harvest the publish folder (optional) or author a WiX `.wxs` that includes the publish files.
3. Build the WiX project to produce `.msi`.

Example (Heat + Candle + Light — WiX classic flow)

```
# harvest published folder (produces AppFiles.wxs)
heat dir "publish/win-x86" -dr INSTALLFOLDER -cg AppFiles -scom -sreg -sfrag -out AppFiles.wxs

# compile .wxs -> .wixobj
candle Product.wxs AppFiles.wxs

# link -> MSI
light Product.wixobj AppFiles.wixobj -out "ConnectivityTracker-win-x86.msi"
```

Repeat the publish + harvest + build flow with `publish/win-arm64` to create an ARM64 MSI.

Minimal `Product.wxs` example (drop into `Installer/` and adjust attributes):

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">
  <Product Id="*" Name="Connectivity Tracker" Language="1031" Version="1.0.0.0" Manufacturer="YourCompany" UpgradeCode="PUT-GUID-HERE">
    <Package InstallerVersion="500" Compressed="yes" InstallScope="perMachine" />
    <MediaTemplate/>

    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFilesFolder">
        <Directory Id="INSTALLFOLDER" Name="Connectivity Tracker"/>
      </Directory>
    </Directory>

    <!-- harvested AppFiles.wxs should provide ComponentGroup Id="AppFiles" -->
    <DirectoryRef Id="INSTALLFOLDER" />

    <Feature Id="MainFeature" Title="Connectivity Tracker" Level="1">
      <ComponentGroupRef Id="AppFiles" />
    </Feature>
  </Product>
</Wix>
```

Notes
- Use `Guid="*"` for component GUIDs when auto-generating during harvest.
- If you built framework-dependent publish, include a prerequisite note or installer check for the .NET Desktop Runtime.
- For production, set a stable `Product` `UpgradeCode` and increment `Version` for upgrades.

Alternative: create an SDK-style WiX project (`WixToolset.Sdk`) inside the repo and reference the harvested `.wxs` files; then `dotnet build` the installer project.

---

## Build MSI (short CLI recipe)

```
# publish (example for x86)
dotnet publish "Connectivity Tracker/Connectivity Tracker.csproj" -c Release -r win-x86 -o ./publish/win-x86

# harvest and build (WiX)
heat dir ./publish/win-x86 -dr INSTALLFOLDER -cg AppFiles -out AppFiles.wxs
candle Product.wxs AppFiles.wxs
light Product.wixobj AppFiles.wixobj -out "ConnectivityTracker-win-x86.msi"
```

Repeat for `win-arm64` and produce `ConnectivityTracker-win-arm64.msi`.

---

## Signing & Distribution
- Sign MSI with your code-signing certificate (SignTool) before publishing.
- Test installation on matching architecture (VM or physical device).
- Consider GitHub Releases or your distribution pipeline for publishing.

---

## Troubleshooting & tips 💡
- If MSI fails to install, run: `msiexec /i ConnectivityTracker.msi /l*v install.log`
- To reduce installer size, prefer framework-dependent installers and require the .NET Desktop Runtime.
- For auto-update or store distribution, consider MSIX/WinGet instead of MSI.

---

If you want, I can add a WiX installer project to this repository (one MSI per-architecture) — tell me which packaging preferences you prefer (self-contained vs framework-dependent, installer UI, autostart option, code signing).
