using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace RDL
{
    public class Files
    {
        public Files()
        {

        }

        /*
         DirectoryInfo sourcedinfo = new DirectoryInfo(@"E:\source");
        DirectoryInfo destinfo = new DirectoryInfo(@"E:\destination");
        copy.CopyAll(sourcedinfo ,destinfo);     
         */


        public static bool CopyFullDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            bool res = false;
            try
            {
                //check if the target directory exists
                if (Directory.Exists(target.FullName) == false)
                {
                    Directory.CreateDirectory(target.FullName);
                }

                //copy all the files into the new directory

                foreach (FileInfo fi in source.GetFiles())
                {
                    fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                }


                //copy all the sub directories using recursion

                foreach (DirectoryInfo diSourceDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
                    CopyFullDirectory(diSourceDir, nextTargetDir);
                }
                //success here
                res = true;
            }
            catch (IOException ie)
            {
                res = false;
                //handle it here
            }
            return res;
        }

        public static string GetFileContent(string path)
        {
            string res = "";
            try
            {
                string p = HttpContext.Current.Server.MapPath(path);
              
                string d = Path.GetDirectoryName(p);
                if (File.Exists(p))
                {
                    using (StreamReader sr = File.OpenText(p))
                    {
                        res = sr.ReadToEnd();
                    }
                }                
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        public static void DeleteDirectoryFiles(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
        }

        public static List<FileItemInfo> DirSearch(string dir)
        {
            var res = new List<FileItemInfo>();
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    res.Add(new FileItemInfo { Name = Path.GetFileName(d), URL = GetAbsolutePathFromPhysical(d), IsFolder = true,  Items= DirSearch(d) });
                }
                foreach (string f in Directory.GetFiles(dir))
                {
                    res.Add(new FileItemInfo { Name = Path.GetFileName(f), URL = GetAbsolutePathFromPhysical(f), IsFolder = false,  Items = new List<FileItemInfo>() });
                }                    
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return res;
        }


        public static string GetAbsolutePathFromPhysical(string path) {
            var res = "";
            res = ("\\" + path.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty)).Replace("\\", "/");
            return res;
        }

        public static List<FileItemInfo1> GetTargetFiles(int id, string toGetdirectory)
        {
            var items = new List<FileItemInfo1>();
            var path = GetTargetFilesPath(id, toGetdirectory);
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);

                items = files.Select(x => new FileItemInfo1 { Name = Path.GetFileName(x) }).ToList();
            }
            return items;
        }

        static string GetTargetFilesPath(int id, string toGetdirectory)
        {
            var res = "";
            var d = toGetdirectory + id;
            var path = HttpContext.Current.Server.MapPath(d);
            res = path;
            return res;
        }

        public class FileItemInfo
        {
            public string Name { set; get; }
            public string URL { set; get; }
            public bool IsFolder { set; get; }
            public List<FileItemInfo> Items { set; get; }
        }
    }
    public class FileItemInfo1
    {
        public string Name { set; get; }
        public string URL { set; get; }
        public bool IsFolder { set; get; }
        public List<FileItemInfo1> Items { set; get; }
    }
}