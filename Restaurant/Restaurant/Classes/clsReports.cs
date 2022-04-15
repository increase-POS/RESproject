using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Restaurant.View.storage;
using Restaurant.Classes.ApiClasses;
//Restaurant.Classes
namespace Restaurant.Classes
{
    class clsReports
    {
        public static void setReportLanguage(List<ReportParameter> paramarr)
        {

            paramarr.Add(new ReportParameter("lang", AppSettings.Reportlang));

        }

        public static void Header(List<ReportParameter> paramarr)
        {

            ReportCls rep = new ReportCls();
            paramarr.Add(new ReportParameter("companyName", AppSettings.companyName));
            paramarr.Add(new ReportParameter("Fax", AppSettings.Fax));
            paramarr.Add(new ReportParameter("Tel", AppSettings.Mobile));
            paramarr.Add(new ReportParameter("Address", AppSettings.Address));
            paramarr.Add(new ReportParameter("Email", AppSettings.Email));
            paramarr.Add(new ReportParameter("logoImage", "file:\\" + rep.GetLogoImagePath()));
            paramarr.Add(new ReportParameter("show_header", AppSettings.show_header));
        }
        public static void HeaderNoLogo(List<ReportParameter> paramarr)
        {

            ReportCls rep = new ReportCls();
            paramarr.Add(new ReportParameter("companyName", AppSettings.companyName));
            paramarr.Add(new ReportParameter("Fax", AppSettings.Fax));
            paramarr.Add(new ReportParameter("Tel", AppSettings.Mobile));
            paramarr.Add(new ReportParameter("Address", AppSettings.Address));
            paramarr.Add(new ReportParameter("Email", AppSettings.Email));


        }
        public static void bankdg(List<ReportParameter> paramarr)
        {

            
            paramarr.Add(new ReportParameter("trTransferNumber", AppSettings.resourcemanagerreport.GetString("trTransferNumberTooltip")));


        }
        public static void bondsDocReport(LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            DateFormConv(paramarr);

        }
        //public static void bondsReport(IEnumerable<Bonds> bondsQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        //{
        //    rep.ReportPath = reppath;
        //    rep.EnableExternalImages = true;
        //    rep.DataSources.Clear();

        //    paramarr.Add(new ReportParameter("trDocNumTooltip", AppSettings.resourcemanagerreport.GetString("trDocNumTooltip")));
        //    paramarr.Add(new ReportParameter("trRecipientTooltip", AppSettings.resourcemanagerreport.GetString("trRecipientTooltip")));

        //    paramarr.Add(new ReportParameter("trPaymentTypeTooltip", AppSettings.resourcemanagerreport.GetString("trPaymentTypeTooltip")));

        //    paramarr.Add(new ReportParameter("trDocDateTooltip", AppSettings.resourcemanagerreport.GetString("trDocDateTooltip")));

        //    paramarr.Add(new ReportParameter("trPayDate", AppSettings.resourcemanagerreport.GetString("trPayDate")));
        //    paramarr.Add(new ReportParameter("trCashTooltip", AppSettings.resourcemanagerreport.GetString("trCashTooltip")));

        //    foreach (var c in bondsQuery)
        //    {

        //        c.amount = decimal.Parse(HelpClass.DecTostring(c.amount));
        //    }
        //    rep.DataSources.Add(new ReportDataSource("DataSetBond", bondsQuery));

        //    DateFormConv(paramarr);
        //    AccountSideConv(paramarr);
        //    cashTransTypeConv(paramarr);

        //}


        //public static void orderReport(IEnumerable<Invoice> invoiceQuery, LocalReport rep, string reppath)
        //{
        //    rep.ReportPath = reppath;
        //    rep.EnableExternalImages = true;
        //    rep.DataSources.Clear();
        //    foreach(var o in invoiceQuery)
        //    {
        //        string status = "";
        //        switch (o.status)
        //        {
        //            case "tr":
        //                status = AppSettings.resourcemanager.GetString("trDelivered");
        //                break;
        //            case "rc":
        //                status = AppSettings.resourcemanager.GetString("trInDelivery");
        //                break;
        //            default:
        //                status = "";
        //                break;
        //        }
        //        o.status = status;
        //        o.deserved = decimal.Parse(HelpClass.DecTostring(o.deserved));
        //    }
        //    rep.DataSources.Add(new ReportDataSource("DataSetInvoice", invoiceQuery));
        //}
        public static void orderReport(IEnumerable<Invoice> invoiceQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            foreach (var o in invoiceQuery)
            {
                o.deserved = decimal.Parse(HelpClass.DecTostring(o.deserved));
            }
            DeliverStateConv(paramarr);
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trOrders")));

            paramarr.Add(new ReportParameter("trInvoiceNumber", AppSettings.resourcemanagerreport.GetString("trInvoiceNumber")));
            paramarr.Add(new ReportParameter("trSalesMan", AppSettings.resourcemanagerreport.GetString("trSalesMan")));
            paramarr.Add(new ReportParameter("trCustomer", AppSettings.resourcemanagerreport.GetString("trCustomer")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trCashTooltip", AppSettings.resourcemanagerreport.GetString("trCashTooltip")));
            paramarr.Add(new ReportParameter("trState", AppSettings.resourcemanagerreport.GetString("trState")));
            
            DateFormConv(paramarr);


            rep.DataSources.Add(new ReportDataSource("DataSetInvoice", invoiceQuery));
        }
        public static void DeliverStateConv(List<ReportParameter> paramarr)
        {
            paramarr.Add(new ReportParameter("trDelivered", AppSettings.resourcemanagerreport.GetString("trDelivered")));
            paramarr.Add(new ReportParameter("trInDelivery", AppSettings.resourcemanagerreport.GetString("trInDelivery")));

        }

        public static void bankAccReport(IEnumerable<CashTransfer> cash, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {

            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
             paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trBankAccounts")));
            paramarr.Add(new ReportParameter("trTransferNumberTooltip", AppSettings.resourcemanagerreport.GetString("trTransferNumberTooltip")));
            paramarr.Add(new ReportParameter("trBank", AppSettings.resourcemanagerreport.GetString("trBank")));
            paramarr.Add(new ReportParameter("trDepositeNumTooltip", AppSettings.resourcemanagerreport.GetString("trDepositeNumTooltip")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trCashTooltip", AppSettings.resourcemanagerreport.GetString("trCashTooltip")));
            DateFormConv(paramarr);
            foreach (var c in cash)
            {
                ///////////////////
                c.cash = decimal.Parse(HelpClass.DecTostring(c.cash));
                string s;
                switch (c.processType)
                {
                    case "cash":
                        s = AppSettings.resourcemanagerreport.GetString("trCash");
                        break;
                    case "doc":
                        s = AppSettings.resourcemanagerreport.GetString("trDocument");
                        break;
                    case "cheque":
                        s = AppSettings.resourcemanagerreport.GetString("trCheque");
                        break;
                    case "balance":
                        s = AppSettings.resourcemanagerreport.GetString("trCredit");
                        break;
                    case "card":
                        s = AppSettings.resourcemanagerreport.GetString("trCreditCard");
                        break;
                    default:
                        s = c.processType;
                        break;
                }
                ///////////////////
                c.processType = s;
                string name = "";
                switch (c.side)
                {
                    case "bnd": break;
                    case "v": name = AppSettings.resourcemanagerreport.GetString("trVendor"); break;
                    case "c": name = AppSettings.resourcemanagerreport.GetString("trCustomer"); break;
                    case "u": name = AppSettings.resourcemanagerreport.GetString("trUser"); break;
                    case "s": name = AppSettings.resourcemanagerreport.GetString("trSalary"); break;
                    case "e": name = AppSettings.resourcemanagerreport.GetString("trGeneralExpenses"); break;
                    case "m":
                        if (c.transType == "d")
                            name = AppSettings.resourcemanagerreport.GetString("trAdministrativeDeposit");
                        if (c.transType == "p")
                            name = AppSettings.resourcemanagerreport.GetString("trAdministrativePull");
                        break;
                    case "sh": name = AppSettings.resourcemanagerreport.GetString("trShippingCompany"); break;
                    default: break;
                }
                string fullName = "";
                if (!string.IsNullOrEmpty(c.agentName))
                    fullName = name + " " + c.agentName;
                else if (!string.IsNullOrEmpty(c.usersLName))
                    fullName = name + " " + c.usersLName;
                else if (!string.IsNullOrEmpty(c.shippingCompanyName))
                    fullName = name + " " + c.shippingCompanyName;
                else
                    fullName = name;
                /////////////////////
                c.side = fullName;

                string type;
                if (c.transType.Equals("p")) type = AppSettings.resourcemanagerreport.GetString("trPull");
                else type = AppSettings.resourcemanagerreport.GetString("trDeposit");
                ////////////////////
                c.transType = type;
            }
            rep.DataSources.Add(new ReportDataSource("DataSetBankAcc", cash));
        }

        public static void paymentAccReport(IEnumerable<CashTransfer> cash, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            //foreach (var c in cash)
            //{
            //    ///////////////////
            //    c.cash = decimal.Parse(SectionData.DecTostring(c.cash));
            //    string s;
            //    switch (c.processType)
            //    {
            //        case "cash":
            //            s = AppSettings.resourcemanagerreport.GetString("trCash");
            //            break;
            //        case "doc":
            //            s = AppSettings.resourcemanagerreport.GetString("trDocument");
            //            break;
            //        case "cheque":
            //            s = AppSettings.resourcemanagerreport.GetString("trCheque");
            //            break;
            //        case "balance":
            //            s = AppSettings.resourcemanagerreport.GetString("trCredit");
            //            break;
            //        default:
            //            s = c.processType;
            //            break;
            //    }


            //}


            AccountSideConv(paramarr);

            cashTransTypeConv(paramarr);
            cashTransferProcessTypeConv(paramarr);
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trPayments")));

            paramarr.Add(new ReportParameter("trTransferNumberTooltip", AppSettings.resourcemanagerreport.GetString("trTransferNumberTooltip")));
            paramarr.Add(new ReportParameter("trRecepient", AppSettings.resourcemanagerreport.GetString("trRecepient")));
            paramarr.Add(new ReportParameter("trPaymentTypeTooltip", AppSettings.resourcemanagerreport.GetString("trPaymentTypeTooltip")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trCashTooltip", AppSettings.resourcemanagerreport.GetString("trCashTooltip")));
            paramarr.Add(new ReportParameter("accuracy", AppSettings.accuracy));
            paramarr.Add(new ReportParameter("trUnKnown", AppSettings.resourcemanagerreport.GetString("trUnKnown")));
            paramarr.Add(new ReportParameter("trCashCustomer", AppSettings.resourcemanagerreport.GetString("trCashCustomer")));

            DateFormConv(paramarr);


            foreach (var c in cash)
            {

                c.cash = decimal.Parse(HelpClass.DecTostring(c.cash));
                // c.notes = SectionData.DecTostring(c.cash);
                c.agentName = AgentUnKnownConvert(c.agentId, c.side, c.agentName);

            }
            rep.DataSources.Add(new ReportDataSource("DataSetBankAcc", cash));
        }
        public static string AgentUnKnownConvert(int? agentId, string side, string agentName)
        {

            if (agentId == null)
            {
                if (side == "v")
                {
                    agentName = AppSettings.resourcemanagerreport.GetString("trUnKnown");
                }
                else if (side == "c")
                {
                    agentName = AppSettings.resourcemanagerreport.GetString("trCashCustomer");
                }
            }
            return agentName;

        }
        public static string AgentCompanyUnKnownConvert(int? agentId, string side, string agentCompany)
        {
            if (agentId == null)
            {
                agentCompany = AppSettings.resourcemanagerreport.GetString("trUnKnown");

            }
            return agentCompany;
        }


        public static void receivedAccReport(IEnumerable<CashTransfer> cash, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {

            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            foreach (var c in cash)
            {

                c.cash = decimal.Parse(HelpClass.DecTostring(c.cash));
            }
            DateFormConv(paramarr);
            AccountSideConv(paramarr);

            cashTransTypeConv(paramarr);
            cashTransferProcessTypeConv(paramarr);
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trReceived")));
            paramarr.Add(new ReportParameter("trTransferNumberTooltip", AppSettings.resourcemanagerreport.GetString("trTransferNumberTooltip")));
            paramarr.Add(new ReportParameter("trDepositor", AppSettings.resourcemanagerreport.GetString("trDepositor")));
            paramarr.Add(new ReportParameter("trPaymentTypeTooltip", AppSettings.resourcemanagerreport.GetString("trPaymentTypeTooltip")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trCashTooltip", AppSettings.resourcemanagerreport.GetString("trCashTooltip")));
            paramarr.Add(new ReportParameter("accuracy", AppSettings.accuracy));
            paramarr.Add(new ReportParameter("trUnKnown", AppSettings.resourcemanagerreport.GetString("trUnKnown")));
            paramarr.Add(new ReportParameter("trCashCustomer", AppSettings.resourcemanagerreport.GetString("trCashCustomer")));

            rep.DataSources.Add(new ReportDataSource("DataSetBankAcc", cash));
        }

        public static void SubscriptionAcc(IEnumerable<AgentMembershipCash> cash, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {

            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            foreach (var c in cash)
            {

                c.total = decimal.Parse(HelpClass.DecTostring(c.total));
                c.subscriptionTypeconv = subscriptionTypeConverter(c.subscriptionType);
                c.EndDateconv= unlimitedEndDateConverter(c.subscriptionType,c.EndDate);
              
                    }
            DateFormConv(paramarr);
            //   cashTransTypeConv(paramarr);
            //    cashTransferProcessTypeConv(paramarr);
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trSubscriptions")));
            paramarr.Add(new ReportParameter("trCustomer", AppSettings.resourcemanagerreport.GetString("trCustomer")));
            paramarr.Add(new ReportParameter("trSubscriptionType", AppSettings.resourcemanagerreport.GetString("trSubscriptionType")));
            paramarr.Add(new ReportParameter("trTransferNumberTooltip", AppSettings.resourcemanagerreport.GetString("trTransferNumberTooltip")));
            paramarr.Add(new ReportParameter("trDepositor", AppSettings.resourcemanagerreport.GetString("trDepositor")));
            paramarr.Add(new ReportParameter("trPaymentTypeTooltip", AppSettings.resourcemanagerreport.GetString("trPaymentTypeTooltip")));
            paramarr.Add(new ReportParameter("trExpireDate", AppSettings.resourcemanagerreport.GetString("trExpireDate")));
            paramarr.Add(new ReportParameter("trAmount", AppSettings.resourcemanagerreport.GetString("trAmount")));
            paramarr.Add(new ReportParameter("accuracy", AppSettings.accuracy));

            rep.DataSources.Add(new ReportDataSource("DataSetBankAcc", cash));
        }
        public static string subscriptionTypeConverter(string subscriptionType)
        {
     
            switch (subscriptionType)
            {
                case "f": return AppSettings.resourcemanagerreport.GetString("trFree");

                case "m": return AppSettings.resourcemanagerreport.GetString("trMonthly");

                case "y": return AppSettings.resourcemanagerreport.GetString("trYearly");

                case "o": return AppSettings.resourcemanagerreport.GetString("trOnce");

                default: return AppSettings.resourcemanagerreport.GetString("");

            }
        }

        public static string unlimitedEndDateConverter(string subscriptionType, DateTime? EndDate )
        {
            if (subscriptionType != null && EndDate != null)
            {
                string sType = subscriptionType;
                DateTime sDate = (DateTime)EndDate;

                if (sType == "o")
                    return AppSettings.resourcemanager.GetString("trUnlimited");
                else
                {
                 
                   
                    switch (AppSettings.dateFormat)
                    {
                        case "ShortDatePattern":
                            return sDate.ToString(@"dd/MM/yyyy");
                        case "LongDatePattern":
                            return sDate.ToString(@"dddd, MMMM d, yyyy");
                        case "MonthDayPattern":
                            return sDate.ToString(@"MMMM dd");
                        case "YearMonthPattern":
                            return sDate.ToString(@"MMMM yyyy");
                        default:
                            return sDate.ToString(@"dd/MM/yyyy");
                    }

                }
            }
            else return "";
        }
        public static void posAccReport(IEnumerable<CashTransfer> cash, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var c in cash)
            {

                c.cash = decimal.Parse(HelpClass.DecTostring(c.cash));
            }

            paramarr.Add(new ReportParameter("trTransferNumberTooltip", AppSettings.resourcemanagerreport.GetString("trTransferNumberTooltip")));
            paramarr.Add(new ReportParameter("trCreator", AppSettings.resourcemanagerreport.GetString("trCreator")));
            paramarr.Add(new ReportParameter("trStatus", AppSettings.resourcemanagerreport.GetString("trStatus")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trCashTooltip", AppSettings.resourcemanagerreport.GetString("trCashTooltip")));
            paramarr.Add(new ReportParameter("trConfirmed", AppSettings.resourcemanagerreport.GetString("trConfirmed")));
            paramarr.Add(new ReportParameter("trCanceled", AppSettings.resourcemanagerreport.GetString("trCanceled")));
            paramarr.Add(new ReportParameter("trWaiting", AppSettings.resourcemanagerreport.GetString("trWaiting")));

            DateFormConv(paramarr);

            rep.DataSources.Add(new ReportDataSource("DataSetBankAcc", cash));
        }

        public static void posAccReportSTS(IEnumerable<CashTransfer> cash, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            posAccReport(cash, rep, reppath, paramarr);
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));

            paramarr.Add(new ReportParameter("trAccoutant", AppSettings.resourcemanagerreport.GetString("trAccoutant")));
            paramarr.Add(new ReportParameter("trAmount", AppSettings.resourcemanagerreport.GetString("trAmount")));



        }
        public string posTransfersStatusConverter(byte isConfirm1, byte isConfirm2)
        {

            if ((isConfirm1 == 1) && (isConfirm2 == 1))
                return AppSettings.resourcemanager.GetString("trConfirmed");
            else if ((isConfirm1 == 2) || (isConfirm2 == 2))
                return AppSettings.resourcemanager.GetString("trCanceled");
            else
                return AppSettings.resourcemanager.GetString("trWaiting");
        }



        public static void invItem(IEnumerable<InventoryItemLocation> itemLocations, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            rep.DataSources.Add(new ReportDataSource("DataSetInvItemLocation", itemLocations));
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
        }
        public static void section(IEnumerable<Section> sections, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetSection", sections));
        }
        public static void location(IEnumerable<Location> locations, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetLocation", locations));
        }
        public static void itemLocation(IEnumerable<ItemLocation> itemLocations, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemLocation", itemLocations));
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
        }
        public static void bankReport(IEnumerable<Bank> banksQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetBank", banksQuery));
        }
        public static void tablesReport(IEnumerable<Tables> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetTables", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trTheTables")));
            //table columns
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trPersonsCount", AppSettings.resourcemanagerreport.GetString("trPersonsCount")));
            paramarr.Add(new ReportParameter("trSection", AppSettings.resourcemanagerreport.GetString("trSection")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));
           
        }

        public static void hallSectionsReport(IEnumerable<HallSection> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetHallSections", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trSections")));
            //table columns
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trDetails", AppSettings.resourcemanagerreport.GetString("trDetails")));
            paramarr.Add(new ReportParameter("trBranchStore", AppSettings.resourcemanagerreport.GetString("trBranch/Store")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));

        }

        public static void CustomerReport(IEnumerable<Agent> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetAgent", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trCustomers")));
            //table columns
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trCompany", AppSettings.resourcemanagerreport.GetString("trCompany")));
            paramarr.Add(new ReportParameter("trMobile", AppSettings.resourcemanagerreport.GetString("trMobile")));

        }

        public static void VendorReport(IEnumerable<Agent> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetAgent", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trVendors")));
            //table columns
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trCompany", AppSettings.resourcemanagerreport.GetString("trCompany")));
            paramarr.Add(new ReportParameter("trMobile", AppSettings.resourcemanagerreport.GetString("trMobile")));

        }

        public static void UserReport(IEnumerable<User> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetUser", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trUsers")));
            //table columns
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trJob", AppSettings.resourcemanagerreport.GetString("trJob")));
            paramarr.Add(new ReportParameter("trWorkHours", AppSettings.resourcemanagerreport.GetString("trWorkHours")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));

        }

        public static void BranchesReport(IEnumerable<Branch> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetBranch", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trBranches")));
            //table columns

            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trMobile", AppSettings.resourcemanagerreport.GetString("trMobile")));
            paramarr.Add(new ReportParameter("trBranchAddress", AppSettings.resourcemanagerreport.GetString("trAddress")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));

        }
        public static void PosReport(IEnumerable<Pos> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetPos", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trPOSs")));
            //table columns
            paramarr.Add(new ReportParameter("trPosCode", AppSettings.resourcemanagerreport.GetString("trPosCode")));
            paramarr.Add(new ReportParameter("trPosName", AppSettings.resourcemanagerreport.GetString("trPosName")));
            paramarr.Add(new ReportParameter("trBranchName", AppSettings.resourcemanagerreport.GetString("trBranchName")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));

        }
        public static void StoresReport(IEnumerable<Branch> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetBranch", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trStores")));
            //table columns
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trMobile", AppSettings.resourcemanagerreport.GetString("trMobile")));
            paramarr.Add(new ReportParameter("trBranchAddress", AppSettings.resourcemanagerreport.GetString("trAddress")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));

        }



        public static void BanksReport(IEnumerable<Bank> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetBank", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trBanks")));
            //table columns
            paramarr.Add(new ReportParameter("trBankName", AppSettings.resourcemanagerreport.GetString("trBankName")));
            paramarr.Add(new ReportParameter("trAccNumber", AppSettings.resourcemanagerreport.GetString("trAccNumber")));
            paramarr.Add(new ReportParameter("trBankAddress", AppSettings.resourcemanagerreport.GetString("trAddress")));
            paramarr.Add(new ReportParameter("trMobile", AppSettings.resourcemanagerreport.GetString("trMobile")));

        }

        public static void CardsReport(IEnumerable<Card> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetCard", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trCards")));
            //table columns
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));
  
        }

        // Catalog
        public static void categoryReport(IEnumerable<Category> categoryQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            //foreach (var r in categoryQuery)
            //{
            //    r.taxes = decimal.Parse(HelpClass.PercentageDecTostring(r.taxes));
            //}
            rep.DataSources.Add(new ReportDataSource("DataSetCategory", categoryQuery));
            paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trCategories")));
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trDetails", AppSettings.resourcemanagerreport.GetString("trDetails")));
        }

        public static void itemReport(IEnumerable<Item> items, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            itemdata(items, rep, reppath, paramarr);


            paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trItems")));
        }
        public static void FoodReport(IEnumerable<Item> items, LocalReport rep, string reppath, List<ReportParameter> paramarr,string categoryName)
        {
            string title = AppSettings.resourcemanagerreport.GetString("trFoods");
            itemdata(items, rep, reppath, paramarr);
            if (categoryName == "appetizers")
            {
                categoryName= AppSettings.resourcemanagerreport.GetString("trAppetizers");
            }
            else if (categoryName == "beverages")
            {
                categoryName = AppSettings.resourcemanagerreport.GetString("trBeverages");
            }
            else if (categoryName == "fastFood")
            {
                categoryName = AppSettings.resourcemanagerreport.GetString("trFastFood");
            }
            else if (categoryName == "mainCourses")
            {
                categoryName = AppSettings.resourcemanagerreport.GetString("trMainCourses"); ;
            }
            else if (categoryName == "desserts")
            {
                categoryName = AppSettings.resourcemanagerreport.GetString("trDesserts");
            }
            else if (categoryName == "package")
            {
                categoryName = AppSettings.resourcemanagerreport.GetString("trPackages");
            }
            
            title = title + " / " + categoryName;

            paramarr.Add(new ReportParameter("Title", title));

        }
        public static void itemdata(IEnumerable<Item> items, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            //foreach (Item r in _items)
            //{
            //    r.taxes = decimal.Parse(HelpClass.DecTostring(r.taxes));
            //}
            rep.DataSources.Add(new ReportDataSource("DataSetItem", items));
         
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trDetails", AppSettings.resourcemanagerreport.GetString("trDetails")));
            paramarr.Add(new ReportParameter("trCategory", AppSettings.resourcemanagerreport.GetString("trCategorie")));
        }
        public static void unitReport(IEnumerable<Unit> unitQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetUnit", unitQuery));
            paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trUnits")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trUnitName")));
            paramarr.Add(new ReportParameter("trNotes", AppSettings.resourcemanagerreport.GetString("trNote")));

        }
        //
        public static void LocationsReport(IEnumerable<Location> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetLocation", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trLocations")));
            //table columns
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trSection", AppSettings.resourcemanagerreport.GetString("trSection")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));

        }

        public static void SectionReport(IEnumerable<Section> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetSection", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trSections")));
            //table columns
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trBranchStore", AppSettings.resourcemanagerreport.GetString("trBranch/Store")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));

        }


        public static void ItemsDestructive(IEnumerable<InventoryItemLocation> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemsDestructive", invoiceItems)); 
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trDestructives")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trSection", AppSettings.resourcemanagerreport.GetString("trSection") + "-" + AppSettings.resourcemanagerreport.GetString("trLocation")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem") + "-" + AppSettings.resourcemanagerreport.GetString("trUnit")));
           
            paramarr.Add(new ReportParameter("trQuantity", AppSettings.resourcemanagerreport.GetString("trAmount")));
            DateFormConv(paramarr);
                
        }

        public static void ItemsShortage(IEnumerable<InventoryItemLocation> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemsShortage", invoiceItems));
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trShortages")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trInventoryNum")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trSection", AppSettings.resourcemanagerreport.GetString("trSectionLocation")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItemUnit") ));

            paramarr.Add(new ReportParameter("trQuantity", AppSettings.resourcemanagerreport.GetString("trAmount")));
            DateFormConv(paramarr);

        }

        public static void Stocktaking(IEnumerable<InventoryItemLocation> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetStocktaking", invoiceItems));
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trStocktakingItems")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trSectionLocation", AppSettings.resourcemanagerreport.GetString("trSectionLocation")));
            paramarr.Add(new ReportParameter("trItemUnit", AppSettings.resourcemanagerreport.GetString("trItemUnit") ));
            paramarr.Add(new ReportParameter("trRealAmount", AppSettings.resourcemanagerreport.GetString("trRealAmount")));
            paramarr.Add(new ReportParameter("trInventoryAmount", AppSettings.resourcemanagerreport.GetString("trInventoryAmount")));
            paramarr.Add(new ReportParameter("trDestoryCount", AppSettings.resourcemanagerreport.GetString("trDestoryCount")));
          
            DateFormConv(paramarr);

        }

        public static void ItemsStorage(IEnumerable<ItemLocation> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemsStorage", invoiceItems));
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trLocations")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trItemUnit", AppSettings.resourcemanagerreport.GetString("trItemUnit")));
            paramarr.Add(new ReportParameter("trSectionLocation", AppSettings.resourcemanagerreport.GetString("trSectionLocation")));
            paramarr.Add(new ReportParameter("trQuantity", AppSettings.resourcemanagerreport.GetString("trQuantity")));
            paramarr.Add(new ReportParameter("trStartDate", AppSettings.resourcemanagerreport.GetString("trStartDate")));
            paramarr.Add(new ReportParameter("trEndDate", AppSettings.resourcemanagerreport.GetString("trEndDate")));
            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));
            paramarr.Add(new ReportParameter("trOrderNum", AppSettings.resourcemanagerreport.GetString("trOrderNum")));
            DateFormConv(paramarr);

        }

        public static void StorageInvoice(IEnumerable<ItemTransfer> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetStorageInvoice", invoiceItems));
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trInvoice")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));
            paramarr.Add(new ReportParameter("trPrice", AppSettings.resourcemanagerreport.GetString("trPrice")));
            paramarr.Add(new ReportParameter("trTotal", AppSettings.resourcemanagerreport.GetString("trTotal")));
           
            DateFormConv(paramarr);

        }

        public static void StorageCosts(IEnumerable<StorageCost> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetStorageCost", invoiceItems));
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trStorageCostPerDay")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trStorageCost", AppSettings.resourcemanagerreport.GetString("trStorageCost")));
        

            DateFormConv(paramarr);

        }


        public static void SpendingOrder(IEnumerable<ItemTransfer> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetSpendingOrder", invoiceItems));
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trInvoice"))); 
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trQuantity", AppSettings.resourcemanagerreport.GetString("trQuantity")));

            DateFormConv(paramarr);

        }


        public static void ResidentialSectorReport(IEnumerable<ResidentialSectors> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataResidentialSectors", Query));
            //title
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trResidentialSectors")));
            //table columns
           
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));

            paramarr.Add(new ReportParameter("trNote", AppSettings.resourcemanagerreport.GetString("trNote")));

        } 
        public static void ErrorsReport(IEnumerable<ErrorClass> Query, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetError", Query));
        }

        public static void couponReport(IEnumerable<Coupon> CouponQuery2, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {

            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var c in CouponQuery2)
            {
                c.discountValue = decimal.Parse(HelpClass.DecTostring(c.discountValue));
                c.invMin = decimal.Parse(HelpClass.DecTostring(c.invMin));
                c.invMax = decimal.Parse(HelpClass.DecTostring(c.invMax));
               
                string state = "";
                //(c.isActive == 1) && ((c.endDate > DateTime.Now)||(c.endDate == null)) && ((c.quantity == 0) || (c.quantity > 0 && c.remainQ != 0))
                if ((c.isActive == 1) && ((c.endDate > DateTime.Now) || (c.endDate == null)) && ((c.quantity == 0) || (c.quantity > 0 && c.remainQ != 0)))
                    state = AppSettings.resourcemanagerreport.GetString("trValid");
                else
                    state = AppSettings.resourcemanagerreport.GetString("trExpired");

                c.state = state;

            }

            rep.DataSources.Add(new ReportDataSource("DataSetCoupon", CouponQuery2));
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
           // paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trCoupons")));
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trValue", AppSettings.resourcemanagerreport.GetString("trValue")));
            paramarr.Add(new ReportParameter("trQuantity", AppSettings.resourcemanagerreport.GetString("trQuantity")));
            paramarr.Add(new ReportParameter("trRemainQuantity", AppSettings.resourcemanagerreport.GetString("trRemainQuantity")));
            paramarr.Add(new ReportParameter("trvalidity", AppSettings.resourcemanagerreport.GetString("trvalidity")));
            paramarr.Add(new ReportParameter("trUnlimited", AppSettings.resourcemanagerreport.GetString("trUnlimited")));

        }
        public string unlimitedCouponConv(decimal quantity)
        {
           
            if (quantity == 0)
                return AppSettings.resourcemanagerreport.GetString("trUnlimited");
            else
                return quantity.ToString();
        }
        public static void couponExportReport(LocalReport rep, string reppath, List<ReportParameter> paramarr, string barcode)
        {

            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            ReportCls repcls = new ReportCls();


            paramarr.Add(new ReportParameter("invNumber", barcode));
            paramarr.Add(new ReportParameter("barcodeimage", "file:\\" + repcls.BarcodeToImage(barcode, "barcode")));

        }

        public static void packageReport(IEnumerable<Item> packageQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();


            rep.DataSources.Add(new ReportDataSource("DataSetItem", packageQuery));
            //    paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
            paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trPackageItems")));
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trPackage")));
            paramarr.Add(new ReportParameter("trDetails", AppSettings.resourcemanagerreport.GetString("trDetails")));
            paramarr.Add(new ReportParameter("trCategory", AppSettings.resourcemanagerreport.GetString("trCategorie")));

        }
        public static void serviceReport(IEnumerable<Item> serviceQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();


            rep.DataSources.Add(new ReportDataSource("DataSetItem", serviceQuery));
            //    paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));trTheService trTheServices
            paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trTheServices")));
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trTheService")));
            paramarr.Add(new ReportParameter("trDetails", AppSettings.resourcemanagerreport.GetString("trDetails")));
            paramarr.Add(new ReportParameter("trCategory", AppSettings.resourcemanagerreport.GetString("trCategorie")));

        }
        public static void offerReport(IEnumerable<Offer> OfferQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var o in OfferQuery)
            {

                o.discountValue = decimal.Parse(HelpClass.DecTostring(o.discountValue));
            }

            rep.DataSources.Add(new ReportDataSource("DataSetOffer", OfferQuery));
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
            paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trDiscount", AppSettings.resourcemanagerreport.GetString("trValue")));
            paramarr.Add(new ReportParameter("trStartDate", AppSettings.resourcemanagerreport.GetString("trStartDate")));
            paramarr.Add(new ReportParameter("trEndDate", AppSettings.resourcemanagerreport.GetString("trEndDate")));



        }
        public static void cardReport(IEnumerable<Card> cardsQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetCard", cardsQuery));
        }
        public static void shippingReport(IEnumerable<ShippingCompanies> shippingCompanies, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetShipping", shippingCompanies));
        }
        public static string itemTypeConverter(string value)
        {
            string s = "";
            switch (value)
            {
                case "n": s = AppSettings.resourcemanagerreport.GetString("trNormals"); break;
                case "d": s = AppSettings.resourcemanagerreport.GetString("trHaveExpirationDates"); break;
                case "sn": s = AppSettings.resourcemanagerreport.GetString("trHaveSerialNumbers"); break;
                case "sr": s = AppSettings.resourcemanagerreport.GetString("trServices"); break;
                case "p": s = AppSettings.resourcemanagerreport.GetString("trPackages"); break;
            }

            return s;
        }
        public static string BranchStoreConverter(string type)
        {
            string s = "";
            switch (type)
            {
                case "b": s = AppSettings.resourcemanagerreport.GetString("tr_Branch"); break;
                case "s": s = AppSettings.resourcemanagerreport.GetString("tr_Store"); break;

            }

            return s;
        }
        public static void PurStsReport(IEnumerable<ItemTransferInvoice> tempquery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var r in tempquery)
            {
                r.CopdiscountValue = decimal.Parse(HelpClass.DecTostring(r.CopdiscountValue));
                r.couponTotalValue = decimal.Parse(HelpClass.DecTostring(r.couponTotalValue));//
                r.OdiscountValue = decimal.Parse(HelpClass.DecTostring(r.OdiscountValue));
                r.offerTotalValue = decimal.Parse(HelpClass.DecTostring(r.offerTotalValue));
                r.ITprice = decimal.Parse(HelpClass.DecTostring(r.ITprice));
                r.subTotal = decimal.Parse(HelpClass.DecTostring(r.subTotal));
                r.totalNet = decimal.Parse(HelpClass.DecTostring(r.totalNet));
                r.discountValue = decimal.Parse(HelpClass.DecTostring(r.discountValue));
                r.tax = decimal.Parse(HelpClass.DecTostring(r.tax));
                if (r.itemAvg != null)
                {
                    r.itemAvg = double.Parse(HelpClass.DecTostring(decimal.Parse(r.itemAvg.ToString())));

                }
            }

            rep.DataSources.Add(new ReportDataSource("DataSetITinvoice", tempquery));


        }
        public static void PurItemStsReport(IEnumerable<ItemTransferInvoice> tempquery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            PurStsReport(tempquery, rep, reppath);
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
         
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trCompany", AppSettings.resourcemanagerreport.GetString("trCompany")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));
            paramarr.Add(new ReportParameter("trInvoices", AppSettings.resourcemanagerreport.GetString("trInvoices")));
            paramarr.Add(new ReportParameter("trPrice", AppSettings.resourcemanagerreport.GetString("trPrice")));
    
            paramarr.Add(new ReportParameter("trTotal", AppSettings.resourcemanagerreport.GetString("trTotal")));
            paramarr.Add(new ReportParameter("trI_nvoice", AppSettings.resourcemanager.GetString("trItem") + "/" + AppSettings.resourcemanager.GetString("trInvoices")));

        }

        public static void saleitemStsReport(IEnumerable<ItemTransferInvoice> tempquery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {

            itemTypeConv(paramarr);
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
            PurStsReport(tempquery, rep, reppath);

        }

        public static void SalePromoStsReport(IEnumerable<ItemTransferInvoice> tempquery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            PurStsReport(tempquery, rep, reppath);

            itemTransferDiscountTypeConv(paramarr);
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
            /*
             =IIF(Fields!CopdiscountType.Value="2",
Parameters!trPercentageDiscount.Value,
Parameters!trValueDiscount.Value)
             * */
        }
        public static void itemTransferDiscountTypeConv(List<ReportParameter> paramarr)
        {

            paramarr.Add(new ReportParameter("trValueDiscount", AppSettings.resourcemanagerreport.GetString("trValueDiscount")));
            paramarr.Add(new ReportParameter("trPercentageDiscount", AppSettings.resourcemanagerreport.GetString("trPercentageDiscount")));




        }

        public static void itemTypeConv(List<ReportParameter> paramarr)
        {
            paramarr.Add(new ReportParameter("trNormal", AppSettings.resourcemanagerreport.GetString("trNormal")));
            paramarr.Add(new ReportParameter("trHaveExpirationDate", AppSettings.resourcemanagerreport.GetString("trHaveExpirationDate")));
            paramarr.Add(new ReportParameter("trHaveSerialNumber", AppSettings.resourcemanagerreport.GetString("trHaveSerialNumber")));
            paramarr.Add(new ReportParameter("trService", AppSettings.resourcemanagerreport.GetString("trService")));
            paramarr.Add(new ReportParameter("trPackage", AppSettings.resourcemanagerreport.GetString("trPackage")));
        }
        //clsReports.SaleInvoiceStsReport(itemTransfers, rep, reppath, paramarr);

        public static void SaleInvoiceStsReport(IEnumerable<ItemTransferInvoice> tempquery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            PurStsReport(tempquery, rep, reppath);
            paramarr.Add(new ReportParameter("isTax", AppSettings.invoiceTax_bool.ToString()));
            itemTransferInvTypeConv(paramarr);

        }

        public static void SaledailyReport(IEnumerable<ItemTransferInvoice> tempquery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            string date = "";
            PurStsReport(tempquery, rep, reppath);
            if (tempquery == null || tempquery.Count() == 0)
            {
                date = "";
            }
            else
            {
                date = HelpClass.DateToString(tempquery.FirstOrDefault().updateDate);
            }
            paramarr.Add(new ReportParameter("isTax", AppSettings.invoiceTax_bool.ToString()));
            paramarr.Add(new ReportParameter("invDate", date));

            paramarr.Add(new ReportParameter("trPaymentMethodsheader", AppSettings.resourcemanagerreport.GetString("trPaymentMethods")));

            paramarr.Add(new ReportParameter("trCash", AppSettings.resourcemanagerreport.GetString("trCash")));
            paramarr.Add(new ReportParameter("trDocument", AppSettings.resourcemanagerreport.GetString("trDocument")));
            paramarr.Add(new ReportParameter("trCheque", AppSettings.resourcemanagerreport.GetString("trCheque")));
            paramarr.Add(new ReportParameter("trCredit", AppSettings.resourcemanagerreport.GetString("trCredit")));
            paramarr.Add(new ReportParameter("trMultiplePayment", AppSettings.resourcemanagerreport.GetString("trMultiplePayment")));

            itemTransferInvTypeConv(paramarr);

        }
        public static void ProfitReport(IEnumerable<ItemUnitInvoiceProfit> tempquery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var r in tempquery)
            {

                r.totalNet = decimal.Parse(HelpClass.DecTostring(r.totalNet));
                r.invoiceProfit = decimal.Parse(HelpClass.DecTostring(r.invoiceProfit));
                r.itemProfit = decimal.Parse(HelpClass.DecTostring(r.itemProfit));
                r.itemunitProfit = decimal.Parse(HelpClass.DecTostring(r.itemunitProfit));
            }
            rep.DataSources.Add(new ReportDataSource("DataSetProfit", tempquery));
            paramarr.Add(new ReportParameter("title", AppSettings.resourcemanagerreport.GetString("trProfits")));
            paramarr.Add(new ReportParameter("Currency", AppSettings.Currency));
            itemTransferInvTypeConv(paramarr);
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trType", AppSettings.resourcemanagerreport.GetString("trType")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trTotal", AppSettings.resourcemanagerreport.GetString("trTotal")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trPOS", AppSettings.resourcemanagerreport.GetString("trPOS")));
            paramarr.Add(new ReportParameter("trProfits", AppSettings.resourcemanagerreport.GetString("trProfits")));
        
        }

        public static void AccTaxReport(IEnumerable<ItemTransferInvoiceTax> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetITinvoice", invoiceItems));
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNo.")));// tt
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));
            paramarr.Add(new ReportParameter("trTotal", AppSettings.resourcemanagerreport.GetString("trTotal")));
            paramarr.Add(new ReportParameter("trTaxValue", AppSettings.resourcemanagerreport.GetString("trTaxValue")));
            paramarr.Add(new ReportParameter("trTaxPercentage", AppSettings.resourcemanagerreport.GetString("trTaxPercentage")));
            paramarr.Add(new ReportParameter("trTotalInvoice", AppSettings.resourcemanagerreport.GetString("trTotalInvoice")));
            paramarr.Add(new ReportParameter("trItemUnit", AppSettings.resourcemanagerreport.GetString("trItemUnit")));
            paramarr.Add(new ReportParameter("trOnItem", AppSettings.resourcemanagerreport.GetString("trOnItem")));
            paramarr.Add(new ReportParameter("trPrice", AppSettings.resourcemanagerreport.GetString("trPrice")));

            paramarr.Add(new ReportParameter("trSum", AppSettings.resourcemanagerreport.GetString("trTotalTax")));
            foreach (var r in invoiceItems)
            {
                r.OneItemPriceNoTax = decimal.Parse(HelpClass.DecTostring(r.OneItemPriceNoTax));
                r.subTotalNotax = decimal.Parse(HelpClass.DecTostring(r.subTotalNotax));//
                r.ItemTaxes = decimal.Parse(HelpClass.PercentageDecTostring(r.ItemTaxes));
                r.itemUnitTaxwithQTY = decimal.Parse(HelpClass.DecTostring(r.itemUnitTaxwithQTY));
                r.subTotalTax = decimal.Parse(HelpClass.DecTostring(r.subTotalTax));

                r.totalNoTax = decimal.Parse(HelpClass.DecTostring(r.totalNoTax));
                r.tax = decimal.Parse(HelpClass.PercentageDecTostring(r.tax));
                r.invTaxVal = decimal.Parse(HelpClass.DecTostring(r.invTaxVal));
                r.totalNet = decimal.Parse(HelpClass.DecTostring(r.totalNet));

            }
            paramarr.Add(new ReportParameter("Currency", AppSettings.Currency));

        }

        public static string ReportTabTitle(string firstTitle, string secondTitle)
        {
            string trtext = "";
            //////////////////////////////////////////////////////////////////////////////
            if (firstTitle == "invoice")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trInvoices");
            else if (firstTitle == "quotation")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trQuotations");
            else if (firstTitle == "promotion")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trThePromotion");
            else if (firstTitle == "internal")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trInternal");
            else if (firstTitle == "external")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trExternal");
            else if (firstTitle == "banksReport")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trBanks");
            else if (firstTitle == "destroied")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trDestructives");
            else if (firstTitle == "usersReport")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trUsers");
            else if (firstTitle == "storageReports")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trStorage");
            else if (firstTitle == "stocktaking")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trStocktaking");
            else if (firstTitle == "stock")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trStock");
            else if (firstTitle == "purchaseOrders")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trPurchaseOrders");
            else if (firstTitle == "saleOrders")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trSalesOrders");

            else if (firstTitle == "saleItems" || firstTitle == "purchaseItem")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trItems");
            else if (firstTitle == "recipientReport")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trReceived");
            else if (firstTitle == "accountStatement")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trAccountStatement");
            else if (firstTitle == "paymentsReport")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trPayments");
            else if (firstTitle == "posReports")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trPOS");
            else if (firstTitle == "dailySalesStatistic")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trDailySales");
            else if (firstTitle == "accountProfits")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trProfits");
            else if (firstTitle == "accountFund")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trCashBalance");
            else if (firstTitle == "quotations")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trQTReport");
            else if (firstTitle == "transfers")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trTransfers");
            else if (firstTitle == "fund")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trCashBalance");
            else if (firstTitle == "DirectEntry")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trDirectEntry");
            else if (firstTitle == "tax")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trTax");
            else if (firstTitle == "closing")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trDailyClosing");
            else if (firstTitle == "orders")
                firstTitle = AppSettings.resourcemanagerreport.GetString("trOrderreport");
            //trCashBalance trDirectEntry
            //trTransfers administrativePull
            //////////////////////////////////////////////////////////////////////////////

            if (secondTitle == "branch")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trBranches");
            else if (secondTitle == "pos")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trPOS");
            else if (secondTitle == "vendors" || secondTitle == "vendor")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trVendors");
            else if (secondTitle == "customers" || secondTitle == "customer")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trCustomers");
            else if (secondTitle == "users" || secondTitle == "user")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trUsers");
            else if (secondTitle == "items" || secondTitle == "item")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trItems");
            else if (secondTitle == "coupon")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trCoupons");
            else if (secondTitle == "offers")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trOffers");
            else if (secondTitle == "invoice")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trInvoices");
            else if (secondTitle == "order")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trOrders");
            else if (secondTitle == "quotation")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trQTReport");
            else if (secondTitle == "operator")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trOperator");
            else if (secondTitle == "operations")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trOperations");//trOperations
            else if (secondTitle == "payments")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trPayments");
            else if (secondTitle == "recipient")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trReceived");
            else if (secondTitle == "destroied")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trDestructives");
            else if (secondTitle == "agent")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trCustomers");
            else if (secondTitle == "stock")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trStock");
            else if (secondTitle == "external")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trExternal");
            else if (secondTitle == "internal")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trInternal");
            else if (secondTitle == "stocktaking")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trStocktaking");
            else if (secondTitle == "archives")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trArchive");
            else if (secondTitle == "shortfalls")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trShortages");
            else if (secondTitle == "location")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trLocation"); 
            else if (secondTitle == "locations")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trLocations"); 
            else if (secondTitle == "collect")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trCollect");
            else if (secondTitle == "shipping")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trShipping");
            else if (secondTitle == "salary")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trSalary");
            else if (secondTitle == "generalExpenses")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trGeneralExpenses");
            else if (secondTitle == "administrativePull")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trAdministrativePull");
            else if (secondTitle == "AdministrativeDeposit")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trAdministrativeDeposit");
            else if (secondTitle == "BestSeller")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trBestSeller");
            else if (secondTitle == "MostPurchased")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trMostPurchased");
            else if (secondTitle == "pull")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trPull");
            else if (secondTitle == "deposit")
                secondTitle = AppSettings.resourcemanagerreport.GetString("trDeposit");
            //////////////////////////////////////////////////////////////////////////////
            if (firstTitle == "" && secondTitle!="") {
                trtext = secondTitle;
            } else if(secondTitle=="" && firstTitle != "")
            {
                trtext = firstTitle;
            }
            else
            {
                trtext = firstTitle + " / " + secondTitle;
            }
           
            return trtext;
        }
        public static void PurInvStsReport(IEnumerable<ItemTransferInvoice> tempquery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            PurStsReport(tempquery, rep, reppath);
            itemTransferInvTypeConv(paramarr);
            paramarr.Add(new ReportParameter("isTax", AppSettings.invoiceTax_bool.ToString()));

            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trType", AppSettings.resourcemanagerreport.GetString("trType")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trPOS", AppSettings.resourcemanagerreport.GetString("trPOS")));
            paramarr.Add(new ReportParameter("trCompany", AppSettings.resourcemanagerreport.GetString("trCompany")));
            paramarr.Add(new ReportParameter("trUser", AppSettings.resourcemanagerreport.GetString("trUser")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));
            paramarr.Add(new ReportParameter("trDiscount", AppSettings.resourcemanagerreport.GetString("trDiscount")));
            paramarr.Add(new ReportParameter("trPrice", AppSettings.resourcemanagerreport.GetString("trPrice")));
            paramarr.Add(new ReportParameter("trTax", AppSettings.resourcemanagerreport.GetString("trTax")));
            paramarr.Add(new ReportParameter("trTotal", AppSettings.resourcemanagerreport.GetString("trTotal")));
            paramarr.Add(new ReportParameter("trVendor", AppSettings.resourcemanagerreport.GetString("trVendor")));
        }


        public static void PurOrderStsReport(IEnumerable<ItemTransferInvoice> tempquery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            PurStsReport(tempquery, rep, reppath);
            itemTransferInvTypeConv(paramarr);

            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
         
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trPOS", AppSettings.resourcemanagerreport.GetString("trPOS")));
            paramarr.Add(new ReportParameter("trCompany", AppSettings.resourcemanagerreport.GetString("trCompany")));
            paramarr.Add(new ReportParameter("trUser", AppSettings.resourcemanagerreport.GetString("trUser")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));
    
            paramarr.Add(new ReportParameter("trPrice", AppSettings.resourcemanagerreport.GetString("trPrice")));
             
            paramarr.Add(new ReportParameter("trVendor", AppSettings.resourcemanagerreport.GetString("trVendor")));

        }


        public static void posReport(IEnumerable<Pos> possQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetPos", possQuery));
        }

        public static void customerReport(IEnumerable<Agent> customersQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("AgentDataSet", customersQuery));
        }

        public static void branchReport(IEnumerable<Branch> branchQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetBranches", branchQuery));
        }

        public static void userReport(IEnumerable<User> usersQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetUser", usersQuery));
        }

        public static void vendorReport(IEnumerable<Agent> vendorsQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("AgentDataSet", vendorsQuery));
        }

        public static void storeReport(IEnumerable<Branch> storesQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetBranches", storesQuery));
        }
        public static void purchaseInvoiceReport(List<ItemTransfer> invoiceItems, LocalReport rep, string reppath)
        {
            foreach (var i in invoiceItems)
            {
                i.price = decimal.Parse(HelpClass.DecTostring(i.price));
            }
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemTransfer", invoiceItems));
            rep.EnableExternalImages = true;

        }
        public static void storage(IEnumerable<Storage> storageQuery, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var r in storageQuery)
            {
                if (r.startDate != null)
                    r.startDate = DateTime.Parse(HelpClass.DateToString(r.startDate));//
                if (r.endDate != null)
                    r.endDate = DateTime.Parse(HelpClass.DateToString(r.endDate));
                //r.inventoryDate = DateTime.Parse(HelpClass.DateToString(r.inventoryDate));
                //r.IupdateDate = DateTime.Parse(HelpClass.DateToString(r.IupdateDate));

                //r.diffPercentage = decimal.Parse(HelpClass.DecTostring(r.diffPercentage));
                r.storageCostValue = decimal.Parse(HelpClass.DecTostring(r.storageCostValue));
            }
            rep.DataSources.Add(new ReportDataSource("DataSetStorage", storageQuery));
        }

        /* free zone
         =iif((Fields!section.Value="FreeZone")And(Fields!location.Value="0-0-0"),
"-",Fields!section.Value+"-"+Fields!location.Value)
         * */
        public static void ClosingStsReport(IEnumerable<POSOpenCloseModel> query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var r in query)
            {
                r.cash = decimal.Parse(HelpClass.DecTostring(r.cash));
                r.openCash = decimal.Parse(HelpClass.DecTostring(r.openCash));

            }
            rep.DataSources.Add(new ReportDataSource("DataSetBalanceSTS", query));
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trPOS", AppSettings.resourcemanagerreport.GetString("trPOS")));
            paramarr.Add(new ReportParameter("trOpenDate", AppSettings.resourcemanagerreport.GetString("trOpenDate")));
            paramarr.Add(new ReportParameter("trOpenCash", AppSettings.resourcemanagerreport.GetString("trOpenCash")));
            paramarr.Add(new ReportParameter("trCloseDate", AppSettings.resourcemanagerreport.GetString("trCloseDate")));
            paramarr.Add(new ReportParameter("trCloseCash", AppSettings.resourcemanagerreport.GetString("trCloseCash")));

            paramarr.Add(new ReportParameter("Currency", AppSettings.Currency));


        }
        public static void ClosingOpStsReport(IEnumerable<OpenClosOperatinModel> query, LocalReport rep, string reppath, List<ReportParameter> paramarr, POSOpenCloseModel openclosrow)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var r in query)
            {
                r.cash = decimal.Parse(HelpClass.DecTostring(r.cash));
                //  r.openCash = decimal.Parse(SectionData.DecTostring(r.openCash));
                r.notes = closingDescriptonConverter(r);


            }
            rep.DataSources.Add(new ReportDataSource("DataSetBalanceSTS", query));
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trPOS", AppSettings.resourcemanagerreport.GetString("trPOS")));
            paramarr.Add(new ReportParameter("trOpenDate", AppSettings.resourcemanagerreport.GetString("trOpenDate")));
            paramarr.Add(new ReportParameter("trOpenCash", AppSettings.resourcemanagerreport.GetString("trOpenCash")));
            paramarr.Add(new ReportParameter("trCloseDate", AppSettings.resourcemanagerreport.GetString("trCloseDate")));
            paramarr.Add(new ReportParameter("trCloseCash", AppSettings.resourcemanagerreport.GetString("trCloseCash")));

            paramarr.Add(new ReportParameter("Currency", AppSettings.Currency));

            paramarr.Add(new ReportParameter("OpenDate", openclosrow.openDate.ToString()));
            paramarr.Add(new ReportParameter("OpenCash", HelpClass.DecTostring(openclosrow.openCash)));
            paramarr.Add(new ReportParameter("CloseDate", openclosrow.updateDate.ToString()));
            paramarr.Add(new ReportParameter("CloseCash", HelpClass.DecTostring(openclosrow.cash)));
            paramarr.Add(new ReportParameter("pos", openclosrow.branchName + " / " + openclosrow.posName));

            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trDescription", AppSettings.resourcemanagerreport.GetString("trDescription")));
            paramarr.Add(new ReportParameter("trCashTooltip", AppSettings.resourcemanagerreport.GetString("trCashTooltip")));



        }
        public static string closingDescriptonConverter(OpenClosOperatinModel s)
        {

            string name = "";
            switch (s.side)
            {
                case "bnd": break;
                case "v": name = AppSettings.resourcemanager.GetString("trVendor"); break;
                case "c": name = AppSettings.resourcemanager.GetString("trCustomer"); break;
                case "u": name = AppSettings.resourcemanager.GetString("trUser"); break;
                case "s": name = AppSettings.resourcemanager.GetString("trSalary"); break;
                case "e": name = AppSettings.resourcemanager.GetString("trGeneralExpenses"); break;
                case "m":
                    if (s.transType == "d")
                        name = AppSettings.resourcemanager.GetString("trAdministrativeDeposit");
                    if (s.transType == "p")
                        name = AppSettings.resourcemanager.GetString("trAdministrativePull");
                    break;
                case "sh": name = AppSettings.resourcemanager.GetString("trShippingCompany"); break;
                default: break;
            }

            if (!string.IsNullOrEmpty(s.agentName))
                name = name + " " + s.agentName;
            else if (!string.IsNullOrEmpty(s.usersName) && !string.IsNullOrEmpty(s.usersLName))
                name = name + " " + s.usersName + " " + s.usersLName;
            else if (!string.IsNullOrEmpty(s.shippingCompanyName))
                name = name + " " + s.shippingCompanyName;
            else if ((s.side != "e") && (s.side != "m"))
                name = name + " " + AppSettings.resourcemanager.GetString("trUnKnown");

            if (s.transType.Equals("p"))
            {
                if ((s.side.Equals("bn")) || (s.side.Equals("p")))
                {
                    return AppSettings.resourcemanager.GetString("trPull") + " " +
                           AppSettings.resourcemanager.GetString("trFrom") + " " +
                           name;//receive
                }
                else if ((!s.side.Equals("bn")) || (!s.side.Equals("p")))
                {
                    return AppSettings.resourcemanager.GetString("trPayment") + " " +
                           AppSettings.resourcemanager.GetString("trTo") + " " +
                           name;//دفع
                }
                else return "";
            }
            else if (s.transType.Equals("d"))
            {
                if ((s.side.Equals("bn")) || (s.side.Equals("p")))
                {
                    return AppSettings.resourcemanager.GetString("trDeposit") + " " +
                           AppSettings.resourcemanager.GetString("trTo") + " " +
                           name;
                }
                else if ((!s.side.Equals("bn")) || (!s.side.Equals("p")))
                {
                    return AppSettings.resourcemanager.GetString("trReceiptOperation") + " " +
                           AppSettings.resourcemanager.GetString("trFrom") + " " +
                           name;//قبض
                }
                else return "";
            }
            else return "";

        }
        public static void storageStock(IEnumerable<Storage> storageQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            storage(storageQuery, rep, reppath);
            DateFormConv(paramarr);
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trSection", AppSettings.resourcemanagerreport.GetString("trSection")));
            paramarr.Add(new ReportParameter("trLocation", AppSettings.resourcemanagerreport.GetString("trLocation")));
            paramarr.Add(new ReportParameter("trItemUnit", AppSettings.resourcemanagerreport.GetString("trItemUnit")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trSectionLocation", AppSettings.resourcemanagerreport.GetString("trSectionLocation")));
            paramarr.Add(new ReportParameter("trStartDate", AppSettings.resourcemanagerreport.GetString("trStartDate")));
            paramarr.Add(new ReportParameter("trEndDate", AppSettings.resourcemanagerreport.GetString("trEndDate")));
            paramarr.Add(new ReportParameter("trMinCollect", AppSettings.resourcemanagerreport.GetString("trMinStock")));
            paramarr.Add(new ReportParameter("trMaxCollect", AppSettings.resourcemanagerreport.GetString("trMaxStock")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));
        }
        // stocktaking 
        

        public static void Stocktakingparam(List<ReportParameter> paramarr)
        {
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trItemUnit", AppSettings.resourcemanagerreport.GetString("trItemUnit")));
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trType", AppSettings.resourcemanagerreport.GetString("trType")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trDiffrencePercentage", AppSettings.resourcemanagerreport.GetString("trDiffrencePercentage")));
            paramarr.Add(new ReportParameter("trItemsCount", AppSettings.resourcemanagerreport.GetString("trItemsCount")));
            paramarr.Add(new ReportParameter("trDestroyedCount", AppSettings.resourcemanagerreport.GetString("trDestroyedCount")));
            paramarr.Add(new ReportParameter("trReason", AppSettings.resourcemanagerreport.GetString("trReason")));
        }

        public static void StocktakingArchivesReport(IEnumerable<InventoryClass> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            foreach (var r in Query)
            {

                //r.inventoryDate = DateTime.Parse(HelpClass.DateToString(r.inventoryDate));
                //r.IupdateDate = DateTime.Parse(HelpClass.DateToString(r.IupdateDate));

                r.diffPercentage = decimal.Parse(HelpClass.DecTostring(r.diffPercentage));
                //r.storageCostValue = decimal.Parse(HelpClass.DecTostring(r.storageCostValue));
            }


            rep.DataSources.Add(new ReportDataSource("DataSetInventoryClass", Query));
            DateFormConv(paramarr);
            InventoryTypeConv(paramarr);
            Stocktakingparam(paramarr);
        }

        public static void StocktakingShortfallsReport(IEnumerable<ItemTransferInvoice> Query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();

            //foreach (var r in Query)
            //{
            //    //if (r.startDate != null)
            //    //    r.startDate = DateTime.Parse(HelpClass.DateToString(r.startDate));//
            //    //if (r.endDate != null)
            //    //    r.endDate = DateTime.Parse(HelpClass.DateToString(r.endDate));

            //    //r.inventoryDate = DateTime.Parse(HelpClass.DateToString(r.inventoryDate));
            //    //r.IupdateDate = DateTime.Parse(HelpClass.DateToString(r.IupdateDate));

            //    //r.diffPercentage = decimal.Parse(HelpClass.DecTostring(r.diffPercentage));
            //    //r.storageCostValue = decimal.Parse(HelpClass.DecTostring(r.storageCostValue));
            //}


            rep.DataSources.Add(new ReportDataSource("DataSetItemTransferInvoice", Query));
            DateFormConv(paramarr);
            InventoryTypeConv(paramarr);
            Stocktakingparam(paramarr);

        }
        /*
        = Switch(Fields!inventoryType.Value="a",Parameters!trArchived.Value
,Fields!inventoryType.Value="n",Parameters!trSaved.Value
,Fields!inventoryType.Value="d",Parameters!trDraft.Value
)

         * */
        //

        public static void cashTransferStsBank(IEnumerable<CashTransferSts> cashTransfers, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            cashTransferSts(cashTransfers, rep, reppath);
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
            paramarr.Add(new ReportParameter("trPull", AppSettings.resourcemanagerreport.GetString("trPull")));
            paramarr.Add(new ReportParameter("trDeposit", AppSettings.resourcemanagerreport.GetString("trDeposit")));
           
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));

            paramarr.Add(new ReportParameter("trType", AppSettings.resourcemanagerreport.GetString("trType")));
            paramarr.Add(new ReportParameter("trAccoutant", AppSettings.resourcemanagerreport.GetString("trAccoutant")));
            paramarr.Add(new ReportParameter("trBank", AppSettings.resourcemanagerreport.GetString("trBank")));
            paramarr.Add(new ReportParameter("trUser", AppSettings.resourcemanagerreport.GetString("trUser")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trAmount", AppSettings.resourcemanagerreport.GetString("trAmount")));

        }

        public static string StsStatementPaymentConvert(string value)
        {
            string s = "";
            switch (value)
            {
                case "cash":
                    s = AppSettings.resourcemanagerreport.GetString("trCash");
                    break;
                case "doc":
                    s = AppSettings.resourcemanagerreport.GetString("trDocument");
                    break;
                case "cheque":
                    s = AppSettings.resourcemanagerreport.GetString("trCheque");
                    break;
                case "balance":
                    s = AppSettings.resourcemanagerreport.GetString("trCredit");
                    break;
                case "card":
                    s = AppSettings.resourcemanagerreport.GetString("trAnotherPaymentMethods");
                    break;
                case "inv":
                    s = AppSettings.resourcemanagerreport.GetString("trInv");
                    break;
                default:
                    s = value;
                    break;


            }
            return s;
        }
        public static void cashTransferStsStatement(IEnumerable<CashTransferSts> cashTransfers, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            cashTransferStatSts(cashTransfers, rep, reppath);

            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo")));
            paramarr.Add(new ReportParameter("trTransferNumberTooltip", AppSettings.resourcemanagerreport.GetString("trTransferNumberTooltip")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trDescription", AppSettings.resourcemanagerreport.GetString("trDescription")));
            paramarr.Add(new ReportParameter("trPayment", AppSettings.resourcemanagerreport.GetString("trPayment")));
            paramarr.Add(new ReportParameter("trCashTooltip", AppSettings.resourcemanagerreport.GetString("trCashTooltip")));




        }
        public static void cashTransferStsPayment(IEnumerable<CashTransferSts> cashTransfers, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            cashTransferSts(cashTransfers, rep, reppath);

            cashTransferProcessTypeConv(paramarr);
            DateFormConv(paramarr);
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trPaymentTypeTooltip", AppSettings.resourcemanagerreport.GetString("trPaymentTypeTooltip")));
            paramarr.Add(new ReportParameter("trAccoutant", AppSettings.resourcemanagerreport.GetString("trAccoutant")));
            paramarr.Add(new ReportParameter("trRecipientTooltip", AppSettings.resourcemanagerreport.GetString("trRecipientTooltip")));
            paramarr.Add(new ReportParameter("trCompany", AppSettings.resourcemanagerreport.GetString("trCompany")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trAmount", AppSettings.resourcemanagerreport.GetString("trAmount")));
            //trDepositor

        }
        public static void cashTransferStsPos(IEnumerable<CashTransferSts> cashTransfers, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            cashTransferSts(cashTransfers, rep, reppath);
            cashTransTypeConv(paramarr);
            DateFormConv(paramarr);

        }

        public static void cashTransferSts(IEnumerable<CashTransferSts> cashTransfers, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var r in cashTransfers)
            {
                r.updateDate = DateTime.Parse(HelpClass.DateToString(r.updateDate));
                r.cash = decimal.Parse(HelpClass.DecTostring(r.cash));
                r.agentName = AgentUnKnownConvert(r.agentId, r.side, r.agentName);
                r.agentCompany = AgentCompanyUnKnownConvert(r.agentId, r.side, r.agentCompany);

            }
            rep.DataSources.Add(new ReportDataSource("DataSetCashTransferSts", cashTransfers));
        }
        public static void cashTransferStatSts(IEnumerable<CashTransferSts> cashTransfers, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (CashTransferSts r in cashTransfers)
            {
                r.updateDate = DateTime.Parse(HelpClass.DateToString(r.updateDate));
                r.cash = decimal.Parse(HelpClass.DecTostring(r.cash));

                r.paymentreport = processTypeAndCardConverter(r.Description3, r.cardName);


            }
            rep.DataSources.Add(new ReportDataSource("DataSetCashTransferSts", cashTransfers));
        }
        public static string processTypeAndCardConverter(string processType, string cardName)
        {
            string pType = processType;
            string cName = cardName;

            switch (pType)
            {
                case "cash": return AppSettings.resourcemanagerreport.GetString("trCash");
                //break;
                case "doc": return AppSettings.resourcemanagerreport.GetString("trDocument");
                //break;
                case "cheque": return AppSettings.resourcemanagerreport.GetString("trCheque");
                //break;
                case "balance": return AppSettings.resourcemanagerreport.GetString("trCredit");
                //break;
                case "card": return cName;
                //break;
                case "inv": return "-";
                case "multiple": return AppSettings.resourcemanagerreport.GetString("trMultiplePayment");

                //break;
                default: return pType;
                    //break;
            }

        }
        public static void FundStsReport(IEnumerable<BalanceSTS> query, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var r in query)
            {

                r.balance = decimal.Parse(HelpClass.DecTostring(r.balance));
            }
            rep.DataSources.Add(new ReportDataSource("DataSetBalanceSTS", query));
            paramarr.Add(new ReportParameter("title", AppSettings.resourcemanagerreport.GetString("trBalance")));
            paramarr.Add(new ReportParameter("Currency", AppSettings.Currency));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trPOS", AppSettings.resourcemanagerreport.GetString("trPOS")));
            paramarr.Add(new ReportParameter("trBalance", AppSettings.resourcemanagerreport.GetString("trBalance")));


        }


        public static void cashTransferStsRecipient(IEnumerable<CashTransferSts> cashTransfers, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            cashTransferSts(cashTransfers, rep, reppath);

            cashTransferProcessTypeConv(paramarr);
            DateFormConv(paramarr);
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trPaymentTypeTooltip", AppSettings.resourcemanagerreport.GetString("trPaymentTypeTooltip")));
            paramarr.Add(new ReportParameter("trAccoutant", AppSettings.resourcemanagerreport.GetString("trAccoutant")));
            paramarr.Add(new ReportParameter("trRecipientTooltip", AppSettings.resourcemanagerreport.GetString("trDepositor")));
            paramarr.Add(new ReportParameter("trCompany", AppSettings.resourcemanagerreport.GetString("trCompany")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trAmount", AppSettings.resourcemanagerreport.GetString("trAmount")));
            

        }
        public static void itemTransferInvoice(IEnumerable<ItemTransferInvoice> itemTransferInvoices, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemTransferInvoice", itemTransferInvoices));

        }
        public static void DateFormConv(List<ReportParameter> paramarr)
        {

            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
        }

        public static void InventoryTypeConv(List<ReportParameter> paramarr)
        {

            paramarr.Add(new ReportParameter("trArchived", AppSettings.resourcemanagerreport.GetString("trArchived")));
            paramarr.Add(new ReportParameter("trSaved", AppSettings.resourcemanagerreport.GetString("trSaved")));
            paramarr.Add(new ReportParameter("trDraft", AppSettings.resourcemanagerreport.GetString("trDraft")));
        }
        public static void cashTransTypeConv(List<ReportParameter> paramarr)
        {

            paramarr.Add(new ReportParameter("trPull", AppSettings.resourcemanagerreport.GetString("trPull")));
            paramarr.Add(new ReportParameter("trDeposit", AppSettings.resourcemanagerreport.GetString("trDeposit")));

        }

        public static void cashTransferProcessTypeConv(List<ReportParameter> paramarr)
        {
            paramarr.Add(new ReportParameter("trCash", AppSettings.resourcemanagerreport.GetString("trCash")));
            paramarr.Add(new ReportParameter("trDocument", AppSettings.resourcemanagerreport.GetString("trDocument")));
            paramarr.Add(new ReportParameter("trCheque", AppSettings.resourcemanagerreport.GetString("trCheque")));
            paramarr.Add(new ReportParameter("trCredit", AppSettings.resourcemanagerreport.GetString("trCredit")));
            paramarr.Add(new ReportParameter("trInv", AppSettings.resourcemanagerreport.GetString("trInv")));
            paramarr.Add(new ReportParameter("trCard", AppSettings.resourcemanagerreport.GetString("trCreditCard")));
        }
        public static void itemTransferInvTypeConv(List<ReportParameter> paramarr)
        {
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
            paramarr.Add(new ReportParameter("trPurchaseInvoice", AppSettings.resourcemanagerreport.GetString("trPurchaseInvoice")));
            paramarr.Add(new ReportParameter("trPurchaseInvoiceWaiting", AppSettings.resourcemanagerreport.GetString("trPurchaseInvoiceWaiting")));
            paramarr.Add(new ReportParameter("trSalesInvoice", AppSettings.resourcemanagerreport.GetString("trSalesInvoice")));
            paramarr.Add(new ReportParameter("trSalesReturnInvoice", AppSettings.resourcemanagerreport.GetString("trSalesReturnInvoice")));
            paramarr.Add(new ReportParameter("trPurchaseReturnInvoice", AppSettings.resourcemanagerreport.GetString("trPurchaseReturnInvoice")));
            paramarr.Add(new ReportParameter("trPurchaseReturnInvoiceWaiting", AppSettings.resourcemanagerreport.GetString("trPurchaseReturnInvoiceWaiting")));
            paramarr.Add(new ReportParameter("trDraftPurchaseBill", AppSettings.resourcemanagerreport.GetString("trDraftPurchaseBill")));
            paramarr.Add(new ReportParameter("trSalesDraft", AppSettings.resourcemanagerreport.GetString("trSalesDraft")));
            paramarr.Add(new ReportParameter("trSalesReturnDraft", AppSettings.resourcemanagerreport.GetString("trSalesReturnDraft")));

            paramarr.Add(new ReportParameter("trSaleOrderDraft", AppSettings.resourcemanagerreport.GetString("trSaleOrderDraft")));
            paramarr.Add(new ReportParameter("trSaleOrder", AppSettings.resourcemanagerreport.GetString("trSaleOrder")));
            paramarr.Add(new ReportParameter("trPurchaceOrderDraft", AppSettings.resourcemanagerreport.GetString("trPurchaceOrderDraft")));
            paramarr.Add(new ReportParameter("trPurchaceOrder", AppSettings.resourcemanagerreport.GetString("trPurchaceOrder")));
            paramarr.Add(new ReportParameter("trQuotationsDraft", AppSettings.resourcemanagerreport.GetString("trQuotationsDraft")));
            paramarr.Add(new ReportParameter("trQuotations", AppSettings.resourcemanagerreport.GetString("trQuotations")));
            paramarr.Add(new ReportParameter("trDestructive", AppSettings.resourcemanagerreport.GetString("trDestructive")));
            paramarr.Add(new ReportParameter("trShortage", AppSettings.resourcemanagerreport.GetString("trShortage")));
            paramarr.Add(new ReportParameter("trImportDraft", AppSettings.resourcemanagerreport.GetString("trImportDraft")));
            paramarr.Add(new ReportParameter("trImport", AppSettings.resourcemanagerreport.GetString("trImport")));
            paramarr.Add(new ReportParameter("trImportOrder", AppSettings.resourcemanagerreport.GetString("trImportOrder")));
            paramarr.Add(new ReportParameter("trExportDraft", AppSettings.resourcemanagerreport.GetString("trExportDraft")));

            paramarr.Add(new ReportParameter("trExport", AppSettings.resourcemanagerreport.GetString("trExport")));

            paramarr.Add(new ReportParameter("trExportOrder", AppSettings.resourcemanagerreport.GetString("trExportOrder")));

        }
        public static void invoiceSideConv(List<ReportParameter> paramarr)
        {


            paramarr.Add(new ReportParameter("trVendor", AppSettings.resourcemanagerreport.GetString("trVendor")));
            paramarr.Add(new ReportParameter("trCustomer", AppSettings.resourcemanagerreport.GetString("trCustomer")));


        }
        public static void AccountSideConv(List<ReportParameter> paramarr)
        {

            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));

            paramarr.Add(new ReportParameter("trVendor", AppSettings.resourcemanagerreport.GetString("trVendor")));
            paramarr.Add(new ReportParameter("trCustomer", AppSettings.resourcemanagerreport.GetString("trCustomer")));
            paramarr.Add(new ReportParameter("trUser", AppSettings.resourcemanagerreport.GetString("trUser")));
            paramarr.Add(new ReportParameter("trSalary", AppSettings.resourcemanagerreport.GetString("trSalary")));
            paramarr.Add(new ReportParameter("trGeneralExpenses", AppSettings.resourcemanagerreport.GetString("trGeneralExpenses")));

            paramarr.Add(new ReportParameter("trAdministrativeDeposit", AppSettings.resourcemanagerreport.GetString("trAdministrativeDeposit")));

            paramarr.Add(new ReportParameter("trAdministrativePull", AppSettings.resourcemanagerreport.GetString("trAdministrativePull")));
            paramarr.Add(new ReportParameter("trShippingCompany", AppSettings.resourcemanagerreport.GetString("trShippingCompany")));


        }
        public static void itemTransferInvoiceExternal(IEnumerable<ItemTransferInvoice> itemTransferInvoices, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {

            itemTransferInvTypeConv(paramarr);
            invoiceSideConv(paramarr);

            itemTransferInvoice(itemTransferInvoices, rep, reppath);
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trType", AppSettings.resourcemanagerreport.GetString("trType")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trAgentType", AppSettings.resourcemanagerreport.GetString("trAgentType")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trItemUnit", AppSettings.resourcemanagerreport.GetString("trItemUnit")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));
 
        }
        public static void itemTransferInvoiceDirect(IEnumerable<ItemTransferInvoice> itemTransferInvoices, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {

            itemTransferInvTypeConv(paramarr);
            invoiceSideConv(paramarr);

            itemTransferInvoice(itemTransferInvoices, rep, reppath);
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
          
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));

        }
        public static void itemTransferInvoiceInternal(IEnumerable<ItemTransferInvoice> itemTransferInvoices, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            itemTransferInvTypeConv(paramarr);
            itemTransferInvoice(itemTransferInvoices, rep, reppath);
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trType", AppSettings.resourcemanagerreport.GetString("trType")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
 
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));

            paramarr.Add(new ReportParameter("trFromBranch", AppSettings.resourcemanagerreport.GetString("trFromBranch")));
            paramarr.Add(new ReportParameter("trToBranch", AppSettings.resourcemanagerreport.GetString("trToBranch")));
            
        }
        public static void itemTransferInvoiceDestroied(IEnumerable<ItemTransferInvoice> itemTransferInvoices, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            itemTransferInvoice(itemTransferInvoices, rep, reppath);
            paramarr.Add(new ReportParameter("dateForm", AppSettings.dateFormat));
            paramarr.Add(new ReportParameter("trBranch", AppSettings.resourcemanagerreport.GetString("trBranch")));
            paramarr.Add(new ReportParameter("trNo", AppSettings.resourcemanagerreport.GetString("trNo.")));
            paramarr.Add(new ReportParameter("trUser", AppSettings.resourcemanagerreport.GetString("trUser")));
            paramarr.Add(new ReportParameter("trDate", AppSettings.resourcemanagerreport.GetString("trDate")));
            paramarr.Add(new ReportParameter("trItemUnit", AppSettings.resourcemanagerreport.GetString("trItemUnit")));
            paramarr.Add(new ReportParameter("trReason", AppSettings.resourcemanagerreport.GetString("trReason")));
            paramarr.Add(new ReportParameter("trQTR", AppSettings.resourcemanagerreport.GetString("trQTR")));


        }
 
        //public static void itemReport(IEnumerable<Item> itemQuery, LocalReport rep, string reppath)
        //{
        //    rep.ReportPath = reppath;
        //    rep.EnableExternalImages = true;
        //    rep.DataSources.Clear();
        //    rep.DataSources.Add(new ReportDataSource("DataSetItem", itemQuery));

        //}
      
        //public static void properyReport(IEnumerable<Property> propertyQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        //{
        //    rep.ReportPath = reppath;
        //    rep.EnableExternalImages = true;
        //    rep.DataSources.Clear();
        //    rep.DataSources.Add(new ReportDataSource("DataSetProperty", propertyQuery));
        //    paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trProperties")));
        //    paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trProperty")));
        //    paramarr.Add(new ReportParameter("trValues", AppSettings.resourcemanagerreport.GetString("trValues")));
        //}

        public static void storageCostReport(IEnumerable<StorageCost> storageCostQuery, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            foreach (var s in storageCostQuery)
            {
                s.cost = decimal.Parse(HelpClass.DecTostring(s.cost));
            }
            rep.DataSources.Add(new ReportDataSource("DataSetStorageCost", storageCostQuery));
            paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trStorageCost")));
            paramarr.Add(new ReportParameter("trName", AppSettings.resourcemanagerreport.GetString("trName")));
            paramarr.Add(new ReportParameter("trCost", AppSettings.resourcemanagerreport.GetString("trStorageCost")));

        }
  
        public static void inventoryReport(IEnumerable<InventoryItemLocation> invItemsLocations, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetInventory", invItemsLocations));
            paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trStocktakingItems")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trSec_Loc", AppSettings.resourcemanagerreport.GetString("trSectionLocation")));//
            //paramarr.Add(new ReportParameter("trItem_UnitName", AppSettings.resourcemanagerreport.GetString("trUnitName")+"-" + AppSettings.resourcemanagerreport.GetString("")));
            paramarr.Add(new ReportParameter("trItem_UnitName", AppSettings.resourcemanagerreport.GetString("trItemUnit")));
            paramarr.Add(new ReportParameter("trRealAmount", AppSettings.resourcemanagerreport.GetString("trRealAmount")));
            paramarr.Add(new ReportParameter("trInventoryAmount", AppSettings.resourcemanagerreport.GetString("trInventoryAmount")));
            paramarr.Add(new ReportParameter("trDestroyCount", AppSettings.resourcemanagerreport.GetString("trDestoryCount")));
        }


        public static void ItemsExportReport(IEnumerable<ItemTransfer> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemsExport", invoiceItems));
            paramarr.Add(new ReportParameter("trTitle", AppSettings.resourcemanagerreport.GetString("trItemsImport/Export")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trQuantity", AppSettings.resourcemanagerreport.GetString("trQuantity")));
        }
        public static void ReceiptPurchaseReport(IEnumerable<ItemTransfer> invoiceItems, LocalReport rep, string reppath, List<ReportParameter> paramarr)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemsExport", invoiceItems));
            paramarr.Add(new ReportParameter("Title", AppSettings.resourcemanagerreport.GetString("trReceiptOfPurchasesBill")));// tt
            paramarr.Add(new ReportParameter("trNum", AppSettings.resourcemanagerreport.GetString("trNum")));
            paramarr.Add(new ReportParameter("trItem", AppSettings.resourcemanagerreport.GetString("trItem")));
            paramarr.Add(new ReportParameter("trUnit", AppSettings.resourcemanagerreport.GetString("trUnit")));
            paramarr.Add(new ReportParameter("trAmount", AppSettings.resourcemanagerreport.GetString("trQuantity")));
        }
        public static void itemLocation(IEnumerable<ItemLocation> itemLocations, LocalReport rep, string reppath)
        {
            rep.ReportPath = reppath;
            rep.EnableExternalImages = true;
            rep.DataSources.Clear();
            rep.DataSources.Add(new ReportDataSource("DataSetItemLocation", itemLocations));
        }



    }
}
