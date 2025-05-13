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
    }


}

