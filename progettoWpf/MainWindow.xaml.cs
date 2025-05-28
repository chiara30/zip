using System.Windows;
using classi;
using Microsoft.Win32;
using System.IO;
//using ICSharpCode.SharpZipLib.Zip;
using Ionic.Zip;
using ICSharpCode.SharpZipLib.GZip;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices;


namespace progettoWpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {

        InitializeComponent();    
        
    }

    //private void CompressFile_Button_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        //scelta file(files) da comprimere
    //        OpenFileDialog file = new OpenFileDialog();
    //        file.Filter = "txt files (*.txt)|*.txt|image files (*.jpg;*.png;*.webp)|*.jpg;*.png;*.webp|All files (*.*)|*.*";
    //        file.Multiselect = true;

    //        Nullable<bool> resultF = file.ShowDialog();
    //        if (resultF == true)     //utente seleziona 'apri'
    //        {
    //            string[] file_path = file.FileNames;
    //            for (int i = 0; i < file_path.Length; i++)
    //            {
    //                MessageBox.Show("File da comprimere " + file_path.GetValue(i));
    //            }

    //            MessageBox.Show("Creazione archivio " + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ".zip");

    //            //non ho il caso che non seleziona niente perchè la finestra non si chiude senza aver selezionato qualcosa
    //            var archive_path = System.IO.Path.ChangeExtension(file.FileName, ".zip");

    //            //if (File.Exists(archive_path)) File.Delete(archive_path); //sovrascrive

    //            FileStream fs = File.Create(archive_path);
    //            using ZipOutputStream zip = new ZipOutputStream(fs);

    //            var win = new Pass_ZipStream();
    //            var res = win.ShowDialog();
    //            if (res is null) return;
    //            if (res is true) Class1.Compress(zip, file_path, win.Result);             
    //            if (res is false) Class1.Compress(zip, file_path, "");

    //            MessageBox.Show("Compressione file riuscita");             
    //        }
    //    }
    //    catch (Exception err)
    //    {
    //        MessageBox.Show(err.ToString());
    //    }
    //}

    private void CompressFile_Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            //scelta file(files) da comprimere
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "txt files (*.txt)|*.txt|image files (*.jpg;*.png;*.webp)|*.jpg;*.png;*.webp|All files (*.*)|*.*";
            file.Multiselect = true;

            Nullable<bool> resultF = file.ShowDialog();
            if (resultF == true)     //utente seleziona 'apri'
            {
                string[] file_path = file.FileNames;
                for (int i = 0; i < file_path.Length; i++)
                {
                    MessageBox.Show("File da comprimere " + file_path[i]);  //.GetValue(i)
                }

                var archive_path = System.IO.Path.ChangeExtension(file.FileName, ".zip");
                MessageBox.Show("nome archivio " + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ".zip");

                using ZipFile zip = new();

                var list = new List<Filewithpsw>();
                foreach (var f in file_path)
                {
                    list.Add(new Filewithpsw()
                    {
                        Elm = f,
                        Psw = "",
                        Dc = "",
                        File = zip
                    });
                }

                MessageBox.Show("puoi inserire delle password");
                new Password(zip, list, archive_path).ShowDialog();

                MessageBox.Show("Compressione riuscita");
                //BUTTA FINESTRA ELEMENTI

            }
        }
        catch (Exception err)
        {
            MessageBox.Show(err.ToString());
        }
    }

    //private void CompressFolder_Button_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        OpenFolderDialog folder = new OpenFolderDialog();
    //        folder.Multiselect = true;
    //        Nullable<bool> resultF = folder.ShowDialog();
    //        if (resultF == true)     //utente seleziona 'apri'
    //        {
    //            string[] folder_path = folder.FolderNames;
    //            for (int i = 0; i < folder_path.Length; i++)
    //            {
    //                MessageBox.Show("Cartella da comprimere " + folder_path.GetValue(i));
    //            }

    //            //MessageBox.Show("Se selezioni più cartelle, l'archivio prenderà il nome della prima in ordine alfabetico");
    //            MessageBox.Show("Creazione archivio " + folder.SafeFolderName + ".zip");

    //            //non ho il caso che non seleziona niente perchè la finestra non si chiude
    //            var archive_path = System.IO.Path.ChangeExtension(folder.FolderName, ".zip");

    //            FileStream fs = File.Create(archive_path);
    //            ZipOutputStream zip = new ZipOutputStream(fs);

    //            var win = new Pass_ZipStream();
    //            var res = win.ShowDialog();
    //            if (res is null) return;
    //            //if (res is true) Class1.Compress(zip, folder_path, win.Result);
    //            //if (res is false) Class1.Compress(zip, folder_path, "");            

    //            MessageBox.Show("Compressione cartella riuscita");
    //        }
    //    }
    //    catch (Exception err)
    //    {
    //        MessageBox.Show(err.ToString());
    //    }
    //}

    private void CompressFolder_Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            OpenFolderDialog folder = new OpenFolderDialog();
            folder.Multiselect = true;
            Nullable<bool> resultF = folder.ShowDialog();
            if (resultF == true)     //utente seleziona 'apri'
            {
                string[] folder_path = folder.FolderNames;
                for (int i = 0; i < folder_path.Length; i++)
                {
                    MessageBox.Show("Cartella da comprimere " + folder_path[i]);
                }

                var archive_path = System.IO.Path.ChangeExtension(folder.FolderName, ".zip");
                MessageBox.Show("nome archivio " + System.IO.Path.GetFileNameWithoutExtension(folder.FolderName) + ".zip");

                using ZipFile zip = new();

                var list = new List<Filewithpsw>();
                foreach (var f in folder_path)
                {                   
                    var files = Directory.GetFiles(f, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        list.Add(new Filewithpsw
                        {
                            Elm = file,
                            Psw = "",
                            Dc = f,  //Path.GetFileName(f) solo nome cartella
                            File = zip 

                        });
                    }
                }

                MessageBox.Show("puoi inserire delle password per i file");
                new Password(zip, list, archive_path).ShowDialog();

                MessageBox.Show("Compressione riuscita");
            }
        }
        catch (Exception err)
        {
            MessageBox.Show(err.ToString());
        }
    }

    //private void Extract_Button_Click(object sender, RoutedEventArgs e)
    //{
    //    //scelta archivio
    //    OpenFileDialog pick = new OpenFileDialog();
    //    pick.Filter = "zip files (*.zip)|*.zip";
    //    Nullable<bool> result_archive = pick.ShowDialog();
    //    if (result_archive ?? false)
    //    {
    //        //in extract devo usare zipfile per poter gestire le entries         
    //        using ZipFile zip = new ZipFile(pick.FileName);

    //        if (zip.Count == 0)
    //        {
    //            MessageBox.Show("l'archivio " + pick.FileName + " è vuoto, non ci sono elementi da estrarre");
    //            return;
    //        }

    //        //new Picker(zip).ShowDialog();
    //    }
    //}

    private void Extract_Button_Click(object sender, RoutedEventArgs e)
    {
        //scelta archivio
        OpenFileDialog pick = new OpenFileDialog();
        pick.Filter = "zip files (*.zip)|*.zip";
        Nullable<bool> result_archive = pick.ShowDialog();
        if (result_archive ?? false)
        {
            using ZipFile zip = ZipFile.Read(pick.FileName);       
            if (zip.Count == 0)
            {
                MessageBox.Show("l'archivio " + pick.FileName + " è vuoto, non ci sono elementi da estrarre");
                return;
            }

            ObservableCollection<Filewithpsw> list = new ObservableCollection<Filewithpsw>();
            foreach ( var entry in zip.Entries.Where(entry => !entry.IsDirectory))
            {
                list.Add(new Filewithpsw
                {
                    Elm = entry.FileName, Psw = "", Dc = "", ArchiveEntry = entry, File = zip
                });
            }

            foreach (var entry in zip.Entries.Where(entry => entry.IsDirectory))
            {
                //metto cartella vuota
                list.Add(new Filewithpsw
                {
                    Elm = entry.FileName,
                    Psw = "",
                    Dc = Path.GetRelativePath(Directory.GetCurrentDirectory(), entry.FileName),
                    ArchiveEntry = entry,
                    File = zip
                });
            }


            new Picker(zip, list, pick.FileName).ShowDialog();  
        }
    }

    private void AddFile_Button_Click(object sender, RoutedEventArgs e) 
    {        
        try
        {
            //scelta file
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "txt files (*.txt)|*.txt|image files (*.jpg;*.png;*.webp)|*.jpg;*.png;*.webp|All files (*.*)|*.*";
            file.Multiselect = true;

            Nullable<bool> resultF = file.ShowDialog();
            if (resultF == true)     
            {
                string[] file_path = file.FileNames;
                for (int i = 0; i < file_path.Length; i++)
                {
                    MessageBox.Show("File da aggiungere " + file_path[i]);  
                }

                //scelta archivio
                OpenFileDialog pick = new OpenFileDialog();
                pick.Filter = "zip files (*.zip)|*.zip";
                Nullable<bool> result_archive = pick.ShowDialog();
                if (result_archive ?? false)
                {
                    var archive_path = System.IO.Path.ChangeExtension(pick.FileName, ".zip");
                    MessageBox.Show("nome archivio " + System.IO.Path.GetFileNameWithoutExtension(pick.FileName) + ".zip");

                    ZipFile zip = ZipFile.Read(archive_path);

                    var list = new List<Filewithpsw>();
                    foreach (var f in file_path)
                    {
                        list.Add(new Filewithpsw
                        {
                            Elm = f,
                            Psw = "",
                            Dc = "",
                            File = zip
                        });
                    }

                    MessageBox.Show("puoi inserire delle password");
                    new Password(zip, list, archive_path).ShowDialog();

                    MessageBox.Show("File aggiunti correttamente");
                }

            }
        }
        catch (Exception err)
        {
            MessageBox.Show(err.ToString());
        }
    }

    private void AddFolder_Button_Click(object sender, RoutedEventArgs e) 
    {
        //scelta cartelle
        OpenFolderDialog op = new OpenFolderDialog();
        op.Multiselect = true;
        Nullable<bool> result = op.ShowDialog();
        if (result ?? false) 
        {
            string[] folder_path = op.FolderNames;
            for (int i = 0; i < folder_path.Length; i++)
            {
                MessageBox.Show("File da aggiungere " + folder_path[i]);
            }

            //scelta archivio
            OpenFileDialog pick = new OpenFileDialog();
            pick.Filter = "zip files (*.zip)|*.zip";
            Nullable<bool> result_archive = pick.ShowDialog();
            if (result_archive ?? false)
            {
                var archive_path = System.IO.Path.ChangeExtension(pick.FileName, ".zip");
                MessageBox.Show("nome archivio " + System.IO.Path.GetFileNameWithoutExtension(pick.FileName) + ".zip");

                ZipFile zip = ZipFile.Read(archive_path);

                var list = new List<Filewithpsw>();
                foreach (var f in folder_path)
                {
                    var files = Directory.GetFiles(f, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        list.Add(new Filewithpsw
                        {
                            Elm = file,
                            Psw = "",
                            Dc = f,
                            File = zip
                        });
                    }
                }

                MessageBox.Show("puoi inserire delle password");
                new Password(zip, list, archive_path).ShowDialog();

                MessageBox.Show("File aggiunti correttamente");
            }
        }
    }

    private void Aggiorna_Click(object sender, RoutedEventArgs e)
    {
        //scelta archivio
        OpenFileDialog op = new OpenFileDialog();
        op.Filter = "zip files (*.zip)|*.zip";
        Nullable<bool> result = op.ShowDialog();
        if (result ?? false)
        {
            //apro archivio in lettura e mostro entries
            using ZipFile zip = ZipFile.Read(op.FileName);
            if(zip.Count == 0) 
            {
                MessageBox.Show("l'archivio " + op.FileName + " è vuoto, non ci sono elementi da estrarre");
                return;
            }

            ObservableCollection<Filewithpsw> list = new ObservableCollection<Filewithpsw>();
            foreach (var entry in zip.Entries.Where(x => !x.IsDirectory))
            {
                list.Add(new Filewithpsw
                {
                    Elm = entry.FileName,
                    Psw = "",
                    Dc = "",
                    ArchiveEntry = entry,
                    File = zip
                });
            }

            new Picker(zip, list, op.FileName).ShowDialog();
        }
    }

    private void Togli_Click(object sender, RoutedEventArgs e)
    {
        //scelta archivio
        OpenFileDialog op = new OpenFileDialog();
        op.Filter = "zip files (*.zip)|*.zip";
        Nullable<bool> result = op.ShowDialog();
        if (result ?? false)
        {
            //apro archivio in lettura e mostro entries
            using ZipFile zip = ZipFile.Read(op.FileName);
            if (zip.Count == 0)
            {
                MessageBox.Show("l'archivio " + op.FileName + " è vuoto, non ci sono elementi da estrarre");
                return;
            }

            ObservableCollection<Filewithpsw> list = new ObservableCollection<Filewithpsw>();
            foreach (var entry in zip.Entries.Where(x => !x.IsDirectory))
            {
                list.Add(new Filewithpsw
                {
                    Elm = entry.FileName,
                    Psw = "",
                    Dc = "",
                    ArchiveEntry = entry,
                    File = zip
                });
            }

            new Picker(zip, list, op.FileName).ShowDialog();
        }
    }


}

