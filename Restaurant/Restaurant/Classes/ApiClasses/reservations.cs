using Newtonsoft.Json;
using Restaurant.ApiClasses;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
