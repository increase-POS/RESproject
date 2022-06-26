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
using Restaurant.ApiClasses;
using Newtonsoft.Json.Converters;

namespace Restaurant.Classes
{
    public class Printers
    {

        public long printerId { get; set; }
        public string name { get; set; }
        public string printFor { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        /// <summary>
        /// ///////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        /// 

   
        public async Task<int> Save(Printers obj)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "PrinterController/Save";

            var myContent = JsonConvert.SerializeObject(obj);
            parameters.Add("Object", myContent);
            return await APIResult.post(method, parameters);



             
        }

        public async Task<Printers> GetByID(long printerId)
        {

            Printers item = new Printers();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("printerId", printerId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("PrinterController/GetByID", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<Printers>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    break;
                }
            }
            return item;
         
        }


    }
}
