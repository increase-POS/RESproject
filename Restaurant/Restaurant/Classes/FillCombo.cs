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
        static public Branch branch = new Branch();
        static public List<Branch> branchsList ;
        static public List<Branch> branchesAllWithoutMain;
        static public List<Branch> BranchesByBranchandUser;
        static public async Task<IEnumerable<Branch>> RefreshBranches()
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
        static public async Task<IEnumerable<Branch>> RefreshBranchesAllWithoutMain()
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
        static public async Task<IEnumerable<Branch>> RefreshByBranchandUser()
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
        #region ItemTypePurchase
        static public List<string> purchaseTypes = new List<string>() { "PurchaseNormal", "PurchaseExpire" };
       
        static public void FillItemTypePurchase(ComboBox cmb)
        {
            #region fill process type
            var typelist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trNormal"), Value = "PurchaseNormal" },
                new { Text = MainWindow.resourcemanager.GetString("trExpire") , Value = "PurchaseExpire" },
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
        static public Category category = new Category();
        static public List<Category> categoriesList;
        static public int GetCategoryId(string categoryName)
        {
            return categoriesList.Where(x => x.name == categoryName).FirstOrDefault().categoryId;
        }
        static public async Task<IEnumerable<Category>> RefreshCategory()
        {
            categoriesList = await category.Get();
            return categoriesList;
        }
        static public async void FillCategoryPurchase(ComboBox cmb)
        {
            #region FillCategoryPurchase
            if (categoriesList is null)
                await RefreshCategory();
            cmb.ItemsSource = categoriesList.Where(x=>x.type == "p").ToList();
            cmb.SelectedValuePath = "categoryId";
            //cmb.DisplayMemberPath = "name";
            #endregion
        }
        static public async void FillCategorySale(ComboBox cmb)
        {
            #region FillCategorySale
            if (categoriesList is null)
                await RefreshCategory();
            cmb.ItemsSource = categoriesList.Where(x => x.type == "s").ToList();
            cmb.SelectedValuePath = "categoryId";
            cmb.DisplayMemberPath = "name";
            #endregion
        }
        #endregion
        #region tags
       public static Tag tag = new Tag();
        public static async void fillTags(ComboBox cmb, int categoryId)
        {
            var tags = await tag.Get(categoryId);
            cmb.ItemsSource = tags.ToList();
            cmb.SelectedValuePath = "tagId";
            cmb.DisplayMemberPath = "tagName";
        }
        #endregion
        #region Unit
        static public Unit unit = new Unit();
        static public Unit saleUnit = new Unit();
 
        static public List<Unit> unitsList;
        static public async Task<IEnumerable<Unit>> RefreshUnit()
        {

            unitsList = await unit.GetActive();
            saleUnit = unitsList.Where(x => x.name == "saleUnit").FirstOrDefault();
            unitsList = unitsList.Where(u => u.name != "saleUnit").ToList();
            return unitsList;
        }
        static public async void FillUnits(ComboBox cmb)
        {
            #region Fill Active Units
            if (unitsList is null)
                await RefreshUnit();
            cmb.ItemsSource = unitsList.ToList();
            cmb.SelectedValuePath = "unitId";
            cmb.DisplayMemberPath = "name";
            #endregion
        }
        static public async Task FillSmallUnits(ComboBox cmb, int mainUnitId, int itemId)
        {
            #region Fill small units
           var smallUnits = await unit.getSmallUnits(itemId, mainUnitId);
            cmb.ItemsSource = smallUnits.Where(u => u.name != "saleUnit").ToList();
            cmb.SelectedValuePath = "unitId";
            cmb.DisplayMemberPath = "name";
            #endregion
        }
        #endregion
        #region item units
        static public ItemUnit itemUnit = new ItemUnit();
        static public List<ItemUnit> itemUnitList;
        static public async Task<IEnumerable<ItemUnit>> RefreshItemUnit()
        {
            itemUnitList = await itemUnit.GetIU();
            itemUnitList = itemUnitList.Where(u => u.isActive == 1).ToList();
            return itemUnitList;
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
        #region agent
        static public Agent agent = new Agent();
        #region Vendors
        static public List<Agent> vendorsList;
        static public async Task<IEnumerable<Agent>> RefreshVendors()
        {
            vendorsList = await agent.GetAgentsActive("v");
            return vendorsList;
        }
        static public async Task FillComboVendors(ComboBox cmb)
        {
            if (vendorsList is null)
                await RefreshVendors();
            agent = new Agent();
            agent.agentId = 0;
            agent.name = "-";
            vendorsList.Insert(0, agent);
            cmb.ItemsSource = vendorsList;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "agentId";
            cmb.SelectedIndex = -1;
        }
        #endregion
        #endregion
        #region fill cards combo
        static public Card card = new Card();
        static public List<Card> cardsList;
        static public async Task<IEnumerable<Card>> RefreshCards()
        {
            cardsList = await card.GetAll();
            return cardsList;
        }
        static public async Task FillComboCards(ComboBox cmb)
        {
            if (cardsList is null)
                await RefreshCards();
            cmb.ItemsSource = cardsList;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "agentId";
            cmb.SelectedIndex = -1;
        }
        #endregion

        #region section
        static public Section section = new Section();
        static public List<Section> sectionsByBranchList;
        static public async Task<IEnumerable<Section>> RefreshSectionsByBranch()
        {
            sectionsByBranchList = await section.getBranchSections(MainWindow.branchLogin.branchId);
            return sectionsByBranchList;
        }
        static public async Task FillComboSections(ComboBox cmb)
        {
            if (sectionsByBranchList is null)
                await RefreshSectionsByBranch();
            cmb.ItemsSource = sectionsByBranchList;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "sectionId";
            cmb.SelectedIndex = -1;
        }
        #endregion

        #region section

        static public Location location = new Location();
        static public List<Location> locationsList;
        static public List<Location> locationsBySectionList;
        static public async Task<IEnumerable<Location>> RefreshLocationsBySection(int sectionId)
        {
            locationsBySectionList = await location.getLocsBySectionId(sectionId);
            return locationsBySectionList;
        }
        static public async Task FillComboLocationsBySection(ComboBox cmb , int sectionId)
        {
            await RefreshLocationsBySection(sectionId);
            cmb.ItemsSource = locationsBySectionList;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "locationId";
            cmb.SelectedIndex = -1;
        }
        #endregion
       

        static public Item item = new Item();
        static public ItemLocation itemLocation = new ItemLocation();
        static public Invoice invoice = new Invoice();

    }
}
