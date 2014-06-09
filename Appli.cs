using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using UtilN;

namespace Launcher {

    public class Appli {

        public string Path = null, Text = "";
        public Appli[] CascadeMenus = null;

        private void OnClick(object s, EventArgs e) {
            WinAPI.Execute(Path);
        }

        private MenuItem MenuItem {
            get {
                if(CascadeMenus != null) {
                    MenuItem mi = new MenuItem(Text, new MenuItem[0]);
                    foreach(Appli appli in CascadeMenus)
                        mi.MenuItems.Add(appli.MenuItem);
                    return mi;
                } else if(!string.IsNullOrWhiteSpace(Path)) {
                    Path = Environment.ExpandEnvironmentVariables(Path);
                    return new MenuItem(Text, OnClick);
                }
                return new MenuItem("-");
            }
        }

        private static Appli[] Applis {
            get {
                FileStream fs = null;
                try {
                    fs = new FileStream(Application.StartupPath + "\\launcher.json", FileMode.Open);
                    return (Appli[])(new DataContractJsonSerializer(typeof(Appli[]))).ReadObject(fs);
                } catch {
                } finally {
                    if(fs != null)
                        fs.Dispose();
                }
                return null;
            }
        }

        public static ContextMenu ContextMenu {
            get {
                ContextMenu cm = new ContextMenu();
                foreach(Appli appli in Applis)
                    cm.MenuItems.Add(appli.MenuItem);
                return cm;
            }
        }
    }
}
