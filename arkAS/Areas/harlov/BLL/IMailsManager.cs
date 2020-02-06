using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkAS.Areas.harlov
{
    public interface IMailsManager:IDisposable
    {
        #region Mails
        List<h_mails> GetMails(out string msg, aspnet_Users user);
        h_mails GetMail(int id, out string msg, aspnet_Users user);
        h_mails CreateMail(Dictionary<string, string> parameters, out string msg, aspnet_Users user);
        h_mails EditMail(Dictionary<string, string> parameters, int id, out string msg, aspnet_Users user);
        bool ChangeMailsStatus(int id, string name, string value, out string msg, aspnet_Users user);
        bool RemoveMail(int id, out string msg, aspnet_Users user);
        List<h_logMails> GetMailLogsByID(int id);
        dynamic GetMailLogsByID(int id, bool empty = true);
        #endregion
        #region MailsStauses
        List<h_mailStatuses> GetMailStatuses(out string msg, aspnet_Users user);
        h_mailStatuses GetMailStatus(int id, out string msg, aspnet_Users user);
        h_mailStatuses CreateMailStatus(Dictionary<string, string> parameters, out string msg, aspnet_Users user);
        h_mailStatuses EditMailStatus(Dictionary<string, string> parameters, int id, out string msg, aspnet_Users user);
        bool RemoveMailStatus(int id, out string msg, aspnet_Users user);
        #endregion
        #region DeliverySystems
        List<h_deliverySystems> GetDeliverySystems(out string msg, aspnet_Users user);
        h_deliverySystems GetDeliverySystem(int id, out string msg, aspnet_Users user);
        h_deliverySystems CreateDeliverySystem(Dictionary<string, string> parameters, out string msg, aspnet_Users user);
        h_deliverySystems EditDeliverySystem(Dictionary<string, string> parameters, int id, out string msg, aspnet_Users user);
        bool RemoveDeliverySystem(int id, out string msg, aspnet_Users user);
        #endregion
    }
}
