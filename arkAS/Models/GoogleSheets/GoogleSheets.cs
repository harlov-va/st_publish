using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;


namespace arkAS.Models.GoogleSheets
{
    public class GoogleSheets
    {
        public static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        public static string ApplicationName = "Sheets API";
        public UserCredential credential;

        //public string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public string credPath = AppDomain.CurrentDomain.BaseDirectory + "/Models/GoogleSheets/credentials";

        public GoogleSheets()
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "657364068678-tc2rkemtsih2m1frkr5v0nq1j4cd81l0.apps.googleusercontent.com",
                    ClientSecret = "8St7GG4ncZlTeOl1zqtyT3Kk"
                },
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
        }

        public IList<IList<Object>> GetTable(string spreadsheetId, string sheetName, string startCell, string finishCell)
        {
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Имя листа!Начальная ячейка:Конечная ячейка
            string range = sheetName;
            if (startCell != "")
            {
                range += "!" + startCell;
                if (finishCell != "") range += ":" + finishCell;
            }
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> res = response.Values;

            return res;
        }

        public bool InlineEditTable(string spreadsheetId, string sheetName, string text)
        {
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "COLUMNS";
            var list = new List<object>() { text };
            valueRange.Values = new List<IList<object>> { list };

            SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, sheetName);

            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse response = request.Execute();

            return true;
        }

        public bool CreateTable(string spreadsheetId, string sheetName)
        {
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var addSheetRequest = new AddSheetRequest();
            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = sheetName;
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request
            {
                AddSheet = addSheetRequest
            });

            var batchUpdateRequest = service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);
            batchUpdateRequest.Execute();

            return true;
        }

        public List<string> GetTableSheets(string spreadsheetId)
        {
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            SpreadsheetsResource.GetRequest request = service.Spreadsheets.Get(spreadsheetId);
            var res = request.Execute();
            List<string> names = new List<string>();
            foreach (var item in res.Sheets)
            {
                names.Add(item.Properties.Title);
            }

            return names;
        }
    }
}