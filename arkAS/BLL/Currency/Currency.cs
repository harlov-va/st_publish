using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;

namespace arkAS.BLL.Currency
{
    public class Currency
    {
        public static string GetCurrency()
        {
            string url = "http://www.cbr.ru/scripts/XML_daily.asp";
            var res = GetCurrency(url);
            return res;
        }

        public static string GetDynamic(string code)
        {
            string url_pattern = "http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={0}&date_req2={1}&VAL_NM_RQ={2}";
            var date = DateTime.Now;
            string url = string.Format(url_pattern, date.AddDays(-30).ToString(@"dd\/MM\/yyyy"), date.ToString(@"dd\/MM\/yyyy"), code);
            var res = GetCurrency(url);
            return res;
        }

        private static string GetCurrency(string url)
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