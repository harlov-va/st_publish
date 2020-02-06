using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkAS.Areas.harlov
{
    public interface IInvoicesManager: IDisposable
    {
        #region Invoices
        List<h_invoices> GetInvoices(aspnet_Users user, out string msg);
        h_invoices GetInvoice(int id, aspnet_Users user, out string msg);
        h_invoices CreateInvoice(Dictionary<string, string> parameters, aspnet_Users user, out string msg);
        h_invoices EditInvoice(Dictionary<string ,string> parameters, int id, aspnet_Users user, out string msg);
        bool ChangeInvoicesStatus(int id, string value, string name, out string msg, aspnet_Users user);
        bool RemoveInvoice(int id, aspnet_Users user, out string msg);
        #endregion
        #region InvoicesStatuses
        List<h_invoiceStatuses> GetInvoiceStatuses(aspnet_Users user, out string msg);
        h_invoiceStatuses GetInvoiceStatus(int id, aspnet_Users user, out string msg);
        h_invoiceStatuses CreateInvoiceStatus(Dictionary<string, string> parameters, aspnet_Users user, out string msg);
        h_invoiceStatuses EditInvoiceStatus(Dictionary<string,string> parameters, int id, aspnet_Users user, out string msg);
        //bool ChangeInvoicesStatus(int id, string name, string value, aspnet_Users user, out string msg);
        bool RemoveInvoiceStatus(int id, aspnet_Users user, out string msg);
        #endregion
    }
}
