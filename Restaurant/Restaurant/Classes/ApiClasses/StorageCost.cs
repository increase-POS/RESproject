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
    public class StorageCost
    {
        public long storageCostId { get; set; }
        public string name { get; set; }
        public decimal cost { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }


        public bool canDelete { get; set; }
         

        public async Task<List<StorageCost>> Get()
        {
            List<StorageCost> items = new List<StorageCost>();
            IEnumerable<Claim> claims = await APIResult.getList("StorageCost/Get");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<StorageCost>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
     
        public async Task<long> save(StorageCost item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "StorageCost/Save";
            var myContent = JsonConvert.SerializeObject(item);
            parameters.Add("itemObject", myContent);
           return await APIResult.post(method, parameters);
        }
        public async Task<long> delete(long storageCostId, long userId, Boolean final)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", storageCostId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "StorageCost/Delete";
           return await APIResult.post(method, parameters);
        }
        public async Task<int> setCostToUnits(List<long> itemUnitsIds, long storageCostId, long userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "StorageCost/setCostToUnits";
            var myContent = JsonConvert.SerializeObject(itemUnitsIds);
            parameters.Add("itemUnitsIds", myContent);
            parameters.Add("storageCostId", storageCostId.ToString());
            parameters.Add("userId", userId.ToString());
            return await APIResult.post(method, parameters);
        }
        public async Task<List<ItemUnit>> GetStorageCostUnits(long storageCostId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("storageCostId", storageCostId.ToString());
            List<ItemUnit> items = new List<ItemUnit>();
            IEnumerable<Claim> claims = await APIResult.getList("StorageCost/GetStorageCostUnits",parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<ItemUnit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
    }
}
