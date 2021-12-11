using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Restaurant.converters
{
    class deliveryTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            switch (value)
            {
                case "local": return MainWindow.resourcemanager.GetString("trLocaly");
                    //break;
                case "com":   return MainWindow.resourcemanager.GetString("trShippingCompany");
                    //break;
                default: return MainWindow.resourcemanager.GetString("");
                    //break;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
