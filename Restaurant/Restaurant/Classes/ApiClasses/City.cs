using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Restaurant;
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
    public class City

    {
        public long cityId { get; set; }
        public string cityCode { get; set; }
        public Nullable<long> countryId { get; set; }



        public async Task<List<City>> Get()
        {
            List<City> list = new List<City>();

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("city/Get");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<City>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
           
        }

    }
}

