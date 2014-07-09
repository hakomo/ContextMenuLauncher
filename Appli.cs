using Hakomo.Library;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace Launcher {

    public class Appli {

        public string FullPath = null, RelativePath = null, Text = "";
        public Appli[] CascadeMenus = null;

        private MenuItem MenuItem {
            get {
                if(CascadeMenus != null) {
                    MenuItem mi = new MenuItem(Text);
                    foreach(Appli appli in CascadeMenus)
                        mi.MenuItems.Add(appli.MenuItem);
                    return mi;
                } else if(!string.IsNullOrWhiteSpace(FullPath)) {
                    FullPath = Environment.ExpandEnvironmentVariables(FullPath);
                    return new MenuItem(Text, (s, e) => WinAPI.Execute(FullPath));
                } else if(!string.IsNullOrWhiteSpace(RelativePath)) {
                    using(new TemporaryCurrentDirectory(Application.StartupPath))
                        RelativePath = Path.GetFullPath(RelativePath);
                    return new MenuItem(Text, (s, e) => WinAPI.Execute(RelativePath));
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
                    return new Appli[] { new Appli() };
                } finally {
                    if(fs != null)
                        fs.Dispose();
                }
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
