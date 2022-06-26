using Newtonsoft.Json;
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
    public class ResidentialSectorsUsers
    {
        public long id { get; set; }
        public Nullable<long> residentSecId { get; set; }
        public Nullable<long> userId { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }


       

        public async Task<int> UpdateResSectorsByUserId(List<ResidentialSectorsUsers> newList, long userId, long updateUserId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ResidentialSectorsUsers/UpdateResSectorsByUserId";
            var newListParameter = JsonConvert.SerializeObject(newList);
            parameters.Add("newList", newListParameter);
            parameters.Add("userId", userId.ToString());
            parameters.Add("updateUserId", updateUserId.ToString());
            return await APIResult.post(method, parameters);
        }
    }
}
