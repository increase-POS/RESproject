﻿using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
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

namespace Restaurant.View.sectionData
{
    /// <summary>
    /// Interaction logic for win_lvc.xaml
    /// </summary>
    public partial class win_lvc : Window
    {

        int selectedChart = 1;
        IEnumerable<Agent> agentsQuery;
        IEnumerable<User> usersQuery;
        IEnumerable<Branch> branchesQuery;
        IEnumerable<Bank> banksQuery;
        IEnumerable<Card> cardsQuery;
        IEnumerable<Pos> posQuery;
        IEnumerable<Classes.Section> sectionsQuery;
        IEnumerable<Tables> tablesQuery;
        IEnumerable<ResidentialSectors> sectoresQuery;

        List<double> chartList;
        List<double> PiechartList;
        List<double> ColumnchartList;

        bool isSameList;
        int data;
        string label;

        public SeriesCollection SeriesCollection { get; set; }

        public win_lvc(IEnumerable<Agent> _agentsQuery, int _data , bool _isSameList)
        {
            try
            {
                InitializeComponent();
                agentsQuery = _agentsQuery;
                isSameList = _isSameList;
                data = _data;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public win_lvc(IEnumerable<User> _usersQuery, int _data)
        {
            try
            {
                InitializeComponent();
                usersQuery = _usersQuery;
                data = _data;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public win_lvc(IEnumerable<Branch> _branchesQuery, int _data , bool _isSameList)
        {
            try
            {
                InitializeComponent();
                branchesQuery = _branchesQuery;
                data = _data;
                isSameList = _isSameList;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public win_lvc(IEnumerable<Pos> _posQuery, int _data)
        {
            try
            {
                InitializeComponent();
                posQuery = _posQuery;
                data = _data;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public win_lvc(IEnumerable<Tables> _tablesQuery, int _data)
        {
            try
            {
                InitializeComponent();
                tablesQuery = _tablesQuery;
                data = _data;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public win_lvc(IEnumerable<Bank> _banksQuery, int _data)
        {
            try
            {
                InitializeComponent();
                banksQuery = _banksQuery;
                data = _data;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public win_lvc(IEnumerable<Card> _cardsQuery, int _data)
        {
            try
            {
                InitializeComponent();
                cardsQuery = _cardsQuery;
                data = _data;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public win_lvc(IEnumerable<ResidentialSectors> _sectorsQuery, int _data)
        {
            try
            {
                InitializeComponent();
                sectoresQuery = _sectorsQuery;
                data = _data;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public win_lvc(IEnumerable<Classes.Section> _sectionsQuery, int _data)
        {
            try
            {
                InitializeComponent();
                sectionsQuery = _sectionsQuery;
                data = _data;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    HelpClass.StartAwait(grid_main);

                #region translate
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
                #endregion

                chartList = new List<double>();
                PiechartList = new List<double>();
                ColumnchartList = new List<double>();
                fillDates();
                fillSelectedChart();

            //    HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}

        }

        private void translate()
        {
            txt_title.Text = MainWindow.resourcemanager.GetString("trReports");
            rdoMonth.Content = MainWindow.resourcemanager.GetString("trMonth");
            rdoYear.Content = MainWindow.resourcemanager.GetString("trYear");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dpStrtDate, MainWindow.resourcemanager.GetString("trStartDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dpEndDate, MainWindow.resourcemanager.GetString("trEndDateHint"));
        }

        public void fillDates()
        {
            dpEndDate.SelectedDate = DateTime.Now;
            dpStrtDate.SelectedDate = dpEndDate.SelectedDate.Value.AddYears(-1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception) { }
        }

        public void fillChart()
        {
            chartList.Clear();
            MyAxis.Labels = new List<string>();
            int startYear = dpStrtDate.SelectedDate.Value.Year;
            int endYear = dpEndDate.SelectedDate.Value.Year;
            int startMonth = dpStrtDate.SelectedDate.Value.Month;
            int endMonth = dpEndDate.SelectedDate.Value.Month;
            if (rdoMonth.IsChecked == true)
            {
                for (int year = startYear; year <= endYear; year++)
                {
                    for (int month = startMonth; month <= 12; month++)
                    {
                        DateTime firstOfThisMonth = DateTime.Now;
                        try
                        {
                            firstOfThisMonth = new DateTime(year, month, dpStrtDate.SelectedDate.Value.Day);
                        }
                        catch
                        {
                            try
                            {
                                firstOfThisMonth = new DateTime(year, month, 29);
                            }
                            catch
                            {
                                firstOfThisMonth = new DateTime(year, month, 28);
                            }
                        }
                        var firstOfNextMonth = firstOfThisMonth.AddMonths(1);

                        if (data == 1)
                        {
                            var Draw = agentsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            if (isSameList)
                                label = MainWindow.resourcemanager.GetString("trCustomers");
                            else 
                                label = MainWindow.resourcemanager.GetString("trVendors");
                        }
                        else if (data == 2)
                        {
                            var Draw = branchesQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            if (isSameList)
                                label = MainWindow.resourcemanager.GetString("trBranches");
                            else
                                label = MainWindow.resourcemanager.GetString("trStores");
                        }
                        else if (data == 3)
                        {
                            var Draw = usersQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trUsers");
                        }
                        else if (data == 4)
                        {
                            var Draw = posQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trPOSs");
                        }
                        else if (data == 5)
                        {
                            var Draw = banksQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trBanks");
                        }
                        else if (data == 6)
                        {
                            var Draw = tablesQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trTables");
                        }
                        else if (data == 7)
                        {
                            var Draw = sectionsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trSections");
                        }
                        else if (data == 8)
                        {
                            var Draw = cardsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trCards");
                        }
                        else if (data == 9)
                        {
                            var Draw = sectoresQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            chartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trResidentialSectors");
                        }
                        MyAxis.Separator.Step = 2;
                        MyAxis.Labels.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "/" + year);
                        if (year == dpEndDate.SelectedDate.Value.Year && month == dpEndDate.SelectedDate.Value.Month)
                        {
                            break;
                        }
                        if (month == 12)
                        {
                            startMonth = 1;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int year = startYear; year <= endYear; year++)
                {
                    var firstOfThisYear = new DateTime(year, 1, dpStrtDate.SelectedDate.Value.Month);
                    var firstOfNextMYear = firstOfThisYear.AddYears(1);
                    if (data == 1)
                    {
                        var Draw = agentsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        if (isSameList)
                            label = MainWindow.resourcemanager.GetString("trCustomers");
                        else
                            label = MainWindow.resourcemanager.GetString("trVendors");
                    }
                    else if (data == 2)
                    {
                        var Draw = branchesQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        if (isSameList)
                            label = MainWindow.resourcemanager.GetString("trBranches");
                        else
                            label = MainWindow.resourcemanager.GetString("trStores");
                    }
                    else if (data == 3)
                    {
                        var Draw = usersQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trUsers");
                    }
                    else if (data == 4)
                    {
                        var Draw = posQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trPOSs");
                    }
                    else if (data == 5)
                    {
                        var Draw = banksQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trBanks");
                    }
                    else if (data == 6)
                    {
                        var Draw = tablesQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trTables");
                    }
                    else if (data == 7)
                    {
                        var Draw = sectionsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trSections");
                    }
                    else if (data == 8)
                    {
                        var Draw = cardsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trCards");
                    }
                    else if (data == 9)
                    {
                        var Draw = sectoresQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        chartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trResidentialSectors");
                    }
                    MyAxis.Separator.Step = 1;
                    MyAxis.Labels.Add(year.ToString());
                }
            }
            SeriesCollection = new SeriesCollection
           {
                 new LineSeries
               {
                        Title = label,
                   Values = chartList.AsChartValues()
               },
           };
            grid1.Children.Clear();
            grid1.Children.Add(charts);
            DataContext = this;

        }

        public void fillPieChart()
        {

            PiechartList.Clear();
            SeriesCollection piechartData = new SeriesCollection();
            List<string> titles = new List<string>();
            int startYear = dpStrtDate.SelectedDate.Value.Year;
            int endYear = dpEndDate.SelectedDate.Value.Year;
            int startMonth = dpStrtDate.SelectedDate.Value.Month;
            int endMonth = dpEndDate.SelectedDate.Value.Month;
            if (rdoMonth.IsChecked == true)
            {
                for (int year = startYear; year <= endYear; year++)
                {
                    for (int month = startMonth; month <= 12; month++)
                    {
                        DateTime firstOfThisMonth = DateTime.Now;
                        try
                        {
                            firstOfThisMonth = new DateTime(year, month, dpStrtDate.SelectedDate.Value.Day);
                        }
                        catch
                        {
                            try
                            {
                                firstOfThisMonth = new DateTime(year, month, 29);
                            }
                            catch
                            {
                                firstOfThisMonth = new DateTime(year, month, 28);
                            }
                        }
                        var firstOfNextMonth = firstOfThisMonth.AddMonths(1);
                        if (data == 1)
                        {
                            var Draw = agentsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            if (isSameList)
                                label = MainWindow.resourcemanager.GetString("trCustomers");
                            else
                                label = MainWindow.resourcemanager.GetString("trVendors");
                        }
                        else if (data == 2)
                        {
                            var Draw = branchesQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            if (isSameList)
                                label = MainWindow.resourcemanager.GetString("trBranches");
                            else
                                label = MainWindow.resourcemanager.GetString("trStores");
                        }
                        else if (data == 3)
                        {
                            var Draw = usersQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trUsers");
                        }
                        else if (data == 4)
                        {
                            var Draw = posQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trPOSs");
                        }
                        else if (data == 5)
                        {
                            var Draw = banksQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trBanks");
                        }
                        else if (data == 6)
                        {
                            var Draw = tablesQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trTables");
                        }
                        else if (data == 7)
                        {
                            var Draw = sectionsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trSections");
                        }
                        else if (data == 8)
                        {
                            var Draw = cardsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trCards");
                        }
                        else if (data == 9)
                        {
                            var Draw = sectoresQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            PiechartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trResidentialSectors");
                        }
                        titles.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "/" + year);
                        if (year == dpEndDate.SelectedDate.Value.Year && month == dpEndDate.SelectedDate.Value.Month)
                        {
                            break;
                        }
                        if (month == 12)
                        {
                            startMonth = 1;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int year = startYear; year <= endYear; year++)
                {
                    var firstOfThisYear = new DateTime(year, 1, dpStrtDate.SelectedDate.Value.Month);
                    var firstOfNextMYear = firstOfThisYear.AddYears(1);
                    if (data == 1)
                    {
                        var Draw = agentsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        if (isSameList)
                            label = MainWindow.resourcemanager.GetString("trCustomers");
                        else
                            label = MainWindow.resourcemanager.GetString("trVendors");
                    }
                    else if (data == 2)
                    {
                        var Draw = branchesQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        if (isSameList)
                            label = MainWindow.resourcemanager.GetString("trBranches");
                        else
                            label = MainWindow.resourcemanager.GetString("trStores");
                    }
                    else if (data == 3)
                    {
                        var Draw = usersQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trUsers");
                    }
                    else if (data == 4)
                    {
                        var Draw = posQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trPOSs");
                    }
                    else if (data == 5)
                    {
                        var Draw = banksQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trBanks");
                    }
                    else if (data == 6)
                    {
                        var Draw = tablesQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trTables");
                    }
                    else if (data == 7)
                    {
                        var Draw = sectionsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trSections");
                    }
                    else if (data == 8)
                    {
                        var Draw = cardsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trCards");
                    }
                    else if (data == 9)
                    {
                        var Draw = sectoresQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        PiechartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trResidentialSectors");
                    }
                    titles.Add(year.ToString());
                }
            }
            for (int i = 0; i < PiechartList.Count(); i++)
            {
                List<double> final = new List<double>();
                List<string> lable = new List<string>();
                final.Add(PiechartList.ToList().Skip(i).FirstOrDefault());
                piechartData.Add(
                  new PieSeries
                  {
                      Values = final.AsChartValues(),
                      Title = titles.ToList().Skip(i).FirstOrDefault().ToString(),
                      DataLabels = true,
                  }
              );
            }
            pieChart.Series = piechartData;
        }

        public void fillColumnChart()
        {
            columnAxis.Labels = new List<string>();
            ColumnchartList.Clear();
            SeriesCollection columnchartData = new SeriesCollection();
            List<string> titles = new List<string>();
            int startYear = dpStrtDate.SelectedDate.Value.Year;
            int endYear = dpEndDate.SelectedDate.Value.Year;
            int startMonth = dpStrtDate.SelectedDate.Value.Month;
            int endMonth = dpEndDate.SelectedDate.Value.Month;
            if (rdoMonth.IsChecked == true)
            {
                for (int year = startYear; year <= endYear; year++)
                {
                    for (int month = startMonth; month <= 12; month++)
                    {
                        DateTime firstOfThisMonth = DateTime.Now;
                        try
                        {
                            firstOfThisMonth = new DateTime(year, month, dpStrtDate.SelectedDate.Value.Day);
                        }
                        catch
                        {
                            try
                            {
                                firstOfThisMonth = new DateTime(year, month, 29);
                            }
                            catch
                            {
                                firstOfThisMonth = new DateTime(year, month, 28);
                            }
                        }
                        var firstOfNextMonth = firstOfThisMonth.AddMonths(1);
                        if (data == 1)
                        {
                            var Draw = agentsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            if (isSameList)
                                label = MainWindow.resourcemanager.GetString("trCustomers");
                            else
                                label = MainWindow.resourcemanager.GetString("trVendors");
                        }
                        else if (data == 2)
                        {
                            var Draw = branchesQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            if (isSameList)
                                label = MainWindow.resourcemanager.GetString("trBranches");
                            else
                                label = MainWindow.resourcemanager.GetString("trStores");
                        }
                        else if (data == 3)
                        {
                            var Draw = usersQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trUsers");
                        }
                        else if (data == 4)
                        {
                            var Draw = posQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trPOSs");
                        }
                        else if (data == 5)
                        {
                            var Draw = banksQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trBanks");
                        }
                        else if (data == 6)
                        {
                            var Draw = tablesQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trTables");
                        }
                        else if (data == 7)
                        {
                            var Draw = sectionsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trSections");
                        }
                        else if (data == 8)
                        {
                            var Draw = cardsQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trCards");
                        }
                        else if (data == 9)
                        {
                            var Draw = sectoresQuery.ToList().Where(c => c.createDate > firstOfThisMonth && c.createDate <= firstOfNextMonth).Count();
                            ColumnchartList.Add(Draw);
                            label = MainWindow.resourcemanager.GetString("trResidentialSectors");
                        }
                        columnAxis.Separator.Step = 2;
                        columnAxis.Labels.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "/" + year);
                        if (year == dpEndDate.SelectedDate.Value.Year && month == dpEndDate.SelectedDate.Value.Month)
                        {
                            break;
                        }
                        if (month == 12)
                        {
                            startMonth = 1;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int year = startYear; year <= endYear; year++)
                {
                    var firstOfThisYear = new DateTime(year, 1, dpStrtDate.SelectedDate.Value.Month);
                    var firstOfNextMYear = firstOfThisYear.AddYears(1);
                    if (data == 1)
                    {
                        var Draw = agentsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        if (isSameList)
                            label = MainWindow.resourcemanager.GetString("trCustomers");
                        else
                            label = MainWindow.resourcemanager.GetString("trVendors");
                    }
                    else if (data == 2)
                    {
                        var Draw = branchesQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        if (isSameList)
                            label = MainWindow.resourcemanager.GetString("trBranches");
                        else
                            label = MainWindow.resourcemanager.GetString("trStores");
                    }
                    else if (data == 3)
                    {
                        var Draw = usersQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trUsers");
                    }
                    else if (data == 4)
                    {
                        var Draw = posQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trPOSs");
                    }
                    else if (data == 5)
                    {
                        var Draw = banksQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trBanks");
                    }
                    else if (data == 6)
                    {
                        var Draw = tablesQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trTables");
                    }
                    else if (data == 7)
                    {
                        var Draw = sectionsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trSections");
                    }
                    else if (data == 8)
                    {
                        var Draw = cardsQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trCards");
                    }
                    else if (data == 9)
                    {
                        var Draw = sectoresQuery.ToList().Where(c => c.createDate > firstOfThisYear && c.createDate <= firstOfNextMYear).Count();
                        ColumnchartList.Add(Draw);
                        label = MainWindow.resourcemanager.GetString("trResidentialSectors");
                    }
                    columnAxis.Separator.Step = 1;
                    columnAxis.Labels.Add(year.ToString());
                }
            }
            columnchartData.Add(
                 new ColumnSeries
                 {
                     Title = label,
                     Values = ColumnchartList.AsChartValues(),
                 }
             );
            columnChart.Series = columnchartData;
        }

        private void dpStrtDate_CalendarClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (dpEndDate.SelectedDate.Value.Year - dpStrtDate.SelectedDate.Value.Year > 1)
                {
                    rdoYear.IsChecked = true;
                    fillSelectedChart();
                }
                else
                {
                    fillSelectedChart();
                }
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void dpEndDate_CalendarClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (dpEndDate.SelectedDate.Value.Year - dpStrtDate.SelectedDate.Value.Year > 1)
                {
                    rdoYear.IsChecked = true;
                    fillSelectedChart();
                }
                else
                {
                    fillSelectedChart();
                }
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                rdoMonth.IsChecked = true;
                fillDates();
                selectedChart = 1;
                fillSelectedChart();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void rdoYear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (dpEndDate.SelectedDate.Value.Year - dpStrtDate.SelectedDate.Value.Year > 1)
                {
                    rdoYear.IsChecked = true;
                    fillSelectedChart();
                }
                else
                {
                    fillSelectedChart();
                }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void rdoMonth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (dpEndDate.SelectedDate.Value.Year - dpStrtDate.SelectedDate.Value.Year > 1)
                {
                    rdoYear.IsChecked = true;
                    fillSelectedChart();
                }
                else
                {
                    fillSelectedChart();
                }
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void fillSelectedChart()
        {
            grid1.Visibility = Visibility.Hidden;
            grd_pieChart.Visibility = Visibility.Hidden;
            grd_columnChart.Visibility = Visibility.Hidden;

            icon_rowChar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E8E8E8"));
            icon_columnChar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E8E8E8"));
            icon_pieChar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E8E8E8"));

            if (selectedChart == 1)
            {
                grid1.Visibility = Visibility.Visible;
                icon_rowChar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#178DD2"));
                fillChart();
            }
            else if (selectedChart == 2)
            {
                grd_pieChart.Visibility = Visibility.Visible;
                icon_pieChar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#178DD2"));
                fillPieChart();
            }
            else if (selectedChart == 3)
            {
                grd_columnChart.Visibility = Visibility.Visible;
                icon_columnChar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#178DD2"));
                fillColumnChart();
            }
        }

        private void btn_rowChart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                selectedChart = 1;
                fillSelectedChart();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void btn_pieChart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                selectedChart = 2;
                fillSelectedChart();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void btn_columnChart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                selectedChart = 3;
                fillSelectedChart();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }
    }
}