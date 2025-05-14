using System.Windows;
using classi;
using Microsoft.Win32;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;

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
          
                MessageBox.Show("Creazione archivio " + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ".zip");

                //non ho il caso che non seleziona niente perchè la finestra non si chiude senza aver selezionato qualcosa
                var archive_path = System.IO.Path.ChangeExtension(file.FileName, ".zip");

                //if (File.Exists(archive_path)) File.Delete(archive_path); //sovrascrive

                FileStream fs = File.Create(archive_path);
                using ZipOutputStream zip = new ZipOutputStream(fs);

                var win = new Pass_ZipStream();
                var res = win.ShowDialog();
                if (res is null) return;
                if (res is true) Class1.Compress(zip, file_path, win.Result);             
                if (res is false) Class1.Compress(zip, file_path, "");
 
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

                FileStream fs = File.Create(archive_path);
                ZipOutputStream zip = new ZipOutputStream(fs);

                var win = new Pass_ZipStream();
                var res = win.ShowDialog();
                if (res is null) return;
                if (res is true) Class1.Compress(zip, folder_path, win.Result);
                if (res is false) Class1.Compress(zip, folder_path, "");            

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
            //in extract devo usare zipfile per poter gestire le entries         
            using ZipFile zip = new ZipFile(pick.FileName);
            
            if (zip.Count == 0)
            {
                MessageBox.Show("l'archivio " + pick.FileName + " è vuoto, non ci sono elementi da estrarre");
                return;
            }
            
            new Picker(zip).ShowDialog();
        }
    }


}

