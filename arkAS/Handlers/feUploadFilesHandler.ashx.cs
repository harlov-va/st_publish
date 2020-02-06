using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;
namespace arkAS.Handlers
{
    /// <summary>
    /// Сводное описание для feUploadFilesHandler
    /// </summary>
    public class feUploadFilesHandler : IHttpHandler
    {
        public static string PatternFolder = "/uploads/patternFolder/";
        public static string GetRootURL(string id)
        {
            return PatternFolder + id + "/";
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var dir = context.Request["dir"] ?? "";

                var shared = context.Request["shared"] ?? "";
                var type = context.Request["type"] ?? "";
                var idStr = context.Request["id"];
                if (string.IsNullOrEmpty(idStr))
                {
                    throw new HttpException(400, "No Id");
                }

                //int id;
                //if (!int.TryParse(idStr, out id))
                //{
                //    throw new HttpException(400, "Bad id format");
                //}

                var url = GetRootURL(idStr);
                var path = HttpContext.Current.Server.MapPath(url);

                context.Response.ContentType = "text/plain";//"application/json";
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
                        if (hpf.ContentLength < 4000000)
                       {
                        string savedFileName = path + dir + "\\" + FileName;
                        //hpf.SaveAs(savedFileName);
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
                                //Thumbnail_url = savedFileName,
                                Name = FileName,
                                Length = hpf.ContentLength,
                                Type = hpf.ContentType
                            });
                        var uploadedFiles = new
                        {
                            files = r.ToArray()
                        };
                        var jsonObj = js.Serialize(uploadedFiles);
                        //jsonObj.ContentEncoding = System.Text.Encoding.UTF8;
                        //jsonObj.ContentType = "application/json;";
                        context.Response.Write(jsonObj.ToString());
                        }

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
public class ViewDataUploadFilesResult
{
    public string Thumbnail_url { get; set; }
    public string Name { get; set; }
    public int Length { get; set; }
    public string Type { get; set; }
}