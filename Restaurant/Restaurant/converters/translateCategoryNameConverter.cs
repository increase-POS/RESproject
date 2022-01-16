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
            if (value != null && value != "")
            {

                Category category = value as Category;
                string s = category.name;

                switch (s)
                {
                    //sales
                    case "appetizers":
                        s = MainWindow.resourcemanager.GetString("trAppetizers");
                        break;
                    case "beverages":
                        s = MainWindow.resourcemanager.GetString("trBeverages");
                        break;
                    case "fastFood":
                        s = MainWindow.resourcemanager.GetString("trFastFood");
                        break;
                    case "mainCourses":
                        s = MainWindow.resourcemanager.GetString("trMainCourses");
                        break;
                    case "desserts":
                        s = MainWindow.resourcemanager.GetString("trDesserts");
                        break;
                    //purchase
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
                }
                return s;
            }
            else return value;
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
