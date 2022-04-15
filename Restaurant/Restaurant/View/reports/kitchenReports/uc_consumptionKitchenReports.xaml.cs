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
    /// Interaction logic for uc_consumptionKitchenReports.xaml
    /// </summary>
    public partial class uc_consumptionKitchenReports : UserControl
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

        public uc_consumptionKitchenReports()
        {
            InitializeComponent();
        }
    }
}
