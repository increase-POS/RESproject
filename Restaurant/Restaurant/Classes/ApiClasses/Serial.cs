using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

namespace POS.Classes
{
    class Serial
    {
        public int serialId { get; set; }
        public Nullable<int> itemId { get; set; }
        public string serialNum { get; set; }
        public Nullable<byte> isActive { get; set; }

        public async Task<List<Serial>> GetItemSerials(int itemId)
        {
            List<Serial> items = new List<Serial>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("serials/Get", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Serial>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        // add serial to item
        public async Task<int> save(Serial item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "serials/Save";
            var myContent = JsonConvert.SerializeObject(item);
            parameters.Add("itemObject", myContent);
           return await APIResult.post(method, parameters);
        }
        // call api method to delete item serial
        public async Task<int> delete(int serialId, int userId, Boolean final)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", serialId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "serials/Delete";
           return await APIResult.post(method, parameters);
        }
     }
}
