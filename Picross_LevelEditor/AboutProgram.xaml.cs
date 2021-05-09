using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Picross_LevelEditor.Languages;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Picross_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy AboutProgram.xaml
    /// </summary>
    public partial class AboutProgram : Window
    {
        public AboutProgram()
        {
            InitializeComponent();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            tbVersionNo.Text = Lang.sVersionNumber + ": " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            tbReleaseDate.Text = Lang.sReleaseDate + ": " + lastModified.Date.ToString("dd/MM/yyyy");
            tbCopyright.Text = fvi.LegalCopyright;
        }
        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Escape || e.Key == Key.Enter || e.Key == Key.Back)
            {
                this.Close();
            }
        }
    }
}
