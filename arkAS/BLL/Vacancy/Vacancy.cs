using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace arkAS.BLL.Vacancy
{
    public class Vacancy
    {
        public static string GetVacancyParams(string keywords, string region, string levelSolary, string experience, string currency)
        {
            var url = "";
            var url_pattern ="";
            string Solary="";
            if (levelSolary != "")
                Solary ="&salary="+levelSolary;
            if (currency == "руб.")
                currency = "RUR";

            if (experience != "doesNotMatter")
            {
                url_pattern = "https://api.hh.ru/vacancies?text={0}&area={1}{2}&experience={3}&currency={4}&only_with_salary=true&items_on_page=20";
                url = string.Format(url_pattern, keywords,region, Solary, experience,currency);
            }
            else {
                url_pattern = "https://api.hh.ru/vacancies?text={0}&area={1}{2}&currency={3}&only_with_salary=true&items_on_page=20";
                 url = string.Format(url_pattern, keywords,region,Solary,currency);
            }
            return GetVacancy(url);
        }

        public static string GetVacancyParamsKeywords(string keywords)
        {
            var url = "";
            var url_pattern = "";
            {
                url_pattern = "https://api.hh.ru/vacancies?text={0}&only_with_salary=true&items_on_page=20";
                url = string.Format(url_pattern, keywords);
            }

            return GetVacancy(url);
        }


        public static string GetSity(){

            var url_pattern = "https://api.hh.ru/areas";
            return GetVacancy(url_pattern);
        }


        private static string GetVacancy(string url)
        {
            var hc = new HttpClient();
            hc.BaseAddress = new Uri(url);
            hc.DefaultRequestHeaders.Add("User-Agent", "MyApp/hh (smssanja@gmail.com)");


            
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