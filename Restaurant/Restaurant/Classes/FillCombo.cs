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
        #region branch
        static Branch branch = new Branch();
        static List<Branch> branchsList ;
        static List<Branch> branchesAllWithoutMain;
        static List<Branch> BranchesByBranchandUser;
        static async Task<IEnumerable<Branch>> RefreshBranches()
        {
            branchsList = await branch.GetAll();
            return branchsList;
        }
        static public async Task fillComboBranchParent(ComboBox cmb)
        {
            if (branchsList is null)
                await RefreshBranches();
            cmb.ItemsSource = branchsList.Where(b => b.type == "b" || b.type == "bs");
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "branchId";
            cmb.SelectedIndex = -1;
        }
        static async Task<IEnumerable<Branch>> RefreshBranchesAllWithoutMain()
        {
            branchesAllWithoutMain = await  branch.GetAllWithoutMain("all");
            return branchesAllWithoutMain;
        }
        static public async Task fillComboBranchesAllWithoutMain(ComboBox cmb)
        {
            if (branchesAllWithoutMain is null)
                await RefreshBranchesAllWithoutMain();
            cmb.ItemsSource = branchesAllWithoutMain;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "branchId";
            cmb.SelectedIndex = -1;

        }
        static async Task<IEnumerable<Branch>> RefreshByBranchandUser()
        {
            BranchesByBranchandUser = await branch.BranchesByBranchandUser(MainWindow.branchLogin.branchId, MainWindow.userLogin.userId);
            return BranchesByBranchandUser;
        }
        static public async Task fillBranchesWithoutCurrent(ComboBox cmb, int currentBranchId, string type = "")
        {
            List<Branch> branches = new List<Branch>();

            if (branchesAllWithoutMain is null)
                await RefreshBranchesAllWithoutMain();
            if (BranchesByBranchandUser is null)
                await RefreshByBranchandUser();

            if (HelpClass.isAdminPermision())
                branches =   branchesAllWithoutMain;
            else
                branches = BranchesByBranchandUser;

            branch = branches.Where(s => s.branchId == currentBranchId).FirstOrDefault<Branch>();
            branches.Remove(branch);
            var br = new Branch();
            br.branchId = 0;
            br.name = "-";
            branches.Insert(0, br);
            cmb.ItemsSource = branches.Where(b => b.type != type && b.branchId != 1);
            cmb.SelectedValuePath = "branchId";
            cmb.DisplayMemberPath = "name";
            cmb.SelectedIndex = -1;
        }
        #endregion
        #region PayType
        static public void FillDefaultPayType(ComboBox cmb)
        {
            #region fill process type
            var typelist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trCash")       , Value = "cash" },
                new { Text = MainWindow.resourcemanager.GetString("trCredit") , Value = "balance" },
                new { Text = MainWindow.resourcemanager.GetString("trAnotherPaymentMethods") , Value = "card" },
                new { Text = MainWindow.resourcemanager.GetString("trMultiplePayment") , Value = "multiple" }, 
                //new { Text = MainWindow.resourcemanager.GetString("trDocument")   , Value = "doc" },
                //new { Text = MainWindow.resourcemanager.GetString("trCheque")     , Value = "cheque" },
                 };
            cmb.DisplayMemberPath = "Text";
            cmb.SelectedValuePath = "Value";
            cmb.ItemsSource = typelist;
            #endregion
        }
        #endregion
        #region DiscountType
        static public void FillDiscountType(ComboBox cmb)
        {
            #region fill process type
            var dislist = new[] {
            new { Text = "", Value = -1 },
            new { Text = MainWindow.resourcemanager.GetString("trValueDiscount"), Value = 1 },
            new { Text = MainWindow.resourcemanager.GetString("trPercentageDiscount"), Value = 2 },
             };

            cmb.DisplayMemberPath = "Text";
            cmb.SelectedValuePath = "Value";
            cmb.ItemsSource = dislist;
            cmb.SelectedIndex = 0;
            #endregion
        }
        private void configure()
        {
            
        }
        #endregion
        #region Category
        //static public void FillCategoryString(ComboBox cmb)
        //{
        //    #region fill process type
        //    var typelist = new[] {
        //        new { Text = MainWindow.resourcemanager.GetString("trRawMaterials"), Value = "RawMaterials" },
        //        new { Text = MainWindow.resourcemanager.GetString("trVegetables") , Value = "Vegetables" },
        //        new { Text = MainWindow.resourcemanager.GetString("trMeat") , Value = "Meat" },
        //        new { Text = MainWindow.resourcemanager.GetString("trDrinks") , Value = "Drinks" }, 
        //         };
        //    cmb.DisplayMemberPath = "Text";
        //    cmb.SelectedValuePath = "Value";
        //    cmb.ItemsSource = typelist;
        //    #endregion
        //}
        static async Task<IEnumerable<Category>> RefreshCategory()
        {
            MainWindow.mainWindow.globalCategories = await MainWindow.mainWindow.globalCategory.Get();
            return MainWindow.mainWindow.globalCategories;
        }
        static public async void FillCategoryPurchase(ComboBox cmb)
        {
            #region FillCategoryPurchase
            if (MainWindow.mainWindow.globalCategories.Count<1)
                await RefreshCategory();
            cmb.ItemsSource = MainWindow.mainWindow.globalCategories.Where(x=>x.type == "p").ToList();
            cmb.SelectedValuePath = "categoryId";
            cmb.DisplayMemberPath = "name";
            #endregion
        }
        static public async void FillCategorySale(ComboBox cmb)
        {
            #region FillCategoryPurchase
            if (MainWindow.mainWindow.globalCategories.Count < 1)
                await RefreshCategory();
            cmb.ItemsSource = MainWindow.mainWindow.globalCategories.Where(x => x.type == "s").ToList();
            cmb.SelectedValuePath = "categoryId";
            cmb.DisplayMemberPath = "name";
            #endregion
        }
        #endregion
        #region FillDeliveryType
        static public void FillDeliveryType(ComboBox cmb)
        {
            var typelist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trLocaly")     , Value = "local" },
                new { Text = MainWindow.resourcemanager.GetString("trShippingCompany")   , Value = "com" },
                 };
            cmb.DisplayMemberPath = "Text";
            cmb.SelectedValuePath = "Value";
            cmb.ItemsSource = typelist;
        }
        #endregion
        #region Countries
        /// <summary>
        /// area code methods
        /// </summary>
        /// <returns></returns>
        /// 
        //phone 
        public static IEnumerable<CountryCode> countrynum;
        public static IEnumerable<City> citynum;
        public static IEnumerable<City> citynumofcountry;
        public static CountryCode countrycodes = new CountryCode();
        public static City cityCodes = new City();

        static async Task<IEnumerable<CountryCode>> RefreshCountry()
        {
            countrynum = await countrycodes.GetAllCountries();
            return countrynum;
        }
        static public async Task<IEnumerable<City>> RefreshCity()
        {
            citynum = await cityCodes.Get();
            return citynum;
        }
        static public async Task fillCountries(ComboBox cmb)
        {
            if (countrynum is null)
                await RefreshCountry();

            cmb.ItemsSource = countrynum.ToList();
            cmb.SelectedValuePath = "countryId";
            cmb.DisplayMemberPath = "code";
        }
        static public async Task fillCountriesLocal(ComboBox cmb , int countryid,Border border)
        {
            if (citynum is null)
                await RefreshCity();
            FillCombo.citynumofcountry = FillCombo.citynum.Where(b => b.countryId == countryid).OrderBy(b => b.cityCode).ToList();
            cmb.ItemsSource = FillCombo.citynumofcountry;
            cmb.SelectedValuePath = "cityId";
            cmb.DisplayMemberPath = "cityCode";
            if (FillCombo.citynumofcountry.Count() > 0)
                border.Visibility = Visibility.Visible;
            else
                border.Visibility = Visibility.Collapsed;
        }
        #endregion
        #region fill user type
        /*
    static public void fillUserType(ComboBox cmb)
    {
        var typelist = new[] {
            new { Text = MainWindow.resourcemanager.GetString("trAdmin")       , Value = "ad" },
            new { Text = MainWindow.resourcemanager.GetString("trEmployee")   , Value = "u" },
             };
        cmb.DisplayMemberPath = "Text";
        cmb.SelectedValuePath = "Value";
        cmb.ItemsSource = typelist;

    }
    */
        #endregion
        #region Vendors
        static Agent agent = new Agent();
        static List<Agent> agentsList;
        static public async Task<IEnumerable<Agent>> RefreshVendors()
        {
            agentsList = await agent.GetAgentsActive("v");
            return agentsList;
        }
        static public async Task FillComboVendors(ComboBox cmb)
        {
            if (agentsList is null)
                await RefreshVendors();
            agent = new Agent();
            agent.agentId = 0;
            agent.name = "-";
            agentsList.Insert(0, agent);
            cmb.ItemsSource = agentsList;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "agentId";
            cmb.SelectedIndex = -1;
        }
        #endregion
    }
}
