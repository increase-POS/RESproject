using Restaurant;
using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Restaurant.converters
{
    public class permissionsNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "locations_addRange":
                    value = AppSettings.resourcemanager.GetString("trAddRange");
                    break;
                case "section_selectLocation":
                    value = AppSettings.resourcemanager.GetString("trSelectLocations");
                    break;
                case "reciptOfInvoice_recipt":
                    value = AppSettings.resourcemanager.GetString("trReciptOfInvoice");
                    break;
                case "itemsStorage_transfer":
                    value = AppSettings.resourcemanager.GetString("trTransfer");
                    break;
                case "importExport_import":
                    value = AppSettings.resourcemanager.GetString("trImport");
                    break;
                case "importExport_export":
                    value = AppSettings.resourcemanager.GetString("trExport");
                    break;
                case "itemsDestroy_destroy":
                    value = AppSettings.resourcemanager.GetString("trDestructive");
                    break;
                case "inventory_archiving":
                    value = AppSettings.resourcemanager.GetString("trArchive");
                    break;
                case "reciptInvoice_executeOrder":
                    value = AppSettings.resourcemanager.GetString("trExecuteOrder");
                    break;
                case "reciptInvoice_quotation":
                    value = AppSettings.resourcemanager.GetString("trQuotations");
                    break;
                case "offer_items":
                    value = AppSettings.resourcemanager.GetString("trItems");
                    break;
                case "package_items":
                    value = AppSettings.resourcemanager.GetString("trItems");
                    break;

                case "medals_customers":
                    value = AppSettings.resourcemanager.GetString("trCustomers");
                    break;
                case "membership_customers":
                    value = AppSettings.resourcemanager.GetString("trCustomers");
                    break;
                case "membership_subscriptionFees":
                    value = AppSettings.resourcemanager.GetString("trSubscriptionFees");
                    break;
                case "salesOrders_delivery":
                    value = AppSettings.resourcemanager.GetString("trDelivery");
                    break;
                case "posAccounting_transAdmin":
                    value = AppSettings.resourcemanager.GetString("trTransfersAdmin");
                    break;
                case "Permissions_users":
                    value = AppSettings.resourcemanager.GetString("trUsers");
                    break;
                case "importExport_package":
                    value = AppSettings.resourcemanager.GetString("trPackage");
                    break;
                case "importExport_unitConversion":
                    value = AppSettings.resourcemanager.GetString("trUnitConversion");
                    break;
                case "ordersAccounting_allBranches":
                    value = AppSettings.resourcemanager.GetString("trBranchs/Stores");
                    break;
                case "storageAlerts_minMaxItem":
                    value = AppSettings.resourcemanager.GetString("trOverrideStorageLimitAlert");
                    break;
                case "storageAlerts_ImpExp":
                    value = AppSettings.resourcemanager.GetString("trMovements");
                    break;
                case "storageAlerts_ctreatePurchaseInvoice":
                    value = AppSettings.resourcemanager.GetString("trPurchaseInvoiceWaiting");
                    break;
                case "storageAlerts_ctreatePurchaseReturnInvoice":
                    value = AppSettings.resourcemanager.GetString("trPurchaseReturnInvoiceWaiting");
                    break;
                case "saleAlerts_executeOrder":
                    value = AppSettings.resourcemanager.GetString("trWaitingExecuteOrder");
                    break;
                case "reciptOfInvoice_inputs":
                    value = AppSettings.resourcemanager.GetString("trDirectEntry");
                    break;

                default:
                    {
                        if (value.ToString().Contains("_basics"))
                        {
                            value = AppSettings.resourcemanager.GetString("trPermissionsBasics");
                        }
                        else if (value.ToString().Contains("_create"))
                        {
                            value = AppSettings.resourcemanager.GetString("trCreate");
                        }
                        else if (value.ToString().Contains("_save"))
                        {
                            value = AppSettings.resourcemanager.GetString("trSave");
                        }
                        else if (value.ToString().Contains("_reports"))
                        {
                            value = AppSettings.resourcemanager.GetString("trReports");
                        }
                        else if (value.ToString().Contains("_return"))
                        {
                            value = AppSettings.resourcemanager.GetString("trReturn");
                        }
                        else if (value.ToString().Contains("_sendEmail"))
                        {
                            value = AppSettings.resourcemanager.GetString("trSendEmail");
                        }
                        else if (value.ToString().Contains("_invoice"))
                        {
                            value = AppSettings.resourcemanager.GetString("trCreateInvocie");
                        }
                        else if (value.ToString().Contains("_payments"))
                        {
                            value = AppSettings.resourcemanager.GetString("trPayments");
                        }
                        else if (value.ToString().Contains("_view"))
                        {
                            value = AppSettings.resourcemanager.GetString("trView");
                        }
                        else if (value.ToString().Contains("_initializeShortage"))
                        {
                            value = AppSettings.resourcemanager.GetString("trInitializeShortage");
                        }
                        else if (value.ToString().Contains("_initializeShortage"))
                        {
                            value = AppSettings.resourcemanager.GetString("trInitializeShortage");
                        }
                        else if (value.ToString().Contains("_openOrder"))
                        {
                            value = AppSettings.resourcemanager.GetString("trOrders");
                        }
                        else if (value.ToString().Contains("_printCount"))
                        {
                            value = AppSettings.resourcemanager.GetString("trPrintCount");
                        }
                        else if (value.ToString().Contains("_statistic"))
                        {
                            value = AppSettings.resourcemanager.GetString("trStatistic");
                        }
                        else if (value.Equals("users_stores") || value.Equals("branches_branches") || value.Equals("stores_branches")
                            || value.ToString().Contains("_branches"))
                        {
                            value = AppSettings.resourcemanager.GetString("trBranchs/Stores");
                        }
                        else if (value.Equals("general_usersSettings") || value.Equals("reports_usersSettings"))
                        {
                            value = AppSettings.resourcemanager.GetString("trUsersSettings");
                        }
                        else if (value.Equals("general_companySettings") || value.Equals("reports_companySettings"))
                        {
                            value = AppSettings.resourcemanager.GetString("trCompanySettings");
                        }
                        break;
                    }
            }

            

             
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
