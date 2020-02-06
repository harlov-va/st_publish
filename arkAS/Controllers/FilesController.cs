using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using arkAS.BLL;
using arkAS.BLL.Core;
using RDL;
using Convert = RDL.Convert;

namespace arkAS.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private readonly FilesManager _mng;

        public FilesController()
        {
            _mng = new FilesManager();
        }

        public ActionResult GetFiles(int itemID)
        {
            var res = false;
            var files = new List<object>();

            try
            {
                files = _mng.GetFiles(itemID)
                    .Select<as_files, object>(f => new { f.id, f.name }).ToList();
                res = true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return Json(new
            {
                result = res,
                items = files
            });
        }

        public ActionResult Upload()
        {
            try
            {
                var file = HttpContext.Request.Files["Filedata"];
                var itemID = Convert.StrToInt(HttpContext.Request.Form["itemID"], 0);

                var item = new as_files
                {
                    filename = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(file.FileName)),
                    name = file.FileName,
                    itemID = itemID,
                    ord = _mng.GetNextOrder()
                };
                
                _mng.SaveFile(item);
                _mng.UploadFile(item, file);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return Content("0");
        }

        public ActionResult DeleteFile(int id)
        {
            var res = false; 

            try
            {
                _mng.DeleteFile(id);
                res = true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return Json(new
            {
                result = res
            });
        }

        public ActionResult UpdateFilesOrder(int itemID, string type, string ids)
        {
            var res = false;

            try
            {
                _mng.UpdateFilesOrder(itemID, type, ids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse));
                res = true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return Json(new
            {
                result = res
            });
        }

        public ActionResult GetFile(int id)
        {
            var file = _mng.GetFile(id);
            if (CanViewInBrowser(Path.GetExtension(file.name)))
            {
                return File(Path.Combine(_mng.GetFileFolderLink(file), file.filename), "application/unknown");
            }
            return File(Path.Combine(_mng.GetFileFolderLink(file), file.filename), "application/unknown", file.name);
        }

        private bool CanViewInBrowser(string ext)
        {
            switch (ext)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".txt":
                    return true;
                default:
                    return false;
            }
        }
    }
}