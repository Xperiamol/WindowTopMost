# 窗口置顶工具 (WindowTopMost)

一个简单易用的Windows小插件，通过可编程的快捷键实现当前选中窗口的置顶功能。

## 功能特性

- 🔥 **全局热键支持**：通过自定义快捷键快速切换窗口置顶状态
- 🎯 **智能识别**：自动识别当前活动窗口并进行置顶操作
- 🔄 **状态切换**：支持置顶和取消置顶的双向切换
- 🎨 **系统托盘**：最小化到系统托盘，不占用任务栏空间
- ⚙️ **自定义设置**：支持自定义热键组合
- 💡 **实时通知**：操作后显示气泡提示，确认操作结果

## 默认快捷键

**Ctrl + Alt + W** - 切换当前活动窗口的置顶状态

## 使用方法

1. **启动程序**：运行 `WindowTopMost.exe`
2. **选择窗口**：点击或切换到你想要置顶的窗口
3. **按下热键**：使用 `Ctrl + Alt + W` 切换置顶状态
4. **查看结果**：系统托盘会显示操作结果通知

## 自定义设置

1. 右键点击系统托盘图标
2. 选择「设置热键」
3. 选择修饰键（Ctrl、Alt、Shift、Win）
4. 选择主按键（A-Z、F1-F12等）
5. 点击确定保存设置

## 系统要求

- Windows 10 或更高版本
- .NET 6.0 Runtime

## 编译和运行

### 前提条件
- Visual Studio 2022 或 .NET 6.0 SDK
- Windows 操作系统

### 编译步骤
```bash
# 克隆或下载项目文件
# 在项目目录中执行以下命令

# 还原依赖包
dotnet restore

# 编译项目
dotnet build

# 运行程序
dotnet run
```

### 发布可执行文件
```bash
# 发布为单文件可执行程序
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 项目结构

```
WindowTopMost/
├── Program.cs          # 程序入口点
├── MainForm.cs         # 主窗体和核心逻辑
├── SettingsForm.cs     # 设置窗体
├── WindowsAPI.cs       # Windows API 调用封装
├── WindowTopMost.csproj # 项目配置文件
└── README.md           # 项目说明文档
```

## 技术实现

- **Windows API**：使用 `GetForegroundWindow()` 获取当前活动窗口
- **窗口置顶**：通过 `SetWindowPos()` 设置 `HWND_TOPMOST` 标志
- **全局热键**：使用 `RegisterHotKey()` 注册系统级热键
- **消息处理**：重写 `WndProc()` 处理 `WM_HOTKEY` 消息
- **系统托盘**：使用 `NotifyIcon` 实现托盘功能

## 注意事项

1. **管理员权限**：某些系统窗口可能需要管理员权限才能置顶
2. **热键冲突**：如果热键被其他程序占用，会显示警告信息
3. **窗口限制**：部分系统保护的窗口可能无法置顶
4. **性能影响**：置顶窗口会始终显示在最前面，可能影响其他操作

## 故障排除

### 热键不工作
- 检查是否有其他程序占用了相同的热键组合
- 尝试更换热键组合
- 确保程序正在运行（检查系统托盘）

### 无法置顶某些窗口
- 尝试以管理员身份运行程序
- 某些系统窗口受保护，无法被第三方程序置顶

### 程序无法启动
- 确保已安装 .NET 6.0 Runtime
- 检查 Windows 版本兼容性

## 许可证

本项目采用 MIT 许可证，详见 LICENSE 文件。

## 贡献

欢迎提交 Issue 和 Pull Request 来改进这个项目！

---

**享受更高效的窗口管理体验！** 🚀
