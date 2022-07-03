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
    public class ItemExtra
    {
        public long itemExtraId { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long> extraId { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }

        public async Task<List<Item>> GetExtraByItemId(long parentIUId)
        {
            List<Item> list = new List<Item>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", parentIUId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsExtra/GetExtraByItemId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<Item>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

 
        }

        public async Task<int> UpdateExtraByItemId(long itemId, List<ItemExtra> newlist, long userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            var myContent = JsonConvert.SerializeObject(newlist);
            parameters.Add("Object", myContent);
            parameters.Add("itemId", itemId.ToString());
            parameters.Add("userId", userId.ToString());
            string method = "ItemsExtra/UpdateExtraByItemId";
           return await APIResult.post(method, parameters);

        }

        

    }
}
