using Restaurant.Classes;
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

namespace Restaurant.View.reports.storageReports
{
    /// <summary>
    /// Interaction logic for uc_storageReports.xaml
    /// </summary>
    public partial class uc_storageReports : UserControl
    {
        private static uc_storageReports _instance;
        public static uc_storageReports Instance
        {
            get
            {
                _instance = new uc_storageReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_storageReports()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region translate
                if (MainWindow.lang.Equals("en"))
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                else
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                await translate();
                #endregion
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async Task translate()
        {
            if (FillCombo.objectsList is null)
                await FillCombo.RefreshObjects();
            // Title
            if (!string.IsNullOrWhiteSpace(FillCombo.objectsList.Where(x => x.name == this.Tag.ToString()).FirstOrDefault().translate))
                txt_mainTitle.Text = MainWindow.resourcemanager.GetString(
               FillCombo.objectsList.Where(x => x.name == this.Tag.ToString()).FirstOrDefault().translate
               );
            // Info
            List<TextBlock> InfoTextBlocksList = FindControls.FindVisualChildren<TextBlock>(this)
                .Where(x => x.Name.Contains("Info") && x.Tag != null).ToList();
            foreach (var item in InfoTextBlocksList)
            {
                if (!string.IsNullOrWhiteSpace(FillCombo.objectsList.Where(x => x.name == item.Tag.ToString()).FirstOrDefault().translate))
                    item.Text = MainWindow.resourcemanager.GetString(
                   FillCombo.objectsList.Where(x => x.name == item.Tag.ToString()).FirstOrDefault().translate
                   );
            }
            // Hint
            List<TextBlock> HintTextBlocksList = FindControls.FindVisualChildren<TextBlock>(this)
                .Where(x => x.Name.Contains("Hint") && x.Tag != null).ToList();
            foreach (var item in HintTextBlocksList)
            {
                if (!string.IsNullOrWhiteSpace(FillCombo.objectsList.Where(x => x.name == item.Tag.ToString()).FirstOrDefault().translateHint))
                    item.Text = MainWindow.resourcemanager.GetString(
                   FillCombo.objectsList.Where(x => x.name == item.Tag.ToString()).FirstOrDefault().translateHint
                   );
            }
        }

        private void Btn_stockStorageReports_Click(object sender, RoutedEventArgs e)
        {
            //    try
            //    {
            //        grid_main.Children.Clear();
            //        grid_main.Children.Add(uc_storageReports.Instance);
            //        Button button = sender as Button;
            //        MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        HelpClass.ExceptionMessage(ex, this);
            //    }
        }

        private void Btn_externalStorageReports_Click(object sender, RoutedEventArgs e)
        {
            //    try
            //    {
            //        grid_main.Children.Clear();
            //        grid_main.Children.Add(uc_storageReports.Instance);
            //        Button button = sender as Button;
            //        MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        HelpClass.ExceptionMessage(ex, this);
            //    }
        }

        private void Btn_internalStorageReports_Click(object sender, RoutedEventArgs e)
        {
            //    try
            //    {
            //        grid_main.Children.Clear();
            //        grid_main.Children.Add(uc_storageReports.Instance);
            //        Button button = sender as Button;
            //        MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        HelpClass.ExceptionMessage(ex, this);
            //    }
        }

        private void Btn_directStorageReports_Click(object sender, RoutedEventArgs e)
        {
            //    try
            //    {
            //        grid_main.Children.Clear();
            //        grid_main.Children.Add(uc_storageReports.Instance);
            //        Button button = sender as Button;
            //        MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        HelpClass.ExceptionMessage(ex, this);
            //    }
        }

        private void Btn_stocktakingStorageReports_Click(object sender, RoutedEventArgs e)
        {
            //    try
            //    {
            //        grid_main.Children.Clear();
            //        grid_main.Children.Add(uc_storageReports.Instance);
            //        Button button = sender as Button;
            //        MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        HelpClass.ExceptionMessage(ex, this);
            //    }
        }

        private void Btn_destroiedStorageReports_Click(object sender, RoutedEventArgs e)
        {
            //    try
            //    {
            //        grid_main.Children.Clear();
            //        grid_main.Children.Add(uc_storageReports.Instance);
            //        Button button = sender as Button;
            //        MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        HelpClass.ExceptionMessage(ex, this);
            //    }
        }


    }
}
