using Restaurant;
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
            if (value.ToString().Contains("_basics"))
            {
                value = MainWindow.resourcemanager.GetString("trPermissionsBasics");
            }
            else if (value.ToString().Contains("_create"))
            {
                value = MainWindow.resourcemanager.GetString("trCreate");
            }
            else if (value.ToString().Contains("_save"))
            {
                value = MainWindow.resourcemanager.GetString("trSave");
            }
            else if(value.ToString().Contains("_reports"))
            {
                value = MainWindow.resourcemanager.GetString("trReports");
            }
            else if (value.ToString().Contains("_return"))
            {
                value = MainWindow.resourcemanager.GetString("trReturn");
            }
            else if (value.ToString().Contains("_sendEmail"))
            {
                value = MainWindow.resourcemanager.GetString("trSendEmail");
            }
            else if (value.ToString().Contains("_invoice"))
            {
                value = MainWindow.resourcemanager.GetString("trCreateInvocie");
            }
            else if (value.ToString().Contains("_payments"))
            {
                value = MainWindow.resourcemanager.GetString("trPayments");
            }
            else if (value.ToString().Contains("_view"))
            {
                value = MainWindow.resourcemanager.GetString("trView");
            }
            else if (value.ToString().Contains("_initializeShortage"))
            {
                value = MainWindow.resourcemanager.GetString("trInitializeShortage");
            }
            else if (value.ToString().Contains("_initializeShortage"))
            {
                value = MainWindow.resourcemanager.GetString("trInitializeShortage");
            }
            else if (value.ToString().Contains("_openOrder"))
            {
                value = MainWindow.resourcemanager.GetString("trOrders");
            }
            else if (value.ToString().Contains("_statistic"))
            {
                value = MainWindow.resourcemanager.GetString("trStatistic");
            }
            else if (value.Equals("users_stores") || value.Equals("branches_branches") || value.Equals("stores_branches")
                || value.ToString().Contains("_branches"))
            {
                value = MainWindow.resourcemanager.GetString("trBranchs/Stores");
            }

            else if (value.Equals("general_usersSettings") || value.Equals("reports_usersSettings") )
            {
                value = MainWindow.resourcemanager.GetString("trUsersSettings");
            }

            else if (value.Equals("general_companySettings") || value.Equals("reports_companySettings") )
            {
                value = MainWindow.resourcemanager.GetString("trCompanySettings");
            }
            
            else switch (value)
                {
                    case "locations_addRange":
                        value = MainWindow.resourcemanager.GetString("trAddRange");
                        break;
                    case "section_selectLocation":
                        value = MainWindow.resourcemanager.GetString("trSelectLocations");
                        break;
                    case "reciptOfInvoice_recipt":
                        value = MainWindow.resourcemanager.GetString("trReciptOfInvoice");
                        break;
                    case "itemsStorage_transfer":
                        value = MainWindow.resourcemanager.GetString("trTransfer");
                        break;
                    case "importExport_import":
                        value = MainWindow.resourcemanager.GetString("trImport");
                        break;
                    case "importExport_export":
                        value = MainWindow.resourcemanager.GetString("trExport");
                        break;
                    case "itemsDestroy_destroy":
                        value = MainWindow.resourcemanager.GetString("trDestructive");
                        break;
                    case "inventory_archiving":
                        value = MainWindow.resourcemanager.GetString("trArchive");
                        break;
                    case "reciptInvoice_executeOrder":
                        value = MainWindow.resourcemanager.GetString("trExecuteOrder");
                        break;
                    case "reciptInvoice_quotation":
                        value = MainWindow.resourcemanager.GetString("trQuotations");
                        break;
                    case "offer_items":
                        value = MainWindow.resourcemanager.GetString("trItems");
                        break;
                    case "package_items":
                        value = MainWindow.resourcemanager.GetString("trItems");
                        break;
                     
                    case "medals_customers":
                        value = MainWindow.resourcemanager.GetString("trCustomers");
                        break;
                    case "membership_customers":
                        value = MainWindow.resourcemanager.GetString("trCustomers");
                        break;
                    case "membership_subscriptionFees":
                        value = MainWindow.resourcemanager.GetString("trSubscriptionFees");
                        break;
                    case "salesOrders_delivery":
                        value = MainWindow.resourcemanager.GetString("trDelivery");
                        break;
                    case "posAccounting_transAdmin":
                        value = MainWindow.resourcemanager.GetString("trTransfersAdmin");
                        break;
                    case "Permissions_users":
                        value = MainWindow.resourcemanager.GetString("trUsers");
                        break;
                    case "importExport_package":
                        value = MainWindow.resourcemanager.GetString("trPackage");
                        break;
                    case "importExport_unitConversion":
                        value = MainWindow.resourcemanager.GetString("trUnitConversion");
                        break;
                    case "ordersAccounting_allBranches":
                        value = MainWindow.resourcemanager.GetString("trBranchs/Stores");
                        break;
                    case "storageAlerts_minMaxItem":
                        value = MainWindow.resourcemanager.GetString("trOverrideStorageLimitAlert");
                        break;
                    case "storageAlerts_ImpExp":
                        value = MainWindow.resourcemanager.GetString("trMovements");
                        break;
                    case "storageAlerts_ctreatePurchaseInvoice":
                        value = MainWindow.resourcemanager.GetString("trPurchaseInvoiceWaiting");
                        break;
                    case "storageAlerts_ctreatePurchaseReturnInvoice":
                        value = MainWindow.resourcemanager.GetString("trPurchaseReturnInvoiceWaiting");
                        break;
                    case "saleAlerts_executeOrder":
                        value = MainWindow.resourcemanager.GetString("trWaitingExecuteOrder");
                        break;
                    case "trUnits":
                        value = MainWindow.resourcemanager.GetString("trWaitingExecuteOrder");
                        break;



                    default: break;
                }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
