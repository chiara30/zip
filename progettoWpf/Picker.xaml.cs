using classi;
using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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


namespace progettoWpf
{
    /// <summary>
    /// Logica di interazione per Picker.xaml
    /// </summary>
    public partial class Picker : Window
    {
        public ZipArchive Archive { get; set; }

        public Picker(ZipArchive archive)
        {
            Archive = archive;
            InitializeComponent();
            lista.SelectAll();

        }

        private void Estrai_entries_click(object sender, RoutedEventArgs e)
        {
           
            //selected item è IList e non IList<T>
            //var selectedE = lista.SelectedItems as List<ZipArchiveEntry>;   //selectedE null

            var selectedE = lista.SelectedItems.Cast<ZipArchiveEntry>().ToList();

            this.Close();

            MessageBox.Show("Scegli dove estrarre");
            OpenFolderDialog folder = new OpenFolderDialog();
            Nullable<bool> result = folder.ShowDialog();
            if (result ?? false)
            {
                Class1.Extract(selectedE, folder.FolderName);
                MessageBox.Show("Decompressione riuscita");
            }
   
        }
    }
}
