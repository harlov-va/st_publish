using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace arkAS.BLL.HH
{
    public class Specializations
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
    public class Areases
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "parent_id")]
        public int ParentId { get; set; }
        [JsonProperty(PropertyName = "areas")]
        public string[] Areas { get; set; }
    }
    public class Countries
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }        
    }


    public class HH
    {
        public string GetSpecializations() 
        {
            return "https://api.hh.ru/specializations";
        }
        public async Task GetCatalogs(string catalogName)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(catalogName);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", "Ark AS/1.0 (777seyran@gmail.com)");
                    // New code:
                    var response = await client.GetAsync(client.BaseAddress);
                    if (response.IsSuccessStatusCode)
                    {
                        var t = await response.Content.ReadAsStringAsync();
                        List<Specializations> getInfo = null;
                        getInfo = JsonConvert.DeserializeObject<List<Specializations>>(t);


                    }
                }
                catch (Exception ex)
                {

                    RDL.Debug.LogError(ex, "", new { catalogName });
                }

            }
        }
    }
}