namespace classi;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;

public static class Class1
{
    public static void Compress(ZipArchive archive, IEnumerable<string> list)
    {
        foreach (var item in list)
        {
            if (File.Exists(item))
            {
                // file
                archive.CreateEntryFromFile(item, Path.GetFileName(item));
            }
            else if (Directory.Exists(item))
            {
                // cartella
                var files = Directory.GetFiles(item, "*", System.IO.SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var relative = Path.Combine(Path.GetFileName(item), Path.GetRelativePath(item, file));
                    archive.CreateEntryFromFile(file, relative);
                }

                var dirs = Directory.GetDirectories(item, "*", System.IO.SearchOption.AllDirectories);
                foreach (var dir in dirs.Where(x => Directory.GetFileSystemEntries(x).Length == 0))
                {
                    archive.CreateEntry($"{Path.Combine(Path.GetFileName(item), Path.GetRelativePath(item, dir))}/");
                }
            }
            else
            {
                //non esiste
                throw new FileNotFoundException($"Un file non è stato trovato: {item}");
            }

        }
    }

    public static void Extract(List<ZipArchiveEntry> list, string destination)
    {
        //ESTRAE TUTTO in archive.Entries
        foreach (ZipArchiveEntry entry in list)
        {
            Debug.WriteLine(entry);
            var relativePath = entry.FullName;
            var directoryPath = Path.GetDirectoryName(relativePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                string fullDirectoryPath = Path.Combine(destination, directoryPath);
                Directory.CreateDirectory(fullDirectoryPath);
            }
            if (!string.IsNullOrEmpty(entry.Name))
            {
                var destinationPath = Path.Combine(destination, relativePath);
                entry.ExtractToFile(destinationPath, overwrite: true);
            }
        }

        //var to_extract = new List<string>();

        //foreach (var elm in list)
        //{
        //    if (elm.EndsWith('/'))
        //    {
        //        var dir = Directory.CreateDirectory(elm);
        //        to_extract.Add(dir.ToString());

        //        //var selectedE = archive.Entries.Where(x => x.FullName.StartsWith(elm))
        //        //                               .Select(x => x.FullName);
        //        //to_extract.AddRange(selectedE);
        //    }
        //    else
        //    {
        //        to_extract.Add(elm);
        //    }

        //}

        //foreach (var e in to_extract)
        //{
        //    var entry = archive.Entries.FirstOrDefault(x => x.Name == e);
        //}




    }




    public static void Extract(IList<ZipArchiveEntry> list, string extr_path)
    {
        //Attempting to extract explicit directories (entries with names that end in directory separator characters) 
        // will not result in the creation of a directory.
        foreach (var entry in list)
        {
            if (File.Exists(entry.ToString()))
            {
                entry.ExtractToFile(extr_path, overwrite: true);
            }
            else if (Directory.Exists(entry.ToString()))
            {
                //estraggo i file
                var files = Directory.GetFiles(Path.GetFullPath(entry.FullName));
                foreach (var file in files)
                {




                }

                var archive = entry.Archive;
                archive.ExtractToDirectory(extr_path);
            }
            else
            {
                //non è stato selezionato niente
                throw new FileNotFoundException($"Un file non è stato trovato: {entry}");
            }


        }
    }



    public static void AddToArchive(ZipArchive archive, string[] list)
    {
        foreach (var elm in list)
        {
            var file = new FileInfo(elm);
            archive.CreateEntryFromFile(file.FullName, file.Name);
        }

    }

    public static IEnumerable<int> H()
    {
        var i = 0;
        while(true)
        {
            yield return i;
            i++;
        }
    }

    public static void Compress(string[] item_path)    //creo l'archivio nella stessa directory del file/cartella da comprimere e gli lascio il suo nome
    {
        for(var i=0; i<item_path.Length; i++)
        {
            Debug.WriteLine("path elementi " + item_path.GetValue(i));
            
        }

        //se i file sono di più arriveranno comunque tutti dalla stessa cartella quindi la dc è la stessa
        //posso prendere quella di uno solo
        var path_archivio = Path.ChangeExtension(item_path[0], ".zip");
        Debug.WriteLine("path archivio " + path_archivio);

        using ZipArchive archive = ZipFile.Open(path_archivio, ZipArchiveMode.Create);

        //se c'è stata una selezione multipla sono comunque elementi dello stesso tipo(file o cartelle)
        if (File.Exists(item_path[0]))
        { 


            foreach (var item in item_path)
            {
                
                Debug.WriteLine(item);
                
                archive.CreateEntryFromFile(item, Path.GetFileName(item));
            }
        }
        else if (Directory.Exists(item_path[0]))
        {
            //inserisco nell'archivio ricorsivamente cartelle, sottocartelle e file per mantenerne la struttura 
            foreach(var item in item_path)
            {
                var files = Directory.GetFiles(item, "*", System.IO.SearchOption.TopDirectoryOnly);
                if (files.Length != 0)
                {
                    Debug.WriteLine("file");
                    
                    
                }

                Debug.WriteLine("item--> " + item);   //item è una cartella selezionata dall'utente
                //var root = Path.GetDirectoryName(item);
                Debug.WriteLine("item_dc--> " + Path.GetDirectoryName(item));
                    //Debug.WriteLine("item_name--> " + Path.GetFileName(item)); // il nome di item 
                AddFolder(archive, Path.GetDirectoryName(item), item);
                
            }
            
        }
        ////creo archivio con prima cartella         
        //ZipFile.CreateFromDirectory(item_path[0], path_archivio);
        ////apro archivio in modifica
        //ZipArchive ar = ZipFile.Open(path_archivio, ZipArchiveMode.Update);
        ////aggiungi cartelle
        //foreach (var item in item_path)
        //{

        //    var entry = ar.CreateEntry(item + "/");
        //    var stream = entry.Open();  

        //}
    }

    private static void AddFolder(ZipArchive archive, string root, string folder)
    {
       

        foreach(var file in Directory.GetFiles(root, "*", System.IO.SearchOption.AllDirectories)) 
        {
            
            Debug.WriteLine("folder--> " + folder); //cartella selezionata dall'utente
            Debug.WriteLine("root--> " + root); 
            Debug.WriteLine("file--> " + file);
            Debug.WriteLine("path files nella cartella dell'utente--> " + Path.GetRelativePath(root, file));
            Debug.WriteLine("combine--> " + Path.Combine(root, Path.GetRelativePath(root, file)));   
            var percorso_nella_cartella = Path.Combine(root, Path.GetRelativePath(root, file));
            archive.CreateEntryFromFile(file, percorso_nella_cartella);
        }
    }

    public static ZipArchiveEntry[] ShowEntries(string archive_path)
    {
        //apro archivio in lettura
        ZipArchive a = ZipFile.OpenRead(archive_path);
        //ottengo le entries
        var entries = a.Entries;
        //controllo
        foreach(var entry in entries)
        {
            if (File.Exists(entry.FullName))
            {
                Debug.WriteLine("file di testo\n");
                return entries.ToArray();
            }
            else if (Directory.Exists(entry.FullName))
            {
                Debug.WriteLine("cartella");
                return entries.ToArray();
            }
            
        }
        return entries.ToArray();
    }

    

    public static void AddToArchive(string source_path)
    {
        try
        {
            //apro l'archivio zip in modalità aggiornamento
            using FileStream fs = new FileStream(source_path, FileMode.Open);
            using ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Update);
            //creo una nuova voce vuota
            ZipArchiveEntry entry = archive.CreateEntry("nuovo.txt");
            try
            {
                //scrivo nella nuova voce 
                using (StreamWriter write_stream = new StreamWriter(entry.Open()))
                {
                    write_stream.WriteLine("Scrivo nella nuova voce\n");
                }
            }
            catch(NullReferenceException e) { Console.WriteLine(e.Message); }
        }
        catch(FileNotFoundException e) { Console.WriteLine(e.Message); }
                
    }

    

    public static void ExtractFile()    //string zip_folder_path, string filename, string dest_path
    {
        //DirectoryInfo folder;
        //folder = Directory.CreateDirectory("C:/Users/MambrucchiC/Desktop/temp");
        //ZipFile.ExtractToDirectory(zip_folder, "C:/Users/MambrucchiC/Desktop/temp");
        ////ZipFile.OpenRead()

        //FileInfo[] file_ToExtract = folder.GetFiles();
        //FileInfo file = file_ToExtract.First();     //è l'unico solo dopo aver fatto 'CompressFile', se come parametro arriva una CARTELLA compressa l'utente deve scegliere il file
        //string file_path = file.ToString();

        //try
        //{
        //    File.Move(file_path, "C:/Users/MambrucchiC/Desktop/file_fuori.txt");  //"C:/Users/MambrucchiC/Desktop/temp/nuovo.txt", nome da far scegliere
        //}
        //catch (IOException) { }
        //folder.Delete(true);
        //l'archivio compresso resta 



        try
        {
            //apro archivio in lettura
            using ZipArchive folder = ZipFile.OpenRead("C:/Users/MambrucchiC/Desktop/out.zip"); //zip_folder_path
            if (folder == null) { return; }

            try
            {
                //trovo la voce da estrarre    
                var file = folder.GetEntry("C:/Users/MambrucchiC/Desktop/temp/test.txt"); //filename
                if (file == null) { return; }
                //estraggo la voce 
                try
                {
                    file.ExtractToFile("C:/Users/MambrucchiC/Desktop"); //dest_path
                }
                catch (IOException e) { Console.WriteLine("eccezione" + e); }   //voce eliminata dall'archivio prima di poter essere estratta
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }    //stringa vuota
        }
        catch (FileNotFoundException e) { Console.WriteLine(e.Message); }
        catch (IOException e) { Console.WriteLine(e.Message); }

    }

    public static void ExtractFolder(string folder_ToExtract)  //string zip_path, string dest_path
    {
        ZipFile.ExtractToDirectory(folder_ToExtract, "C:/Users/MambrucchiC/Desktop/prova_testE");
    }


    //public static void CompressFile(string file_ToCompress)
    //{

    //    DirectoryInfo temp_folder;

    //    //creo cartella temporanea
    //    temp_folder = Directory.CreateDirectory("C:/Users/MambrucchiC/Desktop/temp");   //non devo controllare se esiste già perchè viene eliminata ogni volta

    //    //metto file in cartella
    //    try
    //    {
    //        File.Move(file_ToCompress, "C:/Users/MambrucchiC/Desktop/temp/nuovo.txt");      //file rinominato
    //        //comprimo cartella
    //        ZipFile.CreateFromDirectory("C:/Users/MambrucchiC/Desktop/temp", "C:/Users/MambrucchiC/Desktop/output2.zip");
    //        //cancello cartella temp
    //        temp_folder.Delete(true);      //true per eliminarla anche se contiene un file
    //    }
    //    catch (IOException) { } //file esiste già nella cartella


    //    file_ToCompress = @"C:\Users\MambrucchiC\Downloads\a\c.txt";

    //    using ZipArchive ar = ZipFile.Open("C:/Users/MambrucchiC/Desktop/stream.zip", ZipArchiveMode.Create);
    //    ar.CreateEntryFromFile(file_ToCompress, Path.GetFileName(file_ToCompress));


    //    try
    //    {
    //        //apro file in lettura
    //        using FileStream fs = File.Create("C:/Users/MambrucchiC/Desktop/stream.zip");
    //        try
    //        {
    //            //creo archivio
    //            using ZipArchive ar = new ZipArchive(fs, ZipArchiveMode.Create);
    //            try
    //            {

    //                //aggiungo il file
    //                ar.CreateEntryFromFile(file_ToCompress, Path.GetFileName(file_ToCompress));
    //            }
    //            catch (ArgumentNullException e) { Console.WriteLine(e.Message); }
    //            catch (IOException) { }  //file usato da un altro processo
    //        }
    //        catch (ArgumentException e) { Console.WriteLine(e.Message); }

    //    }
    //    catch (FileNotFoundException e) { Console.WriteLine(e.Message); }

    //}


    //public static void CompressFolder(string folder_ToCompress)
    //{

    //    //restituisce archivio zip nella destinazione        
    //    try
    //    {
    //        Debug.WriteLine(folder_ToCompress);
    //        //File.SetAttributes(folder_ToCompress, (new FileInfo(folder_ToCompress)).Attributes | FileAttributes.ReadOnly);
    //        ZipFile.CreateFromDirectory(folder_ToCompress, folder_ToCompress + ".zip");
    //    }
    //    catch (IOException e) { Console.WriteLine(e.Message); }    //l'archivio esiste già
    //    catch (UnauthorizedAccessException e) { Console.WriteLine(e.Message); }
    //}


}

