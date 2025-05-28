namespace classi;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using System.Reflection;
using System.ComponentModel;
using Ionic.Zip;
using System.Security.Cryptography.X509Certificates;

public static class Class1
{
    
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


