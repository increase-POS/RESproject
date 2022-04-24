using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Restaurant.converters
{
    class OrderPreparing_remainingTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {

           
            decimal remainingTime = 0;
            OrderPreparing orderPreparing = (OrderPreparing) value ;
            remainingTime =  OrderPreparing.calculateRemainingTime(orderPreparing.preparingStatusDate.Value,
                orderPreparing.preparingTime.Value, orderPreparing.status);


                return HelpClass.decimalToTime(remainingTime);

            }
            catch
            {
                return "";

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
