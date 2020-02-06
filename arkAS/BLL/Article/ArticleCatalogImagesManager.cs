using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Article
{
    public class ArticleCatalogImagesManager
    {
        public List<ImageFileInfo> GetTargetFiles(int id, string toGetdirectory)
        {
            var items = new List<ImageFileInfo>();
            var path = GetTargetFilesPath(id, toGetdirectory);
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);

                items = files.Select(x => new ImageFileInfo { Name = Path.GetFileName(x) }).ToList();
            }
            return items;
        }

        string GetTargetFilesPath(int id, string toGetdirectory)
        {
            var res = "";
            var d = toGetdirectory + id;
            var path = HttpContext.Current.Server.MapPath(d);
            res = path;
            return res;
        }
    }
}