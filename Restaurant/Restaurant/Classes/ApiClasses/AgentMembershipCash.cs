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
    public class AgentMembershipCash
    {
        public int agentMembershipsId { get; set; }
        public Nullable<int> subscriptionFeesId { get; set; }
        public Nullable<int> cashTransId { get; set; }
        public Nullable<int> membershipId { get; set; }
        public Nullable<int> agentId { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public byte isActive { get; set; }

        public decimal Amount { get; set; }

        public bool canDelete { get; set; }


        public async Task<List<AgentMembershipCash>> GetAll()
        {
            List<AgentMembershipCash> items = new List<AgentMembershipCash>();
            IEnumerable<Claim> claims = await APIResult.getList("AgentMembershipCash/GetAll");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<AgentMembershipCash>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }

        public async Task<AgentMembershipCash> GetById(int itemId)
        {
            AgentMembershipCash item = new AgentMembershipCash();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("AgentMembershipCash/GetById", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<AgentMembershipCash>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    break;
                }
            }
            return item;
        }
        public async Task<int> save(AgentMembershipCash item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "AgentMembershipCash/Save";
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
            string method = "AgentMembershipCash/Delete";
           return await APIResult.post(method, parameters);
        }

   
    }
}