using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using Restaurant.ApiClasses;

namespace Restaurant.Classes
{
    public class ItemLocation
    {

        public long itemsLocId { get; set; }
        public Nullable<long> locationId { get; set; }
        public long quantity { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public string notes { get; set; }
        public Nullable<long> invoiceId { get; set; }

        public int sequence { get; set; }

        public Nullable<long> sectionId { get; set; }


        public string itemName { get; set; }
        public string location { get; set; }
        public string section { get; set; }
        public string unitName { get; set; }
        public string itemType { get; set; }
        public Nullable<decimal> storeCost { get; set; }
        public Nullable<byte> isFreeZone { get; set; }

        public string invNumber { get; set; }

       
        public Nullable<bool> isSelected { get; set; }

        //****************************************************
        public async Task<List<ItemLocation>> get(long branchId)
        {
            List<ItemLocation> list = new List<ItemLocation>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
           
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsLocations/Get",parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemLocation>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;          
        }
        public async Task<List<ItemLocation>> getAll(long branchId)
        {

            List<ItemLocation> list = new List<ItemLocation>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsLocations/getAll",parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemLocation>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
          
        }
    
        public async Task<List<ItemLocation>> GetFreeZoneItems(long branchId)
        {
            List<ItemLocation> list = new List<ItemLocation>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsLocations/GetFreeZoneItems",parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemLocation>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
            
        }
             
        public async Task<int> decreaseAmountsInKitchen(List<ItemTransfer> invoiceItems, long branchId, long userId)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/decreaseAmountsInKitchen";

            var myContent = JsonConvert.SerializeObject(invoiceItems);
            parameters.Add("Object", myContent);
            parameters.Add("branchId", branchId.ToString());
            parameters.Add("userId", userId.ToString());

           return await APIResult.post(method, parameters);
        }
       
        public async Task<int> unitsConversion(long branchId,int fromItemUnit , int toItemUnt, int fromQuantity,int toQuantity, long userId, ItemUnit smallUnit)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/unitsConversion";

        
            parameters.Add("branchId", branchId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("fromItemUnit", fromItemUnit.ToString());
            parameters.Add("toItemUnt", toItemUnt.ToString());
            parameters.Add("fromQuantity", fromQuantity.ToString());
            parameters.Add("toQuantity", toQuantity.ToString());



            var myContent = JsonConvert.SerializeObject(smallUnit);
            parameters.Add("Object", myContent);

           return await APIResult.post(method, parameters);



            //// ... Use HttpClient.
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            //if (smallUnit == null)
            //    smallUnit = new ItemUnit();
            //var myContent = JsonConvert.SerializeObject(smallUnit);
            //using (var client = new HttpClient())
            //{
            //    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //    client.BaseAddress = new Uri(Global.APIUri);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
            //    HttpRequestMessage request = new HttpRequestMessage();
            //    request.RequestUri = new Uri(Global.APIUri + "ItemsLocations/unitsConversion?branchId=" + branchId+ "&fromItemUnit="+ fromItemUnit + "&toItemUnit="+toItemUnt+
            //                                                    "&fromQuantity="+fromQuantity + "&toQuantity="+ toQuantity + "&userId=" +userId + "&smallUnit=" + myContent);
            //    request.Headers.Add("APIKey", Global.APIKey);
            //    request.Method = HttpMethod.Post;
            //    //set content type
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var response = await client.SendAsync(request);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        return true;
            //    }
            //    return false;
            //}
        }
        public async Task<int> decreaseItemLocationQuantity(long itemLocId ,int quantity, long userId, string objectName, Notification notification)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/decreaseItemLocationQuantity";


            parameters.Add("itemLocId", itemLocId.ToString());
            parameters.Add("quantity", quantity.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("objectName", objectName);        

            var myContent = JsonConvert.SerializeObject(notification);
            parameters.Add("Object", myContent);

           return await APIResult.post(method, parameters);

        }
        public async Task<int> trasnferItem(long itemLocId ,ItemLocation itemLocation)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/trasnferItem";


            parameters.Add("itemLocId", itemLocId.ToString());
   

            var myContent = JsonConvert.SerializeObject(itemLocation);
            parameters.Add("Object", myContent);

           return await APIResult.post(method, parameters);
        }
      
        public async Task<int> getAmountInBranch(long itemUnitId, long branchId,int isKitchen = 0)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/getAmountInBranch";

            parameters.Add("branchId", branchId.ToString());
            parameters.Add("itemUnitId", itemUnitId.ToString());
            parameters.Add("isKitchen", isKitchen.ToString());
           return await APIResult.post(method, parameters);
        }
        
        public async Task<int> getUnitAmount(long itemUnitId, long branchId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/getUnitAmount";


            parameters.Add("itemUnitId", itemUnitId.ToString());
            parameters.Add("branchId", branchId.ToString());
   
           return await APIResult.post(method, parameters);

        }
      
        public async Task recieptInvoice(List<ItemTransfer> invoiceItems, long branchId, long userId, string objectName, Notification notificationObj)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/receiptInvoice";

            var myContent = JsonConvert.SerializeObject(invoiceItems);
            parameters.Add("Object", myContent);

            var myContent1 = JsonConvert.SerializeObject(notificationObj);
            parameters.Add("notificationObj", myContent1);

            parameters.Add("userId", userId.ToString());

            parameters.Add("branchId", branchId.ToString());
            parameters.Add("objectName", objectName);

           await APIResult.post(method, parameters);

        }
       
        public async Task<int> recieptOrder(List<ItemLocation> invoiceItems,List<ItemTransfer> orderList, long toBranch, long userId, string objectName, Notification notificationObj)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/receiptOrder";

            var myContent = JsonConvert.SerializeObject(invoiceItems);
            parameters.Add("Object", myContent);

            var myContent2 = JsonConvert.SerializeObject(orderList);
            parameters.Add("orderList", myContent2);

            var myContent1 = JsonConvert.SerializeObject(notificationObj);
            parameters.Add("notificationObj", myContent1);

            parameters.Add("userId", userId.ToString());

            parameters.Add("toBranch", toBranch.ToString());
            parameters.Add("objectName", objectName);

           return await APIResult.post(method, parameters);
        }
        public async Task<int> transferToKitchen(List<ItemLocation> invoiceItems,List<ItemTransfer> orderList, long branchId, long userId)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/transferToKitchen";

            var myContent = JsonConvert.SerializeObject(invoiceItems);
            parameters.Add("Object", myContent);

            var myContent2 = JsonConvert.SerializeObject(orderList);
            parameters.Add("orderList", myContent2);

            parameters.Add("userId", userId.ToString());

            parameters.Add("branchId", branchId.ToString());

           return await APIResult.post(method, parameters);
        }
        
        public async Task<int> returnSpendingOrder(List<ItemTransfer> invoiceItems, long branchId, long userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/returnSpendingOrder";

            var myContent = JsonConvert.SerializeObject(invoiceItems);
            parameters.Add("Object", myContent);
            parameters.Add("userId", userId.ToString());
            parameters.Add("branchId", branchId.ToString());
          
           return await APIResult.post(method, parameters);
        }
        public async Task<int> transferAmountbetweenUnits(long locationId, long itemLocId, long toItemUnitId,int fromQuantity, int toQuantity, long userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsLocations/transferAmountbetweenUnits";        

            parameters.Add("locationId", locationId.ToString());
            parameters.Add("itemLocId", itemLocId.ToString());
            parameters.Add("toItemUnitId", toItemUnitId.ToString());
            parameters.Add("fromQuantity", fromQuantity.ToString());
            parameters.Add("toQuantity", toQuantity.ToString());
            parameters.Add("userId", userId.ToString());


           return await APIResult.post(method, parameters);
        }
        public async Task<List<ItemLocation>> getSpecificItemLocation(string itemUnitsIds, long branchId)
        {

            List<ItemLocation> list = new List<ItemLocation>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemUnitsIds", itemUnitsIds.ToString());
            parameters.Add("branchId", branchId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsLocations/getSpecificItemLocation", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemLocation>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
         


            //List<ItemLocation> items = null;
            //var myContent = JsonConvert.SerializeObject(itemUnitsIds);
            //// ... Use HttpClient.
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            //using (var client = new HttpClient())
            //{
            //    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //    client.BaseAddress = new Uri(Global.APIUri);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
            //    HttpRequestMessage request = new HttpRequestMessage();
            //    request.RequestUri = new Uri(Global.APIUri + "ItemsLocations/getSpecificItemLocation?itemUnitsIds=" + myContent+"&branchId= " + branchId);
            //    request.Headers.Add("APIKey", Global.APIKey);
            //    request.Method = HttpMethod.Get;
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = await client.SendAsync(request);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var jsonString = await response.Content.ReadAsStringAsync();
            //        jsonString = jsonString.Replace("\\", string.Empty);
            //        jsonString = jsonString.Trim('"');
            //        // fix date format
            //        JsonSerializerSettings settings = new JsonSerializerSettings
            //        {
            //            Converters = new List<JsonConverter> { new BadDateFixingConverter() },
            //            DateParseHandling = DateParseHandling.None
            //        };
            //        items = JsonConvert.DeserializeObject<List<ItemLocation>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return items;
            //    }
            //    else //web api sent error response 
            //    {
            //        items = new List<ItemLocation>();
            //    }
            //    return items;
            //}
        }
    }
}
