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
    public class Unit
    {
        public long unitId { get; set; }
        public string name { get; set; }
        public short isSmallest { get; set; }
        public Nullable<long> smallestId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> parentid { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }

        public string smallestUnit { get; set; }

        public Boolean canDelete { get; set; }
      

        public async Task<List<Unit>> Get()
        {
            List<Unit> items = new List<Unit>();
            IEnumerable<Claim> claims = await APIResult.getList("Units/get");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Unit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        public async Task<List<Unit>> GetActive()
        {
            List<Unit> items = new List<Unit>();
            IEnumerable<Claim> claims = await APIResult.getList("Units/getActive");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Unit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
   

      
        public async Task<List<Unit>> getSmallUnits(long itemId, long unitId)
        {
            List<Unit> items = new List<Unit>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            parameters.Add("unitId", unitId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Units/getSmallUnits", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Unit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        public async Task<long> save(Unit item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "Units/Save";
            var myContent = JsonConvert.SerializeObject(item);
            parameters.Add("itemObject", myContent);
           return await APIResult.post(method, parameters);
        }
        public async Task<long> delete(long unitId, long userId, Boolean final)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", unitId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "Units/Delete";
           return await APIResult.post(method, parameters);
        }
    }
}
