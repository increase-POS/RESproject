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
    class basicConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool s = (bool)value;
            if (s) return AppSettings.resourcemanager.GetString("trBasic");
            else return AppSettings.resourcemanager.GetString("trUnBasic");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
