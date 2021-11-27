using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POS;
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
namespace POS.Classes
{

    class updateCategoryuser
    {
        public int id { get; set; }
        public Nullable<int> categoryId { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<int> sequence { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }

    }
    class Categoryuser
    {
        public int id { get; set; }
        public Nullable<int> categoryId { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<int> sequence { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }

        // category
        public string name { get; set; }
        public string categoryCode { get; set; }

        public string details { get; set; }
        public string image { get; set; }
        public string notes { get; set; }
        public Nullable<int> parentId { get; set; }
        public Nullable<decimal> taxes { get; set; }

        public Nullable<short> isActive { get; set; }

        //public async Task<List<Categoryuser>> GetAll()
        //{
        //    List<Categoryuser> list = null;
        //    // ... Use HttpClient.
        //    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        //    using (var client = new HttpClient())
        //    {
        //        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //        client.BaseAddress = new Uri(Global.APIUri);
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        //        client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
        //        HttpRequestMessage request = new HttpRequestMessage();
        //        request.RequestUri = new Uri(Global.APIUri + "categoryuser/Get");
        //        request.Headers.Add("APIKey", Global.APIKey);
        //        request.Method = HttpMethod.Get;
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        HttpResponseMessage response = await client.SendAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var jsonString = await response.Content.ReadAsStringAsync();
        //            jsonString = jsonString.Replace("\\", string.Empty);
        //            jsonString = jsonString.Trim('"');
        //            // fix date format
        //            JsonSerializerSettings settings = new JsonSerializerSettings
        //            {
        //                Converters = new List<JsonConverter> { new BadDateFixingConverter() },
        //                DateParseHandling = DateParseHandling.None
        //            };
        //            list = JsonConvert.DeserializeObject<List<Categoryuser>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
        //            return list;
        //        }
        //        else //web api sent error response 
        //        {
        //            list = new List<Categoryuser>();
        //        }
        //        return list;
        //    }
        //}

        public async Task<List<Categoryuser>> GetByUserId(int userId)
        {

            List<Categoryuser> list = new List<Categoryuser>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("userId", userId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("categoryuser/GetByUserId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<Categoryuser>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            //List<Categoryuser> list = null;
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
            //    request.RequestUri = new Uri(Global.APIUri + "categoryuser/GetByUserId?userId=" + userId);
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
            //        list = JsonConvert.DeserializeObject<List<Categoryuser>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return list;
            //    }
            //    else //web api sent error response 
            //    {
            //        list = new List<Categoryuser>();
            //    }
            //    return list;
            //}
        }


        //public async Task<string> Save(Categoryuser newObject)
        //{
        //    // ... Use HttpClient.
        //    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        //    // 
        //    var myContent = JsonConvert.SerializeObject(newObject);

        //    using (var client = new HttpClient())
        //    {
        //        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //        client.BaseAddress = new Uri(Global.APIUri);
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        //        client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
        //        HttpRequestMessage request = new HttpRequestMessage();
        //        // encoding parameter to get special characters
        //        myContent = HttpUtility.UrlEncode(myContent);
        //        request.RequestUri = new Uri(Global.APIUri
        //                                     + "categoryuser/Save?newObject="
        //                                     + myContent);
        //        request.Headers.Add("APIKey", Global.APIKey);
        //        request.Method = HttpMethod.Post;
        //        //set content type
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var response = await client.SendAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var message = await response.Content.ReadAsStringAsync();
        //            message = JsonConvert.DeserializeObject<string>(message);
        //            return message;
        //        }
        //        return "";
        //    }
        //}


        //public async Task<Categoryuser> GetByID(int id)
        //{

        //    Categoryuser item = new Categoryuser();
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    parameters.Add("Id", id.ToString());
        //    //#################
        //    IEnumerable<Claim> claims = await APIResult.getList("categoryuser/GetByID", parameters);

        //    foreach (Claim c in claims)
        //    {
        //        if (c.Type == "scopes")
        //        {
        //            item = JsonConvert.DeserializeObject<Categoryuser>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
        //            break;
        //        }
        //    }
        //    return item;


        //    //Categoryuser Object = new Categoryuser();

        //    //// ... Use HttpClient.
        //    //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        //    //using (var client = new HttpClient())
        //    //{
        //    //    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //    //    client.BaseAddress = new Uri(Global.APIUri);
        //    //    client.DefaultRequestHeaders.Clear();
        //    //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        //    //    client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
        //    //    HttpRequestMessage request = new HttpRequestMessage();
        //    //    request.RequestUri = new Uri(Global.APIUri + "categoryuser/GetByID");
        //    //    request.Headers.Add("Id", id.ToString());
        //    //    request.Headers.Add("APIKey", Global.APIKey);
        //    //    request.Method = HttpMethod.Get;
        //    //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    //    var response = await client.SendAsync(request);

        //    //    if (response.IsSuccessStatusCode)
        //    //    {
        //    //        var jsonString = await response.Content.ReadAsStringAsync();

        //    //        Object = JsonConvert.DeserializeObject<Categoryuser>(jsonString);

        //    //        return Object;
        //    //    }

        //    //    return Object;
        //    //}
        //}


        //public async Task<Boolean> Delete(int Id)
        //{
        //    // ... Use HttpClient.
        //    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //    using (var client = new HttpClient())
        //    {
        //        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //        client.BaseAddress = new Uri(Global.APIUri);
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        //        client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
        //        HttpRequestMessage request = new HttpRequestMessage();
        //        request.RequestUri = new Uri(Global.APIUri + "categoryuser/Delete?Id=" + Id);

        //        request.Headers.Add("APIKey", Global.APIKey);

        //        request.Method = HttpMethod.Post;

        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var response = await client.SendAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //}


        // update list
        public async Task<int> UpdateCatUserList(int userId, List<updateCategoryuser> newlist)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
         
    var myContent = JsonConvert.SerializeObject(newlist);
            parameters.Add("newlist", myContent);
            parameters.Add("userId", userId.ToString());
         
            string method = "categoryuser/UpdateCatUserList";
           return await APIResult.post(method, parameters);

        
            //#################
          

            //string message = "";
            //// ... Use HttpClient.
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            //// 
            //var myContent = JsonConvert.SerializeObject(newlist);

            //using (var client = new HttpClient())
            //{
            //    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //    client.BaseAddress = new Uri(Global.APIUri);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
            //    HttpRequestMessage request = new HttpRequestMessage();
            //    // encoding parameter to get special characters
            //    myContent = HttpUtility.UrlEncode(myContent);
            //    request.RequestUri = new Uri(Global.APIUri + "categoryuser/UpdateCatUserList?newlist=" + myContent);
            //    request.Headers.Add("APIKey", Global.APIKey);
            //   request.Headers.Add("userId", userId.ToString());
            //    request.Method = HttpMethod.Post;
            //    //set content type
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var response = await client.SendAsync(request);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        message = await response.Content.ReadAsStringAsync();
            //        message = JsonConvert.DeserializeObject<string>(message);
            //    }
            //    return message;
            //}
        }
        //

        //public async Task<string> UpdateCatByUsrId(int userId, List<Categoryuser> newlist)
        //{

        //    string message = "";
        //    // ... Use HttpClient.
        //    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        //    // 
        //    var myContent = JsonConvert.SerializeObject(newlist);

        //    using (var client = new HttpClient())
        //    {
        //        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //        client.BaseAddress = new Uri(Global.APIUri);
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        //        client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
        //        HttpRequestMessage request = new HttpRequestMessage();
        //        // encoding parameter to get special characters
        //        myContent = HttpUtility.UrlEncode(myContent);
        //        request.RequestUri = new Uri(Global.APIUri + "categoryuser/UpdateCatByUsrId?newlist=" + myContent);
        //        request.Headers.Add("APIKey", Global.APIKey);
        //        request.Headers.Add("userId", userId.ToString());
        //        request.Method = HttpMethod.Post;
        //        //set content type
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var response = await client.SendAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            message = await response.Content.ReadAsStringAsync();
        //            message = JsonConvert.DeserializeObject<string>(message);
        //        }
        //        return message;
        //    }
        //}


    }
}

