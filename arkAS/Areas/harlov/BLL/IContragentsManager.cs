using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov
{
    public interface IContragentsManager:IDisposable
    {
        #region Contragents
        List<h_contragents> GetContragents(aspnet_Users user, out string msg);
        h_contragents GetContragent(int id, aspnet_Users user, out string msg);
        h_contragents CreateContragent(Dictionary<string, string> parameters, aspnet_Users user, out string msg);
        h_contragents EditContragent(Dictionary<string, string> parameters, int id, aspnet_Users user, out string msg);
        bool RemoveContragent(int id, aspnet_Users user, out string msg);
        #endregion
    }
}