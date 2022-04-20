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
    class decimalToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                TimeSpan span = TimeSpan.FromMinutes(double.Parse(value.ToString()));
                var timeArr = span.ToString().Split(':');

                //string label = (int)span.TotalMinutes + ":" + span.Seconds;
                string label = timeArr[1].ToString().PadLeft(2, '0') + ":" + Math.Round(decimal.Parse(timeArr[2])).ToString().PadLeft(2, '0');
                return label;
            }
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
