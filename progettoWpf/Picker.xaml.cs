using classi;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using ICSharpCode.SharpZipLib.Zip;
using System.Windows.Controls.Primitives;
using ICSharpCode.SharpZipLib.GZip;

namespace progettoWpf
{
    /// <summary>
    /// Logica di interazione per Picker.xaml
    /// </summary>
    public partial class Picker : Window
    {
        public ZipFile Archive { get; set; }

        public Picker(ZipFile archive)
        {
            Archive = archive;
            InitializeComponent();
            lista.SelectAll();
    
        }

        private void Estrai_entries_click(object sender, RoutedEventArgs e)
        {
            var selectedE = lista.SelectedItems.Cast<ZipEntry>().ToList();
            
            this.Close();

            MessageBox.Show("Scegli dove estrarre");
            OpenFolderDialog folder = new OpenFolderDialog();
            Nullable<bool> result = folder.ShowDialog();
            if (result ?? false)
            {               
                //controllo se c'è la password provando a estrarre una entry
                bool pass_protected = false;
                try
                {
                    foreach(var entry in selectedE)
                    {
                        using Stream zipStream = Archive.GetInputStream(entry);
                        Debug.WriteLine("NO PASSWORD");                        
                    }
                }
                catch (ZipException)
                {
                    //c'è password
                    pass_protected = true;
                    Debug.WriteLine("PASSWORD");
                }

                if (pass_protected) //invece che un ulteriore controllo potrei mettere tutto nel blocco catch
                {
                    //non posso confrontare la password inserita con quella dell'archivio perchè zipFile.Password non ha getter
                    //richiedo la password finchè non avviene l'estrazione 
                    bool pass_ok;
                    do
                    {
                        var win = new Pass_ZipFile();
                        var res = win.ShowDialog();
                        if (res is null) return;
                        try
                        {
                            if (res is true)
                            {
                                MessageBox.Show("tentativo di decompressione");
                                Class1.Extract(Archive, selectedE, folder.FolderName, win.Result);                                
                            }                              
                            pass_ok = true;
                        }
                        catch (ZipException)
                        {
                            MessageBox.Show("La password era errata");
                            pass_ok = false;
                        }
                    } while (pass_ok == false);
                }
                else
                {
                    Class1.Extract(Archive, selectedE, folder.FolderName, "");
                }
                
                MessageBox.Show("Decompressione riuscita");
            }            
        }
    }
}
