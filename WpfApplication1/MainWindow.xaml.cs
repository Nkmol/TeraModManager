using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using ModVM;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            w = this;
            //Popup window
            var confirm = (Func<string,string,bool>)((msg, capt) =>
                System.Windows.MessageBox.Show(msg, capt, MessageBoxButton.YesNo) == MessageBoxResult.Yes);
            var window = new Action(() => (new Window()).Show());

            this.DataContext = new ModVM.ModViewModel(confirm);

            InitializeComponent();
        }

        public void SelectAll(object sender, RoutedEventArgs e)
        {
            bool value = (bool)((System.Windows.Controls.CheckBox)sender).IsChecked;
            foreach (object mod in lvMods.Items)
            {
                Console.WriteLine(((Mod)mod).IsChecked);
                if (((Mod)mod).IsChecked != value)
                    ((Mod)mod).IsChecked = value;
            }
        }

        public void OpenGameFolder(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) 
            {
                txtGameFolder.Text = dlg.SelectedPath;
            }
        }

        public void OpenModFolder(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK)
            {
                txtModFolder.Text = dlg.SelectedPath;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Console.WriteLine(WindowState);

            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.WindowState = "Maximized";
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.WindowState = "Minimized";
            }

            Properties.Settings.Default.TeraFolder = txtGameFolder.Text;
            Properties.Settings.Default.ModFolder = txtModFolder.Text;

            //save checkbox states
            //foreach (Mod mod in lvMods.Items)
            //{
            //    string firstLine = File.ReadLines(mod.Path).First();
            //    string text = File.ReadAllText(mod.Path).Replace(firstLine,  "Checked \"" + mod.IsChecked.ToString() + '"');
            //    File.WriteAllText(mod.Path, text);
            //}

            Properties.Settings.Default.Save();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            // Very quick and dirty - but it does the job
            if (Properties.Settings.Default.WindowState == "Maximized")
            {
                WindowState = WindowState.Maximized;
            }

            txtGameFolder.Text = Properties.Settings.Default.TeraFolder;
            txtModFolder.Text = Properties.Settings.Default.ModFolder;
        }

        private void MenuInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(Strings.text["InfoMessage"]);
        }

        public static MainWindow w;
    }
}
