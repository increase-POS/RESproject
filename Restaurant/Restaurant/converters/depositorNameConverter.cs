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
    class depositorNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CashTransfer s = value as CashTransfer;
            string name = "";
            switch (s.side)
            {
                case "bnd":break;
                case "v": name = MainWindow.resourcemanager.GetString("trVendor"); break;
                case "c": name = MainWindow.resourcemanager.GetString("trCustomer"); break;
                case "u": name = MainWindow.resourcemanager.GetString("trUser"); break;
                case "s": name = MainWindow.resourcemanager.GetString("trSalary"); break;
                case "e": name = MainWindow.resourcemanager.GetString("trGeneralExpenses"); break;
                case "m":
                    if(s.transType=="d")
                        name = MainWindow.resourcemanager.GetString("trAdministrativeDeposit");
                    if(s.transType == "p")
                        name = MainWindow.resourcemanager.GetString("trAdministrativePull");
                    break;
                case "sh": name = MainWindow.resourcemanager.GetString("trShippingCompany"); break;
                default: break;
            }

            if (!string.IsNullOrEmpty(s.agentName))
                name = name + " " + s.agentName;
            else if (!string.IsNullOrEmpty(s.usersName) && !string.IsNullOrEmpty(s.usersLName))
                name = name + " " + s.usersName + " " + s.usersLName;
            else if (!string.IsNullOrEmpty(s.shippingCompanyName))
                name = name + " " + s.shippingCompanyName;
            else if ((s.side != "e") && (s.side != "m"))
                name = name + " " + MainWindow.resourcemanager.GetString("trUnKnown");

            return name;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    

}
