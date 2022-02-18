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
    public class TablesReservation
    {
        public long reservationId { get; set; }
        public string code { get; set; }
        public Nullable<int> customerId { get; set; }
        public Nullable<System.DateTime> reservationDate { get; set; }
        public Nullable<System.DateTime> reservationTime { get; set; }
        public Nullable<System.DateTime> endTime { get; set; }
        public Nullable<int> personsCount { get; set; }
        public string status { get; set; }
        public Nullable<int> tableId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }

        /////////////////////////////////
        internal async Task<int> addReservation(TablesReservation reservation, List<Tables> tables)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "Tables/addReservation";
            var myContent = JsonConvert.SerializeObject(reservation);
            parameters.Add("itemObject", myContent);

            myContent = JsonConvert.SerializeObject(tables);
            parameters.Add("tables", myContent);
            return await APIResult.post(method, parameters);
        }
        internal async Task<IEnumerable<TablesReservation>> Get(int branchId = 0, int sectionId = 0)
        {
            List<TablesReservation> items = new List<TablesReservation>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            parameters.Add("sectionId", sectionId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("Tables/GetReservations", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<TablesReservation>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
    }
}
