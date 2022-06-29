using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Restaurant.ApiClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes.ApiClasses
{
    public class itemsTransferIngredients
    {
        public long itemsTransIngredId { get; set; }
        public Nullable<long> itemsTransId { get; set; }
        public Nullable<long> dishIngredId { get; set; }
        public string DishIngredientName { get; set; }
        public string itemName { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }

        public async Task<List<itemsTransferIngredients>> GetItemIngredients(long itemTransferId, long itemUnitId=0)
        {
            List<itemsTransferIngredients> items = new List<itemsTransferIngredients>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemTransferId", itemTransferId.ToString());
            parameters.Add("itemUnitId", itemUnitId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("itemsTransfer/GetItemIngredients", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<itemsTransferIngredients>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }

    }
}
