using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_diningHallTables.xaml
    /// </summary>
    public partial class wd_diningHallTables : Window
    {
        public wd_diningHallTables()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    //Btn_select_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tablesList = new List<Tables>()
            {
                new Tables{ name = "Table-001", personsCount=2, status="empty"},
                new Tables{ name = "Table-002", personsCount=3, status="open"},
                new Tables{ name = "Table-003", personsCount=4, status="reservated"},
                new Tables{ name = "Table-004", personsCount=5, status="empty"},
                new Tables{ name = "Table-005", personsCount=6, status="empty"},
                new Tables{ name = "Table-006", personsCount=7, status="reservated"},
                new Tables{ name = "Table-007", personsCount=8, status="open"},
                new Tables{ name = "Table-008", personsCount=9, status="open"},
                new Tables{ name = "Table-009", personsCount=10, status="reservated"},
                new Tables{ name = "Table-010", personsCount=11, status="empty"},
                new Tables{ name = "Table-011", personsCount=6, status="empty"},
                new Tables{ name = "Table-012", personsCount=12, status="open"},
                new Tables{ name = "Table-013", personsCount=2, status="reservated"},
                new Tables{ name = "Table-014", personsCount=8, status="empty"},
                new Tables{ name = "Table-015", personsCount=3, status="empty"},
                new Tables{ name = "Table-016", personsCount=9, status="reservated"},
                new Tables{ name = "Table-017", personsCount=5, status="open"},
            };
            BuildBillDesign();
        }
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                //SectionData.ExceptionMessage(ex, this);
            }
        }

        #region
        List<Tables> tablesList = new List<Tables>();
         
        void BuildBillDesign()
        {
            wp_tablesContainer.Children.Clear();




            foreach (var item in tablesList)
            {
                #region button
                Button tableButton = new Button();
                tableButton.Tag = item.name;
                tableButton.Margin = new Thickness(5);
                tableButton.Padding = new Thickness(0);
                tableButton.Background = null;
                tableButton.BorderBrush = null;

                if (item.personsCount <= 2)
                {
                    tableButton.Height = 90;
                    tableButton.Width = 80;
                } 
                else if (item.personsCount == 3)
                {
                    tableButton.Height = 130;
                    tableButton.Width = 90;
                }
                 else if (item.personsCount == 4)
                {
                    tableButton.Height = 140;
                    tableButton.Width = 100;
                }
                 else if (item.personsCount == 5)
                {
                    tableButton.Height = 150;
                    tableButton.Width = 110;
                }
                 else if (item.personsCount == 6)
                {
                    tableButton.Height = 160;
                    tableButton.Width = 120;
                }
                 else if (item.personsCount == 7)
                {
                    tableButton.Height = 170;
                    tableButton.Width = 130;
                }
                 else if (item.personsCount == 8)
                {
                    tableButton.Height = 180;
                    tableButton.Width = 140;
                }
                 else if (item.personsCount == 9)
                {
                    tableButton.Height = 190;
                    tableButton.Width = 150;
                }
                else if (item.personsCount > 9)
                {
                    tableButton.Height = 200;
                    tableButton.Width = 160;
                }
                
                tableButton.Click += tableButton_Click;

                #region Grid Container
                Grid gridContainer = new Grid();
                int rowCount = 3;
                RowDefinition[] rd = new RowDefinition[rowCount];
                for (int i = 0; i < rowCount; i++)
                {
                    rd[i] = new RowDefinition();
                }
                rd[0].Height = new GridLength(1, GridUnitType.Star);
                rd[1].Height = new GridLength(20, GridUnitType.Pixel);
                rd[2].Height = new GridLength(20, GridUnitType.Pixel);
                for (int i = 0; i < rowCount; i++)
                {
                    gridContainer.RowDefinitions.Add(rd[i]);
                }
                /////////////////////////////////////////////////////
                #region Path table
                Path pathTable = new Path();
                pathTable.Stretch = Stretch.Fill;
                pathTable.Margin = new Thickness(5);

                if (item.status == "open")
                    pathTable.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                else if (item.status == "reservated") 
                    pathTable.Fill = Application.Current.Resources["BlueTables"] as SolidColorBrush;
                else
                    pathTable.Fill = Application.Current.Resources["GreenTables"] as SolidColorBrush;

                if (item.personsCount <= 2)
                pathTable.Data = App.Current.Resources["tablePersons2"] as Geometry;
                else if (item.personsCount == 3)
                pathTable.Data = App.Current.Resources["tablePersons3"] as Geometry;
                else if (item.personsCount == 4)
                pathTable.Data = App.Current.Resources["tablePersons4"] as Geometry;
                else if (item.personsCount == 5)
                pathTable.Data = App.Current.Resources["tablePersons5"] as Geometry;
                else if (item.personsCount == 6)
                pathTable.Data = App.Current.Resources["tablePersons6"] as Geometry;
                else if (item.personsCount == 7)
                pathTable.Data = App.Current.Resources["tablePersons7"] as Geometry;
                else if (item.personsCount == 8)
                pathTable.Data = App.Current.Resources["tablePersons8"] as Geometry;
                else if (item.personsCount == 9)
                pathTable.Data = App.Current.Resources["tablePersons9"] as Geometry;
                else if (item.personsCount > 9)
                pathTable.Data = App.Current.Resources["tablePersons9Plus"] as Geometry;

                gridContainer.Children.Add(pathTable);
                #endregion
                #region   personCount 
                if (item.personsCount > 9)
                {
                    var itemPersonCountText = new TextBlock();
                    itemPersonCountText.Text = item.personsCount.ToString();
                    itemPersonCountText.Foreground = Application.Current.Resources["White"] as SolidColorBrush;
                    itemPersonCountText.FontSize = 32;
                    itemPersonCountText.VerticalAlignment = VerticalAlignment.Center;
                    itemPersonCountText.HorizontalAlignment = HorizontalAlignment.Center;
                    gridContainer.Children.Add(itemPersonCountText);
                }
                #endregion
                #region   name
                var itemNameText = new TextBlock();
                itemNameText.Text = item.name;
                itemNameText.VerticalAlignment = VerticalAlignment.Center;
                itemNameText.HorizontalAlignment = HorizontalAlignment.Center;
                itemNameText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                Grid.SetRow(itemNameText, 1);
                gridContainer.Children.Add(itemNameText);
                #endregion
                #region   status
                var itemStatusText = new TextBlock();
                itemStatusText.Text = item.status;
                itemStatusText.VerticalAlignment = VerticalAlignment.Center;
                itemStatusText.HorizontalAlignment = HorizontalAlignment.Center;
                itemStatusText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                Grid.SetRow(itemStatusText, 2);
                gridContainer.Children.Add(itemStatusText);
                #endregion
                tableButton.Content = gridContainer;
                
                #endregion
                wp_tablesContainer.Children.Add(tableButton);
                #endregion
            }
         }
        void tableButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tableName =button.Tag.ToString();
            var table = tablesList.Where(x => x.name == tableName).FirstOrDefault();
            MessageBox.Show("Hey you Click me! I'm  " + table.name + " & person Count is " + table.personsCount);
        }
        
        #endregion
    }
}
