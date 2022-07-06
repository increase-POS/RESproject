using Newtonsoft.Json;
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
using Restaurant.ApiClasses;
using Restaurant.Classes.ApiClasses;

namespace Restaurant.Classes
{
    public class OpenClosOperatinModel
    {
        public long cashTransId { get; set; }
        public string transType { get; set; }
        public Nullable<long> posId { get; set; }
        public Nullable<long> userId { get; set; }
        public Nullable<long> agentId { get; set; }
        public Nullable<long> invId { get; set; }
        public string transNum { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<decimal> cash { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> createUserId { get; set; }
        public string notes { get; set; }
        public Nullable<long> posIdCreator { get; set; }
        public Nullable<byte> isConfirm { get; set; }
        public Nullable<long> cashTransIdSource { get; set; }
        public string side { get; set; }
        public string opSideNum { get; set; }
        public string docName { get; set; }
        public string docNum { get; set; }
        public string docImage { get; set; }
        public Nullable<long> bankId { get; set; }
        public string bankName { get; set; }
        public string agentName { get; set; }
        public string usersName { get; set; }
        public string usersLName { get; set; }
        public string posName { get; set; }
        public string posCreatorName { get; set; }
        public Nullable<byte> isConfirm2 { get; set; }
        public long cashTrans2Id { get; set; }
        public Nullable<long> pos2Id { get; set; }

        public string pos2Name { get; set; }
        public string processType { get; set; }
        public Nullable<long> cardId { get; set; }
        public Nullable<long> bondId { get; set; }
        public string createUserName { get; set; }
        public string updateUserName { get; set; }
        public string updateUserJob { get; set; }
        public string updateUserAcc { get; set; }
        public string createUserJob { get; set; }
        public string createUserLName { get; set; }
        public string updateUserLName { get; set; }
        public string cardName { get; set; }
        public Nullable<System.DateTime> bondDeserveDate { get; set; }
        public Nullable<byte> bondIsRecieved { get; set; }
        public string agentCompany { get; set; }
        public Nullable<long> shippingCompanyId { get; set; }
        public string shippingCompanyName { get; set; }
        public string userAcc { get; set; }

        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorname { get; set; }
        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public Nullable<long> branch2Id { get; set; }
        public string branch2Name { get; set; }




    }
    public class POSOpenCloseModel
    {
        public long cashTransId { get; set; }
        public string transType { get; set; }
        public Nullable<long> posId { get; set; }

        public string transNum { get; set; }

        public Nullable<decimal> cash { get; set; }//close

        public string notes { get; set; }

        public Nullable<byte> isConfirm { get; set; }
        public Nullable<long> cashTransIdSource { get; set; }
        public string side { get; set; }

        public string posName { get; set; }



        public string processType { get; set; }


        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }

        public Nullable<System.DateTime> updateDate { get; set; }//close
        public Nullable<System.DateTime> openDate { get; set; }
        public Nullable<decimal> openCash { get; set; }
        public Nullable<long> openCashTransId { get; set; }



    }
    public class ItemTransferInvoiceTax
    {// new properties
        public Nullable<System.DateTime> updateDate { get; set; }




        public string agentCompany { get; set; }




        // ItemTransfer
        public long ITitemsTransId { get; set; }
        public Nullable<long> ITitemUnitId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> ITitemId { get; set; }
        public Nullable<long> ITunitId { get; set; }
        public string ITitemName { get; set; }
        public string ITunitName { get; set; }

        public Nullable<long> ITquantity { get; set; }
        public Nullable<decimal> ITprice { get; set; }




        public string ITnotes { get; set; }

        public string ITbarcode { get; set; }

        //invoice
        public long invoiceId { get; set; }
        public string invNumber { get; set; }
        public string invBarcode { get; set; }
        public Nullable<long> agentId { get; set; }

        public string invType { get; set; }
        public string discountType { get; set; }

        public Nullable<decimal> discountValue { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }
        public Nullable<decimal> paid { get; set; }
        public Nullable<decimal> deserved { get; set; }
        public Nullable<System.DateTime> deservedDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }
        public Nullable<System.DateTime> IupdateDate { get; set; }
        public Nullable<long> IupdateUserId { get; set; }

        public string invCase { get; set; }

        public string Inotes { get; set; }
        public string vendorInvNum { get; set; }

        public string branchName { get; set; }
        public string posName { get; set; }
        public Nullable<System.DateTime> vendorInvDate { get; set; }
        public Nullable<long> branchId { get; set; }

        public Nullable<decimal> tax { get; set; }
        public Nullable<int> taxtype { get; set; }
        public Nullable<long> posId { get; set; }

        public string ITtype { get; set; }

        public string branchType { get; set; }

        public string posCode { get; set; }
        public string agentName { get; set; }

        public string agentType { get; set; }
        public string agentCode { get; set; }

        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }
        public Nullable<decimal> itemUnitPrice { get; set; }

        public Nullable<decimal> totalwithTax { get; set; }
        public Nullable<decimal> totalNoTax { get; set; }
        public Nullable<decimal> subTotalTax { get; set; }
        public Nullable<decimal> subTotalNotax { get; set; }

        public Nullable<decimal> OneitemUnitTax { get; set; }
        public Nullable<decimal> itemUnitTaxwithQTY { get; set; }
        public Nullable<decimal> invTaxVal { get; set; }
        public Nullable<decimal> OneItemOfferVal { get; set; }
        public Nullable<decimal> OneItemPriceNoTax { get; set; }
        public Nullable<decimal> ItemTaxes { get; set; }
        public Nullable<decimal> OneItemPricewithTax { get; set; }

        public Nullable<int> itemsRowsCount { get; set; }
        // public Nullable<decimal> totalNet { get; set; }

    }
    public class ItemUnitInvoiceProfit
    {
        //4test
        //  public Nullable<decimal> itemAdminPay { get; set; }
        //public Nullable<decimal> itemPricePercent { get; set; }
        //public Nullable<decimal> invoiceTotal { get; set; }
       
        //public Nullable<decimal> AdminPay { get; set; }
        //public Nullable<decimal> itemunitProfitOld { get; set; }
        /////////////// الارباح


        public decimal shippingCost { get; set; }
        public decimal realShippingCost { get; set; }
        public decimal shippingProfit { get; set; }
        public decimal totalNoShip { get; set; }
        public decimal totalNetNoShip { get; set; }
        public string ITitemName { get; set; }
        public string ITunitName { get; set; }
        //public int ITitemsTransId { get; set; }*
        public Nullable<long> ITitemUnitId { get; set; }

        public Nullable<long> ITitemId { get; set; }
        public Nullable<long> ITunitId { get; set; }
        public Nullable<long> ITquantity { get; set; }

        //public Nullable<System.DateTime> ITupdateDate { get; set; }*
        //  public Nullable<int> IT.createUserId { get; set; } 
        //public Nullable<int> ITupdateUserId { get; set; }*

        public Nullable<decimal> ITprice { get; set; }
        //public string ITbarcode { get; set; }*

        //public string ITUpdateuserNam { get; set; }*
        //public string ITUpdateuserLNam { get; set; }*
        //public string ITUpdateuserAccNam { get; set; }*
        public long invoiceId { get; set; }
        public string invNumber { get; set; }
        public string invBarcode { get; set; }
        //public Nullable<int> agentId { get; set; }*
        public Nullable<long> posId { get; set; }
        public string invType { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }

        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }
        //public Nullable<int> updateUserId { get; set; }*
        //public Nullable<int> branchId { get; set; }*
        public Nullable<decimal> discountValue { get; set; }
        public string discountType { get; set; }
        public Nullable<decimal> tax { get; set; }
        // public string name { get; set; }
        //  isApproved { get; set; }


        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }


        public string posName { get; set; }
        public string posCode { get; set; }
        //public string agentName { get; set; }*
        //public string agentCode { get; set; }*
        //public string agentType { get; set; }*

        //public string uuserName { get; set; }*
        //public string uuserLast { get; set; }*
        //public string uUserAccName { get; set; }*
        //public string agentCompany { get; set; }*
        public Nullable<decimal> subTotal { get; set; }
        public decimal purchasePrice { get; set; }
        public decimal totalwithTax { get; set; }
        public decimal subTotalNet { get; set; } // with invoice discount 
        public decimal itemunitProfit { get; set; }
        public decimal invoiceProfit { get; set; }
        public decimal itemProfit { get; set; }

    }
    public class BalanceSTS
    {

        public long posId { get; set; }
        public string posName { get; set; }
        public Nullable<byte> posIsActive { get; set; }
        public Nullable<decimal> balance { get; set; }
        public string posCode { get; set; }
        public long branchId { get; set; }
        public string branchName { get; set; }
        public string branchCode { get; set; }

        public string branchType { get; set; }
        public Nullable<byte> banchIsActive { get; set; }

    }
    public class CashTransferSts
    {
        public Nullable<long> invShippingCompanyId { get; set; }
        public Nullable<long> shipUserId { get; set; }
        public Nullable<long> invAgentId { get; set; }
        public Nullable<decimal> agentBalance { get; set; }
        public Nullable<byte> agentBType { get; set; }
        public Nullable<decimal> userBalance { get; set; }
        public Nullable<byte> userBType { get; set; }
        public Nullable<decimal> shippingBalance { get; set; }
        public Nullable<byte> shippingCompaniesBType { get; set; }
        private string description;
        private string description1;
        private string description3;
        private string bIsReceived;
        public string bondNumber { get; set; }
        public Nullable<long> fromposId { get; set; }
        public string fromposName { get; set; }
        public Nullable<long> frombranchId { get; set; }
        public string frombranchName { get; set; }
        public Nullable<long> toposId { get; set; }
        public string toposName { get; set; }
        public Nullable<long> tobranchId { get; set; }
        public string tobranchName { get; set; }

        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public Nullable<long> branch2Id { get; set; }
        public string branch2Name { get; set; }
        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreator { get; set; }
        public int depositCount { get; set; }
        public decimal depositSum { get; set; }
        public int pullCount { get; set; }
        public decimal pullSum { get; set; }


        public Nullable<long> posId { get; set; }//
        public Nullable<long> userId { get; set; }
        public Nullable<long> agentId { get; set; }//

        public string transNum { get; set; }//
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }//

        public Nullable<long> updateUserId { get; set; }//
        public Nullable<long> createUserId { get; set; }
        public string notes { get; set; }
        public Nullable<long> posIdCreator { get; set; }
        public Nullable<byte> isConfirm { get; set; }
        public Nullable<long> cashTransIdSource { get; set; }

        public string opSideNum { get; set; }
        public string docName { get; set; }

        public string docImage { get; set; }

        public string bankName { get; set; }//
        public string agentName { get; set; }//
        public string usersName { get; set; }
        public string usersLName { get; set; }
        public string posName { get; set; }
        public string posCreatorName { get; set; }
        public Nullable<byte> isConfirm2 { get; set; }
        public long cashTrans2Id { get; set; }
        public Nullable<long> pos2Id { get; set; }

        public string pos2Name { get; set; }

        public int processTypeCount { get; set; }

        public decimal cardTotal { get; set; }
        public decimal docTotal { get; set; }
        public decimal chequeTotal { get; set; }
        public decimal balanceTotal { get; set; }
        public decimal invoiceTotal { get; set; }
        public decimal adminTotal { get; set; }
        public string createUserName { get; set; }
        public string updateUserName { get; set; }//
        public string updateUserJob { get; set; }
        public string updateUserAcc { get; set; }
        public string createUserJob { get; set; }
        public string createUserLName { get; set; }
        public string cardName { get; set; }//
        public Nullable<System.DateTime> bondDeserveDate { get; set; }
        public Nullable<byte> bondIsRecieved { get; set; }
        public string agentCompany { get; set; }//

        public Nullable<long> shippingCompanyId { get; set; }//
        public string shippingCompanyName { get; set; }//
        public string invAgentName { get; set; }
        public string invShippingCompanyName { get; set; }
        public string userAcc { get; set; }


        //invoice



        public Nullable<decimal> deserved { get; set; }
        public Nullable<System.DateTime> deservedDate { get; set; }

        public long cashTransId { get; set; }
        public string transType { get; set; }//
        public string desc { get; set; }//
        public Nullable<long> invId { get; set; }//
        public Nullable<decimal> cash { get; set; }//
        public decimal cashTotal { get; set; }//
        public string side { get; set; }//
        public string docNum { get; set; }//
        public Nullable<long> bankId { get; set; }//
        public string processType { get; set; }//
        public string paymentreport { get; set; }//
        public Nullable<long> cardId { get; set; }//
        public Nullable<long> bondId { get; set; }//
        public string invNumber { get; set; }//
        public string invBarcode { get; set; }
        public string invType { get; set; }//
        public Nullable<decimal> totalNet { get; set; }//
        public string Description
        {
            get => processType == "cash" ? description = AppSettings.resourcemanager.GetString("trCash")//
                 : processType == "card" ? description = cardName + " " + AppSettings.resourcemanager.GetString("trNum:") + " : " + docNum
                 : processType == "doc" ? description = AppSettings.resourcemanager.GetString("trBond") + " " + AppSettings.resourcemanager.GetString("trNum:") + " : " + bondNumber
                 : processType == "cheque" ? description = AppSettings.resourcemanager.GetString("trCheque") + " " + AppSettings.resourcemanager.GetString("trNum:") + " : " + docNum
                 : processType == "inv" ? description = AppSettings.resourcemanager.GetString("trInv")//yasmine
                 : AppSettings.resourcemanager.GetString("trCredit");

            set => description = value;
        }
        public string Description1
        {//
            get =>
                description1 = (side == "mb" ) ? description1 = AppSettings.resourcemanager.GetString("trMembership")
                : description1 = (transType == "p" && processType != "inv") ? description1 = AppSettings.resourcemanager.GetString("trPayment")
                : description1 = (transType == "d" && processType != "inv") ? description1 = AppSettings.resourcemanager.GetString("trReceipt")
                : invId > 0 && processType == "inv" ? description1 = AppSettings.resourcemanager.GetString("tr_Invoice") + " " + AppSettings.resourcemanager.GetString("trNum:") + " : " + invNumber
                : ""
                ; set => description1 = value;
        }
        public string Description2
        {
            get; set;
        }
        public string Description3
        {
            get => bondId > 0 ?
                description3 = AppSettings.resourcemanager.GetString("trBond") + " " + AppSettings.resourcemanager.GetString("trNum:") + " : " + bondNumber
                 :
                processType;

            set => description3 = value;
        }

        public string BIsReceived
        {
            get; set;
        }
    }
    public class Storage
    {
        public string itemType { get; set; }
        //storagecost
        public Nullable<long> storageCostId { get; set; }
        public string storageCostName { get; set; }
        public decimal storageCostValue { get; set; }


        //
        public int min { get; set; }
        public int max { get; set; }

        public Nullable<long> minUnitId { get; set; }
        public Nullable<long> maxUnitId { get; set; }
        public string minUnitName { get; set; }
        public string maxUnitName { get; set; }
        private string minAll;
        private string maxAll;
        public string MinAll { get => minAll = minUnitName + " " + min.ToString(); set => minAll = value; }
        public string MaxAll { get => maxAll = maxUnitName + " " + max.ToString(); set => maxAll = value; }
        // item unit
        public string itemName { get; set; }
        public string unitName { get; set; }
        public long itemUnitId { get; set; }

        public long itemId { get; set; }
        public long unitId { get; set; }

        public string barcode { get; set; }
        //item location
        public string CreateuserName { get; set; }
        public string CreateuserLName { get; set; }
        public string CreateuserAccName { get; set; }
        public string UuserName { get; set; }
        public string UuserLName { get; set; }
        public string UuserAccName { get; set; }

        //
        public string branchName { get; set; }

        public string branchType { get; set; }
        //itemslocations

        public long itemsLocId { get; set; }
        public long locationId { get; set; }
        public Nullable<decimal> quantity { get; set; }

        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }

        public string IULnote { get; set; }
        public Nullable<decimal> storeCost { get; set; }

        public string cuserName { get; set; }
        public string cuserLast { get; set; }
        public string cUserAccName { get; set; }
        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }
        // Location
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
        private string sectionLoactionName;

        public Nullable<byte> LocisActive { get; set; }
        public long sectionId { get; set; }
        public string Locnote { get; set; }
        public long branchId { get; set; }
        public Nullable<byte> LocisFreeZone { get; set; }


        // section

        public string Secname { get; set; }
        public Nullable<byte> SecisActive { get; set; }
        public string Secnote { get; set; }
        public Nullable<byte> SecisFreeZone { get; set; }
        public string SectionLoactionName { get => sectionLoactionName = Secname + " - " + x + y + z; set => sectionLoactionName = value; }


        private string itemUnits;
        private string loactionName;
        public string LoactionName { get => loactionName = x + y + z; set => loactionName = value; }
        public string ItemUnits { get => itemUnits = itemName + " - " + unitName; set => itemUnits = value; }

    }
    public class InventoryClass

    {
        public string userFalls { get; set; }
        private string itemUnits;
        public string ItemUnits { get => itemUnits = itemName + " - " + unitName; set => itemUnits = value; }
        public int shortfalls { get; set; }
        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public long inventoryILId { get; set; }
        public Nullable<bool> isDestroyed { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<int> amountDestroyed { get; set; }
        public Nullable<int> realAmount { get; set; }
        public Nullable<long> itemLocationId { get; set; }
        public Nullable<byte> isActive { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public long itemId { get; set; }
        public string itemName { get; set; }

        public long unitId { get; set; }
        public long itemUnitId { get; set; }
        public string unitName { get; set; }
        public long sectionId { get; set; }
        public string Secname { get; set; }

        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
        public string itemType { get; set; }
        public Nullable<System.DateTime> inventoryDate { get; set; }
        public string inventoryNum { get; set; }
        public string inventoryType { get; set; }
        public long inventoryId { get; set; }
        public decimal diffPercentage { get; set; }
        public int nCount { get; set; }
        public int dCount { get; set; }
        public int aCount { get; set; }
        public int itemCount { get; set; }
        public int DestroyedCount { get; set; }

    }
    public class ItemUnitCombo
    {

        public long itemUnitId { get; set; }
        public string itemUnitName { get; set; }

    }
    public class CouponCombo
    {

        public long Copcid { get; set; }
        public string Copname { get; set; }
    }

    public class OfferCombo
    {

        public long OofferId { get; set; }
        public string Oname { get; set; }
    }

    public class InvoiceClassCombo
    {

        public long invoiceId { get; set; }
        public string invNumber { get; set; }
        public string invBarcode { get; set; }
    }

    public class ItemTransferInvoice
    {// new properties
        public string shippingCompanyName { get; set; }
        public string shipUserName { get; set; }
        public string shipUserLastName { get; set; }
        public string categoryName { get; set; }
        public Nullable<long> categoryId { get; set; }
        public int archived { get; set; }
        public double? itemAvg { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public string causeFalls { get; set; }
        public string causeDestroy { get; set; }
        public string userdestroy { get; set; }
        public string userFalls { get; set; }
        public Nullable<long> userId { get; set; }
        public string inventoryNum { get; set; }
        public string inventoryType { get; set; }
        public Nullable<DateTime> inventoryDate { get; set; }
        public int itemCount { get; set; }
        public Nullable<decimal> subTotal { get; set; }
        public string agentCompany { get; set; }
        public string itemName { get; set; }
        public string unitName { get; set; }
        public long itemsTransId { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public Nullable<long> itemId { get; set; }
        public Nullable<long> unitId { get; set; }
        public Nullable<long> quantity { get; set; }
        public Nullable<decimal> price { get; set; }
        public string barcode { get; set; }

        // ItemTransfer
        public long ITitemsTransId { get; set; }
        public Nullable<long> ITitemUnitId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public Nullable<long> ITitemId { get; set; }
        public Nullable<long> ITunitId { get; set; }
        public string ITitemName { get; set; }
        public string ITunitName { get; set; }
        private string ITitemUnitName;
        public Nullable<long> ITquantity { get; set; }
        public Nullable<decimal> ITprice { get; set; }


        public Nullable<System.DateTime> ITcreateDate { get; set; }
        public Nullable<System.DateTime> ITupdateDate { get; set; }
        public Nullable<long> ITcreateUserId { get; set; }
        public Nullable<long> ITupdateUserId { get; set; }
        public string ITnotes { get; set; }

        public string ITbarcode { get; set; }
        public string ITCreateuserName { get; set; }
        public string ITCreateuserLName { get; set; }
        public string ITCreateuserAccName { get; set; }

        public string ITUpdateuserName { get; set; }
        public string ITUpdateuserLName { get; set; }
        public string ITUpdateuserAccName { get; set; }
        //invoice
        public long invoiceId { get; set; }
        public string invNumber { get; set; }
        public string invBarcode { get; set; }
        public Nullable<long> agentId { get; set; }
        public Nullable<long> createUserId { get; set; }
        public string invType { get; set; }
        public string discountType { get; set; }
        public Nullable<decimal> ITdiscountValue { get; set; }
        public Nullable<decimal> discountValue { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }
        public Nullable<decimal> paid { get; set; }
        public Nullable<decimal> deserved { get; set; }
        public Nullable<System.DateTime> deservedDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }
        public Nullable<System.DateTime> IupdateDate { get; set; }
        public Nullable<long> IupdateUserId { get; set; }
        public Nullable<long> invoiceMainId { get; set; }
        public string invCase { get; set; }
        public Nullable<System.TimeSpan> invTime { get; set; }
        public string Inotes { get; set; }
        public string vendorInvNum { get; set; }
        public string name { get; set; }
        public string branchName { get; set; }
        public Nullable<System.DateTime> vendorInvDate { get; set; }
        public Nullable<long> branchId { get; set; }
        public Nullable<int> itemsCount { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<int> taxtype { get; set; }
        public Nullable<long> posId { get; set; }
        public Nullable<byte> isApproved { get; set; }
        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }
        public string ITtype { get; set; }
        private string invTypeNumber;//number
        //public string InvTypeNumber { get => invTypeNumber = invType + "-" + invNumber; set => invTypeNumber = value; }
        public string InvTypeNumber
        {
            get => invType == "ex" ? invTypeNumber = AppSettings.resourcemanager.GetString("trExport") + "-" + invNumber
                 : invType == "im" ? invTypeNumber = AppSettings.resourcemanager.GetString("trImport") + "-" + invNumber
                 : invType == "p" ? invTypeNumber = AppSettings.resourcemanager.GetString("trPurchaseInvoice") + "-" + invNumber
                 : invType == "pw" ? invTypeNumber = AppSettings.resourcemanager.GetString("trPurchaseInvoiceWaiting") + "-" + invNumber
                 : invType == "s" ? invTypeNumber = AppSettings.resourcemanager.GetString("trSalesInvoice") + "-" + invNumber
                 : invType == "sb" ? invTypeNumber = AppSettings.resourcemanager.GetString("trSalesReturnInvoice") + "-" + invNumber
                 : invType == "pb" ? invTypeNumber = AppSettings.resourcemanager.GetString("trPurchaseReturnInvoice") + "-" + invNumber
                 : invType == "pbw" ? invTypeNumber = AppSettings.resourcemanager.GetString("trPurchaseReturnInvoiceWaiting") + "-" + invNumber
                 : invType == "pd" ? invTypeNumber = AppSettings.resourcemanager.GetString("trDraftPurchaseBill") + "-" + invNumber
                 : invType == "sd" ? invTypeNumber = AppSettings.resourcemanager.GetString("trSalesDraft") + "-" + invNumber
                 : invType == "sbd" ? invTypeNumber = AppSettings.resourcemanager.GetString("trSalesReturnDraft") + "-" + invNumber
                 : invType == "pbd" ? invTypeNumber = AppSettings.resourcemanager.GetString("trPurchaseReturnDraft") + "-" + invNumber
                 : invType == "ord" ? invTypeNumber = AppSettings.resourcemanager.GetString("trSaleOrderDraft") + "-" + invNumber
                 : invType == "or" ? invTypeNumber = AppSettings.resourcemanager.GetString("trSaleOrder") + "-" + invNumber
                 : invType == "ors" ? invTypeNumber = AppSettings.resourcemanager.GetString("trSaleOrder") + "-" + invNumber

                : invType == "pod" ? invTypeNumber = AppSettings.resourcemanager.GetString("trPurchaceOrderDraft") + "-" + invNumber
                 : invType == "po" ? invTypeNumber = AppSettings.resourcemanager.GetString("trPurchaceOrder") + "-" + invNumber
                  : invType == "pos" ? invTypeNumber = AppSettings.resourcemanager.GetString("trPurchaceOrder") + "-" + invNumber

                : invType == "qd" ? invTypeNumber = AppSettings.resourcemanager.GetString("trQuotationsDraft") + "-" + invNumber
                 : invType == "q" ? invTypeNumber = AppSettings.resourcemanager.GetString("trQuotations") + "-" + invNumber
                 : invType == "qs" ? invTypeNumber = AppSettings.resourcemanager.GetString("trQuotations") + "-" + invNumber

                : invType == "d" ? invTypeNumber = AppSettings.resourcemanager.GetString("trDestructive") + "-" + invNumber
                 : invType == "sh" ? invTypeNumber = AppSettings.resourcemanager.GetString("trShortage") + "-" + invNumber
                 : invType == "imd" ? invTypeNumber = AppSettings.resourcemanager.GetString("trImportDraft") + "-" + invNumber
                 : invType == "imw" ? invTypeNumber = AppSettings.resourcemanager.GetString("trImportOrder") + "-" + invNumber
                 : invType == "exd" ? invTypeNumber = AppSettings.resourcemanager.GetString("trExportDraft") + "-" + invNumber
                 : invType == "exw" ? invTypeNumber = AppSettings.resourcemanager.GetString("trExportOrder") + "-" + invNumber

                 : "";
            set => invTypeNumber = value;
        }

        // for report
        public int countP { get; set; }
        public int countS { get; set; }
        public int count { get; set; }

        public Nullable<decimal> totalS { get; set; }
        public Nullable<decimal> totalNetS { get; set; }
        public Nullable<decimal> totalP { get; set; }
        public Nullable<decimal> totalNetP { get; set; }
        public string branchType { get; set; }
        public string posName { get; set; }
        public string posCode { get; set; }
        public string agentName { get; set; }


        public string agentType { get; set; }
        public string agentCode { get; set; }
        public string cuserName { get; set; }
        public string cuserLast { get; set; }
        public string cUserAccName { get; set; }
        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }
        private string agentTypeAgent;
        public string AgentTypeAgent
        {
            get => agentType == "v" ? agentTypeAgent = AppSettings.resourcemanager.GetString("trVendor") + "-"
                                      :
                                       agentTypeAgent = AppSettings.resourcemanager.GetString("trCustomer") + "-"
                                       ;
            set => agentTypeAgent = value;
        }
        private string agentNameAgent;
        public string AgentNameAgent
        {
            get => agentName == "unknown" ? agentNameAgent = AppSettings.resourcemanager.GetString("trUnKnown")
                                      :
                                       agentTypeAgent = agentName
                                       ;
            set => agentTypeAgent = value;
        }

        public int countPb { get; set; }
        public int countD { get; set; }
        public Nullable<decimal> totalPb { get; set; }
        public Nullable<decimal> totalD { get; set; }
        public Nullable<decimal> totalNetPb { get; set; }
        public Nullable<decimal> totalNetD { get; set; }


        public Nullable<decimal> paidPb { get; set; }
        public Nullable<decimal> deservedPb { get; set; }
        public Nullable<decimal> discountValuePb { get; set; }
        public Nullable<decimal> paidD { get; set; }
        public Nullable<decimal> deservedD { get; set; }
        public Nullable<decimal> discountValueD { get; set; }
        // coupon


        public int CopcId { get; set; }
        public string Copname { get; set; }
        public string Copcode { get; set; }
        public Nullable<byte> CopisActive { get; set; }
        public Nullable<byte> CopdiscountType { get; set; }
        public Nullable<decimal> CopdiscountValue { get; set; }
        public Nullable<System.DateTime> CopstartDate { get; set; }
        public Nullable<System.DateTime> CopendDate { get; set; }
        public string Copnotes { get; set; }
        public Nullable<int> Copquantity { get; set; }
        public Nullable<int> CopremainQ { get; set; }
        public Nullable<decimal> CopinvMin { get; set; }
        public Nullable<decimal> CopinvMax { get; set; }
        public Nullable<System.DateTime> CopcreateDate { get; set; }
        public Nullable<System.DateTime> CopupdateDate { get; set; }
        public Nullable<long> CopcreateUserId { get; set; }
        public Nullable<long> CopupdateUserId { get; set; }
        public string Copbarcode { get; set; }
        public Nullable<decimal> couponTotalValue { get; set; }
        // offer

        public long OofferId { get; set; }
        public string Oname { get; set; }
        public string Ocode { get; set; }
        public Nullable<byte> OisActive { get; set; }
        public string OdiscountType { get; set; }
        public Nullable<decimal> OdiscountValue { get; set; }
        public Nullable<System.DateTime> OstartDate { get; set; }
        public Nullable<System.DateTime> OendDate { get; set; }
        public Nullable<System.DateTime> OcreateDate { get; set; }
        public Nullable<System.DateTime> OupdateDate { get; set; }
        public Nullable<long> OcreateUserId { get; set; }
        public Nullable<long> OupdateUserId { get; set; }
        public string Onotes { get; set; }
        public Nullable<int> Oquantity { get; set; }
        public long Oitemofferid { get; set; }
        public Nullable<decimal> offerTotalValue { get; set; }

        //external
        public long movbranchid { get; set; }
        public string movbranchname { get; set; }
        // internal
        public string exportBranch { get; set; }
        public string importBranch { get; set; }
        public long exportBranchId { get; set; }
        public long importBranchId { get; set; }
        private string itemUnits;
        private int cusCount;
        private int venCount;
        private int pCount;
        private int sCount;
        private int pbCount;
        private int sbCount;
        public string ItemUnits { get => itemUnits = itemName + " - " + unitName; set => itemUnits = value; }
        public int CusCount { get => cusCount; set => cusCount = value; }
        public int VenCount { get => venCount; set => venCount = value; }

        public int PCount { get => pCount; set => pCount = value; }
        public int SCount { get => sCount; set => sCount = value; }
        public int PbCount { get => pbCount; set => pbCount = value; }
        public int SbCount { get => sbCount; set => sbCount = value; }
        private int importCount;
        private int exportCount;
        public string ITitemUnitName1 { get => ITitemUnitName = ITitemName + " - " + ITunitName; set => ITitemUnitName = value; }
        public int ImportCount { get => importCount; set => importCount = value; }
        public int ExportCount { get => exportCount; set => exportCount = value; }
        public string processType { get; set; }

 
    }

    public class OrderPreparingSTS
    {
        public long orderPreparingId { get; set; }
        public string orderNum { get; set; }
        public Nullable<System.DateTime> orderTime { get; set; }
        public Nullable<long> invoiceId { get; set; }
        public string notes { get; set; }
        public Nullable<decimal> preparingTime { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }


        // item
        public string itemName { get; set; }
        public Nullable<long> itemUnitId { get; set; }
        public int quantity { get; set; }
        //order
        public string status { get; set; }
        public int num { get; set; }
        public decimal remainingTime { get; set; }
        public string tables { get; set; }
        public string waiter { get; set; }
        //invoice

        public string invType { get; set; }
        public Nullable<long> shippingCompanyId { get; set; }
        public string branchName { get; set; }
        public Nullable<long> branchId { get; set; }

      //  public List<itemOrderPreparingModel> items { get; set; }
        //
        public Nullable<long> categoryId { get; set; }
        public string categoryName { get; set; }
        public Nullable<decimal> realDuration { get; set; }
        public string invNumber { get; set; }
        public string invBarcode { get; set; }
        public Nullable<long> tagId { get; set; }
        public string tagName { get; set; }
        public Nullable<System.DateTime> listedDate { get; set; }

        public string shipUserName { get; set; }
        public string shipUserLastName { get; set; }
        public string shippingCompanyName { get; set; }
        public Nullable<long> shipUserId { get; set; }
     
        public Nullable<long> agentId { get; set; }
        public string agentName { get; set; }
        public string agentCompany { get; set; }
        public string agentType { get; set; }
        public string agentCode { get; set; }
        public List<orderPreparingStatus> orderStatusList { get; set; }
        public decimal orderDuration { get; set; }
        public string orderDurationConv { get; set; }
        public string statusConv { get; set; }
        public string categoryNameConv { get; set; }
  
    }
   

    public class SalesMembership
    { 
        
        //invoice
        public long invoiceId { get; set; }
        public string invNumber { get; set; }
        public string invBarcode { get; set; }
        public string invType { get; set; }
        public string discountType { get; set; }
        public Nullable<decimal> discountValue { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> totalNet { get; set; }
        public Nullable<decimal> paid { get; set; }
        public Nullable<decimal> deserved { get; set; }
        public Nullable<System.DateTime> deservedDate { get; set; }
        public Nullable<System.DateTime> invDate { get; set; }
   
        public Nullable<long> invoiceMainId { get; set; }
        public string invCase { get; set; }
        public Nullable<System.TimeSpan> invTime { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }//
        public Nullable<System.DateTime> updateDate { get; set; }

        public Nullable<decimal> tax { get; set; }
 
        public Nullable<byte> isApproved { get; set; }
        public int count { get; set; }
 
       

        //pos
        public Nullable<long> posId { get; set; }
        public string posName { get; set; }
        public string posCode { get; set; }
        //branch

        public Nullable<long> branchCreatorId { get; set; }
        public string branchCreatorName { get; set; }
        public Nullable<long> branchId { get; set; }
        public string branchName { get; set; }
        public string branchType { get; set; }


        //agent
        public Nullable<long> agentId { get; set; }
        public string agentCompany { get; set; }
        public string agentName { get; set; }

        public string agentType { get; set; }
        public string agentCode { get; set; }
        public string vendorInvNum { get; set; }


        public Nullable<System.DateTime> vendorInvDate { get; set; }
        //user
        public Nullable<long> createUserId { get; set; }
        public string cuserName { get; set; }
        public string cuserLast { get; set; }
        public string cUserAccName { get; set; }
        public string uuserName { get; set; }
        public string uuserLast { get; set; }
        public string uUserAccName { get; set; }
        public Nullable<long> userId { get; set; }
        //membership

        public Nullable<long> membershipId { get; set; }
        public string membershipsCode { get; set; }
        public string membershipsName { get; set; }

        public List<CouponInvoice> CouponInvoiceList { get; set; }
        public List<ItemTransfer> itemsTransferList { get; set; }
        public List<InvoicesClass> invoiceClassDiscountList { get; set; }

        public decimal invclassDiscount { get; set; }
        public decimal couponDiscount { get; set; }
        public decimal offerDiscount { get; set; }
        public decimal totalDiscount { get; set; }

        public Nullable<System.DateTime> endDate { get; set; }
        public string subscriptionType { get; set; }
        //invClass
        public string invoicesClassName { get; set; }
        public Nullable<long> invClassDiscountId { get; set; }

        public Nullable<long> invClassId { get; set; }
        public byte invClassdiscountType { get; set; }
        public decimal invClassdiscountValue { get; set; }
        public decimal finalDiscount { get; set; }

    }

    class Statistics
    {

        //****************************************************
        public async Task<List<ItemUnitInvoiceProfit>> GetInvoiceProfit(long mainBranchId, long userId)
        {

            List<ItemUnitInvoiceProfit> list = new List<ItemUnitInvoiceProfit>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetInvoiceProfit", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemUnitInvoiceProfit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
        }

        public async Task<List<ItemUnitInvoiceProfit>> GetItemProfit(long mainBranchId, long userId)
        {
            List<ItemUnitInvoiceProfit> list = new List<ItemUnitInvoiceProfit>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetItemProfit", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemUnitInvoiceProfit>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
        }



        // المشتريات
        #region Purchases
     

        // الاصناف في الفواتير

        public async Task<List<ItemTransferInvoice>> GetPuritem(long mainBranchId, long userId)
        {
            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetPuritem", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

           
        }

        // عدد الاصناف في الفواتير
        public async Task<List<ItemTransferInvoice>> GetPuritemcount(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetPuritemcount", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

          
        }
        //    الشراء عدد العناصر في فواتير طلبات
        public async Task<List<ItemTransferInvoice>> GetPurorderitemcount(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetPurorderitemcount", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
 
        }
        #endregion

       
        // مبيعات
        #region Sales 
        // الفواتير مع العناصر
        public async Task<List<ItemTransferInvoice>> GetSaleitem(long mainBranchId, long userId)
        {
            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetSaleitem", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }


        //  للمبيعات عدد العناصر في فواتير الطلبات
        public async Task<List<ItemTransferInvoice>> Getorderitemcount(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/Getorderitemcount", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


        }


      
        public async Task<List<ItemTransferInvoice>> GetSaleitemcount(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetSaleitemcount", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
 
        }
        // الفواتير  مع الكوبون
        public async Task<List<ItemTransferInvoice>> GetSalecoupon(long mainBranchId, long userId)
        {
            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetSalecoupon", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            //List<ItemTransferInvoice> list = null;
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
            //    request.RequestUri = new Uri(Global.APIUri + "Statistics/GetSalecoupon?mainBranchId=" + mainBranchId + "&userId=" + userId);
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
            //        list = JsonConvert.DeserializeObject<List<ItemTransferInvoice>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return list;
            //    }
            //    else //web api sent error response 
            //    {
            //        list = new List<ItemTransferInvoice>();
            //    }
            //    return list;
            //}
        }

       
        // الفواتير مع العناصر التي لديها اوفر
        public async Task<List<ItemTransferInvoice>> GetPromoOffer(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetPromoOffer", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        // الاشتراكات
       
        public async Task<List<SalesMembership>> GetSaleMembership(long mainBranchId, long userId)
        {

            List<SalesMembership> list = new List<SalesMembership>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetSaleMembership", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<SalesMembership>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }
        //Inv class
        public async Task<List<SalesMembership>> GetInvoiceClass(long mainBranchId, long userId)
        {

            List<SalesMembership> list = new List<SalesMembership>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetInvoiceClass", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<SalesMembership>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }
        #endregion 
        //
        #region combo

        public List<ItemUnitCombo> GetIUComboList(List<ItemTransferInvoice> ITInvoice)
        {
            List<ItemUnitCombo> iulist = new List<ItemUnitCombo>();

            iulist = ITInvoice.GroupBy(x => x.ITitemUnitId)
                   .Select(g => new ItemUnitCombo { itemUnitId = (long)g.FirstOrDefault().ITitemUnitId, itemUnitName = g.FirstOrDefault().ITitemName + " - " + g.FirstOrDefault().ITunitName }).ToList();
            return iulist;

        }
        public List<CouponCombo> GetCopComboList(List<ItemTransferInvoice> ITInvoice)
        {
            List<CouponCombo> iulist = new List<CouponCombo>();

            iulist = ITInvoice.GroupBy(x => x.CopcId)
                   .Select(g => new CouponCombo { Copcid = g.FirstOrDefault().CopcId, Copname = g.FirstOrDefault().Copname }).ToList();
            return iulist;

        }
        public List<OfferCombo> GetOfferComboList(List<ItemTransferInvoice> ITInvoice)
        {
            List<OfferCombo> iulist = new List<OfferCombo>();

            iulist = ITInvoice.GroupBy(x => x.OofferId)
                   .Select(g => new OfferCombo { OofferId = g.FirstOrDefault().OofferId, Oname = g.FirstOrDefault().Oname }).ToList();
            return iulist;

        }
        public List<InvoiceClassCombo> GetInvoiceClassComboList(List<SalesMembership> ITInvoice)
        {
            List<InvoiceClassCombo> iulist = new List<InvoiceClassCombo>();

            iulist = ITInvoice.GroupBy(x => x.invoiceId)
                   .Select(g => new InvoiceClassCombo { invoiceId = g.FirstOrDefault().invoiceId, invNumber = g.FirstOrDefault().invNumber }).ToList();
            return iulist;

        }
        #endregion
        //OfferCombo





        // المخزون 
        #region Storage

        public async Task<List<Storage>> GetStorage(long mainBranchId, long userId)
        {

            List<Storage> list = new List<Storage>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetStorage", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<Storage>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            #region
            //List<Storage> list = null;
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
            //    request.RequestUri = new Uri(Global.APIUri + "Statistics/GetStorage?mainBranchId=" + mainBranchId + "&userId=" + userId);
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
            //        list = JsonConvert.DeserializeObject<List<Storage>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return list;
            //    }
            //    else //web api sent error response 
            //    {
            //        list = new List<Storage>();
            //    }
            //    return list;
            //}
            #endregion
        }

        #region
        // حركة الاصناف التي دخلت الى الفرع
        //public async Task<List<ItemTransferInvoice>> GetInItems()
        //{
        //    List<ItemTransferInvoice> list = null;
        //    // ... Use HttpClient.
        //    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        //    using (var client = new HttpClient())
        //    {
        //        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //        client.BaseAddress = new Uri(Global.APIUri);
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        //        client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
        //        HttpRequestMessage request = new HttpRequestMessage();
        //        request.RequestUri = new Uri(Global.APIUri + "Statistics/GetInItems");
        //        request.Headers.Add("APIKey", Global.APIKey);
        //        request.Method = HttpMethod.Get;
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        HttpResponseMessage response = await client.SendAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var jsonString = await response.Content.ReadAsStringAsync();
        //            jsonString = jsonString.Replace("\\", string.Empty);
        //            jsonString = jsonString.Trim('"');
        //            // fix date format
        //            JsonSerializerSettings settings = new JsonSerializerSettings
        //            {
        //                Converters = new List<JsonConverter> { new BadDateFixingConverter() },
        //                DateParseHandling = DateParseHandling.None
        //            };
        //            list = JsonConvert.DeserializeObject<List<ItemTransferInvoice>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
        //            return list;
        //        }
        //        else //web api sent error response 
        //        {
        //            list = new List<ItemTransferInvoice>();
        //        }
        //        return list;
        //    }
        //}
        #endregion
        // حركة الاصناف الخارجية (مع الزبائن والموردين)
        public async Task<List<ItemTransferInvoice>> GetExternalMov(long mainBranchId, long userId)
        {
            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetExternalMov", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            #region
            //List<ItemTransferInvoice> list = null;
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
            //    request.RequestUri = new Uri(Global.APIUri + "Statistics/GetExternalMov?mainBranchId=" + mainBranchId + "&userId=" + userId);
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
            //        list = JsonConvert.DeserializeObject<List<ItemTransferInvoice>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return list;
            //    }
            //    else //web api sent error response 
            //    {
            //        list = new List<ItemTransferInvoice>();
            //    }
            //    return list;
            //}
            #endregion
        }
        //حركة الإدخال المباشر
        public async Task<List<ItemTransferInvoice>> GetDirectInMov(long mainBranchId, long userId)
        {
            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetDirectInMov", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
        }
        //حركة الاصناف الداخلية بين الفروع
        public async Task<List<ItemTransferInvoice>> GetInternalMov(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetInternalMov", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
           
        }


        //GetSpendingStorage
        //فواتير الصرف المنفذة من المخزن الى المطبخ 
        public async Task<List<ItemTransferInvoice>> GetSpendingStorage(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetSpendingStorage", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        #endregion


        // الجرد
        #region
        // عناصر الجرد

        public async Task<List<InventoryClass>> GetInventory(long mainBranchId, long userId)
        {

            List<InventoryClass> list = new List<InventoryClass>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetInventory", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<InventoryClass>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

            
        }

      

        // العناصر التالفة
        public async Task<List<ItemTransferInvoice>> GetDesItems(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetDesItems", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        // العناصر الناقصة
        public async Task<List<ItemTransferInvoice>> GetFallsItems(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetFallsItems", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        #endregion


        // المحاسبة
        #region Accountant
        // المدفوعات
        public async Task<List<CashTransferSts>> GetPayments()
        {

            List<CashTransferSts> list = new List<CashTransferSts>();
           
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetPayments");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<CashTransferSts>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


        }

        //المقبوضات
        public async Task<List<CashTransferSts>> GetReceipt()
        {

            List<CashTransferSts> list = new List<CashTransferSts>();

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetReceipt");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<CashTransferSts>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }


        

        //كشف حساب
        public async Task<List<CashTransferSts>> GetStatement()
        {

            List<CashTransferSts> list = new List<CashTransferSts>();
            //Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("mainBranchId", mainBranchId.ToString());
            //parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetStatement");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<CashTransferSts>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        // رصيد نقاط البيع والفروع

        public async Task<List<BalanceSTS>> GetBalance(long mainBranchId, long userId)
        {

            List<BalanceSTS> list = new List<BalanceSTS>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetBalance", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<BalanceSTS>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

      
        // الضرائب
        public async Task<List<ItemTransferInvoiceTax>> GetInvItemTax(long mainBranchId, long userId)
        {

            List<ItemTransferInvoiceTax> list = new List<ItemTransferInvoiceTax>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetInvItemTax", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoiceTax>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


        }

        public async Task<List<CashTransfer>> GetBytypeAndSideForPos(string type, string side)
        {
            // string type, string side
            List<CashTransfer> list = new List<CashTransfer>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("type", type.ToString());
            parameters.Add("side", side.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetBytypeAndSideForPos", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<CashTransfer>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


        }

        // الفتح والاغلاق
        public async Task<List<POSOpenCloseModel>> GetPosCashOpenClose(long mainBranchId, long userId)
        {

            List<POSOpenCloseModel> list = new List<POSOpenCloseModel>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetPosCashOpenClose", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<POSOpenCloseModel>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


        }


        //العمليات المنفذة بين تاريخ الفتح والاغلاق
        public async Task<List<OpenClosOperatinModel>> GetTransBetweenOpenClose(long openCashTransId, long closeCashTransId)
        {

            List<OpenClosOperatinModel> list = new List<OpenClosOperatinModel>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("openCashTransId", openCashTransId.ToString());
            parameters.Add("closeCashTransId", closeCashTransId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetTransBetweenOpenClose", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<OpenClosOperatinModel>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


        }


        // combo
        #region
        public class VendorCombo
        {
            private long? vendorId;
            private string vendorName;
            private string side;
            private string userAcc;
            private long? userId;

            public long? VendorId { get => vendorId; set => vendorId = value; }
            //public string VendorName { get => vendorName; set => vendorName = value; }
            public string VendorName
            {
                get => vendorName == null ? (side == "c" ? vendorName = AppSettings.resourcemanager.GetString("trCashCustomer")
                                                        : vendorName = AppSettings.resourcemanager.GetString("trUnKnown"))
                   : vendorName;
                set => vendorName = value;
            }
            public string Side { get => side; set => side = value; }
            public string UserAcc { get => userAcc; set => userAcc = value; }
            public long? UserId { get => userId; set => userId = value; }
        }
        public List<VendorCombo> getVendorCombo(List<CashTransferSts> ITInvoice, string x)
        {
            List<VendorCombo> iulist = new List<VendorCombo>();

            //iulist = ITInvoice.Where(g => g.side == x).GroupBy(g => g.agentId).Select(g => new VendorCombo { VendorId = g.FirstOrDefault().agentId, VendorName = g.FirstOrDefault().agentName }).ToList();
            //iulist = ITInvoice.Where(g => g.side == x).GroupBy(g => g.agentId).Select(g => new VendorCombo { VendorId = g.FirstOrDefault().agentId, VendorName = g.FirstOrDefault().agentName, Side = g.FirstOrDefault().side }).ToList();
            iulist = ITInvoice.Where(g => g.side == x && (g.invShippingCompanyId == null || (g.invShippingCompanyId != null && g.shipUserId != null))).GroupBy(g => g.agentId).Select(g => new VendorCombo { VendorId = g.FirstOrDefault().agentId, VendorName = g.FirstOrDefault().agentName, Side = g.FirstOrDefault().side }).ToList();
            return iulist;

        }
        public List<VendorCombo> getCustomerForStatementCombo(List<CashTransferSts> ITInvoice, string x)
        {
            List<VendorCombo> iulist = new List<VendorCombo>();

            iulist = ITInvoice.Where(g => g.side == x).GroupBy(g => g.invAgentId).Select(g => new VendorCombo { VendorId = g.FirstOrDefault().invAgentId, VendorName = g.FirstOrDefault().invAgentName }).ToList();
            return iulist;

        }
        public List<VendorCombo> getUserAcc(List<CashTransferSts> ITInvoice, string x)
        {
            List<VendorCombo> iulist = new List<VendorCombo>();

            iulist = ITInvoice.Where(g => g.side == x).GroupBy(g => g.userId).Select(g => new VendorCombo { UserId = g.FirstOrDefault().userId, UserAcc = g.FirstOrDefault().userAcc }).ToList();
            return iulist;

        }
        public class PaymentsTypeCombo
        {
            private string paymentsTypeName;
            private string paymentsTypeText;

            public string PaymentsTypeName { get => paymentsTypeName; set => paymentsTypeName = value; }
            public string PaymentsTypeText
            {
                get => paymentsTypeName == "cash" ? paymentsTypeText = AppSettings.resourcemanager.GetString("trCash")
                    : paymentsTypeName == "doc" ? paymentsTypeText = AppSettings.resourcemanager.GetString("trDocument")
                    : paymentsTypeName == "cheque" ? paymentsTypeText = AppSettings.resourcemanager.GetString("trCheque")
                    : paymentsTypeName == "balance" ? paymentsTypeText = AppSettings.resourcemanager.GetString("trCredit")
                    : paymentsTypeName == "card" ? paymentsTypeText = AppSettings.resourcemanager.GetString("trAnotherPaymentMethods")
                    : paymentsTypeName == "inv" ? paymentsTypeText = AppSettings.resourcemanager.GetString("trInv")
                    : paymentsTypeName == "admin" ? paymentsTypeText = AppSettings.resourcemanager.GetString("trAdministrative")
                    : "";
                set => paymentsTypeText = value;
            }

        }
        public List<PaymentsTypeCombo> getPaymentsTypeCombo(List<CashTransferSts> ITInvoice)
        {
            List<PaymentsTypeCombo> iulist = new List<PaymentsTypeCombo>();

            iulist = ITInvoice.Where(g => g.processType != null).GroupBy(g => g.processType).Select(g => new PaymentsTypeCombo { PaymentsTypeName = g.FirstOrDefault().processType }).ToList();
            return iulist;

        }

        public List<PaymentsTypeCombo> getPaymentsTypeComboBySide(List<CashTransferSts> ITInvoice, string side)
        {
            List<PaymentsTypeCombo> iulist = new List<PaymentsTypeCombo>();

            iulist = ITInvoice.Where(g => g.processType != null && g.side == side).GroupBy(g => g.processType).Select(g => new PaymentsTypeCombo { PaymentsTypeName = g.FirstOrDefault().processType }).ToList();
            return iulist;

        }

        public class AccountantCombo
        {
            private string accountant;

            public string Accountant { get => accountant; set => accountant = value; }
        }
        public List<AccountantCombo> getAccounantCombo(List<CashTransferSts> ITInvoice, string x)
        {
            List<AccountantCombo> iulist = new List<AccountantCombo>();

            iulist = ITInvoice.Where(g => g.side == x).GroupBy(g => g.updateUserAcc).Select(g => new AccountantCombo { Accountant = g.FirstOrDefault().updateUserAcc }).ToList();
            return iulist;

        }
        public class ShippingCombo
        {
            private string shippingName;
            private long? shippingId;

            public string ShippingName { get => shippingName; set => shippingName = value; }
            public long? ShippingId { get => shippingId; set => shippingId = value; }
        }
        public List<ShippingCombo> getShippingCombo(List<CashTransferSts> ITInvoice)
        {
            List<ShippingCombo> iulist = new List<ShippingCombo>();

            iulist = ITInvoice.Where(g => g.shippingCompanyId != null).GroupBy(g => g.shippingCompanyId).Select(g => new ShippingCombo { ShippingId = g.FirstOrDefault().shippingCompanyId, ShippingName = g.FirstOrDefault().shippingCompanyName }).ToList();
            return iulist;

        }
        public List<ShippingCombo> getShippingForStatementCombo(List<CashTransferSts> ITInvoice)
        {
            List<ShippingCombo> iulist = new List<ShippingCombo>();

            iulist = ITInvoice.Where(g => g.invShippingCompanyId != null && g.shipUserId == null).GroupBy(g => g.invShippingCompanyId).Select(g => new ShippingCombo { ShippingId = g.FirstOrDefault().invShippingCompanyId, ShippingName = g.FirstOrDefault().invShippingCompanyName }).ToList();
            return iulist;

        }
        public class branchFromCombo
        {
            private string branchFromName;
            private long? branchFromId;

            public string BranchFromName { get => branchFromName; set => branchFromName = value; }
            public long? BranchFromId { get => branchFromId; set => branchFromId = value; }
        }
        //public List<branchFromCombo> getFromCombo(List<CashTransferSts> ITInvoice)
        //{
        //    List<branchFromCombo> iulist = new List<branchFromCombo>();

        //    iulist = ITInvoice.GroupBy(g => g.frombranchId).Select(g => new branchFromCombo { BranchFromId = g.FirstOrDefault().frombranchId, BranchFromName = g.FirstOrDefault().frombranchName }).ToList();
        //    return iulist;

        //}

        public List<branchFromCombo> getFromCombo(List<CashTransfer> ITInvoice)
        {
            List<branchFromCombo> iulist = new List<branchFromCombo>();

            iulist = ITInvoice.GroupBy(g => g.branchId).Select(g => new branchFromCombo { BranchFromId = g.FirstOrDefault().branchId, BranchFromName = g.FirstOrDefault().branchName }).ToList();
            return iulist;

        }

        public class branchToCombo
        {
            private string branchToName;
            private long? branchToId;

            public string BranchToName { get => branchToName; set => branchToName = value; }
            public long? BranchToId { get => branchToId; set => branchToId = value; }
        }
        //public List<branchToCombo> getToCombo(List<CashTransferSts> ITInvoice)
        //{
        //    List<branchToCombo> iulist = new List<branchToCombo>();

        //    iulist = ITInvoice.GroupBy(g => g.tobranchId).Select(g => new branchToCombo { BranchToId = g.FirstOrDefault().tobranchId, BranchToName = g.FirstOrDefault().tobranchName }).ToList();
        //    return iulist;

        //}
        public List<branchToCombo> getToCombo(List<CashTransfer> ITInvoice)
        {
            List<branchToCombo> iulist = new List<branchToCombo>();

            iulist = ITInvoice.GroupBy(g => g.branch2Id).Select(g => new branchToCombo { BranchToId = g.FirstOrDefault().branch2Id, BranchToName = g.FirstOrDefault().branch2Name }).ToList();
            return iulist;

        }
        public class posFromCombo
        {
            private string posFromName;
            private long? posFromId;
            private long? branchId;

            public string PosFromName { get => posFromName; set => posFromName = value; }
            public long? PosFromId { get => posFromId; set => posFromId = value; }
            public long? BranchId { get => branchId; set => branchId = value; }
        }
        //public List<posFromCombo> getFromPosCombo(List<CashTransferSts> ITInvoice)
        //{
        //    List<posFromCombo> iulist = new List<posFromCombo>();

        //    iulist = ITInvoice.GroupBy(g => g.fromposId).Select(g => new posFromCombo { PosFromId = g.FirstOrDefault().fromposId, PosFromName = g.FirstOrDefault().fromposName, BranchId = g.FirstOrDefault().frombranchId }).ToList();
        //    return iulist;

        //}

        public List<posFromCombo> getFromPosCombo(List<CashTransfer> ITInvoice)
        {
            List<posFromCombo> iulist = new List<posFromCombo>();

            iulist = ITInvoice.GroupBy(g => g.posId).Select(g => new posFromCombo { PosFromId = g.FirstOrDefault().posId, PosFromName = g.FirstOrDefault().posName, BranchId = g.FirstOrDefault().branchId }).ToList();
            return iulist;
        }
        public class posToCombo
        {
            private string posToName;
            private long? posToId;
            private long? branchId;

            public string PosToName { get => posToName; set => posToName = value; }
            public long? PosToId { get => posToId; set => posToId = value; }
            public long? BranchId { get => branchId; set => branchId = value; }
        }
        public List<posToCombo> getToPosCombo(List<CashTransfer> ITInvoice)
        {
            List<posToCombo> iulist = new List<posToCombo>();

            iulist = ITInvoice.GroupBy(g => g.pos2Id).Select(g => new posToCombo { PosToId = g.FirstOrDefault().pos2Id, PosToName = g.FirstOrDefault().pos2Name, BranchId = g.FirstOrDefault().branch2Id }).ToList();
            //iulist = ITInvoice.Where(g => g.toposId != posFromId).GroupBy(g => g.toposId).Select(g => new posToCombo { PosToId = g.FirstOrDefault().toposId, PosToName = g.FirstOrDefault().toposName, BranchId = g.FirstOrDefault().tobranchId }).ToList();
            return iulist;

        }
        #endregion
        #endregion

        // اليومية
        #region Daily

        // فواتير اليومية العامة في قسم التقارير
        public async Task<List<ItemTransferInvoice>> Getdailyinvoice(long mainBranchId, long userId, string date)
        {


            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());
            parameters.Add("date", date);
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/Getdailyinvoice", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


        }
       
        // فواتير اليوميةالخاصة بمستخدم
        public async Task<List<ItemTransferInvoice>> GetUserdailyinvoice(long mainBranchId, long userId)
        {


            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetUserdailyinvoice", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        

      
        #endregion

        #region Kitchen //المطبخ
        // preparing order
        public async Task<List<OrderPreparingSTS>> GetPreparingOrders(long mainBranchId, long userId)
        {
            List<OrderPreparingSTS> items = new List<OrderPreparingSTS>();
          
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetPreparingOrders", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<OrderPreparingSTS>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }

        // فواتير الصرف 
        public async Task<List<ItemTransferInvoice>> GetSpendingRequest(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetSpendingRequest", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }
        // فواتير الصرف عناصر  
        public async Task<List<ItemTransferInvoice>> GetSpendingItems(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetSpendingItems", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        // الاستهلاك
        //فواتير الاستهلاك 
        public async Task<List<ItemTransferInvoice>> GetConsumption(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetConsumption", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }
        //عناصر فواتير الاستهلاك
        public async Task<List<ItemTransferInvoice>> GetConsumptionItems(long mainBranchId, long userId)
        {

            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetConsumptionItems", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<ItemTransferInvoice>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }
        #endregion

        //التوصيل
        #region Delivery
        public async Task<List<OrderPreparingSTS>> GetDelivery(long mainBranchId, long userId)
        {
            List<OrderPreparingSTS> items = new List<OrderPreparingSTS>();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mainBranchId", mainBranchId.ToString());
            parameters.Add("userId", userId.ToString());
            IEnumerable<Claim> claims = await APIResult.getList("Statistics/GetDelivery", parameters);
            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    items.Add(JsonConvert.DeserializeObject<OrderPreparingSTS>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return items;
        }
        #endregion
        // Combo
        #region combo

        public class itemCombo
        {
            private long itemId;
            private string itemName;
            private long branchId;
            public long ItemId { get => itemId; set => itemId = value; }
            public string ItemName { get => itemName; set => itemName = value; }
            public long BranchId { get => branchId; set => branchId = value; }
        }
        public class ExternalitemCombo
        {
            private long? itemId;
            private string itemName;
            private long? branchId;

            public long? ItemId { get => itemId; set => itemId = value; }
            public string ItemName { get => itemName; set => itemName = value; }
            public long? BranchId { get => branchId; set => branchId = value; }
        }
        public List<itemCombo> getItemCombo(List<Storage> ITInvoice)
        {
            List<itemCombo> iulist = new List<itemCombo>();

            iulist = ITInvoice.Select(g => new itemCombo { ItemId = g.itemId, ItemName = g.itemName, BranchId = g.branchId }).ToList();
            return iulist;

        }

        public List<ExternalitemCombo> getExternalItemCombo(List<ItemTransferInvoice> ITInvoice)
        {
            List<ExternalitemCombo> iulist = new List<ExternalitemCombo>();

            iulist = ITInvoice.Select(g => new ExternalitemCombo { ItemId = g.itemId, ItemName = g.itemName, BranchId = g.branchId }).ToList();
            return iulist;

        }

        public class unitCombo
        {
            private long unitId;
            private string unitName;
            private long itemId;
            private long branchId;
            public long UnitId { get => unitId; set => unitId = value; }
            public string UnitName { get => unitName; set => unitName = value; }
            public long ItemId { get => itemId; set => itemId = value; }
            public long BranchId { get => branchId; set => branchId = value; }
        }
        public class ExternalUnitCombo
        {
            private long? branchId;
            private long? unitId;
            private string unitName;
            private long? itemId;

            public long? UnitId { get => unitId; set => unitId = value; }
            public string UnitName { get => unitName; set => unitName = value; }
            public long? ItemId { get => itemId; set => itemId = value; }
            public long? BranchId { get => branchId; set => branchId = value; }
        }


        public List<unitCombo> getUnitCombo(List<Storage> ITInvoice)
        {
            List<unitCombo> iulist = new List<unitCombo>();
            iulist = ITInvoice.Select(g => new unitCombo { BranchId = g.branchId, UnitId = g.unitId, UnitName = g.unitName, ItemId = g.itemId }).ToList();
            return iulist;
        }

        public List<ExternalUnitCombo> getExternalUnitCombo(List<ItemTransferInvoice> ITInvoice)
        {
            List<ExternalUnitCombo> iulist = new List<ExternalUnitCombo>();
            iulist = ITInvoice.Select(g => new ExternalUnitCombo { BranchId = g.branchId, UnitId = g.unitId, UnitName = g.unitName, ItemId = g.itemId }).ToList();
            return iulist;
        }
        public class sectionCombo
        {
            private long sectionId;
            private string sectionName;
            private long branchId;
            public long SectionId { get => sectionId; set => sectionId = value; }
            public string SectionName { get => sectionName; set => sectionName = value; }
            public long BranchId { get => branchId; set => branchId = value; }
        }

        public List<sectionCombo> getSectionCombo(List<Storage> ITInvoice)
        {
            List<sectionCombo> iulist = new List<sectionCombo>();

            iulist = ITInvoice.Select(g => new sectionCombo { SectionId = (long)g.sectionId, SectionName = g.Secname, BranchId = g.branchId }).ToList();
            return iulist;

        }

        public class locationCombo
        {
            private long locationId;
            private string locationName;
            private long sectionId;
            private long branchId;
            public long LocationId { get => locationId; set => locationId = value; }
            public string LocationName { get => locationName; set => locationName = value; }
            public long SectionId { get => sectionId; set => sectionId = value; }
            public long BranchId { get => branchId; set => branchId = value; }
        }

        public List<locationCombo> getLocationCombo(List<Storage> ITInvoice)
        {
            List<locationCombo> iulist = new List<locationCombo>();

            iulist = ITInvoice.Select(g => new locationCombo { BranchId = g.branchId, LocationId = g.locationId, LocationName = g.LoactionName, SectionId = g.sectionId }).ToList();
            return iulist;

        }


        public class AgentTypeCombo
        {
            private string agentType;
            private long? branchId;

            public long? BranchId { get => branchId; set => branchId = value; }
            public string AgentType { get => agentType; set => agentType = value; }
        }

        public List<AgentTypeCombo> GetExternalAgentTypeCombos(List<ItemTransferInvoice> ITInvoice)
        {
            List<AgentTypeCombo> iulist = new List<AgentTypeCombo>();

            iulist = ITInvoice.Select(g => new AgentTypeCombo { AgentType = g.agentType, BranchId = g.branchId }).ToList();
            return iulist;

        }

        public class AgentCombo
        {
            private long? agentId;
            private string agentName;
            private long? branchId;
            private string agentType;

            public long? AgentId { get => agentId; set => agentId = value; }
            public long? BranchId { get => branchId; set => branchId = value; }
            public string AgentName { get => agentName; set => agentName = value; }
            public string AgentType { get => agentType; set => agentType = value; }
        }

        public List<AgentCombo> GetExternalAgentCombos(List<ItemTransferInvoice> ITInvoice)
        {
            List<AgentCombo> iulist = new List<AgentCombo>();

            iulist = ITInvoice.Select(g => new AgentCombo { AgentId = g.agentId, AgentName = g.agentName, BranchId = g.branchId, AgentType = g.agentType }).ToList();
            return iulist;

        }

        public class InvTypeCombo
        {
            private string invoiceType;
            private long? branchId;

            public string InvoiceType { get => invoiceType; set => invoiceType = value; }
            public long? BranchId { get => branchId; set => branchId = value; }
        }

        public List<InvTypeCombo> GetExternalInvoiceTypeCombos(List<ItemTransferInvoice> ITInvoice)
        {
            List<InvTypeCombo> iulist = new List<InvTypeCombo>();

            iulist = ITInvoice.Select(g => new InvTypeCombo { InvoiceType = g.invType, BranchId = g.branchId }).ToList();
            return iulist;

        }
        public class InvCombo
        {
            private long invoiceId;
            private string invoiceNumber;
            private long? branchId;
            private string invoiceType;

            public long InvoiceId { get => invoiceId; set => invoiceId = value; }
            public string InvoiceNumber { get => invoiceNumber; set => invoiceNumber = value; }
            public long? BranchId { get => branchId; set => branchId = value; }
            public string InvoiceType { get => invoiceType; set => invoiceType = value; }
        }

        public List<InvCombo> GetExternalInvoiceCombos(List<ItemTransferInvoice> ITInvoice)
        {
            List<InvCombo> iulist = new List<InvCombo>();

            iulist = ITInvoice.Select(g => new InvCombo { InvoiceId = g.invoiceId, InvoiceNumber = g.invNumber, BranchId = g.branchId, InvoiceType = g.invType }).ToList();
            return iulist;

        }
        public class internalTypeCombo
        {//type
            private long? branchId;
            private string invType;

            public long? BranchId { get => branchId; set => branchId = value; }
            //  public string InvType { get => invType; set => invType = value; }
            public string InvType { get => invType; set => invType = value; }
            public string trInvType { get; set; }
            //public string InvType
            //{
            //    get => invType == "ex" ? invType = AppSettings.resourcemanager.GetString("trExport")
            //         : invType == "im" ? invType = AppSettings.resourcemanager.GetString("trImport")
            //         : "";

            //    set => invType = value;
            //}
        }


        public List<internalTypeCombo> getTypeCompo(List<ItemTransferInvoice> ITInvoice)
        {
            List<internalTypeCombo> iulist = new List<internalTypeCombo>();
            iulist = ITInvoice.Select(g => new internalTypeCombo { BranchId = g.branchId, InvType = g.invType }).ToList();
            return iulist;
        }
        public class internalOperatorCombo
        {
            private long? branchId;
            private string invNum;

            public long? BranchId { get => branchId; set => branchId = value; }
            public string InvNum { get => invNum; set => invNum = value; }
        }


        public List<internalOperatorCombo> getOperatroCompo(List<ItemTransferInvoice> ITInvoice)
        {
            List<internalOperatorCombo> iulist = new List<internalOperatorCombo>();
            iulist = ITInvoice.Select(g => new internalOperatorCombo { BranchId = g.branchId, InvNum = g.invNumber }).ToList();
            return iulist;
        }
        public class StocktakingArchivesTypeCombo
        {//stocktype
            private long? branchId;
            private string inventoryType;
            private string inventoryTypeText;

            public long? BranchId { get => branchId; set => branchId = value; }
            public string InventoryType { get => inventoryType; set => inventoryType = value; }
            public string InventoryTypeText
            {
                get => inventoryType == "a" ? inventoryTypeText = AppSettings.resourcemanager.GetString("trArchived")
                     : inventoryType == "n" ? inventoryTypeText = AppSettings.resourcemanager.GetString("trSaved")
                     : inventoryType == "d" ? inventoryTypeText = AppSettings.resourcemanager.GetString("trDraft")
                     : "";

                set => inventoryTypeText = value;
            }
        }


        public List<StocktakingArchivesTypeCombo> getStocktakingArchivesTypeCombo(List<InventoryClass> ITInvoice)
        {
            List<StocktakingArchivesTypeCombo> iulist = new List<StocktakingArchivesTypeCombo>();
            iulist = ITInvoice.Select(g => new StocktakingArchivesTypeCombo { BranchId = g.branchId, InventoryType = g.inventoryType }).ToList();
            return iulist;
        }
        public class DestroiedCombo
        {
            private long? branchId;
            private string itemsUnits;
            private long? itemsUnitsId;

            public long? BranchId { get => branchId; set => branchId = value; }
            public string ItemsUnits { get => itemsUnits; set => itemsUnits = value; }
            public long? ItemsUnitsId { get => itemsUnitsId; set => itemsUnitsId = value; }
        }


        public List<DestroiedCombo> getDestroiedCombo(List<ItemTransferInvoice> ITInvoice)
        {
            List<DestroiedCombo> iulist = new List<DestroiedCombo>();
            iulist = ITInvoice.Select(g => new DestroiedCombo { BranchId = g.branchId, ItemsUnitsId = g.itemUnitId, ItemsUnits = g.ItemUnits }).ToList();
            return iulist;
        }
        public class ShortFalls
        {
            private long? branchId;
            private string itemsUnits;
            private long? itemsUnitsId;

            public long? BranchId { get => branchId; set => branchId = value; }
            public string ItemsUnits { get => itemsUnits; set => itemsUnits = value; }
            public long? ItemsUnitsId { get => itemsUnitsId; set => itemsUnitsId = value; }
        }


        public List<ShortFalls> getshortFalls(List<ItemTransferInvoice> ITInvoice)
        {
            List<ShortFalls> iulist = new List<ShortFalls>();
            iulist = ITInvoice.Select(g => new ShortFalls { BranchId = g.branchId, ItemsUnitsId = g.itemUnitId, ItemsUnits = g.ItemUnits }).ToList();
            return iulist;
        }


        #endregion



        public List<CashTransferSts> getstate(List<CashTransferSts> list, int tab, List<CashTransferSts> listAll)
        {
            List<CashTransferSts> list2 = new List<CashTransferSts>();
            IEnumerable<CashTransferSts> temp = list;
            
            if (tab == 1)
            {
                temp = list.Where(t => (t.invShippingCompanyId == null && t.shipUserId == null && t.invAgentId != null) ||
                                          (t.invShippingCompanyId != null && t.shipUserId != null && t.invAgentId != null));
            }
            else if (tab == 3)
            {
                temp = list.Where(t => (t.invShippingCompanyId != null && t.shipUserId == null && t.invAgentId != null)
                                     ||
                                     (t.invShippingCompanyId != null && t.shipUserId == null && t.invAgentId == null)
                );
            }

            list2 = temp.OrderBy(X => X.updateDate).GroupBy(obj => obj.transNum).Select(obj => new CashTransferSts
            {
                bondIsRecieved = obj.FirstOrDefault().bondIsRecieved,
                //processType = obj.FirstOrDefault().processType,
                processType = (obj.FirstOrDefault().processType == "doc" && obj.FirstOrDefault().bondIsRecieved == 1)
                ? (listAll.Where(x => x.bondId == obj.FirstOrDefault().bondId && x.side == "bnd").ToList().Count > 0
                ? listAll.Where(x => x.bondId == obj.FirstOrDefault().bondId && x.side == "bnd").FirstOrDefault().processType : "-")
                : obj.FirstOrDefault().processType,
                bondNumber = obj.FirstOrDefault().bondNumber,
                userId = obj.FirstOrDefault().userId,
                agentId = obj.FirstOrDefault().agentId,
                bondId = obj.FirstOrDefault().bondId,
                transNum = obj.FirstOrDefault().transNum,
                updateDate = obj.FirstOrDefault().updateDate,

                bankName = obj.FirstOrDefault().bankName,
                agentName = obj.FirstOrDefault().agentName,
                usersName = obj.FirstOrDefault().usersName,
                usersLName = obj.FirstOrDefault().usersLName,
                posName = obj.FirstOrDefault().posName,

                updateUserName = obj.FirstOrDefault().updateUserName,
                updateUserAcc = obj.FirstOrDefault().updateUserAcc,
                //cardName = obj.FirstOrDefault().cardName,
                // get pay type from other trans row of bond
                cardName = (obj.FirstOrDefault().processType == "doc" && obj.FirstOrDefault().bondIsRecieved == 1)
                ? (listAll.Where(x => x.bondId == obj.FirstOrDefault().bondId && x.side == "bnd").ToList().Count > 0
                ? listAll.Where(x => x.bondId == obj.FirstOrDefault().bondId && x.side == "bnd").FirstOrDefault().cardName : "-")
                : obj.FirstOrDefault().cardName,

                bondDeserveDate = obj.FirstOrDefault().bondDeserveDate,
                docNum = obj.FirstOrDefault().docNum,
                shippingCompanyId = obj.FirstOrDefault().shippingCompanyId,
                shippingCompanyName = obj.FirstOrDefault().shippingCompanyName,
                userAcc = obj.FirstOrDefault().userAcc,

                cashTransId = obj.FirstOrDefault().cashTransId,
                transType = obj.FirstOrDefault().transType,
                desc = obj.FirstOrDefault().desc,
                invId = obj.FirstOrDefault().invId,
                cash = obj.Sum(x => x.cash),
                cashTotal = 0,
                side = obj.FirstOrDefault().side,

                //invNumber = "",
                invNumber = obj.FirstOrDefault().invNumber,
                invType = obj.FirstOrDefault().invType,
                totalNet = obj.FirstOrDefault().totalNet,

                invShippingCompanyId = obj.FirstOrDefault().invShippingCompanyId,
                invShippingCompanyName = obj.FirstOrDefault().invShippingCompanyName,
                shipUserId = obj.FirstOrDefault().shipUserId,
                invAgentId = obj.FirstOrDefault().invAgentId,
                invAgentName = obj.FirstOrDefault().invAgentName,

                Description = obj.FirstOrDefault().Description,

                Description1 = obj.FirstOrDefault().Description1,

                Description3 = obj.FirstOrDefault().Description3,

            }).Where(t => !(t.side == "bnd" && t.bondIsRecieved == 1)).ToList();
            decimal rowtotal = 0;

            foreach (CashTransferSts row in list2)
            {
                row.Description2 = row.bondId > 0
                ?
               (row.bondIsRecieved == 0 ?
                   AppSettings.resourcemanager.GetString("trBondNotRecieved") :
                  AppSettings.resourcemanager.GetString("trBondRecieved") + "-" + getProcessType(row.processType, row.cardName))
                 :
                  ((row.side == "c") && (row.invShippingCompanyId != null) && (row.processType == "inv") ?
                                                                  row.Description1 + " + " + AppSettings.resourcemanager.GetString("trDeliveryCost")
                                                                : row.Description1);

                row.BIsReceived = row.bondId > 0
               ? ((row.bondIsRecieved == 0 && row.transType == "d") || (row.bondIsRecieved == 0 && row.transType == "p") ?
                   "0" :
                   "1")
                  :
                  "2";

                if (row.transType == "d" && !(row.processType == "doc" && row.bondIsRecieved != 1) && row.side != "mb")
                {
                    rowtotal += (decimal)row.cash;
                }
                else if (row.transType == "p" && !(row.processType == "doc" && row.bondIsRecieved != 1) && row.side != "mb")
                {// p
                    rowtotal -= (decimal)row.cash;
                }
                row.cashTotal = rowtotal;


            }

            return list2;


        }

        private string getProcessType(string value, string name)
        {
            switch (value)
            {
                case "cash": return AppSettings.resourcemanager.GetString("trCash");
                case "doc": return AppSettings.resourcemanager.GetString("trDocument");
                case "cheque": return AppSettings.resourcemanager.GetString("trCheque");
                case "balance": return AppSettings.resourcemanager.GetString("trCredit");
                //case "card": return AppSettings.resourcemanager.GetString("trAnotherPaymentMethods");
                case "card": return name;
                case "inv": return AppSettings.resourcemanager.GetString("trInv");
                default: return value;
            }
        }

        private string getProcessType(string value)
        {
            switch (value)
            {
                case "cash": return AppSettings.resourcemanager.GetString("trCash");
                case "doc": return AppSettings.resourcemanager.GetString("trDocument");
                case "cheque": return AppSettings.resourcemanager.GetString("trCheque");
                case "balance": return AppSettings.resourcemanager.GetString("trCredit");
                case "card": return AppSettings.resourcemanager.GetString("trAnotherPaymentMethods");
                case "inv": return AppSettings.resourcemanager.GetString("trInv");
                default: return value;
            }
        }

    }
}
