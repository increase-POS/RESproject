using Restaurant.Classes.ApiClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes
{
    public class BillDetailsSales
    {
        public int index { get; set; }
        public string image { get; set; }
        public long itemId { get; set; }
        public long itemUnitId { get; set; }
        public string itemName { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public decimal basicPrice { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
        public long? offerId { get; set; }
        public decimal OfferValue { get; set; }
        public string OfferType { get; set; }
        public string forAgents { get; set; }
        public long itemsTransId { get; set; }

        public List<itemsTransferIngredients> itemsIngredients { get; set; }
    }


    public class BillDetailsPurchase
    {
        public int ID { get; set; }
        public long itemId { get; set; }
        public long itemUnitId { get; set; }
        public string Product { get; set; }
        public string Unit { get; set; }
        public string UnitName { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public long OrderId { get; set; }
        
    }
}
