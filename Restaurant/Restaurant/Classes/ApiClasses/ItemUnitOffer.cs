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
    public class ItemUnitOffer
    {
        public long ioId { get; set; }
        public Nullable<long> iuId { get; set; }
        public Nullable<long> offerId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<int> quantity { get; set; }
        public string offerName { get; set; }
        public string unitName { get; set; }
        public string itemName { get; set; }
        public string code { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long> unitId { get; set; }

        public async Task<int> updategroup(long offerId, List<ItemUnitOffer> newitoflist, long userId)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsOffers/UpdateItemsByOfferId";

            var myContent = JsonConvert.SerializeObject(newitoflist);
            parameters.Add("Object", myContent);
            parameters.Add("offerId", offerId.ToString());
            parameters.Add("userId", userId.ToString());



           return await APIResult.post(method, parameters);

        }
     

        public async Task<List<ItemUnitOffer>> GetItemsByOfferId(long offerId)
        {
            List<ItemUnitOffer> list = new List<ItemUnitOffer>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("offerId", offerId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsOffers/GetItemsByOfferId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemUnitOffer>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
           
        }
    }
}

