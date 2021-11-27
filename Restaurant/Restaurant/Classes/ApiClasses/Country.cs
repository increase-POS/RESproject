using Newtonsoft.Json;
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
using Newtonsoft.Json.Converters;

namespace Restaurant.ApiClasses
{
    public class Country
    {

        public int countryId { get; set; }
        public string code { get; set; }
        public string currency { get; set; }
        public string name { get; set; }
        public byte isDefault { get; set; }

        private string urimainpath = "Country/";
        /// <summary>
        /// ///////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<List<Country>> GetAll()
        {

            List<Country> list = new List<Country>();
            //  Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("mainBranchId", mainBranchId.ToString());
            //parameters.Add("userId", userId.ToString());
            //parameters.Add("date", date.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList(urimainpath+ "GetAll");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<Country>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            ////List<Country> memberships = null;

            ////HttpResponseMessage response = new HttpResponseMessage();
            ////using (var client = new HttpClient())
            ////{
            ////    Uri uri = new Uri(Global.APIUri + urimainpath + "GetAll");
            ////    response = await ApiConnect.ApiGetConnect(uri);

            ////    response = await ApiConnect.ApiGetConnect(uri);
            ////    if (response.IsSuccessStatusCode)
            ////    {
            ////        var jsonString = await response.Content.ReadAsStringAsync();

            ////        memberships = JsonConvert.DeserializeObject<List<Country>>(jsonString);

            ////        return memberships;
            ////    }
            ////    else //web api sent error response 
            ////    {
            ////        memberships = new List<Country>();
            ////    }
            ////    return memberships;
            ////}

        }

        public async Task<List<Country>> GetAllRegion()
        {
            List<Country> list = new List<Country>();
            //  Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("mainBranchId", mainBranchId.ToString());
            //parameters.Add("userId", userId.ToString());
            //parameters.Add("date", date.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList(urimainpath + "GetAllRegion");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<Country>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            //List<Country> memberships = null;

            //HttpResponseMessage response = new HttpResponseMessage();
            //using (var client = new HttpClient())
            //{
            //    Uri uri = new Uri(Global.APIUri + urimainpath + "GetAllRegion");
            //    response = await ApiConnect.ApiGetConnect(uri);

            //    response = await ApiConnect.ApiGetConnect(uri);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var jsonString = await response.Content.ReadAsStringAsync();

            //        memberships = JsonConvert.DeserializeObject<List<Country>>(jsonString);

            //        return memberships;
            //    }
            //    else //web api sent error response 
            //    {
            //        memberships = new List<Country>();
            //    }
            //    return memberships;
            //}

        }

       
        public async Task<int> UpdateIsdefault(int countryId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("countryId", countryId.ToString());

            //parameters.Add("userId", userId.ToString());
            //parameters.Add("final", final.ToString());

            string method = urimainpath+ "UpdateIsdefault";
            //  return await APIResult.post(method, parameters);
            return await APIResult.post(method, parameters);

         
        }


    }
}
