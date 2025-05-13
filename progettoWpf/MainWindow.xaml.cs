using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using classi;
using Microsoft.Win32;
using System.IO;
using System.IO.Enumeration;
using System.IO.Compression;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using System.DirectoryServices;
using System.IO.Pipes;

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
                    MessageBox.Show("File da comprimere " + file_path.GetValue(i));
                }

                //MessageBox.Show("Se selezioni più file, l'archivio prenderà il nome del primo in ordine alfabetico");
                MessageBox.Show("Creazione archivio " + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ".zip");

                //non ho il caso che non seleziona niente perchè la finestra non si chiude senza aver selezionato qualcosa
                var archive_path = System.IO.Path.ChangeExtension(file.FileName, ".zip");
                if (File.Exists(archive_path)) File.Delete(archive_path);
                using ZipArchive archive = ZipFile.Open(archive_path, ZipArchiveMode.Create);
               
                Class1.Compress(archive, file_path);
                MessageBox.Show("Compressione file riuscita");
            }
        }
        catch (Exception err)
        {
            MessageBox.Show(err.ToString());
        }

    }

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
                    MessageBox.Show("Cartella da comprimere " + folder_path.GetValue(i));

                }

                //MessageBox.Show("Se selezioni più cartelle, l'archivio prenderà il nome della prima in ordine alfabetico");
                MessageBox.Show("Creazione archivio " + folder.SafeFolderName + ".zip");

                //non ho il caso che non seleziona niente perchè la finestra non si chiude
                var archive_path = System.IO.Path.ChangeExtension(folder.FolderName, ".zip");
                if (File.Exists(archive_path)) File.Delete(archive_path);
                using ZipArchive archive = ZipFile.Open(archive_path, ZipArchiveMode.Create);

                Class1.Compress(archive, folder_path);
                MessageBox.Show("Compressione cartella riuscita");
            }
        }
        catch (Exception err)
        {
            MessageBox.Show(err.ToString());
        }

    }

    private void Extract_Button_Click(object sender, RoutedEventArgs e)
    {
        //scelta archivio
        OpenFileDialog pick = new OpenFileDialog();
        pick.Filter = "zip files (*.zip)|*.zip";
        Nullable<bool> result_archive = pick.ShowDialog();
        if (result_archive ?? false)
        {
            FileStream zipStream = new FileStream(pick.FileName, FileMode.Open, FileAccess.Read);
            ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
         
            //così l'archivio si chiude e perdo il riferimento prima di poter usare le entries selezionate-->devo usare uno stream 
            //using var archive = ZipFile.OpenRead(pick.FileName);

            //non faccio aprire la finestra se l'archivio è vuoto
            if(archive.Entries.Count == 0)
            {
                MessageBox.Show("l'archivio " + pick.FileName + " è vuoto, non ci sono elementi da estrarre");
                return;
            }
            new Picker(archive).ShowDialog();

            //chiudo manualmente stream e archivio
            zipStream.Close();
            archive.Dispose();
  

        }

    }

    private void AddFile_Button_Click(object sender, RoutedEventArgs e)
    {
        //scelta file da aggiungere
        MessageBox.Show("scegli file da aggiungere a un archivio");
        OpenFileDialog o = new OpenFileDialog();
        o.Multiselect = true;
        o.Filter = "txt files (*.txt)|*.txt|image files (*.jpg;*.png;*.webp)|*.jpg;*.png;*.webp|All Files (*.*)|*.*";
        Nullable<bool> result = o.ShowDialog();
        if( result ?? false)
        {
            string[] file_toAdd = o.FileNames;
            //scelta archivio
            MessageBox.Show("scegli archivio in cui comprimere");
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "(*.zip)|*.zip";
            Nullable<bool> result_op = op.ShowDialog();
            if(result_op ?? false)
            {
                var archive_path = System.IO.Path.ChangeExtension(op.FileName, ".zip");
                using ZipArchive archive = ZipFile.Open(archive_path, ZipArchiveMode.Update);
                Class1.Compress(archive, file_toAdd);
                MessageBox.Show("Elementi aggiunti");
            }
        }
        
    }

    private void AddFolder_Button_Click(object sender, RoutedEventArgs e)
    {
        //scelta cartella da aggiungere
        MessageBox.Show("scegli cartella da aggiungere a un archivio");
        OpenFolderDialog o = new OpenFolderDialog();
        o.Multiselect = true;
        
        Nullable<bool> result = o.ShowDialog();
        if (result ?? false)
        {
            string[] folder_toAdd = o.FolderNames;
            //scelta archivio
            MessageBox.Show("scegli archivio in cui comprimere");
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "(*.zip)|*.zip";
            Nullable<bool> result_op = op.ShowDialog();
            if (result_op ?? false)
            {
                var archive_path = System.IO.Path.ChangeExtension(op.FileName, ".zip");
                using ZipArchive archive = ZipFile.Open(archive_path, ZipArchiveMode.Update);
                Class1.Compress(archive, folder_toAdd);
                MessageBox.Show("Elementi aggiunti");
            }
        }

    }

}

