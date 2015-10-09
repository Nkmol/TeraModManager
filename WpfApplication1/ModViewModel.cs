using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WpfApplication1;
using WpfApplication1.Properties;

namespace ModVM
{
    class ModViewModel 
    {
        public string txtGameFolder { get; set; }
        public string txtModFolder { get; set; }

        public ObservableCollection<Mod> lvMods { get; set; }

        private ICommand _clickLaunch;
        public ICommand ClickLaunch
        {
            get
            {
                return _clickLaunch ?? (_clickLaunch = new CommandHandler(() => Launch(), canLaunch()));
            }
        }

        //Logic for the button command, if the button can be launched
        public bool canLaunch() {
            return true;
        }

        ModManager mm;
        private Func<string, string, bool> confirm;

        public ModViewModel(Func<string, string, bool> confirm)
        {
            this.confirm = confirm;

            txtGameFolder = Settings.Default.TeraFolder;
            txtModFolder = Settings.Default.ModFolder;
            mm = new ModManager(txtModFolder, txtGameFolder);
            mm.loadFiles();

            lvMods = mm.ModsCollection;
            //Add property changed on excisting items on launch
            lvMods.ToList().ForEach(x => x.PropertyChanged += Mod_PropertyChanged);
            //Add the property changed event on every new live added item
            lvMods.CollectionChanged += lvMod_CollectionChanged;
        }

        #region listView

        bool _skipNextChange = false;
        void lvMod_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Mod item in e.NewItems)
                    item.PropertyChanged += Mod_PropertyChanged;

            if (e.OldItems != null)
                foreach (Mod item in e.OldItems)
                    item.PropertyChanged -= Mod_PropertyChanged;
        }

        void Mod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!_skipNextChange) {
                if (e.PropertyName == "IsChecked")
                {
                    Console.WriteLine(((Mod)sender).IsChecked);
                    checkChanged((Mod)sender);
                }
            }
            else _skipNextChange = false;
        }

        public void checkChanged(Mod mod)
        {
            if (!string.IsNullOrEmpty(mod.Code))
            {
                string fileName = mod.Code.Split(' ')[1].Split('\\').Last();

                if (mm.inUse(fileName, mod.IsChecked))
                {
                    if (!confirm(Strings.text["FileInUse"], "Caution"))
                    {
                        //Might be a dirty fix, but because the value is changed and triggers the OnChangedValue again I would have no indicator it is because user canceled confirmBox
                        _skipNextChange = true;
                        mod.IsChecked = false;
                        return;
                    }
                }

                mm.UpdateFile(mod, fileName);
            }
        }
        #endregion

        private bool Launch()
        {
            excecuteMods();
            if (File.Exists(txtGameFolder + @"\TERA-Launcher.exe"))
            {
                Process.Start(txtGameFolder + @"\TERA-Launcher.exe", "-USEALLAVAILABLECORES -VSYNC -D3D11 -NOSTEAM");
                return true;
            }
            else
                return false;

        }

        private void excecuteMods()
        {
            foreach (Mod mod in lvMods)
            {
                if (mod.IsChecked && !string.IsNullOrEmpty(mod.Code))
                {
                    mm.Start(mod);
                }
                else if (!string.IsNullOrEmpty(mod.Code))
                {
                    string file = mod.Code.Split(' ')[1].Split('\\').Last();
                    mm.deleteFile(file, true);           
                }
            }
        }
    }
}
