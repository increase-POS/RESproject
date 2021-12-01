using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restaurant.View.sectionData.branchesAndStores
{
    /// <summary>
    /// Interaction logic for uc_branches.xaml
    /// </summary>
    public partial class uc_branches : UserControl
    {
            public uc_branches()
            {
                try
                {
                    InitializeComponent();
                    if (System.Windows.SystemParameters.PrimaryScreenWidth >= 1440)
                    {
                        txt_deleteButton.Visibility = Visibility.Visible;
                        txt_addButton.Visibility = Visibility.Visible;
                        txt_updateButton.Visibility = Visibility.Visible;
                        txt_add_Icon.Visibility = Visibility.Visible;
                        txt_update_Icon.Visibility = Visibility.Visible;
                        txt_delete_Icon.Visibility = Visibility.Visible;
                    }
                    else if (System.Windows.SystemParameters.PrimaryScreenWidth >= 1360)
                    {
                        txt_add_Icon.Visibility = Visibility.Collapsed;
                        txt_update_Icon.Visibility = Visibility.Collapsed;
                        txt_delete_Icon.Visibility = Visibility.Collapsed;
                        txt_deleteButton.Visibility = Visibility.Visible;
                        txt_addButton.Visibility = Visibility.Visible;
                        txt_updateButton.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        txt_deleteButton.Visibility = Visibility.Collapsed;
                        txt_addButton.Visibility = Visibility.Collapsed;
                        txt_updateButton.Visibility = Visibility.Collapsed;
                        txt_add_Icon.Visibility = Visibility.Visible;
                        txt_update_Icon.Visibility = Visibility.Visible;
                        txt_delete_Icon.Visibility = Visibility.Visible;

                    }
                }
                catch (Exception ex)
                {
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private static uc_branches _instance;
            public static uc_branches Instance
            {
                get
                {
                    //if (_instance == null)
                    _instance = new uc_branches();
                    return _instance;
                }
                set
                {
                    _instance = value;
                }
            }

        string basicsPermission = "branches_basics";
        string storesPermission = "branches_branches";
        Branch branch = new Branch();
            IEnumerable<Branch> branchsQuery;
            IEnumerable<Branch> branchs;
            byte tgl_branchState;
            string searchText = "";
            public static List<string> requiredControlList;
            private void UserControl_Unloaded(object sender, RoutedEventArgs e)
            {
                Instance = null;
                GC.Collect();
            }
            private async void UserControl_Loaded(object sender, RoutedEventArgs e)
            {//load
                try
                {
                    HelpClass.StartAwait(grid_main);
                    requiredControlList = new List<string> { "name", "mobile" };
                    if (MainWindow.lang.Equals("en"))
                    {
                        MainWindow.resourcemanager = new ResourceManager("Restaurant.en_file", Assembly.GetExecutingAssembly());
                        grid_main.FlowDirection = FlowDirection.LeftToRight;
                    }
                    else
                    {
                        MainWindow.resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
                        grid_main.FlowDirection = FlowDirection.RightToLeft;
                    }
                    translate();

                await FillCombo.fillComboBranchParent(cb_parentId);
                await FillCombo.fillCountries(cb_areaMobile);
                await FillCombo.fillCountries(cb_areaPhone);
                btn_stores.IsEnabled = false;

                Keyboard.Focus(tb_code);
                     await Search();
                    Clear();
                    HelpClass.EndAwait(grid_main);
                }
                catch (Exception ex)
                {

                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }

            private void translate()
            {
                txt_title.Text = MainWindow.resourcemanager.GetString("trBranch");

            txt_addButton.Text = MainWindow.resourcemanager.GetString("trAdd");
            txt_updateButton.Text = MainWindow.resourcemanager.GetString("trUpdate");
            txt_deleteButton.Text = MainWindow.resourcemanager.GetString("trDelete");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, MainWindow.resourcemanager.GetString("trSearchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_parentId, MainWindow.resourcemanager.GetString("trSelectBranchHint"));
            txt_baseInformation.Text = MainWindow.resourcemanager.GetString("trBaseInformation");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_name, MainWindow.resourcemanager.GetString("trNameHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_code, MainWindow.resourcemanager.GetString("trCodeHint"));
            txt_contentInformatin.Text = MainWindow.resourcemanager.GetString("trMoreInformation");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_address, MainWindow.resourcemanager.GetString("trAdressHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_mobile, MainWindow.resourcemanager.GetString("trMobileHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_phone, MainWindow.resourcemanager.GetString("trPhoneHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_email, MainWindow.resourcemanager.GetString("trEmailHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, MainWindow.resourcemanager.GetString("trNoteHint"));


            dg_branch.Columns[0].Header = MainWindow.resourcemanager.GetString("trCode");
            dg_branch.Columns[1].Header = MainWindow.resourcemanager.GetString("trName");
            dg_branch.Columns[2].Header = MainWindow.resourcemanager.GetString("trAddress");
            dg_branch.Columns[3].Header = MainWindow.resourcemanager.GetString("trNote");

            btn_clear.ToolTip = MainWindow.resourcemanager.GetString("trClear");

            tt_clear.Content = MainWindow.resourcemanager.GetString("trClear");
            tt_report.Content = MainWindow.resourcemanager.GetString("trPdf");
            tt_excel.Content = MainWindow.resourcemanager.GetString("trExcel");
            tt_count.Content = MainWindow.resourcemanager.GetString("trCount");

            tt_add_Button.Content = MainWindow.resourcemanager.GetString("trAdd");
            tt_update_Button.Content = MainWindow.resourcemanager.GetString("trUpdate");
            tt_delete_Button.Content = MainWindow.resourcemanager.GetString("trDelete");
            btn_stores.Content = MainWindow.resourcemanager.GetString("trBranchs/Stores");
        }
            #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
            private async void Btn_add_Click(object sender, RoutedEventArgs e)
            {//add
                try
                {
                    if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "add") || HelpClass.isAdminPermision())
                    {
                        HelpClass.StartAwait(grid_main);



                        branch = new Branch();
                    if (HelpClass.validate(requiredControlList, this) && HelpClass.IsValidEmail(this))
                    {





                        bool iscodeExist = await HelpClass.isCodeExist(tb_code.Text, "b", "Branch", 0);
                        if (iscodeExist)
                        {
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trDuplicateCodeToolTip"), animation: ToasterAnimation.FadeIn);
                            #region Tooltip_code
                            p_error_code.Visibility = Visibility.Visible;
                            ToolTip toolTip_code = new ToolTip();
                            toolTip_code.Content = MainWindow.resourcemanager.GetString("trDuplicateCodeToolTip");
                            toolTip_code.Style = Application.Current.Resources["ToolTipError"] as Style;
                            p_error_code.ToolTip = toolTip_code;
                            #endregion
                        }
                        else
                        {
                            branch.code = tb_code.Text;
                            branch.name = tb_name.Text;
                            branch.notes = tb_notes.Text;
                            branch.address = tb_address.Text;
                            branch.email = tb_email.Text;
                            branch.mobile = cb_areaMobile.Text + "-" + tb_mobile.Text;
                            if (!tb_phone.Text.Equals(""))
                                branch.phone = cb_areaPhone.Text + "-" + cb_areaPhoneLocal.Text + "-" + tb_phone.Text;
                            branch.createUserId = MainWindow.userLogin.userId;
                            branch.updateUserId = MainWindow.userLogin.userId;
                            branch.type = "b";
                            branch.isActive = 1;
                            branch.parentId = Convert.ToInt32(cb_parentId.SelectedValue);

                            int s = await branch.save(branch);
                            if (s <= 0)
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            else
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);



                                Clear();
                                await RefreshCustomersList();
                                await Search();
                            }
                        }
                    }
                        HelpClass.EndAwait(grid_main);
                    }
                    else
                        Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

                }
                catch (Exception ex)
                {

                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private async void Btn_update_Click(object sender, RoutedEventArgs e)
            {//update
            try
            {
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "update") || HelpClass.isAdminPermision())
                {
                    HelpClass.StartAwait(grid_main);
                    if (HelpClass.validate(requiredControlList, this) && HelpClass.IsValidEmail(this))
                    {


                        bool iscodeExist = await HelpClass.isCodeExist(tb_code.Text, "b", "Branch", 0);
                        if (iscodeExist)
                        {
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trDuplicateCodeToolTip"), animation: ToasterAnimation.FadeIn);
                            #region Tooltip_code
                            p_error_code.Visibility = Visibility.Visible;
                            ToolTip toolTip_code = new ToolTip();
                            toolTip_code.Content = MainWindow.resourcemanager.GetString("trDuplicateCodeToolTip");
                            toolTip_code.Style = Application.Current.Resources["ToolTipError"] as Style;
                            p_error_code.ToolTip = toolTip_code;
                            #endregion
                        }
                        else
                        {

                            branch.code = tb_code.Text;
                            branch.name = tb_name.Text;
                            branch.notes = tb_notes.Text;
                            branch.address = tb_address.Text;
                            branch.email = tb_email.Text;
                            branch.mobile = cb_areaMobile.Text + "-" + tb_mobile.Text;
                            if (!tb_phone.Text.Equals(""))
                                branch.phone = cb_areaPhone.Text + "-" + cb_areaPhoneLocal.Text + "-" + tb_phone.Text;
                            branch.updateUserId = MainWindow.userLogin.userId;
                            branch.type = "b";
                            branch.parentId = Convert.ToInt32(cb_parentId.SelectedValue);

                            int s = await branch.save(branch);
                            if (s <= 0)
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            else
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                                await RefreshCustomersList();
                                await Search();


                            }
                        }
                    }
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            }
            private async void Btn_delete_Click(object sender, RoutedEventArgs e)
            {
                try
                {//delete
                    if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "delete") || HelpClass.isAdminPermision())
                    {
                        HelpClass.StartAwait(grid_main);
                        if (branch.branchId != 0)
                        {
                            if ((!branch.canDelete) && (branch.isActive == 0))
                            {
                                #region
                                Window.GetWindow(this).Opacity = 0.2;
                                wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                                w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxActivate");
                                w.ShowDialog();
                                Window.GetWindow(this).Opacity = 1;
                                #endregion
                                if (w.isOk)
                                    await activate();
                            }
                            else
                            {
                                #region
                                Window.GetWindow(this).Opacity = 0.2;
                                wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                                if (branch.canDelete)
                                    w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxDelete");
                                if (!branch.canDelete)
                                    w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxDeactivate");
                                w.ShowDialog();
                                Window.GetWindow(this).Opacity = 1;
                                #endregion
                                if (w.isOk)
                                {
                                    string popupContent = "";
                                    if (branch.canDelete) popupContent = MainWindow.resourcemanager.GetString("trPopDelete");
                                    if ((!branch.canDelete) && (branch.isActive == 1)) popupContent = MainWindow.resourcemanager.GetString("trPopInActive");

                                    int s = await branch.delete(branch.branchId, MainWindow.userLogin.userId, branch.canDelete);
                                    if (s < 0)
                                        Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                                    else
                                    {
                                        Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopDelete"), animation: ToasterAnimation.FadeIn);

                                        await RefreshCustomersList();
                                        await Search();
                                        Clear();
                                    }
                                }
                            }
                        }
                        HelpClass.EndAwait(grid_main);
                    }
                    else
                        Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

                }
                catch (Exception ex)
                {
                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private async Task activate()
            {//activate
                branch.isActive = 1;
                int s = await branch.save(branch);
                if (s <= 0)
                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                else
                {
                    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopActive"), animation: ToasterAnimation.FadeIn);
                    await RefreshCustomersList();
                    await Search();
                }
            }
            #endregion
            #region events
            private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
            {
                try
                {
                    HelpClass.StartAwait(grid_main);
                    await Search();
                    HelpClass.EndAwait(grid_main);
                }
                catch (Exception ex)
                {
                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private async void Tgl_isActive_Checked(object sender, RoutedEventArgs e)
            {
                try
                {
                    HelpClass.StartAwait(grid_main);
                    if (branchs is null)
                        await RefreshCustomersList();
                    tgl_branchState = 1;
                    await Search();
                    HelpClass.EndAwait(grid_main);
                }
                catch (Exception ex)
                {
                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private async void Tgl_isActive_Unchecked(object sender, RoutedEventArgs e)
            {
                try
                {
                    HelpClass.StartAwait(grid_main);
                    if (branchs is null)
                        await RefreshCustomersList();
                    tgl_branchState = 0;
                    await Search();
                    HelpClass.EndAwait(grid_main);
                }
                catch (Exception ex)
                {

                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private void Btn_clear_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    HelpClass.StartAwait(grid_main);
                    Clear();
                    HelpClass.EndAwait(grid_main);
                }
                catch (Exception ex)
                {

                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private  void Dg_branch_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                try
                {
                    HelpClass.StartAwait(grid_main);
                    //selection
                    if (dg_branch.SelectedIndex != -1)
                    {
                        branch = dg_branch.SelectedItem as Branch;
                        this.DataContext = branch;
                        if (branch != null)
                        {
                            #region delete
                            if (branch.canDelete)
                                btn_delete.Content = MainWindow.resourcemanager.GetString("trDelete");
                            else
                            {
                                if (branch.isActive == 0)
                                    btn_delete.Content = MainWindow.resourcemanager.GetString("trActive");
                                else
                                    btn_delete.Content = MainWindow.resourcemanager.GetString("trInActive");
                            }
                            #endregion
                            HelpClass.getMobile(branch.mobile, cb_areaMobile, tb_mobile);
                            HelpClass.getPhone(branch.phone, cb_areaPhone, cb_areaPhoneLocal, tb_phone);
                        }
                    }
                    HelpClass.clearValidate(requiredControlList, this);
                    HelpClass.EndAwait(grid_main);
                }
                catch (Exception ex)
                {
                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
            {
                try
                {//refresh

                    HelpClass.StartAwait(grid_main);
                    await RefreshCustomersList();
                    await Search();
                    HelpClass.EndAwait(grid_main);
                }
                catch (Exception ex)
                {

                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            #endregion
            #region Refresh & Search
            async Task Search()
            {
                //search
                if (branchs is null)
                    await RefreshCustomersList();
                searchText = tb_search.Text.ToLower();
                branchsQuery = branchs.Where(s => (s.code.ToLower().Contains(searchText) ||
                s.name.ToLower().Contains(searchText) ||
                s.mobile.ToLower().Contains(searchText)
                ) && s.isActive == tgl_branchState);
                RefreshCustomersView();
            }
            async Task<IEnumerable<Branch>> RefreshCustomersList()
            {
                branchs = await branch.Get("c");
                return branchs;
            }
            void RefreshCustomersView()
            {
                dg_branch.ItemsSource = branchsQuery;
                txt_count.Text = branchsQuery.Count().ToString();
            }
            #endregion
            #region validate - clearValidate - textChange - lostFocus - . . . . 
            void Clear()
            {
                this.DataContext = new Branch();

                #region mobile-Phone-fax-email
                brd_areaPhoneLocal.Visibility =  Visibility.Collapsed;
                cb_areaMobile.SelectedIndex = -1;
                cb_areaPhone.SelectedIndex = -1;
                cb_areaPhoneLocal.SelectedIndex = -1;
                tb_mobile.Clear();
                tb_phone.Clear();
                tb_email.Clear();
                #endregion

                // last 
                HelpClass.clearValidate(requiredControlList, this);
                p_error_email.Visibility = Visibility.Collapsed;
            }
            private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
            {
                try
                {
                    //only  digits
                    TextBox textBox = sender as TextBox;
                    HelpClass.InputJustNumber(ref textBox);
                    Regex regex = new Regex("[^0-9]+");
                    e.Handled = regex.IsMatch(e.Text);
                }
                catch (Exception ex)
                {
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private void Code_PreviewTextInput(object sender, TextCompositionEventArgs e)
            {
                try
                {
                    //only english and digits
                    Regex regex = new Regex("^[a-zA-Z0-9. -_?]*$");
                    if (!regex.IsMatch(e.Text))
                        e.Handled = true;
                }
                catch (Exception ex)
                {
                    HelpClass.ExceptionMessage(ex, this);
                }

            }
            private void Spaces_PreviewKeyDown(object sender, KeyEventArgs e)
            {
                try
                {
                    e.Handled = e.Key == Key.Space;
                }
                catch (Exception ex)
                {
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private void ValidateEmpty_TextChange(object sender, TextChangedEventArgs e)
            {
                try
                {
                    HelpClass.validate(requiredControlList, this);
                }
                catch (Exception ex)
                {
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
            private void validateEmpty_LostFocus(object sender, RoutedEventArgs e)
            {
                try
                {
                    HelpClass.validate(requiredControlList, this);
                }
                catch (Exception ex)
                {
                    HelpClass.ExceptionMessage(ex, this);
                }
            }

            #endregion
            #region Phone
            int? countryid;
            private async void Cb_areaPhone_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                try
                {
                    HelpClass.StartAwait(grid_main);
                    if (cb_areaPhone.SelectedValue != null)
                    {
                        if (cb_areaPhone.SelectedIndex >= 0)
                        {
                            countryid = int.Parse(cb_areaPhone.SelectedValue.ToString());
                            await FillCombo.fillCountriesLocal(cb_areaPhoneLocal, (int)countryid, brd_areaPhoneLocal);
                        }
                    }
                    HelpClass.EndAwait(grid_main);
                }
                catch (Exception ex)
                {
                    HelpClass.EndAwait(grid_main);
                    HelpClass.ExceptionMessage(ex, this);
                }
            }
           


        #endregion

        private void Btn_stores_Click(object sender, RoutedEventArgs e)
        {
            // stores
            try
            {
                    HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(storesPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    Window.GetWindow(this).Opacity = 0.2;

                    wd_branchesList w = new wd_branchesList();

                    w.Id = branch.branchId;
                    w.userOrBranch = 'b';
                    w.ShowDialog();
                    //if (w.isActive)
                    //{
                    //}

                    Window.GetWindow(this).Opacity = 1;

                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
    }
    }
