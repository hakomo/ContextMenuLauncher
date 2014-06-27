using Hakomo.Library;
using System;
using System.Runtime.InteropServices;

namespace Launcher {

    class GlobalMouse {

        private delegate int llmp(int cd, int wp, IntPtr lp);

        [DllImport("user32")]
        private static extern int CallNextHookEx(IntPtr hh, int cd, int wp, IntPtr lp);
        [DllImport("user32")]
        private static extern IntPtr SetWindowsHookEx(int id, llmp hp, IntPtr hi, int tid);
        [DllImport("user32")]
        private static extern bool UnhookWindowsHookEx(IntPtr hh);

        private const int WH_MOUSE_LL = 14, WM_APP = 0x8000, WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202,
            WM_RBUTTONDOWN = 0x204, WM_RBUTTONUP = 0x205;

        private static bool b = false, c = false, l = false, r = false;

        public static void Set(IntPtr hw) {
            IntPtr hh = IntPtr.Zero;
            hh = SetWindowsHookEx(WH_MOUSE_LL, (cd, wp, lp) => {
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
                        Mouse.RightDown();
                    } else if(l) {
                        c = true;
                        WinAPI.PostMessage(hw, WM_APP, 0, 0);
                    }
                    return 1;
                } else if(wp == WM_RBUTTONUP) {
                    if(b) {
                        Mouse.RightUp();
                    } else if(!c) {
                        Mouse.RightClick();
                    }
                    r = false;
                    b = l;
                    c = false;
                    return 1;
                } else if(!b && !c) {
                    b |= l || r;
                    if(r)
                        Mouse.RightDown();
                }
                return CallNextHookEx(hh, cd, wp, lp);
            }, Marshal.GetHINSTANCE(typeof(GlobalMouse).Module), 0);
        }
    }
}
