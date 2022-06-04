using Newtonsoft.Json;
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
using Newtonsoft.Json.Converters;
using Restaurant.ApiClasses;

namespace Restaurant.Classes
{
    public class Package
    {
        public int packageId { get; set; }
        public Nullable<int> parentIUId { get; set; }
        public Nullable<int> childIUId { get; set; }
        public int quantity { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }

        public Nullable<System.DateTime> updateDate { get; set; }

        public bool canDelete { get; set; }


        public string itemName { get; set; }
        // item parent
        public Nullable<int> pitemId { get; set; }
        public string pcode { get; set; }
        public string pitemName { get; set; }

        public string type { get; set; }
        public string image { get; set; }


        //units
        public Nullable<int> punitId { get; set; }
        public string punitName { get; set; }

        //item chiled

        public Nullable<int> citemId { get; set; }
        public string ccode { get; set; }
        public string citemName { get; set; }

        public string ctype { get; set; }
        public string cimage { get; set; } 


        //units
        public Nullable<int> cunitId { get; set; }
        public string cunitName { get; set; }

          
        public async Task<List<Package>> GetChildsByParentId(int parentIUId)
        {
            List<Package> list = new List<Package>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("parentIUId", parentIUId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Package/GetChildsByParentId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<Package>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


            //List<Package> memberships = null;
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
            //    request.RequestUri = new Uri(Global.APIUri + "Package/GetChildsByParentId?parentIUId=" + parentIUId);
            //    request.Headers.Add("APIKey", Global.APIKey);
            //    request.Method = HttpMethod.Get;
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = await client.SendAsync(request);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var jsonString = await response.Content.ReadAsStringAsync();

            //        memberships = JsonConvert.DeserializeObject<List<Package>>(jsonString);

            //        return memberships;
            //    }
            //    else //web api sent error response 
            //    {
            //        memberships = new List<Package>();
            //    }
            //    return memberships;
            //}
        }

        public async Task<int> UpdatePackByParentId(int parentIUId, List<Package> newplist, int userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            var myContent = JsonConvert.SerializeObject(newplist);
            parameters.Add("Object", myContent);
            parameters.Add("parentIUId", parentIUId.ToString());
            parameters.Add("userId", userId.ToString());
            string method = "Package/UpdatePackByParentId";
           return await APIResult.post(method, parameters);

        }

        

    }
}
