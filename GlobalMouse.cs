using System;
using System.Runtime.InteropServices;
using UtilN;

namespace Launcher {

    class GlobalMouse {

        private delegate int llmp(int cd, int wp, IntPtr lp);

        [DllImport("user32")]
        private static extern int CallNextHookEx(IntPtr hh, int cd, int wp, IntPtr lp);
        [DllImport("user32")]
        private static extern void mouse_event(int f, int x, int y, int d, IntPtr i);
        [DllImport("user32")]
        private static extern IntPtr SetWindowsHookEx(int id, llmp hp, IntPtr hi, int tid);
        [DllImport("user32")]
        private static extern bool UnhookWindowsHookEx(IntPtr hh);

        private const int MOUSEEVENTF_RIGHTDOWN = 8, MOUSEEVENTF_RIGHTUP = 0x10, WH_MOUSE_LL = 14,
            WM_APP = 0x8000, WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202,
            WM_RBUTTONDOWN = 0x204, WM_RBUTTONUP = 0x205;

        private static bool b = false, c = false, l = false, r = false;

        public static void Set(IntPtr hw) {
            IntPtr hh = IntPtr.Zero;
            hh = SetWindowsHookEx(WH_MOUSE_LL, new llmp((cd, wp, lp) => {
                if(cd < 0) {
                } else if(wp == WM_LBUTTONDOWN) {
                    l = true;
                    if(!b && !c && r) {
                        c = true;
                        WinAPI.PostMessage(hw, WM_APP, 0, 0);
                    }
                } else if(wp == WM_LBUTTONUP) {
                    l = false;
                    b &= !b || r;
                } else if(wp == WM_RBUTTONDOWN) {
                    r = true;
                    if(b) {
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
                    } else if(l) {
                        c = true;
                        WinAPI.PostMessage(hw, WM_APP, 0, 0);
                    }
                    return 1;
                } else if(wp == WM_RBUTTONUP) {
                    if(b) {
                        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
                    } else if(!c) {
                        mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
                    }
                    r = false;
                    b = l;
                    c = false;
                    return 1;
                } else if(!b && !c) {
                    b |= l || r;
                    if(r)
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
                }
                return CallNextHookEx(hh, cd, wp, lp);
            }), Marshal.GetHINSTANCE(typeof(GlobalMouse).Module), 0);
        }
    }
}
