namespace classi;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.GZip;


public static class Class1
{
    public static void Compress(ZipOutputStream archive, IEnumerable<string> list, string password)
    {    
        if (!string.IsNullOrEmpty(password))   
        {
            //la password si applica solo alle entries
            archive.Password = password;
          
        }

        foreach (var item in list)
        {
            if (File.Exists(item))
            {
                // file
                FileInfo fileInfo = new FileInfo(item);
                ZipEntry entry = new ZipEntry(fileInfo.Name)
                {
                    DateTime = fileInfo.LastWriteTime,
                    Size = fileInfo.Length
                    
                };
                
                archive.PutNextEntry(entry);

                byte[] buffer = new byte[4096];
                using (FileStream fileStream = File.OpenRead(item))
                {
                    {
                        int bytesRead;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            archive.Write(buffer, 0, bytesRead);
                        }
                    }
                    archive.CloseEntry();
                }

            }
            else if (Directory.Exists(item))
            {              
                AddDirectory(archive, item, "");
            }
            else
            {
                //non esiste
                throw new FileNotFoundException($"Un file non è stato trovato: {item}");
            }

        }
        archive.Finish();
        archive.Close();
    }

    private static void AddDirectory(ZipOutputStream zipStream, string sourceDir, string parentPath)
    {
        string[] files = Directory.GetFiles(sourceDir);
        string[] directories = Directory.GetDirectories(sourceDir);

        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            string entryName = Path.Combine(parentPath, fileInfo.Name);

            ZipEntry entry = new ZipEntry(entryName)
            {
                DateTime = fileInfo.LastWriteTime,
                Size = fileInfo.Length
            };

            zipStream.PutNextEntry(entry);

            using (FileStream fileStream = File.OpenRead(file))
            {
                fileStream.CopyTo(zipStream);
            }

            zipStream.CloseEntry();
        }

        foreach (string directory in directories)
        {
            string directoryName = Path.Combine(parentPath, Path.GetFileName(directory));
            AddDirectory(zipStream, directory, directoryName);
        }
    }

    public static void Extract(ZipFile archive, List<ZipEntry> list, string destination, string password)
    {
        Debug.WriteLine("metodo estrai");

        if (password != "")  
        {
            archive.Password = password;
        }

        foreach (ZipEntry entry in list)
        {
            Debug.WriteLine(entry);

            var full_path = Path.Combine(Directory.GetCurrentDirectory(), entry.Name);
            if (entry.IsDirectory)
            {
                Directory.CreateDirectory(entry.Name);
            }            

            var dir = Directory.Exists(full_path);
            if (!dir)
            {
                Directory.CreateDirectory(full_path);
            }

            var destinationPath = Path.Combine(destination, entry.Name);

            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

            //estrae file
            using (Stream zipStream = archive.GetInputStream(entry))               
            using (FileStream outputStream = File.Create(destinationPath))
            {
                zipStream.CopyTo(outputStream);
            }
        }
    }
}

