using Restaurant.ApiClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Restaurant.Classes
{
    public class FillCombo
    {
        /*
        /// <summary>
        /// Packages
        /// </summary>
        static Packages package = new Packages();
        static IEnumerable<Packages> packages;
        static public async Task fillPackage(ComboBox combo)
        {
            packages = await package.GetAll();
            combo.ItemsSource = packages.Where(x => x.isActive == 1);
            combo.SelectedValuePath = "packageId";
            combo.DisplayMemberPath = "packageName";
        }
        */
       
 

        #region Countries
        /// <summary>
        /// area code methods
        /// </summary>
        /// <returns></returns>
        /// 
        //phone 
        public static IEnumerable<Country> countrynum;
        public static IEnumerable<City> citynum;
        public static IEnumerable<City> citynumofcountry;
        public static Country countrycodes = new Country();
        public static City cityCodes = new City();

        static public async Task<IEnumerable<Country>> RefreshCountry()
        {
           countrynum = await countrycodes.GetAll();
            return countrynum;
        }
        static public async Task<IEnumerable<City>> RefreshCity()
        {
            citynum = await cityCodes.Get();
            return citynum;
        }
        static public async Task fillCountries(ComboBox combo)
        {
            if (countrynum is null)
                await RefreshCountry();

            combo.ItemsSource = countrynum.ToList();
            combo.SelectedValuePath = "countryId";
            combo.DisplayMemberPath = "code";
        }
        static public async Task fillCountriesLocal(ComboBox combo , int countryid,Border border)
        {
            if (citynum is null)
                await RefreshCity();
            FillCombo.citynumofcountry = FillCombo.citynum.Where(b => b.countryId == countryid).OrderBy(b => b.cityCode).ToList();
            combo.ItemsSource = FillCombo.citynumofcountry;
            combo.SelectedValuePath = "cityId";
            combo.DisplayMemberPath = "cityCode";
            if (FillCombo.citynumofcountry.Count() > 0)
                border.Visibility = Visibility.Visible;
            else
                border.Visibility = Visibility.Collapsed;
        }
        #endregion
        /// <summary>
        /// fill user type
        /// </summary>
        #region fill user type
            /*
        static public void fillUserType(ComboBox combo)
        {
            var typelist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trAdmin")       , Value = "ad" },
                new { Text = MainWindow.resourcemanager.GetString("trEmployee")   , Value = "u" },
                 };
            combo.DisplayMemberPath = "Text";
            combo.SelectedValuePath = "Value";
            combo.ItemsSource = typelist;

        }
        */
        #endregion


    }
}
