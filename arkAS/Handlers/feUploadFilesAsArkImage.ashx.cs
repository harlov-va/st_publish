using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;

namespace arkAS.Handlers
{
    /// <summary>
    /// Сводное описание для feUploadFilesAsArkImage
    /// </summary>
    public class feUploadFilesAsArkImage : IHttpHandler
    {
        public static string PatternFolder = "/uploads/Images/texts/";
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var dir = context.Request["dir"] ?? "";

                var shared = context.Request["shared"] ?? "";
                var type = context.Request["type"] ?? "";


                var url = PatternFolder;
                var path = HttpContext.Current.Server.MapPath(url);

                context.Response.ContentType = "text/plain";
                var r = new List<ViewDataUploadFilesResult>();
                var js = new JavaScriptSerializer();

                foreach (string file in context.Request.Files)
                {
                    try
                    {
                        var hpf = context.Request.Files[file] as HttpPostedFile;
                        string FileName = string.Empty;
                        if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
                        {
                            string[] files = hpf.FileName.Split(new char[] { '\\' });
                            FileName = files[files.Length - 1];
                        }
                        else
                        {
                            FileName = hpf.FileName;
                        }

                        if (hpf.ContentLength == 0)
                        {
                            continue;
                        }

                        string savedFileName = path + dir + "\\" + FileName;
                        using (var s = hpf.InputStream)
                        {
                            var bytes = new byte[hpf.ContentLength];
                            s.Read(bytes, 0, hpf.ContentLength);
                            File.WriteAllBytes(savedFileName, bytes);
                        }
                        hpf.InputStream.Dispose();
                        r.Add(
                            new ViewDataUploadFilesResult()
                            {
                                Name = FileName,
                                Length = hpf.ContentLength,
                                Type = hpf.ContentType
                            });
                        var uploadedFiles = new
                        {
                            files = r.ToArray()
                        };
                        var jsonObj = js.Serialize(uploadedFiles);
                        context.Response.Write(jsonObj.ToString());


                    }
                    catch (Exception ex)
                    {
                        RDL.Debug.LogError(ex);
                    }


                
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}