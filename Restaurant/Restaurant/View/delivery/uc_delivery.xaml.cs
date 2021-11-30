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

namespace Restaurant.View.delivery
{
    /// <summary>
    /// Interaction logic for uc_delivery.xaml
    /// </summary>
    public partial class uc_delivery : UserControl
    {
        private static uc_delivery _instance;
        public static uc_delivery Instance
        {
            get
            {
                _instance = new uc_delivery();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_delivery()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
    }
}
