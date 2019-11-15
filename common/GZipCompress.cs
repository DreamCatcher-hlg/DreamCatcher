using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace MyCommon.common
{
    public class GZipCompress
    {
        private const long BUFFER_SIZE = 20480;
        static string path =@"C:\Users\nuosiyun\Desktop\dz.rar" ;//AppDomain.CurrentDomain.BaseDirectory + "\\test.zip";
        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="dir"></param>
        public static void Compress(DirectoryInfo dir)
        {
           // System.IO.Compression.ZipArchive
            //System.IO.DirectoryInfo directory= System.IO.Directory.CreateDirectory(dir.FullName + "\\test");
            ZipFile.CreateFromDirectory(dir.FullName, AppDomain.CurrentDomain.BaseDirectory + "\\test.zip");
            //foreach (FileInfo fileToCompress in dir.GetFiles())
            //{
            //    Compress(fileToCompress);
            //}
            //ZipArchiveEntry entry = 
        }
        public static void DeCompress(FileInfo file)
        {
            file = new FileInfo(path);
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        entry.ExtractToFile(entry.FullName);
                    }
                }
            }
            file = new FileInfo(path);
            if (File.Exists(file.FullName))
            {
                ZipFile.ExtractToDirectory(file.FullName, AppDomain.CurrentDomain.BaseDirectory + "\\" + file.Name.Substring(0, file.Name.IndexOf(".")));
            }
        }
        public static void CompressEx(DirectoryInfo directorySelected)
        {
            foreach (FileInfo fileToCompress in directorySelected.GetFiles())
            {
                using (FileStream originalFileStream = fileToCompress.OpenRead())
                {
                    if ((File.GetAttributes(fileToCompress.FullName) &
                       FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                    {
                        using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                               CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);

                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 解压缩目录
        /// </summary>
        /// <param name="dir"></param>
        public static void Decompress(DirectoryInfo dir)
        {
            foreach (FileInfo fileToCompress in dir.GetFiles())
            {
                Decompress(fileToCompress);
            }
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToCompress"></param>
        public static void Compress(FileInfo fileToCompress)
        {
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="fileToDecompress"></param>
        public static void Decompress(FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
        }


        public static void Compress(string filepath, string desfilepath)
        {
            
            string sourcepath = filepath;
            Queue<FileSystemInfo> Folders = new Queue<FileSystemInfo>(new DirectoryInfo(sourcepath).GetFileSystemInfos());
            string copytopath = desfilepath;
            copytopath = (copytopath.LastIndexOf(Path.DirectorySeparatorChar) == copytopath.Length - 1) ? copytopath : copytopath + Path.DirectorySeparatorChar + Path.GetFileName(sourcepath);
            Directory.CreateDirectory(copytopath);
            while (Folders.Count > 0)
            {
                FileSystemInfo atom = Folders.Dequeue();
                FileInfo sourcefile = atom as FileInfo;
                if (sourcefile == null)
                {
                    DirectoryInfo directory = atom as DirectoryInfo;
                    Directory.CreateDirectory(directory.FullName.Replace(sourcepath, copytopath));
                    foreach (FileSystemInfo nextatom in directory.GetFileSystemInfos())
                        Folders.Enqueue(nextatom);
                }
                else
                {
                    string sourcefilename = sourcefile.FullName;
                    string zipfilename = sourcefile.FullName.Replace(sourcepath, copytopath) + ".zip";
                    if (!File.Exists(zipfilename))
                    {
                        FileStream sourceStream = null;
                        FileStream destinationStream = null;
                        GZipStream compressedStream = null;
                        try
                        {
                            // Read the bytes from the source file into a byte array
                            sourceStream = new FileStream(sourcefilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                            // Open the FileStream to write to
                            destinationStream = new FileStream(zipfilename, FileMode.OpenOrCreate, FileAccess.Write);
                            // Create a compression stream pointing to the destiantion stream
                            compressedStream = new GZipStream(destinationStream, CompressionMode.Compress, true);
                            long bufferSize = sourceStream.Length < BUFFER_SIZE ? sourceStream.Length : BUFFER_SIZE;
                            byte[] buffer = new byte[bufferSize];
                            int bytesRead = 0;
                            long bytesWritten = 0;
                            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                compressedStream.Write(buffer, 0, bytesRead);
                                bytesWritten += bufferSize;
                            }
                        }
                        catch (ApplicationException)
                        {
                            continue;
                        }
                        finally
                        {
                            // Make sure we allways close all streams
                            if (sourceStream != null) sourceStream.Close();
                            if (compressedStream != null) compressedStream.Close();
                            if (destinationStream != null) destinationStream.Close();
                        }
                    }
                }
            }
        }

    }
}
