using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using Restaurant.ApiClasses;

namespace Restaurant.Classes
{
    public class CountryCode
    {
        public long countryId { get; set; }
        public string code { get; set; }


        public string currency { get; set; }
        public string name { get; set; }
        public byte isDefault { get; set; }
         public long currencyId { get; set; }

        public async Task<List<CountryCode>> GetAllCountries()
        {
            List<CountryCode> list = new List<CountryCode>();
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Countries/GetAllCountries");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<CountryCode>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }     

        public async Task<List<CountryCode>> GetAllRegion()
        {
            List<CountryCode> list = new List<CountryCode>();

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Countries/GetAllRegion");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<CountryCode>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


            //List<CountryCode> CountryCodes = null;
            //// ... Use HttpClient.
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            //using (var client = new HttpClient())
            //{
            //    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //    client.BaseAddress = new Uri(Global.APIUri);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
            //    HttpRequestMessage request = new HttpRequestMessage();
            //    request.RequestUri = new Uri(Global.APIUri + "Countries/GetAllRegion");
            //    request.Headers.Add("APIKey", Global.APIKey);
            //    request.Method = HttpMethod.Get;
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = await client.SendAsync(request);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var jsonString = await response.Content.ReadAsStringAsync();
            //        jsonString = jsonString.Replace("\\", string.Empty);
            //        jsonString = jsonString.Trim('"');
            //        // fix date format
            //        JsonSerializerSettings settings = new JsonSerializerSettings
            //        {
            //            Converters = new List<JsonConverter> { new BadDateFixingConverter() },
            //            DateParseHandling = DateParseHandling.None
            //        };
            //        CountryCodes = JsonConvert.DeserializeObject<List<CountryCode>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return CountryCodes;
            //    }
            //    else //web api sent error response 
            //    {
            //        CountryCodes = new List<CountryCode>();
            //    }
            //    return CountryCodes;
            //}

        }


        public async Task<int> UpdateIsdefault(long countryId)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("countryId", countryId.ToString());

            string method = "Countries/UpdateIsdefault";
           return await APIResult.post(method, parameters);

           
        }










    }
}

