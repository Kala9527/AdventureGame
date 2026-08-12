# AdventureGame

AdventureGame 是一个基于 WPF 的 Windows 像素风冒险闯关游戏原型，使用 C# 和 .NET 构建。项目包含简单的玩家、敌人、平台、金币、道具、关卡加载、渲染服务和游戏循环，适合继续扩展成小型桌面游戏。

## 功能亮点

- WPF 桌面应用，窗口化运行。
- 基础游戏循环、输入处理、碰撞与关卡状态管理。
- 玩家、敌人、平台、金币、道具等模型拆分清晰。
- `appsettings.json` 可配置标题、默认关卡和窗口尺寸。
- WiX 安装包工程预留，便于后续生成 Windows 安装程序。

## 项目结构

```text
.
├─ Models/                  # 游戏实体与状态模型
├─ Services/                # 游戏引擎、关卡加载、渲染服务
├─ Docs/                    # 渲染问题排查文档
├─ Installer/               # WiX 安装包工程
├─ App.xaml
├─ MainWindow.xaml
├─ AdventureGame.csproj
└─ appsettings.example.json
```

## 本地运行

要求 Windows 和 .NET SDK。项目当前目标框架为 `net10.0-windows`，请安装匹配的 .NET SDK 预览 / 正式版本。

```bash
dotnet restore
dotnet run
```

## 构建与发布

普通构建：

```bash
dotnet build -c Release
```

发布 Windows x64 自包含版本：

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

输出通常位于：

```text
bin/Release/net10.0-windows/win-x64/publish/
```

## 安装包

`Installer/` 中包含 WiX 安装包工程。生成 msi 前需要安装 WiX Toolset，并根据本机 SDK 与发布目录调整构建配置。

## 注意事项

- `bin/`、`obj/`、`publish/`、安装包产物、`.msi`、日志等不提交到 GitHub。
- `appsettings.json` 是本地运行配置；公开仓库建议使用 `appsettings.example.json` 作为模板。
- 如果运行时缺少目标 .NET SDK，请先安装对应版本。

## 感谢与支持

感谢你点进这个小游戏项目。它还很年轻，但已经有了能继续长大的骨架。如果你喜欢这种桌面小游戏原型，欢迎 Star、Fork、提建议或帮我指出可以改进的地方，你的支持会让我更有动力继续做下去。
