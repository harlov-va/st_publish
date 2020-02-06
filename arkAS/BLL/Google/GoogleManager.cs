using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using arkAS.BLL.Google;

namespace arkAS.BLL.Google
{
    public class GoogleManager
    {

        private DriveService service;
        public void Authentications()
        {
            String CLIENT_ID = "237325946327-dq2777e5dun4c3kju2ovra0jveknutrn.apps.googleusercontent.com";
            String CLIENT_SECRET = "BjtoQdXDSxmDK6BIeo6F3OS6";
            service = GoogleRepository.AuthenticateOauth(CLIENT_ID, CLIENT_SECRET, Environment.UserName);

            if (service == null)
            {
               //Надо что то сделать
            }
        }

        public bool CreateDocument(string nameDoc, string content)
        {

                File newFile = GoogleRepository.uploadFile(service,nameDoc, content);
                if (newFile != null)
                {
                    return true;
                }
                else
                { return false; }
                

        }

        public bool UpdataDocument(string FileId, string content)
        {

            if (FileId != "")
            {
                File newFile = GoogleRepository.updateFile(service, content, FileId);
                return true;
            }
            else
                return false;

        }

        public string[] DownloadContentFile(string nameFile) 
        {
            string Q_doc = "title = '" + nameFile + "' and mimeType = 'application/vnd.google-apps.document'";
            IList<File> _Files = GoogleRepository.GetFiles(service, Q_doc);
            string[] array = new string[2];
            if (_Files.Count != 0)
            {
                array[0] =  GoogleRepository.downloadFile(service, _Files[0]);
                array[1] = _Files[0].Title;
                return array;
            }
            else
            {
                array[0]="Ошибка";
                return array;
            }
                
        }
        public bool DeleteFiles(string name)
        {
            string Q_doc = "title = '" + name + "' and mimeType = 'application/vnd.google-apps.document'";
            IList<File> _Files = GoogleRepository.GetFiles(service, Q_doc);
            if (_Files.Count != 0)
            {
                FilesResource.DeleteRequest request = service.Files.Delete(_Files[0].Id);
                request.Execute();
                return true;
            }
            else { return false; }
 
        }

        public List<string> GetFiles()
        {
            string Q_doc = "mimeType = 'application/vnd.google-apps.document'";
            IList<File> _Files = GoogleRepository.GetFiles(service, Q_doc);
            List<string> title = new List<string>();
            foreach (var item in _Files)
            {
                title.Add(item.Title);
            }
            return title;
                   
        }
    }
}