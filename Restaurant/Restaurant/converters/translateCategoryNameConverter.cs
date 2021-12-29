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
    class translateCategoryNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
                Category category = value as Category;
                string s = category.name ;

            switch (s)
            {
                case "RawMaterials":
                    s = MainWindow.resourcemanager.GetString("trRawMaterials");
                    break;
                case "Vegetables":
                    s = MainWindow.resourcemanager.GetString("trVegetables");
                    break;
                case "Meat":
                    s = MainWindow.resourcemanager.GetString("trMeat");
                    break;
                case "Drinks":
                    s = MainWindow.resourcemanager.GetString("trDrinks");
                    break;
                default:
                    s = s + "HelloWorld!!";
                    break;
            }

            return s;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch
            {
                return value;
            }
        }


    }

}
