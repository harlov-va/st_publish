using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace arkAS.Models.SuperJob
{
    #region Input 

    public class SuperJobInput
    {
        public string page { get; set; }
        public string count { get; set; }
        public string keyword { get; set; }
        public string town { get; set; }
        public string payment_from { get; set; }
        public string payment_to { get; set; }
        public string type_of_work { get; set; }
        public string gender { get; set; }
        public string experience { get; set; }
        
    }

    #endregion

    #region Output

    [JsonObject]
    public class SuperJobOutput
    {
        [JsonProperty(PropertyName = "total")]
        public int total { get; set; }
        [JsonProperty(PropertyName = "more")]
        public bool more { get; set; }
        [JsonProperty(PropertyName = "subscription_id")]
        public int subscription_id { get; set; }
        [JsonProperty(PropertyName = "subscription_active")]
        public bool subscription_active { get; set; }
        [JsonProperty(PropertyName = "objects")]
        public List<SuperJobOutputObjects> objects { get; set; }
    }

    [JsonObject]
    public class SuperJobOutputObjects
    {
        [JsonProperty(PropertyName = "address")]
        public string address { get; set; }
        [JsonProperty(PropertyName = "age_from")]
        public int? age_from { get; set; }
        [JsonProperty(PropertyName = "age_to")]
        public int? age_to { get; set; }
        [JsonProperty(PropertyName = "agency")]
        public agencyObj agency { get; set; }
        [JsonProperty(PropertyName = "agreement")]
        public bool? agreement { get; set; }
        [JsonProperty(PropertyName = "already_sent_on_vacancy")]
        public bool? already_sent_on_vacancy { get; set; }
        [JsonProperty(PropertyName = "anonymous")]
        public bool? anonymous { get; set; }
        [JsonProperty(PropertyName = "candidat")]
        public string candidat { get; set; }
        [JsonProperty(PropertyName = "catalogues")]
        public List<cataloguesObj> catalogues { get; set; }
        [JsonProperty(PropertyName = "children")]
        public childrenObj children { get; set; }
        [JsonProperty(PropertyName = "client")]
        public clientObj client { get; set; }
        [JsonProperty(PropertyName = "client_logo")]
        public string client_logo { get; set; }
        [JsonProperty(PropertyName = "compensation")]
        public string compensation { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string currency { get; set; }
        [JsonProperty(PropertyName = "date_archived")]
        public string date_archived { get; set; }
        [JsonProperty(PropertyName = "date_pub_to")]
        public string date_pub_to { get; set; }
        [JsonProperty(PropertyName = "date_published")]
        public string date_published { get; set; }
        [JsonProperty(PropertyName = "driving_licence")]
        public List<string> driving_licence { get; set; }
        [JsonProperty(PropertyName = "education")]
        public educationObj education { get; set; }
        [JsonProperty(PropertyName = "experience")]
        public experienceObj experience { get; set; }
        [JsonProperty(PropertyName = "fax")]
        public string fax { get; set; }
        [JsonProperty(PropertyName = "firm_activity")]
        public string firm_activity { get; set; }
        [JsonProperty(PropertyName = "firm_name")]
        public string firm_name { get; set; }
        [JsonProperty(PropertyName = "gender")]
        public genderObj gender { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "id_client")]
        public int? id_client { get; set; }
        [JsonProperty(PropertyName = "is_archive")]
        public bool? is_archive { get; set; }
        [JsonProperty(PropertyName = "is_closed")]
        public bool? is_closed { get; set; }
        [JsonProperty(PropertyName = "is_storage")]
        public bool? is_storage { get; set; }
        [JsonProperty(PropertyName = "languages")]
        public List<List<languagesObj>> languages { get; set; }
        [JsonProperty(PropertyName = "link")]
        public string link { get; set; }
        [JsonProperty(PropertyName = "maritalstatus")]
        public maritalstatusObj maritalstatus { get; set; }
        [JsonProperty(PropertyName = "message_received")]
        public bool? message_received { get; set; }
        [JsonProperty(PropertyName = "metro")]
        public List<metroObj> metro { get; set; }
        [JsonProperty(PropertyName = "moveable")]
        public bool? moveable { get; set; }
        [JsonProperty(PropertyName = "payment")]
        public int? payment { get; set; }
        [JsonProperty(PropertyName = "payment_from")]
        public int? payment_from { get; set; }
        [JsonProperty(PropertyName = "payment_to")]
        public int? payment_to { get; set; }
        [JsonProperty(PropertyName = "phone")]
        public string phone { get; set; }
        [JsonProperty(PropertyName = "place_of_work")]
        public place_of_workObj place_of_work { get; set; }
        [JsonProperty(PropertyName = "profession")]
        public string profession { get; set; }
        [JsonProperty(PropertyName = "rejected")]
        public bool? rejected { get; set; }
        [JsonProperty(PropertyName = "town")]
        public townObj town { get; set; }
        [JsonProperty(PropertyName = "type_of_work")]
        public type_of_workObj type_of_work { get; set; }
        [JsonProperty(PropertyName = "work")]
        public string work { get; set; }
    }

    [JsonObject]
    public class townObj
    {
        [JsonProperty(PropertyName = "declension")]
        public string declension { get; set; }
        [JsonProperty(PropertyName = "genitive")]
        public string genitive { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class agencyObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class cataloguesObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "key")]
        public int? key { get; set; }   
        [JsonProperty(PropertyName = "positions")]
        public List<positionsObj> positions { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class positionsObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "key")]
        public int? key { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class childrenObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class clientObj
    {
        [JsonProperty(PropertyName = "address")]
        public string address { get; set; }
        [JsonProperty(PropertyName = "client_logo")]
        public string client_logo { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "industry")]
        public List<industryObj> industry { get; set; }
        [JsonProperty(PropertyName = "is_blocked")]
        public bool? is_blocked { get; set; }
        [JsonProperty(PropertyName = "link")]
        public string link { get; set; }
        [JsonProperty(PropertyName = "short_reg")]
        public bool? short_reg { get; set; }
        [JsonProperty(PropertyName = "staff_count")]
        public string staff_count { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
        [JsonProperty(PropertyName = "town")]
        public townObj town { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string url { get; set; }
        [JsonProperty(PropertyName = "vacancy_count")]
        public int? vacancy_count { get; set; }
    }

    [JsonObject]
    public class industryObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class educationObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class experienceObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class genderObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class languagesObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class maritalstatusObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class metroObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "id_metro_line")]
        public int? id_metro_line { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class place_of_workObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    [JsonObject]
    public class type_of_workObj
    {
        [JsonProperty(PropertyName = "id")]
        public int? id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
    }

    #endregion

    public class SuperJob
    {
        private static Uri _BaseAddress = new Uri("https://api.superjob.ru/2.0/vacancies/");

        public static SuperJobOutput GetFullInfo(SuperJobInput data)
        {
            SuperJobOutput res = null;
            var hc = new System.Net.Http.HttpClient();
            hc.DefaultRequestHeaders.Add("X-Api-App-Id", "v1.r075be676d686360f202ecffa791e434e8c4f7733c0a64f31463dbb26489ecf7cb182337e.e675c4f6736d325c45aca063189c46fb13548fd6");
            hc.BaseAddress = _BaseAddress;
            var reqString = "";
            if (data.page == "") reqString += "?page=1";
            else reqString += "?page=" + data.page;
            if (data.count == "") reqString += "&count=10";
            else reqString += "&count=" + data.count;
            if (data.keyword != "") reqString += "&keyword=" + data.keyword;
            if (data.town != "") reqString += "&town=" + data.town;
            if (data.payment_from != "") reqString += "&payment_from=" + data.payment_from;
            if (data.payment_to != "") reqString += "&payment_to=" + data.payment_to;
            if (data.type_of_work != "1") reqString += "&type_of_work=" + data.type_of_work;
            if (data.gender != "0") reqString += "&gender=" + data.gender;
            if (data.experience != "0") reqString += "&experience" + data.experience;

            try
            {
                var json = hc.GetAsync(reqString).Result.Content.ReadAsStringAsync().Result;
                res = Newtonsoft.Json.JsonConvert.DeserializeObject<SuperJobOutput>(json);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex, "", new { data });
            }

            return res;
        }
    }
}