using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
using Restaurant.ApiClasses;

namespace Restaurant.Classes
{
    public class ItemUnit
    {
        public long itemUnitId { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long> unitId { get; set; }
        public Nullable<int> unitValue { get; set; }
        public short defaultSale { get; set; }
        public short defaultPurchase { get; set; }
        public decimal price { get; set; }
        public decimal priceWithService { get; set; }
        public string barcode { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> subUnitId { get; set; }
        public decimal purchasePrice { get; set; }
        public Nullable<long> storageCostId { get; set; }
        public byte isActive { get; set; }


        public string mainUnit { get; set; }
        public string smallUnit { get; set; }
        public string countSmallUnit { get; set; }

        public string itemName { get; set; }
        public string itemCode { get; set; }
        public string unitName { get; set; }

        public Boolean canDelete { get; set; }

        public Nullable<decimal> taxes { get; set; }
        public Nullable<decimal> priceTax { get; set; }

        public string type { get; set; }
        public Nullable<long> categoryId { get; set; }
        //**************************************************
        //*************** item unit methods *********************     
        public async Task<ItemUnit> GetById(long itemUnitId)
        {
            ItemUnit item = new ItemUnit();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemUnitId", itemUnitId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsUnits/GetById", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item =JsonConvert.DeserializeObject<ItemUnit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                }
            }
            return item;
        }
        public async Task<List<ItemUnit>> GetItemUnits(long itemId = 0)
        {

            List<ItemUnit> list = new List<ItemUnit>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("ItemsUnits/Get",parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemUnit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;         
        }

        public List<ItemUnit> GetIUbyItem(long itemId, List<ItemUnit>AllIU,List<Unit>AllUnits)
        {

            if (AllIU is null)
                FillCombo.RefreshItemUnit();
            if (AllUnits is null)
                FillCombo.RefreshUnit();

            //AllIU = FillCombo.itemUnitList;
            //AllUnits = FillCombo.unitsList;

            List<ItemUnit> itemUnitsList = new List<ItemUnit>();
            try
            {
                itemUnitsList = ( from IU in AllIU
                            where (IU.itemId == itemId && IU.isActive == 1)
                            join U in AllUnits on IU.unitId equals U.unitId into lj

                            from v in lj.DefaultIfEmpty()
                            join u1 in AllUnits on IU.subUnitId equals u1.unitId into tj
                            from v1 in tj.DefaultIfEmpty()
                            select new ItemUnit()
                            {
                                itemUnitId = IU.itemUnitId,
                                unitId = IU.unitId,
                                mainUnit = v.name,
                                createDate = IU.createDate,
                                createUserId = IU.createUserId,
                                defaultPurchase = IU.defaultPurchase,
                                defaultSale = IU.defaultSale,
                                price = IU.price,
                                subUnitId = IU.subUnitId,
                                smallUnit = v1.name,
                                unitValue = IU.unitValue,
                                barcode = IU.barcode,
                                updateDate = IU.updateDate,
                                updateUserId = IU.updateUserId,
                                storageCostId = IU.storageCostId,

                            }).ToList();

                    return itemUnitsList;
              
            }
            catch
            {
                return itemUnitsList;
            }
        }

        //***************************************
        // add or update item unit
        //***************************************
        public async Task<int> saveItemUnit(ItemUnit itemUnit)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "ItemsUnits/Save";

            var myContent = JsonConvert.SerializeObject(itemUnit);
            parameters.Add("Object", myContent);
           return await APIResult.post(method, parameters);    
        }
        //***************************************
        // delete item unit (barcode)
        //***************************************
        public async Task<int> Delete(long ItemUnitId, long userId, bool final)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("ItemUnitId", ItemUnitId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("final", final.ToString());
            string method = "ItemsUnits/Delete";
           return await APIResult.post(method, parameters);

        }

        public async Task<List<ItemUnit>> Getall()
        {
            List<ItemUnit> list = new List<ItemUnit>();
            IEnumerable<Claim> claims = await APIResult.getList("ItemsUnits/GetallItemsUnits");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemUnit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
          
        }
            
        public async Task<List<ItemUnit>> GetIU()
        {
            List<ItemUnit> list = new List<ItemUnit>();
            IEnumerable<Claim> claims = await APIResult.getList("ItemsUnits/GetIU");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemUnit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
        }
           
        public async Task<List<ItemUnit>> getSmallItemUnits(long itemId, long itemUnitId)
        {

            List<ItemUnit> list = new List<ItemUnit>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("itemId", itemId.ToString());
            parameters.Add("itemUnitId", itemUnitId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("itemsUnits/getSmallItemUnits", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemUnit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
           
        }
        public async Task<int> largeToSmallUnitQuan(long fromItemUnit, long toItemUnit)
        {
            int AvailableAmount = 0;
           
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("fromItemUnit", fromItemUnit.ToString());
            parameters.Add("toItemUnit", toItemUnit.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("itemsUnits/largeToSmallUnitQuan", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    AvailableAmount = int.Parse(c.Value);
                }
            }
            return AvailableAmount;

        }
        public async Task<int> smallToLargeUnit(long fromItemUnit, long toItemUnit)
        {
            int AvailableAmount = 0;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("fromItemUnit", fromItemUnit.ToString());
            parameters.Add("toItemUnit", toItemUnit.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("itemsUnits/smallToLargeUnitQuan",parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    AvailableAmount = int.Parse(c.Value);
                }
            }
            return AvailableAmount;

        }
        public async Task<int> fromUnitToUnitQuantity(int quantity, long itemId, long fromItemUnitId, long toItemUnitId)
        {
            int remain = 0;
            int _ConversionQuantity;
            int _ToQuantity = 0;

            if (quantity != 0)
            {
                List<ItemUnit> smallUnits = await getSmallItemUnits(itemId, fromItemUnitId);

                var isSmall = smallUnits.Find(x => x.itemUnitId == toItemUnitId);
                if (isSmall != null) // from-unit is bigger than to-unit
                {
                    _ConversionQuantity = await largeToSmallUnitQuan(fromItemUnitId, toItemUnitId);
                    _ToQuantity = quantity * _ConversionQuantity;

                }
                else
                {
                    _ConversionQuantity = await smallToLargeUnit(fromItemUnitId, toItemUnitId);

                    if (_ConversionQuantity != 0)
                    {
                        _ToQuantity = quantity / _ConversionQuantity;
                        remain = quantity - (_ToQuantity * _ConversionQuantity); // get remain quantity which cannot be changeed
                    }
                }
            }

            return _ToQuantity;
        }
    }
}
