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
    public class ItemOrderPreparing
    {
        public int itemOrderId { get; set; }
        public Nullable<int> itemUnitId { get; set; }
        public Nullable<int> orderPreparingId { get; set; }
        public Nullable<int> quantity { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
    }
    public class orderPreparingStatus
    {
        public int orderStatusId { get; set; }
        public Nullable<int> orderPreparingId { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }
    }
        public class OrderPreparing
    {
        public int orderPreparingId { get; set; }
        public string orderNum { get; set; }
        public Nullable<int> invoiceId { get; set; }
        public string notes { get; set; }
        public Nullable<decimal> preparingTime { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }


        public string itemName { get; set; }
        public Nullable<int> itemUnitId { get; set; }
        public int quantity { get; set; }
        public string status { get; set; }
        public int num { get; set; }
        public decimal remainingTime { get; set; }

        //-------------------------------------------
        public async Task<List<OrderPreparing>> GetInvoicePreparingOrders( int invoiceId)
        {
            List<OrderPreparing> items = new List<OrderPreparing>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("invoiceId", invoiceId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetInvoicePreparingOrders", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<OrderPreparing>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }

        public async Task<int> savePreparingOrder(OrderPreparing order, List<ItemOrderPreparing> orderItems, orderPreparingStatus statusObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/SaveWithItemsAndStatus";
            var myContent = JsonConvert.SerializeObject(order);
            parameters.Add("orderObject", myContent);
            myContent = JsonConvert.SerializeObject(orderItems);
            parameters.Add("itemsObject", myContent);
            myContent = JsonConvert.SerializeObject(statusObject);
            parameters.Add("statusObject", myContent);
            return await APIResult.post(method, parameters);
        }
    }
}
