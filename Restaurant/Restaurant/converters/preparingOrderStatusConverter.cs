﻿using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Restaurant.converters
{
    class preparingOrderStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            switch (value)
            {
                case "Listed": return AppSettings.resourcemanager.GetString("trListed");
                case "Preparing": return AppSettings.resourcemanager.GetString("trPreparing");
                case "Ready": return AppSettings.resourcemanager.GetString("trReady");
                case "Collected": return AppSettings.resourcemanager.GetString("trCollected");
                default: return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
