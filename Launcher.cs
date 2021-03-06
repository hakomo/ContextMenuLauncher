﻿using Hakomo.Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Launcher {

    public class Launcher : Form {

        private readonly ContextMenu cm = Appli.ContextMenu;
        private readonly GlobalMouse gm;

        public Launcher() {
            FormBorderStyle = FormBorderStyle.None;
            Opacity = 0;
            ShowInTaskbar = false;

            // gm = new GlobalMouse(Handle);
            WinAPI.RegisterHotKey(Handle, 9, 6, Convert.ToByte(Keys.F17));
        }

        protected override void WndProc(ref Message m) {
            int WM_APP = 0x8000, WM_HOTKEY = 0x312, WM_EXITMENULOOP = 0x212;
            base.WndProc(ref m);
            if(m.Msg == WM_APP) {
                Location = Cursor.Position;
                WinAPI.ForceFore(Handle);
                cm.Show(this, new Point());
            } else if(m.Msg == WM_HOTKEY) {
                Rectangle r = Screen.PrimaryScreen.Bounds;
                Location = new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
                WinAPI.ForceFore(Handle);
                cm.Show(this, new Point());
            } else if(m.Msg == WM_EXITMENULOOP) {
                foreach(IntPtr hw in WinAPI.GetTasks()) {
                    if(WinAPI.IsIconic(hw))
                        continue;
                    WinAPI.ForceFore(hw);
                    break;
                }
            }
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        [STAThread]
        private static void Main() {
            Util.Run<Launcher>("0448904A-903D-4FE0-B83F-65E8FD5007E8", delegate {
                Keyboard.Input(new Keys[] { Keys.ControlKey, Keys.ShiftKey }, Keys.F17);
            });
        }
    }
}
