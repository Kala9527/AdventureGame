# AdventureGame

AdventureGame is a WPF Windows pixel-style adventure game prototype built with C# and .NET. It includes basic player, enemy, platform, coin, power-up, level loading, rendering, and game loop structures.

## Highlights

- WPF desktop application.
- Basic game loop, input handling, collision, and level state management.
- Clear model separation for players, enemies, platforms, coins, and power-ups.
- `appsettings.json` for title, default level, and window size.
- WiX installer project prepared for future Windows installer builds.

## Structure

```text
.
├─ Models/
├─ Services/
├─ Docs/
├─ Installer/
├─ App.xaml
├─ MainWindow.xaml
├─ AdventureGame.csproj
└─ appsettings.example.json
```

## Run Locally

Requires Windows and the .NET SDK. The project currently targets `net10.0-windows`, so install the matching .NET SDK preview or release.

```bash
dotnet restore
dotnet run
```

## Build And Publish

Build:

```bash
dotnet build -c Release
```

Publish a Windows x64 self-contained build:

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

Output is usually written to:

```text
bin/Release/net10.0-windows/win-x64/publish/
```

## Installer

The `Installer/` folder contains a WiX installer project. Install WiX Toolset and adjust local build paths before generating an MSI.

## Notes

- `bin/`, `obj/`, `publish/`, installer outputs, `.msi`, and logs are ignored.
- `appsettings.json` is local runtime config; use `appsettings.example.json` as the public template.
- Install the required .NET SDK if the target framework is missing.

## Thanks

Thank you for checking out this small game prototype. If you like this direction, a Star, Fork, issue, or suggestion would be very encouraging.
