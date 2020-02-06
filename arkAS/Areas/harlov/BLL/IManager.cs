using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkAS.Areas.harlov.BLL
{
    public interface IManager:IDisposable
    {
        IContragentsManager Contragents { get; set; }
        IDocumentManager Documents { get; set; }
        IInvoicesManager Invoices { get; set; }
        IMailsManager Mails { get; set; }
        aspnet_Users GetUser();
    }
}
