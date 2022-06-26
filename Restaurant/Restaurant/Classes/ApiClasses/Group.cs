﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Restaurant;
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
using Restaurant.Classes;
using Restaurant.ApiClasses;

namespace Restaurant.Classes
{
    public class Group
    {
        public long groupId { get; set; }
        public string name { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public int isActive { get; set; }
        public Boolean canDelete { get; set; }

       



        public async Task<List<Group>> GetAll()
        {
            List<Group> list = new List<Group>();
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Group/Get");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<Group>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        // get users by groupId
        public async Task<List<User>> GetUsersByGroupId(long groupId)
        {

            List<User> list = new List<User>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("groupId", groupId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Group/GetUsersByGroupId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<User>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            //List<User> list = null;
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
            //    request.RequestUri = new Uri(Global.APIUri + "Group/GetUsersByGroupId?groupId="+ groupId);
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
            //        list = JsonConvert.DeserializeObject<List<User>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return list;
            //    }
            //    else //web api sent error response 
            //    {
            //        list = new List<User>();
            //    }
            //    return list;
            //}
        }
        public async Task<int> Save(Group newObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "Group/Save";

            var myContent = JsonConvert.SerializeObject(newObject);
            parameters.Add("Object", myContent);
           return await APIResult.post(method, parameters);

           
        }
     
        public async Task<int> Delete(long groupId, long userId, bool final)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("groupId", groupId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "Group/Delete";
           return await APIResult.post(method, parameters);

           
        }


        public async Task<int> UpdateGroupIdInUsers(long groupId, List<long> newList, long userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            var myContent = JsonConvert.SerializeObject(newList);
            parameters.Add("newList", myContent);
            parameters.Add("groupId", groupId.ToString());
            parameters.Add("userId", userId.ToString());

            string method = "Group/UpdateGroupIdInUsers";
           return await APIResult.post(method, parameters);

        }


    }
}

