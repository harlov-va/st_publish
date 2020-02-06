using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkAS.Areas.harlov
{
    public interface IDocumentManager:IDisposable
    {
        #region Document
        List<h_documents> GetDocuments(aspnet_Users user, out string msg);
        h_documents GetDocument(int id, aspnet_Users user, out string msg);
        h_documents CreateDocument(Dictionary<string, string> parameters, aspnet_Users user, out string msg);
        h_documents EditDocument(Dictionary<string, string> parameters, int id, aspnet_Users user, out string msg);
        bool ChangeDocumentInLine(int id, string name, string value, aspnet_Users user, out string msg);
        bool RemoveDocument(int id, aspnet_Users user, out string msg);
        //List<h_logDocuments> GetDocLogsByID(int id);
        #endregion
        #region DocTypes
        List<h_docTypes> GetDocTypes(aspnet_Users user, out string msg);
        h_docTypes GetDocType(int id, aspnet_Users user, out string msg);
        h_docTypes CreateDocType(Dictionary<string, string> parameters, aspnet_Users user, out string msg);
        h_docTypes EditDocType(Dictionary<string, string> parameters, int id, aspnet_Users user, out string msg);
        bool RemoveDocType(int id, aspnet_Users user, out string msg);
        #endregion
        #region DocStatuses
        List<h_docStatuses> GetDocStatuses(aspnet_Users user, out string msg);
        h_docStatuses GetDocStatus(int id, aspnet_Users user, out string msg);
        h_docStatuses CreateDocStatus(Dictionary<string, string> parameters, aspnet_Users user, out string msg);
        h_docStatuses EditDocStatus(Dictionary<string , string> parameters, int id, aspnet_Users user, out string msg);
        bool RemoveDocStatus(int id, aspnet_Users user, out string msg);
        #endregion
    }
}
