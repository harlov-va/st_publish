using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace arkAS.BLL.Weather
{
    public class Weather
    {
        private const string APPID = "6b4fec3cfeb666ed226daff42cb09a95";
    
        public static string GetWeatherByCity(string city)
        {
            var url_pattern = "http://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric&lang=ru";
            var url = string.Format(url_pattern, city, APPID);
            return GetWeather(url);
        }

        public static string GetWeatherByLocation(string lon, string lat)
        {
            var url_pattern = "http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&units=metric&lang=ru";
            var url = string.Format(url_pattern, lat, lon, APPID);
            return GetWeather(url);
        }

        private static string GetWeather(string url)
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