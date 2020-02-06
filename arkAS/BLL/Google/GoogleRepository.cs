using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using System.Text;

namespace arkAS.BLL.Google
{
    public class GoogleRepository
    {

        public static DriveService AuthenticateOauth(string clientId, string clientSecret, string userName)
        {

            string[] scopes = new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
                                             DriveService.Scope.DriveAppsReadonly,   // view your drive apps
                                             DriveService.Scope.DriveFile,   // view and manage files created by this app
                                             DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
                                             DriveService.Scope.DriveReadonly,   // view files and documents on your drive
                                             DriveService.Scope.DriveScripts };  // modify your app scripts


            try
            {
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret }
                                                                                             , scopes
                                                                                             , userName
                                                                                             , CancellationToken.None
                                                                                             , new FileDataStore("Daimto.Drive.Auth.Store")).Result;

                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Daimto Drive API Sample",
                });
                return service;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.InnerException);
                return null;

            }

        }


        public static string downloadFile(DriveService _service, File _fileResource)
        {
            if (!String.IsNullOrEmpty(_fileResource.DownloadUrl))
            {
                // если не гугл док
                try
                {
                    var x = _service.HttpClient.GetByteArrayAsync(_fileResource.DownloadUrl);
                    string res = null;
                    byte[] arr = x.Result;
                    char[] mes = new char[arr.Length];
                    mes = Encoding.UTF8.GetChars(arr);
                    foreach (Char c in mes)
                    {
                        res += c.ToString();
                    }
                    return res;
                }
                catch (Exception e)
                {
                    return "Ошибка";
                }
            }
            else
            {
                // Если гугл док
                try
                {
                    string DownloadUrl = _fileResource.ExportLinks["text/html"];
                    var x = _service.HttpClient.GetByteArrayAsync(DownloadUrl);
                    string res = null;
                    byte[] arr = x.Result;
                    char[] mes = new char[arr.Length];
                    mes = Encoding.UTF8.GetChars(arr);
                    foreach (Char c in mes)
                    {
                        res += c.ToString();
                    }
                    return res;
                }
                catch (Exception e)
                {
                    return "Косяк";
                }
                
            }
        }
        //получаем тип файла
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public static File uploadFile(DriveService _service, string nameDoc, string text)
        {
                File body = new File();
                body.Title = nameDoc;//Заполнить чем то
                body.Description = "MyFile";
                body.MimeType = "text/html";//Заполнить обязательно типом документа
               // body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } };

                byte[] bytes = Encoding.UTF8.GetBytes(text); //new byte[text.Length * sizeof(char)];
                //System.Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
                try
                {
                    FilesResource.InsertMediaUpload request = _service.Files.Insert(body, stream, "text/html"); // Тип загр контента заполнить
                    request.Convert = true;   // Конвертируем в гугл док
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    return null;
                }
            

        }

        public static File updateFile(DriveService _service, string text, string _fileName, string _parent = null)
        {
                string Q_doc = "title = '" + _fileName + "' and mimeType = 'application/vnd.google-apps.document'";
                IList<File> _Files = GetFiles(_service, Q_doc);
         
                File body = new File();
                body.Title = _Files[0].Title; // заголовок
                body.Description = "File updated by Diamto Drive Sample";
                body.MimeType = "text/html"; //задать тип файла
               // body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } }; родителя не меняем

                byte[] byteArray = Encoding.UTF8.GetBytes(text);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    string _fileId = _Files[0].Id;
                    FilesResource.UpdateMediaUpload request = _service.Files.Update(body, _fileId, stream, "text/html");
                    request.Upload();
                    request.Convert = true;
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
 
                    return null;
                }
            }

        

        public static IList<File> GetFiles(DriveService service, string search)
        {
            IList<File> Files = new List<File>();
            try
            {
                FilesResource.ListRequest list = service.Files.List();
                list.MaxResults = 1000;
                if (search != null)
                {
                    list.Q = search;
                }
                FileList filesFeed = list.Execute();

                while (filesFeed.Items != null)
                {
                    foreach (File item in filesFeed.Items)
                    {
                        Files.Add(item);
                    }

                    if (filesFeed.NextPageToken == null)
                    {
                        break;
                    }

                    list.PageToken = filesFeed.NextPageToken;

                    filesFeed = list.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Files;
        }



    }
}