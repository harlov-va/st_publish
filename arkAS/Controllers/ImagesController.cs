using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arkAS.Controllers
{
    public class ImagesController : Controller
    {
        [Authorize]
        public ActionResult GetImages(int itemID, string code)
        {
            var res = false;
            var mng = new ImagesManager();
            List<Object> files = new List<Object>();
            try
            {

                List<as_images> lst = mng.GetImages(itemID, code);

                foreach (var item in lst)
                {                  
                    files.Add(new { originalUrl = mng.GetImageLink(item), thumbUrl = mng.GetImageLink(item, true), item.id, item.url, item.name, item.description });
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
          

            return Json(new
            {
                result = res,
                items = files
            });
        }

        [Authorize]
        public ActionResult DeleteImage(int id)
        {
            var res = false;
            var mng = new ImagesManager();
          
            try
            {
                mng.DeleteImage(id);
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return Json(new
            {
                result = res
            });
        }

        [Authorize]
        public ActionResult UpdateImagesOrder(int itemID, string code, string nums)
        {
            var res = false;
            var mng = new ImagesManager();
          
            try
            {
                mng.UpdateImagesOrder(nums, itemID, code);
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return Json(new
            {
                result = res
            });
        }


        [Authorize]
        public ActionResult SaveImage(int id, string name, string url, string description)
        {
            var res = false;
            var mng = new ImagesManager();

            try
            {
                var img = mng.GetImage(id);
                img.name = name;
                img.url = url;
                img.description = description;
                mng.SaveImage(img);
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return Json(new
            {
                result = res
            });
        }

      
        public ActionResult Upload()
        {
            int itemID = RDL.Convert.StrToInt(HttpContext.Request.Form["itemID"].ToString(), 0);
            string code = HttpContext.Request.Form["code"].ToString();
            int thumbWidth = RDL.Convert.StrToInt(HttpContext.Request.Form["thumbWidth"].ToString(), 0);
            int thumbHeight = RDL.Convert.StrToInt(HttpContext.Request.Form["thumbHeight"].ToString(), 0); 
           
            var mng = new ImagesManager();
            var context = HttpContext;
            try
            {
                var  file = context.Request.Files["Filedata"];
                if (itemID == 0)
                {                    
                    return Content("0");
                }

                if (file != null)
                {
                    var item = new as_images { ord = 20000, itemID = itemID, filename = file.FileName, id = 0, description = "", name = "", url = "", typeCode = code };
                    int id = mng.SaveImage(item);

                    if (file.ContentType.ToLower() == "image/jpeg" || file.ContentType.ToLower() == "image/jpg"
                        || file.ContentType.ToLower() == "image/gif" || file.ContentType.ToLower() == "image/png"
                        || file.ContentType.ToLower() == "application/octet-stream")
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromStream(file.InputStream);
                        mng.UploadImage(item, img, thumbWidth, thumbHeight, file.ContentType.ToLower());
                    }
                }
                return Content("1");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Content("0");
        }

        //public JsonResult EventCropImage(int eventId, int startX, int startY, int width, int height, string eventImgPath)
        //{
        //    var res = false;
        //    var ev = DB.Events.FirstOrDefault(x => x.ID == eventId);

        //    if (ev != null)
        //    {
        //        var img =
        //            new Bitmap(
        //                Bitmap.FromFile(
        //                    HttpContext.Server.MapPath(
        //                        SexiLove.Extensions.Helpers.MyImageResizer.ValidateImagePath(eventImgPath))));

        //        var croppedImage = SexiLove.Extensions.Helpers.MyImageResizer.CropImage(img, startX, startY, width, height);

        //        var serverDirectory = Path.GetDirectoryName(HttpContext.Server.MapPath(eventImgPath)) + "\\";

        //        croppedImage.Save(serverDirectory + "cropped_" + Path.GetFileName(eventImgPath));
        //        res = true;

        //        bool isBigImage = width > 280;

        //        ev.isBigImage = isBigImage;
        //        ev.editorChoseImage = eventImgPath;
        //        DB.SubmitChanges();

        //        img.Dispose();
        //        croppedImage.Dispose();
        //    }

        //    return Json(new
        //    {
        //        result = res
        //    }, JsonRequestBehavior.AllowGet);
        //}
    }
}