using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WindowTopMost
{
    public partial class MainForm : Form
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private Dictionary<IntPtr, bool> windowStates = new Dictionary<IntPtr, bool>();
        private const int HOTKEY_ID = 1;
        
        // 配置管理
        private AppConfig config;
        private uint hotKeyModifiers;
        private uint hotKeyVirtualKey;
        
        // 托盘图标
        private Icon normalIcon;
        private Icon activeIcon;
        private bool hasTopMostWindows = false;

        public MainForm()
        {
            // 加载配置
            config = ConfigManager.LoadConfig();
            hotKeyModifiers = config.HotKeyModifiers;
            hotKeyVirtualKey = config.HotKeyVirtualKey;
            
            InitializeComponent();
            InitializeNotifyIcon();
            
            // 隐藏主窗体，只显示托盘图标
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            
            // 强制创建窗体句柄，然后注册热键
            this.CreateHandle();
            EnsureHotKeyRegistration();
        }

        private void InitializeComponent()
        {
            this.Text = "窗口置顶工具";
            this.Size = new Size(300, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeNotifyIcon()
        {
            // 加载自定义图标
            LoadIcons();
            
            // 创建系统托盘图标
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = activeIcon ?? SystemIcons.Application; // 初始显示灰色图标
            notifyIcon.Text = "窗口置顶工具 - Ctrl+Alt+T";
            notifyIcon.Visible = true;

            // 创建右键菜单
            contextMenu = new ContextMenuStrip();
            
            // 设置菜单自动关闭
            contextMenu.AutoClose = true;
            contextMenu.Closing += ContextMenu_Closing;
            
            var settingsItem = new ToolStripMenuItem("设置热键");
            settingsItem.Click += SettingsItem_Click;
            contextMenu.Items.Add(settingsItem);
            
            var clearAllItem = new ToolStripMenuItem("取消所有置顶");
            clearAllItem.Click += ClearAllTopMostItem_Click;
            contextMenu.Items.Add(clearAllItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            var autoStartItem = new ToolStripMenuItem("开机自启");
            autoStartItem.Checked = ConfigManager.IsAutoStartEnabled();
            autoStartItem.Click += AutoStartItem_Click;
            contextMenu.Items.Add(autoStartItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            var aboutItem = new ToolStripMenuItem("关于");
            aboutItem.Click += AboutItem_Click;
            contextMenu.Items.Add(aboutItem);
            
            var exitItem = new ToolStripMenuItem("退出");
            exitItem.Click += ExitItem_Click;
            contextMenu.Items.Add(exitItem);

            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private void LoadIcons()
        {
            try
            {
                string normalIconPath = Path.Combine(Application.StartupPath, "图钉.png");
                string activeIconPath = Path.Combine(Application.StartupPath, "图钉灰.png");
                
                if (File.Exists(normalIconPath))
                {
                    using (var bitmap = new Bitmap(normalIconPath))
                    {
                        normalIcon = Icon.FromHandle(bitmap.GetHicon());
                    }
                }
                
                if (File.Exists(activeIconPath))
                {
                    using (var bitmap = new Bitmap(activeIconPath))
                    {
                        activeIcon = Icon.FromHandle(bitmap.GetHicon());
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果加载图标失败，使用系统默认图标
                normalIcon = SystemIcons.Application;
                activeIcon = SystemIcons.Application;
            }
        }
        
        private void ContextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            // 允许菜单正常关闭
        }
        
        private void UpdateTrayIcon()
        {
            if (notifyIcon != null)
            {
                // 有置顶窗口时显示蓝色图钉(normalIcon)，没有置顶窗口时显示灰色图钉(activeIcon)
                notifyIcon.Icon = hasTopMostWindows ? (normalIcon ?? SystemIcons.Application) : (activeIcon ?? SystemIcons.Application);
            }
        }
        
        private void CheckTopMostWindows()
        {
            bool foundTopMost = false;
            var windowsToRemove = new List<IntPtr>();
            
            foreach (var kvp in windowStates)
            {
                if (!WindowsAPI.IsWindow(kvp.Key))
                {
                    windowsToRemove.Add(kvp.Key);
                }
                else if (kvp.Value && WindowsAPI.IsWindowTopMost(kvp.Key))
                {
                    foundTopMost = true;
                }
            }
            
            // 清理无效窗口
            foreach (var window in windowsToRemove)
            {
                windowStates.Remove(window);
            }
            
            hasTopMostWindows = foundTopMost;
            UpdateTrayIcon();
        }

        private void EnsureHotKeyRegistration()
        {
            // 模拟设置确定按钮的操作，确保热键正确注册
            // 先注销可能存在的旧热键
            WindowsAPI.UnregisterHotKey(this.Handle, HOTKEY_ID);
            
            // 重新应用当前配置的热键设置
            config.HotKeyModifiers = hotKeyModifiers;
            config.HotKeyVirtualKey = hotKeyVirtualKey;
            ConfigManager.SaveConfig(config);
            
            // 注册新热键
            RegisterGlobalHotKey();
            
            // 检查并更新托盘图标状态
            CheckTopMostWindows();
            
            // 更新托盘提示
            if (notifyIcon != null)
            {
                notifyIcon.Text = $"窗口置顶工具 - {GetHotKeyText()}";
            }
        }

        private void RegisterGlobalHotKey()
        {
            // 注册全局热键
            bool success = WindowsAPI.RegisterHotKey(this.Handle, HOTKEY_ID, hotKeyModifiers, hotKeyVirtualKey);
            if (!success)
            {
                MessageBox.Show("热键注册失败，可能已被其他程序占用。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WindowsAPI.WM_HOTKEY)
            {
                if (m.WParam.ToInt32() == HOTKEY_ID)
                {
                    ToggleCurrentWindowTopMost();
                }
            }
            base.WndProc(ref m);
        }

        private void ToggleCurrentWindowTopMost()
        {
            IntPtr foregroundWindow = WindowsAPI.GetForegroundWindow();
            
            if (foregroundWindow == IntPtr.Zero || foregroundWindow == this.Handle)
                return;

            string windowTitle = WindowsAPI.GetWindowTitle(foregroundWindow);
            if (string.IsNullOrEmpty(windowTitle))
                return;

            bool isCurrentlyTopMost = WindowsAPI.IsWindowTopMost(foregroundWindow);
            bool newTopMostState = !isCurrentlyTopMost;
            
            if (WindowsAPI.SetWindowTopMost(foregroundWindow, newTopMostState))
            {
                // 更新窗口状态记录
                windowStates[foregroundWindow] = newTopMostState;
                
                // 检查并更新托盘图标
                CheckTopMostWindows();
                
                // 显示通知
                string message = newTopMostState ? 
                    $"窗口 '{windowTitle}' 已设置为置顶" : 
                    $"窗口 '{windowTitle}' 已取消置顶";
                    
                notifyIcon.ShowBalloonTip(2000, "窗口置顶工具", message, ToolTipIcon.Info);
            }
            else
            {
                notifyIcon.ShowBalloonTip(2000, "错误", "无法设置窗口置顶状态", ToolTipIcon.Error);
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // 双击托盘图标显示当前热键信息
            string hotKeyText = GetHotKeyText();
            MessageBox.Show($"当前热键: {hotKeyText}\n\n使用此热键可以切换当前活动窗口的置顶状态。", 
                          "窗口置顶工具", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            // 显示设置对话框
            using (var settingsForm = new SettingsForm(hotKeyModifiers, hotKeyVirtualKey))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    // 注销旧热键
                    WindowsAPI.UnregisterHotKey(this.Handle, HOTKEY_ID);
                    
                    // 更新热键设置
                    hotKeyModifiers = settingsForm.Modifiers;
                    hotKeyVirtualKey = settingsForm.VirtualKey;
                    
                    // 保存配置
                    config.HotKeyModifiers = hotKeyModifiers;
                    config.HotKeyVirtualKey = hotKeyVirtualKey;
                    ConfigManager.SaveConfig(config);
                    
                    // 注册新热键
                    RegisterGlobalHotKey();
                    
                    // 更新托盘提示
                    notifyIcon.Text = $"窗口置顶工具 - {GetHotKeyText()}";
                }
            }
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("窗口置顶工具 v1.0\n\n" +
                          "功能：通过快捷键切换当前活动窗口的置顶状态\n" +
                          "默认快捷键：Ctrl + Alt + T\n\n" +
                          "使用方法：\n" +
                          "1. 选中要置顶的窗口\n" +
                          "2. 按下快捷键即可切换置顶状态",
                          "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AutoStartItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                bool newAutoStartState = !menuItem.Checked;
                
                if (ConfigManager.SetAutoStart(newAutoStartState))
                {
                    menuItem.Checked = newAutoStartState;
                    config.AutoStart = newAutoStartState;
                    ConfigManager.SaveConfig(config);
                    
                    string message = newAutoStartState ? "已启用开机自启" : "已禁用开机自启";
                    notifyIcon.ShowBalloonTip(2000, "窗口置顶工具", message, ToolTipIcon.Info);
                }
            }
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ClearAllTopMostItem_Click(object sender, EventArgs e)
        {
            int clearedCount = 0;
            var windowsToRemove = new List<IntPtr>();
            
            // 遍历所有已置顶的窗口
            foreach (var kvp in windowStates.ToList())
            {
                var windowHandle = kvp.Key;
                var isTopMost = kvp.Value;
                
                // 只处理置顶的窗口
                if (isTopMost)
                {
                    // 检查窗口是否仍然存在
                    if (WindowsAPI.IsWindow(windowHandle))
                    {
                        // 取消置顶
                        WindowsAPI.SetWindowPos(windowHandle, WindowsAPI.HWND_NOTOPMOST, 0, 0, 0, 0,
                            WindowsAPI.SWP_NOMOVE | WindowsAPI.SWP_NOSIZE | WindowsAPI.SWP_NOACTIVATE);
                        clearedCount++;
                    }
                    windowsToRemove.Add(windowHandle);
                }
            }
            
            // 从字典中移除已处理的窗口
            foreach (var windowHandle in windowsToRemove)
            {
                windowStates.Remove(windowHandle);
            }
            
            // 更新托盘图标
            CheckTopMostWindows();
            
            // 显示通知
            string message = clearedCount > 0 ? 
                $"已取消 {clearedCount} 个窗口的置顶状态" : 
                "没有找到置顶的窗口";
            notifyIcon.ShowBalloonTip(2000, "窗口置顶工具", message, ToolTipIcon.Info);
        }

        private string GetHotKeyText()
        {
            string text = "";
            if ((hotKeyModifiers & WindowsAPI.MOD_CONTROL) != 0) text += "Ctrl+";
            if ((hotKeyModifiers & WindowsAPI.MOD_ALT) != 0) text += "Alt+";
            if ((hotKeyModifiers & WindowsAPI.MOD_SHIFT) != 0) text += "Shift+";
            if ((hotKeyModifiers & WindowsAPI.MOD_WIN) != 0) text += "Win+";
            
            text += ((Keys)hotKeyVirtualKey).ToString();
            return text;
        }

        protected override void SetVisibleCore(bool value)
        {
            // 防止窗体显示
            base.SetVisibleCore(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 注销热键
                WindowsAPI.UnregisterHotKey(this.Handle, HOTKEY_ID);
                
                // 清理托盘图标
                if (notifyIcon != null)
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                }
                
                if (contextMenu != null)
                {
                    contextMenu.Dispose();
                }
                
                // 清理自定义图标
                if (normalIcon != null && normalIcon != SystemIcons.Application)
                {
                    normalIcon.Dispose();
                }
                
                if (activeIcon != null && activeIcon != SystemIcons.Application)
                {
                    activeIcon.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}