using Restaurant.ApiClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System;
namespace Restaurant.Classes
{
    public class FillCombo
    {
        #region  Object
        static public Object objectModel = new Object();
        static public List<Object> objectsList;
        static public async Task<IEnumerable<Object>> RefreshObjects()
        {
            objectsList = await objectModel.GetAll();
            return objectsList;
        }

        #endregion
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
        static public async Task fillBranchesWithoutCurrent(ComboBox cmb, string type = "")
        {
            List<Branch> branches = new List<Branch>();

            if (branchesAllWithoutMain is null)
                await RefreshBranchesAllWithoutMain();
            if (BranchesByBranchandUser is null)
                await RefreshByBranchandUser();

            if (HelpClass.isAdminPermision())
                branches = branchesAllWithoutMain;
            else
                branches = BranchesByBranchandUser;

            branch = branches.Where(s => s.branchId == MainWindow.branchLogin.branchId).FirstOrDefault<Branch>();
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
        static public async Task fillBranchesNoCurrentDefault(ComboBox cmb, string type = "")
        {
            List<Branch> branches = new List<Branch>();

            if (branchesAllWithoutMain is null)
                await RefreshBranchesAllWithoutMain();
            if (BranchesByBranchandUser is null)
                await RefreshByBranchandUser();

            if (HelpClass.isAdminPermision())
                branches = branchesAllWithoutMain;
            else
                branches = BranchesByBranchandUser;

            branch = branches.Where(s => s.branchId == MainWindow.branchLogin.branchId).FirstOrDefault<Branch>();
            branches.Remove(branch);
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
            cmb.SelectedIndex = 0;
            #endregion
        }
        #endregion
        #region job
        static public void FillUserJob(ComboBox cmb)
        {
            #region fill job
            var typelist = new[] {
                //manager
                new { Text = MainWindow.resourcemanager.GetString("trAdmin")       , Value = "admin" },
                new { Text = MainWindow.resourcemanager.GetString("trGeneralManager")       , Value = "generalManager" },
                new { Text = MainWindow.resourcemanager.GetString("trAssistantManager")       , Value = "assistantManager" },
                new { Text = MainWindow.resourcemanager.GetString("trPurchasingManager")       , Value = "purchasingManager" },
                new { Text = MainWindow.resourcemanager.GetString("trSalesManager")       , Value = "salesManager" },
                new { Text = MainWindow.resourcemanager.GetString("trAccountant")       , Value = "accountant" },
                //Kitchen
                new { Text = MainWindow.resourcemanager.GetString("trKitchenManager")       , Value = "kitchenManager" },
                new { Text = MainWindow.resourcemanager.GetString("trExecutiveChef")       , Value = "executiveChef" },
                new { Text = MainWindow.resourcemanager.GetString("trSousChef")       , Value = "sousChef" },
                new { Text = MainWindow.resourcemanager.GetString("trChef")       , Value = "chef" },
                new { Text = MainWindow.resourcemanager.GetString("trDishwasher")       , Value = "dishwasher" },
                new { Text = MainWindow.resourcemanager.GetString("trKitchenEmployee")       , Value = "kitchenEmployee" },
                //hall
                new { Text = MainWindow.resourcemanager.GetString("trHeadWaiter")       , Value = "Headwaiter" },
                new { Text = MainWindow.resourcemanager.GetString("trWaiter")       , Value = "waiter" },
                new { Text = MainWindow.resourcemanager.GetString("trCashier")       , Value = "cashier" },
                new { Text = MainWindow.resourcemanager.GetString("trReceptionist")       , Value = "receptionist" },
                //warehouse manager
                new { Text = MainWindow.resourcemanager.GetString("trWarehouseManager")       , Value = "warehouseManager" },
                new { Text = MainWindow.resourcemanager.GetString("trWarehouseEmployee")       , Value = "warehouseEmployee" },
                //Delivery
                new { Text = MainWindow.resourcemanager.GetString("trDeliveryManager")       , Value = "deliveryManager" },
                new { Text = MainWindow.resourcemanager.GetString("trDeliveryEmployee")       , Value = "deliveryEmployee" },
                //other
                new { Text = MainWindow.resourcemanager.GetString("trCleaningEmployee")       , Value = "cleaningEmployee" },
                new { Text = MainWindow.resourcemanager.GetString("trEmployee")       , Value = "employee" },
                 };
            cmb.DisplayMemberPath = "Text";
            cmb.SelectedValuePath = "Value";
            cmb.ItemsSource = typelist;
            //cmb.SelectedIndex = 0;
            #endregion
        }
        #endregion
        #region ItemTypeSales
        static public List<string> salesTypes = new List<string>() { "SalesNormal", "packageItems" };

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
            return categoriesList.Where(x => x.name.ToLower() == categoryName.ToLower()).FirstOrDefault().categoryId;
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
            cmb.ItemsSource = categoriesList.Where(x=>x.type == "p" && x.isActive == 1).ToList();
            cmb.SelectedValuePath = "categoryId";
            cmb.DisplayMemberPath = "name";
            #endregion
        }
        static public async void FillCategorySale(ComboBox cmb)
        {
            #region FillCategorySale
            if (categoriesList is null)
                await RefreshCategory();
            cmb.ItemsSource = categoriesList.Where(x => x.type == "s").ToList();
            cmb.SelectedValuePath = "categoryId";
            //cmb.DisplayMemberPath = "name";
            #endregion
        }
        #endregion
        #region tags
        public static Tag tag = new Tag();
        public static async Task fillTags(ComboBox cmb, int categoryId)
        {
            List<Tag> tags;
            if (categoryId == -1)
                tags = new List<Tag>();
            else
                tags = await tag.Get(categoryId);
            cmb.ItemsSource = tags;
            cmb.SelectedValuePath = "tagId";
            cmb.DisplayMemberPath = "tagName";
        }
        public static async Task fillTagsWithDefault(ComboBox cmb, int categoryId)
        {
            List<Tag> tags;
            if (categoryId == -1)
                tags = new List<Tag>();
            else
            {
                tags = await tag.Get(categoryId);

                var newTag = new Tag();
                newTag.tagId = 0;
                newTag.tagName = "-";
                tags.Insert(0, newTag);
            }
            cmb.ItemsSource = tags;
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
            List<Unit> smallUnits;
            if (mainUnitId == -1)
                smallUnits = new List<Unit>();
            else             
                smallUnits = await unit.getSmallUnits(itemId, mainUnitId);
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
            agent = new Agent();
            agent.agentId = 0;
            agent.name = "-";
            vendorsList.Insert(0, agent);
            return vendorsList;
        }
        static public async Task FillComboVendors(ComboBox cmb)
        {
            if (vendorsList is null)
                await RefreshVendors();
            cmb.ItemsSource = vendorsList;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "agentId";
            cmb.SelectedIndex = -1;
        }
        #endregion
        #region Customers
        static public List<Agent> customersList;
        static public async Task<IEnumerable<Agent>> RefreshCustomers()
        {
            customersList = await agent.GetAgentsActive("c");
            agent = new Agent();
            agent.agentId = 0;
            agent.name = "-";
            customersList.Insert(0, agent);
            return customersList;
        }
        static public async Task FillComboCustomers(ComboBox cmb)
        {
            if (customersList is null)
                await RefreshCustomers();
            cmb.ItemsSource = customersList;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "agentId";
            cmb.SelectedIndex = -1;
        }
        #endregion
        #endregion
        #region User
        static public User user = new User();
        static public List<User> usersList;

        static public async Task<IEnumerable<User>> RefreshUsers()
        {
            usersList = await user.Get();
            return usersList;
        }

        static public async Task FillComboUsers(ComboBox cmb)
        {
            if (usersList is null)
                await RefreshUsers();
            var users = usersList.Where(x => x.isActive == 1 && x.isAdmin != true).ToList();
            user = new User();
            user.userId = 0;
            user.name = "-";
            users.Insert(0, user);
            cmb.ItemsSource = users;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "userId";
            cmb.SelectedIndex = -1;
        }
        #endregion
        #region ResidentialSectors
        static public ResidentialSectors residentialSec = new ResidentialSectors();
        static public List<ResidentialSectors> residentialSecsList;

        static public async Task<IEnumerable<ResidentialSectors>> RefreshResidentialSectors()
        {
            residentialSecsList = await residentialSec.Get();
            return residentialSecsList;
        }

        static public async Task FillComboResidentialSectors(ComboBox cmb)
        {
            if (residentialSecsList is null)
                await RefreshResidentialSectors();
            cmb.ItemsSource = residentialSecsList;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "residentSecId";
            cmb.SelectedIndex = -1;
        }
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
        #region location

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
            if (sectionId == -1)
                cmb.ItemsSource = new List<Location>();
            else
            {
                await RefreshLocationsBySection(sectionId);
                cmb.ItemsSource = locationsBySectionList;
            }
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "locationId";
            cmb.SelectedIndex = -1;
        }
        #endregion
        #region movements
        static public void FillMovementsProcessType(ComboBox cmb)
        {
            #region fill process type
            var processList = new[] {
                 new { Text = MainWindow.resourcemanager.GetString("trImport"), Value = "im" },
            new { Text = MainWindow.resourcemanager.GetString("trExport"), Value = "ex"},
            };
            cmb.DisplayMemberPath = "Text";
            cmb.SelectedValuePath = "Value";
            cmb.ItemsSource = processList;
            cmb.SelectedIndex = 0;
            #endregion
        }
        #endregion
        #region items
        static public Item item = new Item();
        static public List<Item> purchaseItems;
        static public List<Item> salesItems;
        static public async Task<IEnumerable<Item>> RefreshPurchaseItems()
        {
            purchaseItems = await item.GetPurchaseItems();
            return purchaseItems;
        }
        static public async Task<IEnumerable<Item>> FillComboPurchaseItemsHasQuant(ComboBox cmb)
        {
            var items = await item.GetItemsHasQuant(MainWindow.branchLogin.branchId);

            cmb.ItemsSource = items;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "itemId";
            cmb.SelectedIndex = -1;

            return items;
        }
        static public async Task<IEnumerable<Item>> FillComboPurchaseItems(ComboBox cmb)
        {
            if (purchaseItems == null)
                await RefreshPurchaseItems();

            cmb.ItemsSource = purchaseItems;
            cmb.DisplayMemberPath = "name";
            cmb.SelectedValuePath = "itemId";
            cmb.SelectedIndex = -1;

            return purchaseItems;
        }
        static public async Task<IEnumerable<Item>> RefreshSalesItems()
        {
            salesItems = await item.GetAllSalesItems();
            return salesItems;
        }
        #endregion
        #region reportSetting

        static public PosSetting posSetting = new PosSetting();
        internal static List<Pos> posList = new List<Pos>();
        static SettingCls setModel = new SettingCls();
        static SetValues valueModel = new SetValues();

        public static string sale_copy_count;
        public static string pur_copy_count;
        public static string print_on_save_sale;
        public static string print_on_save_pur;
        public static string email_on_save_sale;
        public static string email_on_save_pur;
        public static string rep_printer_name;
        public static string sale_printer_name;
        public static string salePaperSize;
        public static string rep_print_count;
        public static string docPapersize;
        public static string Allow_print_inv_count;
        public static async Task Getprintparameter()
        {
            List<SetValues> printList = new List<SetValues>();
            printList = await valueModel.GetBySetvalNote("print");
            sale_copy_count = printList.Where(X => X.name == "sale_copy_count").FirstOrDefault().value;

            pur_copy_count = printList.Where(X => X.name == "pur_copy_count").FirstOrDefault().value;

            print_on_save_sale = printList.Where(X => X.name == "print_on_save_sale").FirstOrDefault().value;

            print_on_save_pur = printList.Where(X => X.name == "print_on_save_pur").FirstOrDefault().value;

            email_on_save_sale = printList.Where(X => X.name == "email_on_save_sale").FirstOrDefault().value;

            email_on_save_pur = printList.Where(X => X.name == "email_on_save_pur").FirstOrDefault().value;

            sale_copy_count = printList.Where(X => X.name == "sale_copy_count").FirstOrDefault().value;

            pur_copy_count = printList.Where(X => X.name == "pur_copy_count").FirstOrDefault().value;

            rep_print_count = printList.Where(X => X.name == "rep_copy_count").FirstOrDefault().value;

            Allow_print_inv_count = printList.Where(X => X.name == "Allow_print_inv_count").FirstOrDefault().value;
        }
        public static async Task GetReportlang()
        {
            List<SetValues> replangList = new List<SetValues>();
            replangList = await valueModel.GetBySetName("report_lang");
           MainWindow.Reportlang = replangList.Where(r => r.isDefault == 1).FirstOrDefault().value;

        }
        public static async Task getPrintersNames()
        {

            posSetting = new PosSetting();

            posSetting = await posSetting.GetByposId((int)MainWindow.posLogin.posId);
            posSetting = posSetting.MaindefaultPrinterSetting(posSetting);

            if (posSetting.repname is null || posSetting.repname == "")
            {
                rep_printer_name = "";
            }
            else
            {
                rep_printer_name = Encoding.UTF8.GetString(Convert.FromBase64String(posSetting.repname));
            }
            if (posSetting.salname is null || posSetting.salname == "")
            {
                posSetting.salname = "";
            }
            else
            {
                sale_printer_name = Encoding.UTF8.GetString(Convert.FromBase64String(posSetting.salname));
            }

            salePaperSize = posSetting.saleSizeValue;
            docPapersize = posSetting.docPapersize;

        }
        public static async Task getprintSitting()
        {
            await Getprintparameter();
            await GetReportlang();
            await getPrintersNames();
        }


        #endregion
        #region coupon
        static public Coupon coupon = new Coupon();
        static public void fillDiscountType(ComboBox cmb)
        {
            var dislist = new[] {
            new { Text = MainWindow.resourcemanager.GetString("trValueDiscount"), Value = "1" },
            new { Text = MainWindow.resourcemanager.GetString("trPercentageDiscount"), Value = "2" },
             };
            cmb.DisplayMemberPath = "Text";
            cmb.SelectedValuePath = "Value";
            cmb.ItemsSource = dislist;

        }
        #endregion

        #region Email
        static public void FillSideCombo(ComboBox COMBO)
        {
            #region fill deposit to combo
            var list = new[] {
  new { Text = MainWindow.resourcemanager.GetString("trAccounting")  , Value = "accounting" },
            new { Text = MainWindow.resourcemanager.GetString("trSales")  , Value = "sales" },
            new { Text = MainWindow.resourcemanager.GetString("trPurchases")  , Value = "purchase" },

             };
            COMBO.DisplayMemberPath = "Text";
            COMBO.SelectedValuePath = "Value";
            COMBO.ItemsSource = list;
            #endregion

        }
        #endregion
        static public ItemLocation itemLocation = new ItemLocation();
        static public Invoice invoice = new Invoice();
        static public List<Invoice> invoices;
        static public ShippingCompanies ShipCompany = new ShippingCompanies();


    }
}
