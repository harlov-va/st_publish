using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RDL
{
    public class Img
    {
        public Img()
        {
        }


        public static void UploadProductPhoto(int picID, System.Drawing.Image img, string format,
            int smallWidth = 150, int smallHeight = 120, int mediumWidth = 350, int mediumHeight = 300, int bigWidth = 1250, int bigHeight = 1000)
        {
            try
            {

                System.Drawing.Image origin = img;

                string originWay = GetImagePath(picID, "small", format);


                Bitmap originThum;
                Graphics originF;

                // создаем превью
                int ow = smallWidth; // ширина картинки
                int oh = smallHeight; // высота картинки
                float prop = System.Convert.ToSingle(origin.Width) / System.Convert.ToSingle(origin.Height);
                float normalprop = System.Convert.ToSingle(ow) / System.Convert.ToSingle(oh);
                if (prop > normalprop)// если по ширине длиннее
                {
                    int hh = oh;
                    int ww = System.Convert.ToInt32(hh * prop);
                    originThum = new Bitmap(ow, oh);
                    originF = Graphics.FromImage(originThum);
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
                    originThum = new Bitmap(ow, oh);
                    originF = Graphics.FromImage(originThum);
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
                ImageCodecInfo icJPG = getCodecInfo("image/jpeg"); // getCodecInfo - функция, см. выше
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100); // качество
                originThum.Save(originWay, icJPG, ep);

                // создаем оригинальный
                originWay = GetImagePath(picID, "original", format);

                ow = bigWidth; // ширина картинки
                oh = bigHeight; // высота картинки
                if (origin.Width > ow)// если по ширине длиннее
                {
                    // ужимаем по ширине
                    int ww = ow;
                    int hh = System.Convert.ToInt32(System.Convert.ToSingle(origin.Height) * (System.Convert.ToSingle(ow) / System.Convert.ToSingle(origin.Width)));
                    originThum = new Bitmap(ww, hh);
                    originF = Graphics.FromImage(originThum);
                    originF.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    originF.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    // размер ow х hh
                    // рисуем белый фон
                    SolidBrush brush31 = new SolidBrush(Color.White);
                    Rectangle rect31 = new Rectangle(0, 0, ww, hh);
                    originF.FillRectangle(brush31, rect31);
                    originF.DrawImage(origin, 0, 0, ww, hh);
                }
                else
                {
                    // в том размере, в каком есть
                    originThum = new Bitmap(origin.Width, origin.Height);
                    originF = Graphics.FromImage(originThum);
                    originF.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    originF.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    // рисуем белый фон
                    SolidBrush brush31 = new SolidBrush(Color.White);
                    Rectangle rect31 = new Rectangle(0, 0, origin.Width, origin.Height);
                    originF.FillRectangle(brush31, rect31);
                    originF.DrawImage(origin, 0, 0, origin.Width, origin.Height);
                }

                ep = new EncoderParameters(1);
                icJPG = getCodecInfo("image/jpeg"); // getCodecInfo - функция, см. выше
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100); // качество
                originThum.Save(originWay, icJPG, ep);

                originWay = originWay = GetImagePath(picID, "medium", format);

                ow = mediumWidth; // ширина картинки
                oh = mediumHeight; // высота картинки
                if (origin.Width > ow)// если по ширине длиннее
                {
                    // ужимаем по ширине
                    int ww = ow;
                    int hh = System.Convert.ToInt32(System.Convert.ToSingle(origin.Height) * (System.Convert.ToSingle(ow) / System.Convert.ToSingle(origin.Width)));
                    originThum = new Bitmap(ww, hh);
                    originF = Graphics.FromImage(originThum);
                    originF.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    originF.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    // размер ow х hh
                    // рисуем белый фон
                    SolidBrush brush31 = new SolidBrush(Color.White);
                    Rectangle rect31 = new Rectangle(0, 0, ww, hh);
                    originF.FillRectangle(brush31, rect31);
                    originF.DrawImage(origin, 0, 0, ww, hh);
                }
                else
                {
                    // в том размере, в каком есть
                    originThum = new Bitmap(origin.Width, origin.Height);
                    originF = Graphics.FromImage(originThum);
                    originF.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    originF.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    // рисуем белый фон
                    SolidBrush brush31 = new SolidBrush(Color.White);
                    Rectangle rect31 = new Rectangle(0, 0, origin.Width, origin.Height);
                    originF.FillRectangle(brush31, rect31);
                    originF.DrawImage(origin, 0, 0, origin.Width, origin.Height);
                }


                ep = new EncoderParameters(1);
                icJPG = getCodecInfo("image/jpeg"); // getCodecInfo - функция, см. выше
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100); // качество
                originThum.Save(originWay, icJPG, ep);

                // очищаем память
                origin.Dispose();
                originF.Dispose();
                originThum.Dispose();
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public static Image ResizeImage(Image img, SizeF targetSize)
        {
            // Коэффициент соотношения сторон
            float k;
            float ih = img.Height;
            float iw = img.Width;
            if ( (k = ih / iw) > targetSize.Height / targetSize.Width )
                targetSize.Width = targetSize.Height / k;
            else
                targetSize.Height = k * targetSize.Width;
            Bitmap targetImage = new Bitmap((int)targetSize.Width, (int)targetSize.Height);

            Graphics G = Graphics.FromImage(targetImage);
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            G.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            G.DrawImage(img, new RectangleF(new PointF(0, 0), targetSize));

            return targetImage;
        }

        private static string GetImagePath(int picID, string size, string format)
        {
            string directory = HttpContext.Current.Server.MapPath(String.Format(format, size));
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string res = directory + picID + ".jpg";
            if (File.Exists(res))
            {
                File.Delete(res);
            }
            return res;
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
        public static string GetImageDomain(int itemId, string domainFormat, int domainsCount=4)
        {
            var num = itemId % domainsCount + 1;
            var res = String.Format(domainFormat, num);
            return res;
        }

    }
}