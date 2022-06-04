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
    public class SysEmails
    {

        public int emailId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public bool isSSL { get; set; }
        public string smtpClient { get; set; }
        public string side { get; set; }
        public string notes { get; set; }
        public Nullable<int> branchId { get; set; }
        public byte isActive { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public bool isMajor { get; set; }

        public string branchName { get; set; }
        public bool canDelete { get; set; }

         
        /// <summary>
        /// ///////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        /// 


        public async Task<List<SysEmails>> Get()
        {


            List<SysEmails> list = new List<SysEmails>();
         
            IEnumerable<Claim> claims = await APIResult.getList("SysEmails/Get");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<SysEmails>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
           

        }

        public async Task<int> Save(SysEmails obj)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "SysEmails/Save";

            var myContent = JsonConvert.SerializeObject(obj);
            parameters.Add("Object", myContent);
           return await APIResult.post(method, parameters);


        }

      
        public async Task<SysEmails> GetByBranchIdandSide(int branchId, string side)
        {
            SysEmails item = new SysEmails();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("branchId", branchId.ToString());
            parameters.Add("side", side.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("SysEmails/GetByBranchIdandSide", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<SysEmails>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    break;
                }
            }
            return item;

        }

        public async Task<int> Delete(int emailId, int userId, bool final)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("emailId", emailId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
          

            string method = "SysEmails/Delete";
           return await APIResult.post(method, parameters);

        }





    }
}
