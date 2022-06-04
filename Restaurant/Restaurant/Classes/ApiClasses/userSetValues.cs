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
    public class UserSetValues
    {
        public int id { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<int> valId { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }



        public async Task<List<UserSetValues>> GetAll()
        {

            List<UserSetValues> list = new List<UserSetValues>();

            IEnumerable<Claim> claims = await APIResult.getList("userSetValues/Get");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<UserSetValues>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

           
        }


        //
        public async Task<int> Save(UserSetValues obj)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "userSetValues/Saveu";

            var myContent = JsonConvert.SerializeObject(obj);
            parameters.Add("Object", myContent);
           return await APIResult.post(method, parameters);

            
        }


        //
       
      

    }
}


