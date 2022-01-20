using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Restaurant.ApiClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes.ApiClasses
{
    public class Tables
    {
        public int tableId { get; set; }
        public string name { get; set; }
        public int personsCount { get; set; }
        public Nullable<int> sectionId { get; set; }
        public Nullable<int> branchId { get; set; }
        public string notes { get; set; }
        public string status { get; set; }
        public byte isActive { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }

        public Boolean canDelete { get; set; }
        public string sectionName { get; set; }

        internal async Task<int> save(Tables table)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "Tables/Save";
            var myContent = JsonConvert.SerializeObject(table);
            parameters.Add("itemObject", myContent);
            return await APIResult.post(method, parameters);
        }

        internal async Task<int> delete(int tableId, int userId, bool final)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("tableId", tableId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "Tables/Delete";
            return await APIResult.post(method, parameters);
        }

        internal async Task<IEnumerable<Tables>> Get(int branchId = 0, int sectionId = 0)
        {
            List<Tables> items = new List<Tables>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            parameters.Add("sectionId", sectionId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("Tables/GetAll", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Tables>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
    }
}
