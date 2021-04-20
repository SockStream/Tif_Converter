using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tif_Converter
{
    class Program
    {
        private static string destdir = string.Empty;
        private static string path_texture_converter = string.Empty;

        static void Main(string[] args)
        {
            Console.WriteLine(args.Length);
            if (args.Length < 3)
            {
                Console.WriteLine("Il faut passer en paramètre le chemin du répertoire Data, le chemin de l'executable et le chemin du dossier temporaire");
                return;
            }

            destdir = args[2];
            path_texture_converter = args[1];
            DirSearch_ex3(args[0]);
        }

        static void DirSearch_ex3(string sDir)
        {
            Console.WriteLine(sDir);
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    /*if (!f.Contains(@"Objects\Spaceships"))
                    {
                        continue;
                    }*/

                    /*if (!f.Contains(@"Data\Objects\Spaceships") && !f.Contains(@"Data\Textures"))
                    {
                        continue;
                    }*/

                    FileInfo fileInfo = new FileInfo(f);
                    if (fileInfo.Extension.Equals(".dds") && !File.Exists(fileInfo.FullName.Replace(".dds", ".TIF")))
                    {
                        Convertir_fichier(f);
                    }
                }

                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch_ex3(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        /*private static void LireFichier(string f)
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(f);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(".tif"))
                {
                    int debut = line.IndexOf(">");
                    int end = line.LastIndexOf("<");
                    int lenght = end - debut;
                    string value = line.Substring(debut + 2, lenght - 2);
                    Console.WriteLine(value);
                    if (!liste_dds.Contains(value))
                    {
                        Convertir_fichier(value);
                        liste_dds.Add(value);
                    }
                }

            }
        }*/

        private static void Convertir_fichier(string value)
        {
            DirectoryInfo parent = Directory.GetParent(value);
            //Console.WriteLine(parent.FullName);
            string dds = Path.GetFileNameWithoutExtension(value) + ".dds";
            DirectoryInfo di = new DirectoryInfo(destdir);
            //on purge le dossier
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (string f in Directory.GetFiles(parent.FullName))
            {
                if (f.Contains(dds))
                {
                    FileInfo finfo = new FileInfo(f);
                    string name = finfo.Name;
                    File.Move(f, destdir + name);
                }
            }
            try
            {
                File.Move(destdir + dds, destdir + dds + ".0");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error");
                Console.WriteLine(e.Message);
                return;
            }
            if (File.Exists(destdir + dds + ".a"))
            {
                File.Move(destdir + dds + ".a", destdir + dds + ".0a");
            }

            Convert_Files(destdir);

            foreach (string f in Directory.GetFiles(destdir))
            {
                FileInfo finfo = new FileInfo(f);
                if (finfo.Extension.ToUpper().Equals(".TIF") || finfo.Extension.ToUpper().Equals(".DDS"))
                {
                    string name = finfo.Name;
                    //DEBUG
                    if (File.Exists(parent.FullName + "\\" + name))
                    {
                        File.Delete(parent.FullName + "\\" + name);
                    }
                    File.Move(f, parent.FullName + "\\" + name);
                }
            }
        }

        private static void Convert_Files(string directory)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = path_texture_converter;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = directory;
            startInfo.RedirectStandardOutput = true;

            try
            {
                // Start the process with the info we specified.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    DateTime depart = DateTime.Now;
                    bool done = false;
                    while (!done)
                    {
                        exeProcess.StandardOutput.Peek();
                        string test = exeProcess.StandardOutput.ReadLine();
                        //Console.WriteLine(test);
                        if (test == null || test.Contains("Log file created:") || test.Contains("EXITING.") || ((DateTime.Now - depart).TotalSeconds > 60))
                        {
                            done = true;
                        }
                        System.Threading.Thread.Sleep(10);
                    }
                    exeProcess.Kill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
