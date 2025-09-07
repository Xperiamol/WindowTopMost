using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WindowTopMost
{
    public class AppConfig
    {
        public uint HotKeyModifiers { get; set; } = WindowsAPI.MOD_CONTROL | WindowsAPI.MOD_ALT;
        public uint HotKeyVirtualKey { get; set; } = (uint)Keys.T;
        public bool AutoStart { get; set; } = false;
        public bool EnableTransparency { get; set; } = false;
        public int TransparencyLevel { get; set; } = 200; // 0-255, 默认200 (约78%不透明)
        
        // 透明度调节热键
        public uint TransparencyIncreaseModifiers { get; set; } = WindowsAPI.MOD_CONTROL | WindowsAPI.MOD_ALT;
        public uint TransparencyIncreaseVirtualKey { get; set; } = (uint)Keys.Up;
        public uint TransparencyDecreaseModifiers { get; set; } = WindowsAPI.MOD_CONTROL | WindowsAPI.MOD_ALT;
        public uint TransparencyDecreaseVirtualKey { get; set; } = (uint)Keys.Down;
        public int TransparencyStep { get; set; } = 25; // 每次调节的步长
        

        
        // 通知设置
        public bool EnableNotifications { get; set; } = true; // 是否启用通知
    }

    public static class ConfigManager
    {
        private static readonly string ConfigFileName = "config.json";
        private static readonly string ConfigFilePath = Path.Combine(Application.StartupPath, ConfigFileName);
        private static readonly string AutoStartRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string AppName = "WindowTopMost";

        public static AppConfig LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string jsonString = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<AppConfig>(jsonString);
                    return config ?? new AppConfig();
                }
            }
            catch (Exception ex)
            {
                // 如果配置文件损坏，使用默认配置
                MessageBox.Show($"配置文件加载失败，将使用默认设置。\n错误信息：{ex.Message}", 
                              "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            return new AppConfig();
        }

        public static bool SaveConfig(AppConfig config)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                string jsonString = JsonSerializer.Serialize(config, options);
                File.WriteAllText(ConfigFilePath, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"配置文件保存失败。\n错误信息：{ex.Message}", 
                              "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool SetAutoStart(bool enable)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AutoStartRegistryKey, true))
                {
                    if (key != null)
                    {
                        if (enable)
                        {
                            string exePath = Application.ExecutablePath;
                            key.SetValue(AppName, exePath);
                        }
                        else
                        {
                            key.DeleteValue(AppName, false);
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"自启动设置失败。\n错误信息：{ex.Message}", 
                              "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return false;
        }

        public static bool IsAutoStartEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AutoStartRegistryKey, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(AppName);
                        return value != null && value.ToString() == Application.ExecutablePath;
                    }
                }
            }
            catch
            {
                // 忽略读取错误
            }
            
            return false;
        }
    }
}