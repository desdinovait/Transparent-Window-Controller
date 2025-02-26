using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Desdinova
{
    public class TransparentWindowController : MonoBehaviour
    {
        [Range(0, 255)]
        public int WindowOpacity = 255;


        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;

        private IntPtr hwnd;

        private int currentMonitorIndex = 0;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int LWA_COLORKEY = 0x00000001;
        private const int LWA_ALPHA = 0x00000002;

        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;



        void Start()
        {
            if (!Application.isEditor)
            {
                hwnd = GetActiveWindow();

                SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_LAYERED);

                SetLayeredWindowAttributes(hwnd, 0, (byte)this.WindowOpacity, LWA_COLORKEY | LWA_ALPHA); // You might not need LWA_COLORKEY

            }
        }



        public void SetAlwaysOnTop(bool enable)
        {
            if (!Application.isEditor)
            {
                if (enable)
                    SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
                else
                    SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            }
            else
            {
                Debug.Log("SetAlwaysOnTop run only in the Windows build.");
            }
        }



        public void SetOpacity(int opacity)
        {
            if (!Application.isEditor)
            {
                this.WindowOpacity = Mathf.Clamp(opacity, 0, 255);
                SetLayeredWindowAttributes(hwnd, 0, (byte)this.WindowOpacity, LWA_COLORKEY | LWA_ALPHA); // You might not need LWA_COLORKEY
            }
            else
            {
                Debug.Log("SetOpacity run only in the Windows build.");
            }
        }

        public void SetTransparent(bool enable)
        {
            if (!Application.isEditor)
            {
                if (enable)
                {
                    SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_LAYERED);
                    this.SetOpacity(this.WindowOpacity);
                }
                else
                {
                    SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) & ~WS_EX_LAYERED);
                }
            }
            else
            {
                Debug.Log("SetTransparent run only in the Windows build.");
            }
        }



        public void ChangeMonitor()
        {
            if (!Application.isEditor)
            {
                if (Display.displays.Length < 2)
                {
                    return;
                }

                currentMonitorIndex = (currentMonitorIndex + 1) % Display.displays.Length;

                int newWidth = Display.displays[currentMonitorIndex].systemWidth;
                int newHeight = Display.displays[currentMonitorIndex].systemHeight;

                IntPtr hWnd = GetActiveWindow();

                Screen.SetResolution(newWidth, newHeight, FullScreenMode.FullScreenWindow);

                SetWindowPos(hWnd, IntPtr.Zero, newWidth * currentMonitorIndex, 0, newWidth, newHeight, SWP_NOZORDER | SWP_NOACTIVATE | SWP_SHOWWINDOW);
            }
            else
            {
                Debug.Log("ChangeMonitor run only in the Windows build.");
            }
        }

        public void ExitApplication()
        {
            Application.Quit();
        }

    }
}