using classi;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using Ionic.Zip;
using System.Collections.ObjectModel;

namespace progettoWpf
{
    /// <summary>
    /// Logica di interazione per Picker.xaml
    /// </summary>
    public partial class Picker : Window
    {
        public ZipFile Zip { get; set; }
        public string Path { get; set; }

        //public ObservableCollection<Filewithpsw> File { get; set; }
        //la finestra non si deve aggiornare quindi basta una lista
        public ObservableCollection<Filewithpsw> File { get; set; }

        public Picker(ZipFile zip, ObservableCollection<Filewithpsw> to_extract, string archive_path) 
        {
            Path = archive_path;
            File = to_extract;
            Zip = zip;

            InitializeComponent();
            //lista.SelectAll();           
        }

        private void Estrai_entries_click(object sender, RoutedEventArgs e)
        {
            var selectedE = lista.SelectedItems.Cast<Filewithpsw>().ToList();    

            Close();

            if (selectedE.Count != 0)
            {
                MessageBox.Show("Scegli dove estrarre");
                OpenFolderDialog folder_dest = new OpenFolderDialog();
                Nullable<bool> result = folder_dest.ShowDialog();
                if (result ?? false)
                {
                    //controllo se c'è password
                    bool psw_protected = false;
                    string psw = "";

                    foreach (var elm in selectedE)
                    {
                        var entry = elm.ArchiveEntry;

                        if (entry.UsesEncryption)
                        {
                            //Debug.WriteLine(elm.Elm + " password SI");
                            psw_protected = true;
                            MessageBox.Show("il file " + elm.Elm + " è protetto da passsword");
                            bool ok = false;
                            do
                            {
                                var win = new Pass_ZipFile();
                                var res = win.ShowDialog();
                                //if (res is null) return;
                                if (res is true)
                                {
                                    if (ZipFile.CheckZipPassword(Path, win.Result))
                                    {
                                        ok = true;
                                        psw = win.Result;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Password errata");
                                    }
                                }
                                if (res is false) return;

                            } while (ok == false);
                        }
                        //else
                        //{
                        //    Debug.WriteLine(elm.Elm + " password NO");
                        //}

                        if (psw_protected)
                        {
                            entry.ExtractWithPassword(folder_dest.FolderName, ExtractExistingFileAction.DoNotOverwrite, psw);
                        }
                        else
                        {
                            entry.Extract(folder_dest.FolderName, ExtractExistingFileAction.DoNotOverwrite);
                        }

                        if (elm.Dc != "")
                        {
                            // cartelle vuote
                            if (entry.FileName.EndsWith('/'))
                            {
                                Directory.CreateDirectory(elm.Elm);
                            }
                            
                        }
                        
                    }
                    MessageBox.Show("Decompressione riuscita");
                }
            }
            else
            {
                MessageBox.Show("Nessun elemento selezionato");
                return;
            }
        }

        private void Aggiorna_click(object sender, RoutedEventArgs e)
        {
            var selected = lista.SelectedItems.Cast<Filewithpsw>().ToList();

            Close();

            //you cannot modify the password on any encrypted entry,
            //except by extracting the entry with the original password
            //removing the original entry and then adding a new entry with a new Password.
            //if there is an un-encrypted entry, you can set the
            //password on the entry and then call Save() on the ZipFile

            if (selected.Count != 0)
            {
                List<Filewithpsw> list = new();
                foreach (var elm in selected)
                {
                    string mess = "aggiornare password di " + elm.Elm + "?";
                    var d = MessageBox.Show(mess, "", MessageBoxButton.YesNo);
                    if (d == MessageBoxResult.Yes)
                    {
                        var entry = elm.ArchiveEntry;                        
                        if (entry.UsesEncryption)
                        {
                            MessageBox.Show("inserire vecchia password di " + elm.Elm);
                            //Debug.WriteLine("modifica");
                            //vecchia password
                            bool ok = false;                            
                            do
                            {
                                var win = new Pass_ZipFile();
                                var res = win.ShowDialog();
                                if (res is true)
                                {
                                    if (Class1.CheckPassword(entry, win.Result))
                                    {
                                        ok = true;
                                        elm.Psw = win.Result;
                                        
                                        list.Add(new Filewithpsw()
                                        {
                                            Elm = elm.Elm,
                                            Psw = win.Result,
                                            Dc = "",
                                            ArchiveEntry = elm.ArchiveEntry,
                                            File = Zip
                                        });
                                    }
                                    else
                                    {
                                        MessageBox.Show("Password errata");
                                    }
                                }
                                if (res is false) return;
                            } while (ok == false);
                        }
                        else
                        {
                            //non c'era una password
                            //Debug.WriteLine("metti");
                            list.Add(new Filewithpsw()
                            {
                                Elm = elm.Elm,
                                Psw = "",
                                Dc = "",
                                ArchiveEntry = elm.ArchiveEntry,
                                File = Zip
                            });
                        }

                    }
                    else
                    {
                        //Debug.WriteLine("non cambia");
                        continue;
                    }
                }
                MessageBox.Show("inserisci nuove password");
                new Pass_aggiorna(list).ShowDialog();

                MessageBox.Show("Aggiornata");
            }
            else
            {
                MessageBox.Show("Nessun elemento selezionato");
                return;
            }
        }

        private void Togli_Click(object sender, RoutedEventArgs e)
        {
            var selected = lista.SelectedItems.Cast<Filewithpsw>().ToList();

            Close();

            if (selected.Count != 0)
            {
                List<Filewithpsw> list = new();
                foreach (var elm in selected)
                {
                    var entry = elm.ArchiveEntry;
                    if (entry.UsesEncryption)
                    {
                        string mess = "togliere password di " + elm.Elm + "?";
                        var d = MessageBox.Show(mess, "", MessageBoxButton.YesNo);
                        if (d == MessageBoxResult.Yes)
                        {
                            MessageBox.Show("inserire vecchia password di " + elm.Elm);
                            //vecchia password
                            bool ok = false;
                            do
                            {
                                var win = new Pass_ZipFile();
                                var res = win.ShowDialog();
                                if (res is true)
                                {
                                    if (ZipFile.CheckZipPassword(Path, win.Result))
                                    {
                                        ok = true;
                                        elm.Psw = win.Result;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Password errata");
                                    }
                                }
                                if (res is false) return;
                            } while (ok == false);
                        }
                        else
                        {
                            //Debug.WriteLine("non cambia");
                            continue;
                        }
                    }
                   
                    using var ms = new MemoryStream();

                    if (elm.Psw == "")
                    {
                        // no pass
                        elm.ArchiveEntry.Extract(ms);
                    }
                    else
                    {
                        // pass
                        elm.ArchiveEntry.ExtractWithPassword(ms, elm.Psw);
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                    Zip.RemoveEntry(elm.ArchiveEntry);
                    elm.ArchiveEntry = Zip.AddEntry(elm.ArchiveEntry.FileName, ms);

                    var ar = System.IO.Path.GetFullPath(Zip.Name);
                    Zip.Save(ar);
                }
                
                MessageBox.Show("Rimossa");
            }
            else
            {
                MessageBox.Show("Nessun elemento selezionato");
                return;
            }
        }
    }
}
