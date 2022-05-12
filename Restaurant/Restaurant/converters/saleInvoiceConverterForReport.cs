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
    class saleInvoiceConverterForReport : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                //مبيعات
                case "s":
                    value = AppSettings.resourcemanager.GetString("trDiningHallType");
                    break;
                // طلب خارجي
                case "ts":
                    value = AppSettings.resourcemanager.GetString("trTakeAway");
                    break;
                // خدمة ذاتية
                case "ss":
                    value = AppSettings.resourcemanager.GetString("trSelfService");
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
