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


        public Nullable<int> itemId { get; set; }
        public string itemName { get; set; }
        public Nullable<int> categoryId { get; set; }
        public string categoryName { get; set; }
        public int sequence { get; set; }
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

        //order
        public string status { get; set; }
        public int num { get; set; }
        public decimal remainingTime { get; set; }
        public string waiter { get; set; }

        public string tables { get; set; }
        //invoice
        public string invNum { get; set; }
        public string invType { get; set; }
        public Nullable<int> shippingCompanyId { get; set; }

        public List<ItemOrderPreparing> items { get; set; }

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
        public async Task<List<OrderPreparing>> GetPreparingOrdersWithStatus( int branchId, string status,int duration=0)
        {
            List<OrderPreparing> items = new List<OrderPreparing>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            // status like "Listed, Cooking"
            parameters.Add("status", status);
            parameters.Add("duration", duration.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetPreparingOrdersWithStatus", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<OrderPreparing>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        public async Task<List<OrderPreparing>> GetHallOrdersWithStatus( int branchId, string status,int duration=0)
        {
            List<OrderPreparing> items = new List<OrderPreparing>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            // status like "Listed, Cooking" (optional)
            parameters.Add("status", status);
            // duration in hours (optional)
            parameters.Add("duration", duration.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetHallOrdersWithStatus", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<OrderPreparing>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }

        public async Task<List<Invoice>> GetTakAwayOrdersWithStatus( int branchId, int duration=0)
        {
            List<Invoice> items = new List<Invoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            // duration in hours (optional)
            parameters.Add("duration", duration.ToString());

            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetTakAwayOrdersWithStatus", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Invoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        public async Task<List<Invoice>> GetOrdersWithDelivery( int branchId, string status)
        {
            List<Invoice> items = new List<Invoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            // status in syntax "Listed, Collected" or one status "Collected"
            // status values: Listed, Ready, Collected, InTheWay,Done
            parameters.Add("status", status.ToString());

            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetOrdersWithDelivery", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Invoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
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
        public async Task<int> savePreparingOrders(OrderPreparing order, List<ItemOrderPreparing> orderItems, orderPreparingStatus statusObject,int branchId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/SaveOrdersWithItemsAndStatus";
            var myContent = JsonConvert.SerializeObject(order);
            parameters.Add("orderObject", myContent);
            myContent = JsonConvert.SerializeObject(orderItems);
            parameters.Add("itemsObject", myContent);
            myContent = JsonConvert.SerializeObject(statusObject);
            parameters.Add("statusObject", myContent);
            parameters.Add("branchId", branchId.ToString());
            return await APIResult.post(method, parameters);
        }
        public async Task<int> editPreparingOrderAndStatus(OrderPreparing order, orderPreparingStatus statusObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/EditOrderAndStatus";
            var myContent = JsonConvert.SerializeObject(order);
            parameters.Add("orderObject", myContent);
            myContent = JsonConvert.SerializeObject(statusObject);
            parameters.Add("statusObject", myContent);
            return await APIResult.post(method, parameters);
        }

        public async Task<int> EditInvoiceOrdersStatus(int invoiceId,int shipUserId, orderPreparingStatus statusObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/EditInvoiceOrdersStatus";

            parameters.Add("invoiceId", invoiceId.ToString());
            parameters.Add("shipUserId", shipUserId.ToString());
           string  myContent = JsonConvert.SerializeObject(statusObject);
            parameters.Add("statusObject", myContent);
            return await APIResult.post(method, parameters);
        }
        public async Task<int> updateOrderStatus(orderPreparingStatus statusObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/updateOrderStatus";
            string myContent = JsonConvert.SerializeObject(statusObject);
            parameters.Add("statusObject", myContent);
            return await APIResult.post(method, parameters);
        }
        public async Task<string> generateOrderNumber(string orderCode, string branchCode, int branchId)
        {
            int sequence = await GetLastNumOfInv(orderCode, branchId);
            sequence++;
            string strSeq = sequence.ToString();
            if (sequence <= 999999)
                strSeq = sequence.ToString().PadLeft(6, '0');
            string invoiceNum = orderCode + "-" + branchCode + "-" + strSeq;
            return invoiceNum;
        }
        public async Task<int> GetLastNumOfInv(string orderCode, int branchId)
        {
            int count = 0;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("orderCode", orderCode);
            parameters.Add("branchId", branchId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetLastNumOfOrder", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    count = int.Parse(c.Value);
                    break;
                }
            }
            return count;
        }
        public async Task<int> GetHallOrderCount(string status, int branchId, int duration)
        {
            int count = 0;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("status", status);
            parameters.Add("branchId", branchId.ToString());
            parameters.Add("duration", duration.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetCountHallOrders", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    count = int.Parse(c.Value);
                    break;
                }
            }
            return count;
        }
    }
}
