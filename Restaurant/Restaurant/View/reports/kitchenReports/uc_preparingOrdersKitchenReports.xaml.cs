using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Restaurant.View.reports.kitchenReports
{
    /// <summary>
    /// Interaction logic for uc_preparingOrdersKitchenReports.xaml
    /// </summary>
    public partial class uc_preparingOrdersKitchenReports : UserControl
    {
        private static uc_preparingOrdersKitchenReports _instance;
        public static uc_preparingOrdersKitchenReports Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_preparingOrdersKitchenReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_preparingOrdersKitchenReports()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshView_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cb_branches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Chk_allBranches_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Chk_allBranches_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Cb_category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Chk_allCategories_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Chk_allCategories_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
