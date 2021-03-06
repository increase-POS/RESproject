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
        public long itemOrderId { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public Nullable<long> orderPreparingId { get; set; }
        public Nullable<int> quantity { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> itemsTransId { get; set; }

        public Nullable<long> itemId { get; set; }
        public string itemName { get; set; }
        public Nullable<long> categoryId { get; set; }
        public string categoryName { get; set; }
        public int sequence { get; set; }

        public List<itemsTransferIngredients> itemsIngredients { get; set; }
        public List<ItemTransfer> itemExtras { get; set; }
    }
    public class orderPreparingStatus
    {
        public long orderStatusId { get; set; }
        public Nullable<long> orderPreparingId { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }
    }
    public class OrderPreparing
    {
        public long orderPreparingId { get; set; }
        public string orderNum { get; set; }
        public Nullable<long> invoiceId { get; set; }
        public string notes { get; set; }
        public Nullable<decimal> preparingTime { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }


        public string itemName { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public Nullable<long> itemsTransId { get; set; }
        public int quantity { get; set; }

        //order
        public string status { get; set; }
        public int num { get; set; }
        public decimal remainingTime { get; set; }
        public string waiter { get; set; }
        public Nullable<System.DateTime> preparingStatusDate { get; set; }

        public string tables { get; set; }
        //invoice
        public string invNum { get; set; }
        public string invType { get; set; }
        public Nullable<long> shippingCompanyId { get; set; }

        public List<ItemOrderPreparing> items { get; set; }

        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }
        public Nullable<System.TimeSpan> invTime { get; set; }
        //category
        public Nullable<long> categoryId { get; set; }
        public string categoryCode { get; set; }
        public string categoryName { get; set; }
        public string sectionTable { get; set; }
        //-------------------------------------------

        // for check dataGrid
        public bool IsChecked { get; set; }

        public async Task<List<OrderPreparing>> GetInvoicePreparingOrders(long invoiceId)
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
        public async Task<List<OrderPreparing>> GetKitchenPreparingOrders(long branchId, string status, long duration =0)
        {
            List<OrderPreparing> items = new List<OrderPreparing>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            // status like "Listed, Cooking"
            parameters.Add("status", status);
            parameters.Add("duration", duration.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetKitchenPreparingOrders", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<OrderPreparing>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        public async Task<List<OrderPreparing>> GetHallOrdersWithStatus(long branchId, string status,int duration=0)
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

        public async Task<List<Invoice>> GetOrdersByTypeWithStatus(long branchId,string type, int duration=0)
        {
            List<Invoice> items = new List<Invoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            parameters.Add("type", type);
            // duration in hours (optional)
            parameters.Add("duration", duration.ToString());

            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetOrdersByTypeWithStatus", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<Invoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        public async Task<List<Invoice>> GetOrdersWithDelivery(long branchId, string status)
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

        public async Task<long> savePreparingOrder(OrderPreparing order, List<ItemOrderPreparing> orderItems, orderPreparingStatus statusObject)
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
        public async Task<long> savePreparingOrders(OrderPreparing order, List<ItemOrderPreparing> orderItems, orderPreparingStatus statusObject, long branchId,string statusesOfPreparingOrder)
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
            parameters.Add("statusesOfPreparingOrder", statusesOfPreparingOrder);
            return await APIResult.post(method, parameters);
        }
        public async Task<int> EditPreparingOrdersPrepTime(List<OrderPreparing> orders, decimal preparingTime, long userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/EditPreparingOrdersPrepTime";
            var myContent = JsonConvert.SerializeObject(orders);
            parameters.Add("orderObject", myContent);
          
            parameters.Add("preparingTime", preparingTime.ToString());
            parameters.Add("userId", userId.ToString());
            return await APIResult.post(method, parameters);
        }
        public async Task<int> editPreparingOrdersAndStatus(List<OrderPreparing> preparingOrders, orderPreparingStatus statusObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/EditOrderListAndStatus";
            var myContent = JsonConvert.SerializeObject(preparingOrders);
            parameters.Add("orderObject", myContent);
            myContent = JsonConvert.SerializeObject(statusObject);
            parameters.Add("statusObject", myContent);
            return await APIResult.post(method, parameters);
        }

        public async Task<int> EditInvoiceOrdersStatus(long invoiceId, long? shipUserId, long shippingCompanyId, orderPreparingStatus statusObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/EditInvoiceOrdersStatus";

            parameters.Add("invoiceId", invoiceId.ToString());
            parameters.Add("shipUserId", shipUserId.ToString());
            parameters.Add("shippingCompanyId", shippingCompanyId.ToString());
           string  myContent = JsonConvert.SerializeObject(statusObject);
            parameters.Add("statusObject", myContent);
            return await APIResult.post(method, parameters);
        }

        public async Task<int> EditInvoiceDelivery(long invoiceId, long? shipUserId, long shippingCompanyId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/EditInvoiceDelivery";

            parameters.Add("invoiceId", invoiceId.ToString());
            parameters.Add("shipUserId", shipUserId.ToString());
            parameters.Add("shippingCompanyId", shippingCompanyId.ToString());

            return await APIResult.post(method, parameters);
        }
        public async Task<int> finishInvoiceOrders(long invoiceId, long userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/finishInvoiceOrders";

            parameters.Add("invoiceId", invoiceId.ToString());
            parameters.Add("userId", userId.ToString());

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

        public async Task<int> updateListOrdersStatus(List<OrderPreparing> preparingOrders, orderPreparingStatus statusObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "OrderPreparing/updateListOrdersStatus";
            string myContent = JsonConvert.SerializeObject(preparingOrders);
            parameters.Add("preparingOrders", myContent);

            myContent = JsonConvert.SerializeObject(statusObject);
            parameters.Add("statusObject", myContent);
            return await APIResult.post(method, parameters);
        }
        public async Task<string> generateOrderNumber(string orderCode, string branchCode, long branchId)
        {
            int sequence = await GetLastNumOfInv(orderCode, branchId);
            sequence++;
            string strSeq = sequence.ToString();
            if (sequence <= 999999)
                strSeq = sequence.ToString().PadLeft(6, '0');
            string invoiceNum = orderCode + "-" + branchCode + "-" + strSeq;
            return invoiceNum;
        }
        public async Task<int> GetLastNumOfInv(string orderCode, long branchId)
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
        public async Task<int> GetHallOrderCount(string status, long branchId, long duration)
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

        static public decimal calculateRemainingTime(DateTime preparingStatusDate, decimal preparingTime, string status)
        {
            decimal remainingTime = 0;
            if (status == "Listed")
            {
                remainingTime = (decimal)preparingTime;
            }
            else
            {
                preparingStatusDate = preparingStatusDate.AddMinutes((double)preparingTime);
                if (preparingStatusDate > DateTime.Now)
                {
                    TimeSpan remainingTimeTmp = preparingStatusDate - DateTime.Now;
                    remainingTime = (decimal)remainingTimeTmp.TotalMinutes;
                }
            }

            return remainingTime;
        }
        public async Task<List<OrderPreparing>> GetOrdersByInvoiceId(long invoiceId)
        {
            List<OrderPreparing> items = new List<OrderPreparing>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("invoiceId", invoiceId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetOrdersByInvoiceId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<OrderPreparing>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        public async Task<List<OrderPreparing>> GetOrdersforPrintByInvoiceId(long invoiceId)
        {
            List<OrderPreparing> items = new List<OrderPreparing>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("invoiceId", invoiceId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("OrderPreparing/GetOrdersforPrintByInvoiceId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<OrderPreparing>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }

    }
}
