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

    public class invoiceClassDiscount
    {
        public long invClassDiscountId { get; set; }
        public Nullable<long> invClassId { get; set; }
        public Nullable<long> invoiceId { get; set; }
        public decimal discountValue { get; set; }
        public byte discountType { get; set; }
    }
    public class InvoicesClass
    {
        public long invClassId { get; set; }
        public string name { get; set; }
        public decimal minInvoiceValue { get; set; }
        public decimal maxInvoiceValue { get; set; }
        public decimal discountValue { get; set; }
        public byte discountType { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> createUserId { get; set; }
        public string notes { get; set; }
        public byte isActive { get; set; }

        public bool canDelete { get; set; }

        public long invClassMemberId { get; set; }
        public Nullable<long> membershipId { get; set; }
        public Nullable<decimal> finalDiscount { get; set; }

        public async Task<List<InvoicesClass>> GetAll()
        {
            List<InvoicesClass> items = new List<InvoicesClass>();
            IEnumerable<Claim> claims = await APIResult.getList("invoicesClass/GetAll");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<InvoicesClass>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }

        public async Task<InvoicesClass> GetById(long itemId)
        {
            InvoicesClass item = new InvoicesClass();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("invoicesClass/GetById", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<InvoicesClass>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    break;
                }
            }
            return item;
        }
        public async Task<long> save(InvoicesClass item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "invoicesClass/Save";
            var myContent = JsonConvert.SerializeObject(item);
            parameters.Add("itemObject", myContent);
           return await APIResult.post(method, parameters);
        }
        public async Task<long> delete(long itemId, long userId, Boolean final)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "invoicesClass/Delete";
           return await APIResult.post(method, parameters);
        }

        public async Task<List<InvoicesClass>> GetInvclassByMembershipId(long invClassId)
        {
            List<InvoicesClass> items = new List<InvoicesClass>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", invClassId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("invoicesClass/GetInvclassByMembershipId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<InvoicesClass>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));

                }
            }
            return items;
        }
    }
}
