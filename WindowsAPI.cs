using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowTopMost
{
    public static class WindowsAPI
    {
        // 窗口置顶相关常量
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_NOACTIVATE = 0x0010;

        // 热键相关常量
        public const int WM_HOTKEY = 0x0312;
        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x0004;
        public const int MOD_WIN = 0x0008;

        // 获取窗口信息相关常量
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_LAYERED = 0x00080000;
        
        // 透明度相关常量
        public const int GWL_EXSTYLE_LAYERED = -20;
        public const int LWA_ALPHA = 0x00000002;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        public static string GetWindowTitle(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// 检查窗口是否置顶
        /// </summary>
        public static bool IsWindowTopMost(IntPtr hWnd)
        {
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) != 0;
        }

        /// <summary>
        /// 设置窗口置顶状态
        /// </summary>
        public static bool SetWindowTopMost(IntPtr hWnd, bool topMost)
        {
            if (!IsWindow(hWnd)) return false;
            
            int insertAfter = topMost ? HWND_TOPMOST : HWND_NOTOPMOST;
            return SetWindowPos(hWnd, insertAfter, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        /// <summary>
        /// 设置窗口透明度
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="opacity">透明度 (0-255, 0为完全透明，255为完全不透明)</param>
        /// <returns>设置是否成功</returns>
        public static bool SetWindowOpacity(IntPtr hWnd, byte opacity)
        {
            if (!IsWindow(hWnd)) return false;
            
            // 获取当前窗口样式
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            
            // 添加分层窗口样式
            if ((exStyle & WS_EX_LAYERED) == 0)
            {
                SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED);
            }
            
            // 设置透明度
            return SetLayeredWindowAttributes(hWnd, 0, opacity, LWA_ALPHA);
        }

        /// <summary>
        /// 移除窗口透明度效果
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns>移除是否成功</returns>
        public static bool RemoveWindowOpacity(IntPtr hWnd)
        {
            if (!IsWindow(hWnd)) return false;
            
            // 获取当前窗口样式
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            
            // 移除分层窗口样式
            if ((exStyle & WS_EX_LAYERED) != 0)
            {
                SetWindowLong(hWnd, GWL_EXSTYLE, exStyle & ~WS_EX_LAYERED);
                return true;
            }
            
            return false;
        }
    }
}