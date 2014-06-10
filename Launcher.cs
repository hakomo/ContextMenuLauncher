using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UtilN;

namespace Launcher {

    public class Launcher : Form {

        private readonly ContextMenu cm = Appli.ContextMenu;

        [DllImport("user32")]
        private static extern void keybd_event(byte v, byte s, int f, int i);

        private const int VK_CONTROL = 0x11, VK_SHIFT = 0x10, VK_MENU = 0x12, KEYEVENTF_KEYUP = 2;
 
        private Launcher() {
            cm.MenuItems[8].Click += delegate {
                keybd_event(VK_CONTROL, 0, 0, 0);
                keybd_event(VK_MENU, 0, 0, 0);
                keybd_event(VK_SHIFT, 0, 0, 0);
                keybd_event(87, 0, 0, 0);
                keybd_event(87, 0, KEYEVENTF_KEYUP, 0);
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
                keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, 0);
                keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
            };

            FormBorderStyle = FormBorderStyle.None;
            Opacity = 0;
            ShowInTaskbar = false;

            GlobalMouse.Set(Handle);
            
            WinAPI.RegisterHotKey(Handle, 9, 6, 'L');
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            if(m.Msg == 0x312) {
                Rectangle r = Screen.PrimaryScreen.WorkingArea;
                Location = new Point((r.Left + r.Right) / 2 - 98, (r.Top + r.Bottom) / 2 - 106);
            } else if(m.Msg == 0x8000) {
                Location = Cursor.Position;
            } else {
                return;
            }
            WinAPI.ForceFore(Handle);
            cm.Show(this, new Point());
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
            Util.RunMutex(() => Application.Run(new Launcher()), "0448904A-903D-4FE0-B83F-65E8FD5007E8");
        }

        private void InitializeComponent() {
            Load += new EventHandler(Launcher_Load);
        }

        private void Launcher_Load(object s, EventArgs e) {

        }
    }
}
