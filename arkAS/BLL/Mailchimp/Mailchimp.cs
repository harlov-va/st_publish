using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace arkAS.BLL.Mailchimp
{
    public class Mailchimp
    {
        private const string Apikey = "c3e92899aa79dbf342ba4f115cca348e-us14";

        public static string GetAllLists()
        {
            var url_pattern = "https://us14.api.mailchimp.com/3.0/lists?apikey={0}";
            var url = string.Format(url_pattern, Apikey);
            return MailGetRequest(url);
        }
      
        private static string MailGetRequest(string url)
        {
            var hc = new HttpClient();
            hc.BaseAddress = new Uri(url);
            string res = null;
            try
            {
                res = hc.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return res;
        }
    }
}

 