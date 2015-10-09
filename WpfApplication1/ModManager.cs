using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModVM
{
    class ModManager
    {
        string modFolder;
        string gameFolder;

        ObservableCollection<Mod> _ModsCollection = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> ModsCollection
        {
            get { return _ModsCollection; }
        }

        DirectoryInfo di;
        DirectoryInfo[] Folders;
        List<FileInfo[]> codeFiles = new List<FileInfo[]>();
        List<string> filesInUse = new List<string>();

        string selectedFilePath;
        string selectedFileName;
        //string pathCH = @"\Client\S1Game\CookedPC\Art_Data\Packages\_CH\";

        public ModManager(string modFolder, string gameFolder)
        {
            this.modFolder = modFolder;
            this.gameFolder = gameFolder;
            di = new DirectoryInfo(modFolder);
        }

        public void loadFiles()
        {
            Folders = di.GetDirectories();
            foreach (DirectoryInfo folder in Folders)
            {
                if (folder.Name != "BACKUP")
                {
                    FileInfo[] fi = folder.GetFiles("*.tm");
                    if (fi.Length > 0)
                    {
                        ReadProperties(fi[0].FullName);
                        codeFiles.Add(fi);
                    }
                }
            }
        }

        public bool deleteFile(string file, bool CH = true)
        {
            //if (File.Exists(gameFolder + pathCH + file))
            //{
            //    File.Delete(gameFolder + pathCH + file);
            //    return true;
            //}
            return false;
        }

        private void ReadProperties(string path)
        {
            string line;
            var properties = new Dictionary<string, string>();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(path);
            while (!string.IsNullOrEmpty((line = file.ReadLine())))
            {
                string[] split = line.Split(new Char[] { '"' });
                // properties.Add(split[0].ToUpper(), split[1].Replace('"', ' '));
                properties[split[0].ToUpper().Trim()] = split[1].Trim();
            }

            //Read the remaining code as one string
            while ((line = file.ReadLine()) != null)
            {
                string s;
                if (properties.TryGetValue("CODE", out s))
                    properties["CODE"] += line + " ";
                else
                    properties["CODE"] = line + " ";
            }
            properties["PATH"] = path;
            CreateProperties(properties);
            file.Close();

        }

        private void CreateProperties(Dictionary<string, string> properties)
        {
            string s;
            Mod newMod = new Mod()
            {
                Path = properties["PATH"]
            };

            //Make Properties optional
            if (properties.TryGetValue("CHECKED", out s))
                newMod.IsChecked = Boolean.Parse(s);

            if (properties.TryGetValue("NAME", out s))
                newMod.Name = s;

            if (properties.TryGetValue("AUTHOR", out s))
                newMod.Author = s;

            if (properties.TryGetValue("TYPE", out s))
                newMod.Type = s;

            if (properties.TryGetValue("DATE", out s))
                newMod.Date = DateTime.Parse(s).ToString("yyyy-MM-dd");

            if (properties.TryGetValue("CODE", out s))
                newMod.Code = s;

            _ModsCollection.Add(newMod);
        }

        public void Start(Mod mod)
        {
            string[] s = mod.Code.Split(' ');
            int counter = 0;
            while (counter <= s.Length)
            {
                switch (s[counter])
                {
                    case "SELECT":
                        selectFile(s[counter + 1]);
                        break;
                    case "REPLACEWITH":
                        replaceFile(s[counter + 1], mod.Name);
                        break;
                }
                counter += 2;
            }
        }

        private void selectFile(string path)
        {
            selectedFilePath = path.Replace("%TeraGame%", gameFolder);

            selectedFileName = path.Split('\\').Last();
            string dir = (gameFolder + @"\BACKUP\" + path.Replace(@"%TeraGame%\", "")).Replace(selectedFileName, "");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.Copy(selectedFilePath, dir + "\\" + selectedFileName, true);
        }

        private void replaceFile(string path, string modFolderName)
        {
            string savePatch = gameFolder + "_" + path.Split('\\').Last();
            Console.WriteLine(savePatch);
            if (!Directory.Exists(savePatch))
                Directory.CreateDirectory(savePatch);

            string fileDir = savePatch + "\\" + selectedFileName;
            if (!File.Exists(fileDir))
            {
                File.Copy(path.Replace("%ModDir%", modFolder), fileDir, true);
            }
        }

        public bool inUse(string fileName, bool value)
        {
            if (value)
                return filesInUse.Exists(x => x == fileName);
            else
                return false;
        }

        public void UpdateFile(Mod mod, string fileName)
        {
            if (mod.IsChecked)
            {
                filesInUse.Add(fileName);
            }
            else if (!mod.IsChecked)
            {
                filesInUse.Remove(fileName);
            }
        }
    }
}
