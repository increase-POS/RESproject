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
    public class Offer
    {
        public long offerId { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public byte isActive { get; set; }
        public string discountType { get; set; }
        public decimal discountValue { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public string notes { get; set; }
        public Boolean canDelete { get; set; }
        public string forAgents { get; set; }
        public long membershipOfferId { get; set; }
        public Nullable<long> membershipId { get; set; }

        public async Task<List<Offer>> Get()
        {
            List<Offer> items = new List<Offer>();
            IEnumerable<Claim> claims = await APIResult.getList("Offers/Get");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Offer>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        public async Task<Offer> getOfferById(long itemId)
        {
            Offer item = new Offer();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Offers/GetOfferByID", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<Offer>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    break;
                }
            }
            return item;
        }
        public async Task<long> save(Offer item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "Offers/Save";
            var myContent = JsonConvert.SerializeObject(item);
            parameters.Add("itemObject", myContent);
           return await APIResult.post(method, parameters);
        }
        public async Task<long> delete(long itemId, long userId, Boolean final)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "Offers/Delete";
           return await APIResult.post(method, parameters);
        }

        public async Task<List<Offer>> GetOffersByMembershipId(long membershipId)
        {
            List<Offer> items = new List<Offer>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", membershipId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Offers/GetOffersByMembershipId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Offer>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));

                }
            }
            return items;
        }
    }
}
