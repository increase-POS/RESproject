﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Restaurant.ApiClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Restaurant.Classes
{
    public class SubscriptionFees
    {
        public int subscriptionFeesId { get; set; }
        public Nullable<int> membershipId { get; set; }
        public int monthsCount { get; set; }
        public decimal Amount { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public byte isActive { get; set; }

        public bool canDelete { get; set; }


        public async Task<List<SubscriptionFees>> GetAll()
        {
            List<SubscriptionFees> items = new List<SubscriptionFees>();
            IEnumerable<Claim> claims = await APIResult.getList("subscriptionFees/GetAll");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<SubscriptionFees>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }

      
        public async Task<int> save(SubscriptionFees item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "subscriptionFees/Save";
            var myContent = JsonConvert.SerializeObject(item);
            parameters.Add("itemObject", myContent);
           return await APIResult.post(method, parameters);
        }
        public async Task<int> delete(int itemId, int userId, Boolean final)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "subscriptionFees/Delete";
           return await APIResult.post(method, parameters);
        }
    }
}
