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
    public class unitItemsListConverter : IValueConverter
    {
        ItemUnit itemUnit = new ItemUnit();
         
        //public  object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    var result  = Task.Run(() => itemUnit.GetItemUnits(int.Parse(value.ToString()))).Result;
        //    return  result;
        //}
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var result = Task.Run(() => itemUnit.GetItemUnits(int.Parse(value.ToString()))).Result;

            var result = itemUnit.GetIUbyItem(int.Parse(value.ToString()),
                FillCombo.itemUnitList,
               FillCombo.unitsList);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
     
}
