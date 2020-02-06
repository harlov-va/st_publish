using RDL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace arkAS.Handlers
{
    /// <summary>
    /// Сводное описание для UploadifyImageHandler
    /// </summary>
    public class UploadifyImageHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string toSaveDirectory = context.Request.Params["toSaveDirectory"];
            var suggestionID = RDL.Convert.StrToInt(context.Request.Params["suggestionID"], 0);
            var isPropImages = RDL.Convert.StrToInt(context.Request.Params["isPropImages"], 0);
            string propertyName = context.Request.Params["propertyName"];
            string propertyValue = context.Request.Params["propertyValue"] == null ? "" : context.Request.Params["propertyValue"];

            HttpPostedFile file = context.Request.Files["Filedata"];
            if (file != null && !string.IsNullOrEmpty(toSaveDirectory) && suggestionID > 0)
            {
                string serverUrl = context.Server.MapPath("~" + toSaveDirectory + "/" + suggestionID + "/");//сохраняем рисунки с измен размерами

                if (isPropImages > 0)
                {
                    if (propertyValue == "")
                        serverUrl = context.Server.MapPath("~" + toSaveDirectory + "/" + suggestionID + "/property/" + propertyName + "/");
                    else
                        serverUrl = context.Server.MapPath("~" + toSaveDirectory + "/" + suggestionID + "/property/" + propertyName + "/" + propertyValue + "/");

                }

                if (Directory.Exists(serverUrl))
                {
                    Files.DeleteDirectoryFiles(serverUrl);
                }
                else
                {
                    Directory.CreateDirectory(serverUrl);
                }

                var fileGuid = Guid.NewGuid();          //
                var fileGuidName = fileGuid.ToString() + Path.GetExtension(file.FileName);

                context.Response.Write("1");


                //------------преобразование размера---------------------------------------------------------
                Image bmp = Bitmap.FromStream(file.InputStream);
                //Image bmp_resized = mng.ResizeImage(bmp, new SizeF(1024, 1024));   //преобразовали к размеру 1024*...
                Image bmp_resized = ResizeImage(bmp, new SizeF(1024, 1024));   //преобразовали к размеру 1024*...
                bmp_resized.Save(serverUrl + fileGuidName);                       //сохранили в папке
             
                Image bmp_resized_mini = ResizeImage(bmp, new SizeF(300, 300));    //получаем миниатюру 300*...               
                string fileNameMini = Path.GetFileNameWithoutExtension(fileGuidName) + "_mini";
                fileNameMini = fileNameMini + Path.GetExtension(fileGuidName);
                bmp_resized_mini.Save(serverUrl + fileNameMini);
            }
            else
                context.Response.Write("0");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public Image ResizeImage(Image img, SizeF targetSize)
        {

            // Коэффициент соотношения сторон
            float k;
            if ((k = (float)img.Height / img.Width * targetSize.Width / targetSize.Height) > 1)
                targetSize.Width = targetSize.Width * k;
            else
                targetSize.Height = k * targetSize.Height;
            Bitmap targetImage = new Bitmap((int)targetSize.Width, (int)targetSize.Height);

            Graphics G = Graphics.FromImage(targetImage);
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            G.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            G.DrawImage(img, new RectangleF(new PointF(0, 0), targetSize));

            return targetImage;

        }
    }
}