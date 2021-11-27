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
using System.Security.Claims;

namespace POS.Classes
{
    class ItemsProp
    {
        public int itemPropId { get; set; }
        public Nullable<int> propertyItemId { get; set; }
        public Nullable<int> itemId { get; set; }

        public string propValue { get; set; }
        public string propName { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }

        public async Task<int> Save(ItemsProp itemsProp)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsProp/Save";

            var myContent = JsonConvert.SerializeObject(itemsProp);
            parameters.Add("Object", myContent);
           return await APIResult.post(method, parameters);

            //string message = "";
            //// ... Use HttpClient.
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            //var myContent = JsonConvert.SerializeObject(itemsProp);

            //using (var client = new HttpClient())
            //{
            //    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //    client.BaseAddress = new Uri(Global.APIUri);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    client.DefaultRequestHeaders.Add("Keep-Alive", "3600");

            //    HttpRequestMessage request = new HttpRequestMessage();
            //    // set unicode
            //    request.Content = new StringContent(myContent, System.Text.Encoding.UTF8, "text/xml");

            //    request.RequestUri = new Uri(Global.APIUri + "ItemsProp/Save?itemsPropObject=" + myContent);
            //    request.Headers.Add("APIKey", Global.APIKey);
            //    //set content type
            //    request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("charset", "utf-8"));
            //    request.Method = HttpMethod.Post;

            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var response = await client.SendAsync(request);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        return true;
            //    }
            //    return false;
            //}
        }

        public async Task<int> Delete(int itemPropId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemPropId", itemPropId.ToString());
       
            string method = "itemsProp/Delete";
           return await APIResult.post(method, parameters);



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
            //    request.RequestUri = new Uri(Global.APIUri + "itemsProp/Delete?itemPropId=" + itemPropId );
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

        public async Task<List<ItemsProp>> Get(int itemId)
        {


            List<ItemsProp> list = new List<ItemsProp>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsProp/Get");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemsProp>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            //List<ItemsProp> ItemsProps = null;
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
            //    request.RequestUri = new Uri(Global.APIUri + "ItemsProp/Get?itemId="+itemId);
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
            //        ItemsProps = JsonConvert.DeserializeObject<List<ItemsProp>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return ItemsProps;
            //    }
            //    else //web api sent error response 
            //    {
            //        ItemsProps = new List<ItemsProp>();
            //    }
            //    return ItemsProps;
            //}
        }
    }
}
