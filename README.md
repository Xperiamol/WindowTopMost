一个简单易用的Windows小插件，通过可编程的快捷键实现当前选中窗口的置顶功能。

下载压缩包-解压运行WindowTopMost.exe
1. 右键点击系统托盘图标
2. 选择「设置热键」
3. 选择修饰键（Ctrl、Alt、Shift、Win）
4. 选择主按键（A-Z、F1-F12等）
5. 点击确定保存设置

## 系统要求

- Windows 10 或更高版本
- .NET 6.0 Runtime

### 前提条件
- Visual Studio 2022 或 .NET 6.0 SDK
- Windows 操作系统

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
 应用图标来自www.iconfont.cn
