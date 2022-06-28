using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Restaurant.ApiClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Restaurant.Classes
{
    public class ErrorClass
    {
        public long errorId { get; set; }
        public string num { get; set; }
        public string msg { get; set; }
        public string stackTrace { get; set; }
        public string targetSite { get; set; }
        public Nullable<long> posId { get; set; }
        public Nullable<long> branchId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<long> createUserId { get; set; }

 

        /// <summary>
        /// ///////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<List<ErrorClass>> Get()
        {
            List<ErrorClass> items = new List<ErrorClass>();
            IEnumerable<Claim> claims = await APIResult.getList("errorcontroller/GetAll");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<ErrorClass>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
       
        public async Task<long> save(ErrorClass item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "errorcontroller/Save";
            var myContent = JsonConvert.SerializeObject(item);
            parameters.Add("itemObject", myContent);
           return await APIResult.post(method, parameters);
        }

    }
}
