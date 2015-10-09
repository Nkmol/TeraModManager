using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Strings
{
    static string version = "0.1";
    public static Dictionary<string, string> text = new Dictionary<string, string>{
        {"EmptyPaths", "Provide both your Tera folder and the mod folder." },
        {"ClientNotFound", "Make sure you select the Tera Client folder."},
        {"InfoMessage", "This application is made so people can easially manage their mods of Tera Online. \n \nCreated by: Nkmol\nLicense: GPLv3\nVersion: " + version + " \n\nhttp://www.gnu.org/licenses/quick-guide-gplv3.html"},
        {"FileInUse", "This file is already in use by a other mod. If the mod does not state that it is compatible with your current mod, then please do not use this mod. \nDo you want to proceed?"}
    };
}
