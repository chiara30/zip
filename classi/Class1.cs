namespace classi;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using System.Reflection;
//using ICSharpCode.SharpZipLib.Zip;
using System.ComponentModel;
using Ionic.Zip;
using System.Security.Cryptography.X509Certificates;

public static class Class1
{
    //public static void Compress(ZipOutputStream archive, IEnumerable<string> list, string password)
    //{    
    //    if (!string.IsNullOrEmpty(password))   
    //    {
    //        //la password si applica solo alle entries
    //        archive.Password = password;

    //    }

    //    foreach (var item in list)
    //    {
    //        if (File.Exists(item))
    //        {
    //            // file
    //            FileInfo fileInfo = new FileInfo(item);
    //            ZipEntry entry = new ZipEntry(fileInfo.Name)
    //            {
    //                DateTime = fileInfo.LastWriteTime,
    //                Size = fileInfo.Length

    //            };

    //            archive.PutNextEntry(entry);

    //            byte[] buffer = new byte[4096];
    //            using (FileStream fileStream = File.OpenRead(item))
    //            {
    //                {
    //                    int bytesRead;
    //                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
    //                    {
    //                        archive.Write(buffer, 0, bytesRead);
    //                    }
    //                }
    //                archive.CloseEntry();
    //            }

    //        }
    //        else if (Directory.Exists(item))
    //        {              
    //            AddDirectory(archive, item, "");
    //        }
    //        else
    //        {
    //            //non esiste
    //            throw new FileNotFoundException($"Un file non è stato trovato: {item}");
    //        }

    //    }
    //    archive.Finish();
    //    archive.Close();
    //}

    
    public static void Compress(ZipFile zip, IEnumerable<Filewithpsw> files, string zip_path)    
    {       
        var empty = false;

        foreach (var file in files)
        {
            if (File.Exists(file.Elm))
            {
                if (file.Dc != "")
                {
                    //recupero struttura cartella
                    var rel = Path.Combine(Path.GetFileName(file.Dc), Path.GetRelativePath(file.Dc, file.Elm));
                    var dc = Path.GetDirectoryName(rel);

                    var e = zip.AddFile(file.Elm, dc);
                    
                    if(empty == false) {
                        // cartelle vuote
                        foreach (var folder in Directory.GetDirectories(file.Dc).Where(x => !Directory.EnumerateFileSystemEntries(x).Any()))
                        {                           
                            var relative = Path.Combine(Path.GetFileName(file.Dc), Path.GetRelativePath(file.Dc, folder)) + "/";
                            var dir = Path.GetDirectoryName(relative);
                            
                            zip.AddDirectory(folder, dir);
                            empty = true;
                        }                       
                    }

                    if (!string.IsNullOrEmpty(file.Psw))
                    {
                        e.Password = file.Psw;
                    }
                }
                else
                {                
                    var entry = zip.AddFile(file.Elm, file.Dc);
                    if (!string.IsNullOrEmpty(file.Psw))
                    {
                        entry.Password = file.Psw;
                    }
                }
            }            
            else
            {
                throw new FileNotFoundException($"Un file non è stato trovato: {file}");
            }

        }
        zip.Save(zip_path);

    }

    //private static void AddDirectory(ZipOutputStream zipStream, string sourceDir, string parentPath)
    //{
    //    string[] files = Directory.GetFiles(sourceDir);
    //    string[] directories = Directory.GetDirectories(sourceDir);

    //    foreach (string file in files)
    //    {
    //        FileInfo fileInfo = new FileInfo(file);
    //        string entryName = Path.Combine(parentPath, fileInfo.Name);

    //        ZipEntry entry = new ZipEntry(entryName)
    //        {
    //            DateTime = fileInfo.LastWriteTime,
    //            Size = fileInfo.Length
    //        };

    //        zipStream.PutNextEntry(entry);

    //        using (FileStream fileStream = File.OpenRead(file))
    //        {
    //            fileStream.CopyTo(zipStream);
    //        }

    //        zipStream.CloseEntry();
    //    }

    //    foreach (string directory in directories)
    //    {
    //        string directoryName = Path.Combine(parentPath, Path.GetFileName(directory));
    //        AddDirectory(zipStream, directory, directoryName);
    //    }
    //}

    public static void Extract(List<Filewithpsw> list)
    {
        
    }

    //public static void Extract(ZipFile archive, IEnumerable<ZipEntry> list, string destination, string password)
    //{
    //    Debug.WriteLine("metodo estrai");

    //    if (password != "")
    //    {
    //        archive.Password = password;
    //    }

    //    foreach (ZipEntry entry in list)
    //    {
    //        Debug.WriteLine(entry);

    //        var full_path = Path.Combine(Directory.GetCurrentDirectory(), entry.Name);
    //        if (entry.IsDirectory)
    //        {
    //            Directory.CreateDirectory(entry.Name);
    //        }

    //        var dir = Directory.Exists(full_path);
    //        if (!dir)
    //        {
    //            Directory.CreateDirectory(full_path);
    //        }

    //        var destinationPath = Path.Combine(destination, entry.Name);

    //        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

    //        //estrae file
    //        using (Stream zipStream = archive.GetInputStream(entry))
    //        using (FileStream outputStream = File.Create(destinationPath))
    //        {
    //            zipStream.CopyTo(outputStream);
    //        }
    //    }
    //}

    public static bool CheckPassword(ZipEntry entry, string psw) {
        try
        {
            entry.ExtractWithPassword(Stream.Null, psw);
            return true;
        }
        catch (BadPasswordException)
        {
            return false;
        }
    }

}

public class Filewithpsw
{
    //classe per abbinare elementi alle password
    public required string Elm { get; set; }

    public required string Psw { get; set; }
    //public bool Psw_protected { get; set; }

    public string PswStrana
    {
        get => Psw;
        set     //sostuituisce entry con nuova password
        {
            // OGNI VOLTA CHE VIENE SETTATO
            using var ms = new MemoryStream();

            if (Psw == "")
            {
                // no pass
                ArchiveEntry.Extract(ms);
            }
            else
            {
                // pass
                ArchiveEntry.ExtractWithPassword(ms, Psw);
            }
            ms.Seek(0, SeekOrigin.Begin);
            File.RemoveEntry(ArchiveEntry);
            if (value != "")
            {
                File.Password = value;
            }
            
            ArchiveEntry = File.AddEntry(ArchiveEntry.FileName, ms);
            Psw = value;

            var ar = Path.GetFullPath(File.Name);
            File.Save(ar);
        }
    }

    public required string Dc { get; set; }

    public ZipEntry ArchiveEntry { get; set; }

    public required ZipFile File { get; set; }

    

    public override string ToString() => Elm;
    //quando passo la classe alla finestra Picker visualizza la stringa Elm altrimenti compare solo 'Filewithpsw'
}


