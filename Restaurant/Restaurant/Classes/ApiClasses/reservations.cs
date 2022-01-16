using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes.ApiClasses
{
    public class reservations
    {
        public long reservationId { get; set; }
        public string code { get; set; }
        public Nullable<int> customerId { get; set; }
        public Nullable<System.DateTime> reservationDate { get; set; }
        public Nullable<System.TimeSpan> reservationTime { get; set; }
        public Nullable<int> personsCount { get; set; }
        public string status { get; set; }
        public Nullable<int> tableId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
    }
}
