using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using RDL;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace arkAS.BLL.Core
{
    public class ImagesManager
    {
          #region System
        public LocalSqlServer db;
        private bool _disposed;

        public ImagesManager()
        {
            db = new LocalSqlServer();
            _disposed = false;
            //SERIALIZE WILL FAIL WITH PROXIED ENTITIES
            //dbContext.Configuration.ProxyCreationEnabled = false;
            //ENABLING COULD CAUSE ENDLESS LOOPS AND PERFORMANCE PROBLEMS
            //dbContext.Configuration.LazyLoadingEnabled = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (db != null)
                        db.Dispose();
                }
                db = null;
                _disposed = true;
            }
        }
        #endregion


        public int SaveImage(as_images item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.as_images.Add(item);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }
                CacheManager.PurgeCacheItems("as_images_" + item.typeCode + "_" + item.itemID);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return item.id;
        }
        public as_images GetImage(int id) {
            var res = new as_images();
            res = db.as_images.FirstOrDefault(x=>x.id==id);
            return res;
        }

        public List<as_images> GetImages(int itemID, string code)
        {
            List<as_images> res = new List<as_images>();
            
            string key = "as_images_" + code + "_"+ itemID;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                return (List<as_images>)CacheManager.Cache[key];
            }
            else
            {

                res = db.as_images.Where(x=>x.itemID==itemID && x.typeCode==code).OrderBy(x=>x.ord).ToList();
                CacheManager.CacheData(key, res);
            }
            return res;
        }
        public bool UpdateImage(int id, string name, string url, string description)
        {

            var res = false;
            var item = db.as_images.FirstOrDefault(x => x.id == id);
            if (item != null)
            {
                item.description = description;
                item.name = name;
                item.url = url;
                db.SaveChanges();
                CacheManager.PurgeCacheItems("as_images_" + item.typeCode + "_" + item.itemID);
                res = true;
            }
             
            return res;
        }

        public bool DeleteImage(int id)
        {
            bool res = false;
            var item = db.as_images.SingleOrDefault(x => x.id == id);
               
           try
            {
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
                CacheManager.PurgeCacheItems("as_images_" + item.typeCode +"_"+ item.itemID);
            
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }

        public bool UpdateImagesOrder(string ids, int itemID, string code)
        {      
          //  db.updateGalleryOrder(ids, itemid); GAG!~
            CacheManager.PurgeCacheItems("as_images_" + code + "_" + itemID);
            var items = GetImages(itemID, code);
            var imageIDs = ids.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
           var ord = 0;
            foreach (var id in imageIDs) {
                var img = items.FirstOrDefault(x => x.id.ToString() == id);
                if (img != null) {
                    img.ord = ord;
                    ord++;
                }
            }
            db.SaveChanges();
            
            CacheManager.PurgeCacheItems("as_images_" + code+"_"+itemID);
            return true;
        }
        public string GetImageLink(as_images item, bool isThumb = false)
        {
            string res = "";
            res = string.Format("{0}/{1}{2}{3}", GetImageFolderLink(item), Path.GetFileNameWithoutExtension(item.filename), isThumb ? "_thumb": "", Path.GetExtension(item.filename));
            return res;
        }

        public string GetImageFolderLink(as_images item)
        {
            string res = "";
            res = string.Format("/uploads/images/{0}/{1}/", item.typeCode, item.itemID);
            return res;
        }


        internal void UploadImage(as_images item, System.Drawing.Image img, int thumbWidth, int thumbHeight, string contentType = "image/jpeg")
        {
            try
            {
                var folderPath = HttpContext.Current.Server.MapPath(GetImageFolderLink(item));
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                System.Drawing.Image origin = img;
                string thumbPath = HttpContext.Current.Server.MapPath(GetImageLink(item, true));

                Bitmap thumbBitmap;
                Graphics originF;

                if (thumbWidth > 0 && thumbHeight > 0)
                {
                    // создаем превью
                    int ow = thumbWidth; // ширина картинки
                    int oh = thumbHeight; // высота картинки
                    float prop = System.Convert.ToSingle(origin.Width) / System.Convert.ToSingle(origin.Height);
                    float normalprop = System.Convert.ToSingle(ow) / System.Convert.ToSingle(oh);
                    if (prop > normalprop)// если по ширине длиннее
                    {
                        int hh = oh;
                        int ww = System.Convert.ToInt32(hh * prop);
                        thumbBitmap = new Bitmap(ow, oh);
                        originF = Graphics.FromImage(thumbBitmap);
                        originF.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        originF.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        // размер ow х hh
                        SolidBrush brush31 = new SolidBrush(Color.White);
                        Rectangle rect31 = new Rectangle(0, 0, ww, hh);
                        originF.FillRectangle(brush31, rect31);

                        originF.DrawImage(origin, 0, 0, ww, hh);
                    }
                    else
                    {
                        int ww = ow;
                        int hh = System.Convert.ToInt32(ww / prop);
                        thumbBitmap = new Bitmap(ow, oh);
                        originF = Graphics.FromImage(thumbBitmap);
                        originF.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        originF.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        // размер ow х hh
                        // рисуем белый фон
                        SolidBrush brush31 = new SolidBrush(Color.White);
                        Rectangle rect31 = new Rectangle(0, 0, ww, hh);
                        originF.FillRectangle(brush31, rect31);

                        originF.DrawImage(origin, 0, 0, ww, hh);
                    }

                    EncoderParameters ep = new EncoderParameters(1);
                    ImageCodecInfo icJPG = getCodecInfo(contentType); 
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                    thumbBitmap.Save(thumbPath, icJPG, ep);
                    originF.Dispose();
                    thumbBitmap.Dispose();               
                }               

                // создаем оригинальный
                var originPath = HttpContext.Current.Server.MapPath(GetImageLink(item));
                Bitmap originBitmap;     
                var ep1 = new EncoderParameters(1);
                var   icJPG1 = getCodecInfo(contentType); // getCodecInfo - функция, см. выше
                ep1.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100); // качество
                originBitmap = new Bitmap(img);
                originBitmap.Save(originPath, icJPG1, ep1);               
                
                origin.Dispose();
                originBitmap.Dispose();
               
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        private static ImageCodecInfo getCodecInfo(string mt)
        {
            ImageCodecInfo[] ici = ImageCodecInfo.GetImageEncoders();
            int idx = 0;
            for (int ii = 0; ii < ici.Length; ii++)
            {
                if (ici[ii].MimeType == mt)
                {
                    idx = ii;
                    break;
                }
            }
            return ici[idx];
        }
    }
}