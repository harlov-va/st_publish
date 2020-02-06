using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;


namespace arkAS.Models.GoogleSheets
{
    public class GoogleDrive
    {
        public static string[] Scopes = { DriveService.Scope.DriveReadonly };
        public static string ApplicationName = "Sheets API";
        public UserCredential credential;

        //public string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public string credPath = AppDomain.CurrentDomain.BaseDirectory + "/Models/GoogleSheets/credentials";
        
        public GoogleDrive()
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "657364068678-tc2rkemtsih2m1frkr5v0nq1j4cd81l0.apps.googleusercontent.com",
                    ClientSecret = "8St7GG4ncZlTeOl1zqtyT3Kk"
                },
                Scopes,
                "dmitriy.zabi@gmail.com",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
        }

        public bool? Editable(string id)
        {
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            FilesResource.ListRequest listRequest = service.Files.List();

            IList<Google.Apis.Drive.v2.Data.File> files = listRequest.Execute()
                .Items;
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Id == id) return file.Editable;
                }
            }
            return false;
        }

    }
}