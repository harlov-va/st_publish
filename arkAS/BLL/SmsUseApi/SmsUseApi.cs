using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace arkAS.BLL.SmsUseApi
{
    public class SmsUseApi
    {
        private const string APIKEY = "805C792A9C8C5B78";
        private const string ID = "27267";
        private const string From = "test";


        public static string SendSmsUseApi(string phone, string text)
        {
            var url_pattern = "http://bytehand.com:3800/send?id={0}&key={1}&to={2}&from={3}&text={4}";
            var url = string.Format(url_pattern, ID, APIKEY, phone, From, text);
            return SendSms(url);
        }

        private static string SendSms(string url)
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