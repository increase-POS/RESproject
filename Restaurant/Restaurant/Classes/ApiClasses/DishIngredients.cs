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
    public class DishIngredients
    {
        public int membershipId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string subscriptionType { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public byte isActive { get; set; }
        public Nullable<decimal> subscriptionFee { get; set; }
        public bool canDelete { get; set; }
        public bool isFreeDelivery { get; set; }
        public decimal deliveryDiscountPercent { get; set; }

        public async Task<List<DishIngredients>> GetAll()
        {
            List<DishIngredients> items = new List<DishIngredients>();
            IEnumerable<Claim> claims = await APIResult.getList("dishIngredients/GetAll");
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<DishIngredients>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
  

        public async Task<DishIngredients> GetById(int itemId)
        {
            DishIngredients item = new DishIngredients();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("dishIngredients/GetById", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<DishIngredients>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    break;
                }
            }
            return item;
        }
        public async Task<int> save(DishIngredients item)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "dishIngredients/Save";
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
            string method = "dishIngredients/Delete";
           return await APIResult.post(method, parameters);
        }
    

    }
}
