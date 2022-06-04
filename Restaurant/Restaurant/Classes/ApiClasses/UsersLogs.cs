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
using Restaurant.ApiClasses;

namespace Restaurant.Classes
{
    public class UsersLogs
    {

        public int logId { get; set; }
        public Nullable<System.DateTime> sInDate { get; set; }
        public Nullable<System.DateTime> sOutDate { get; set; }
        public Nullable<int> posId { get; set; }
        public Nullable<int> userId { get; set; }
        public bool canDelete { get; set; }

     

     

        public async Task<int> Save(UsersLogs obj)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "UsersLogs/Save";

            var myContent = JsonConvert.SerializeObject(obj);
            parameters.Add("Object", myContent);
           return await APIResult.post(method, parameters);

        }

        public async Task<UsersLogs> GetByID( int logId)
        {
            UsersLogs item = new UsersLogs();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("logId", logId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("UsersLogs/GetByID", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<UsersLogs>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    break;
                }
            }
            return item;

        }

      

    }
}
