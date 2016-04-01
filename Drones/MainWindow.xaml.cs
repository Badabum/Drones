using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using Drones.Common;
using Drones.Models;
using GHQualification.Models;
using Telerik.Windows.Controls.Charting;
using Line = System.Windows.Shapes.Line;

namespace Drones
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int gridCellSize = 3;
        private DataModel _dataModel;
        private readonly OrdersProcessor _ordersProcessor;
        private readonly ObservableCollection<DataPoint> _simpleSeries;
        private readonly ObservableCollection<DataPoint> _improvedSeries;
        private readonly ObservableCollection<DataPoint> _bestSeries;
        const string busyDay = "busy_day";
        const string redundancy = "redundancy";
        const string mamka = "mother_of_all_warehouses";
        public MainWindow()
        {

            InitializeComponent();
            
            var path = $@"C:\Users\Ihor\Documents\visual studio 2015\Projects\Drones\Drones\{busyDay}.in";
            var reader = new FileOperations(path);
            _dataModel = reader.ReadFileAsync();
            DrawGrid(_dataModel.GeneralInfo.Columns,_dataModel.GeneralInfo.Rows,gridCellSize);
            DrawObjects(_dataModel.Warehouses, Brushes.Green,3, gridCellSize);
            DrawObjects(_dataModel.Orders, Brushes.Yellow,1,gridCellSize);
            _ordersProcessor =  new OrdersProcessor(_dataModel);
             _simpleSeries = new ObservableCollection<DataPoint>();
            _improvedSeries = new ObservableCollection<DataPoint>();
            _bestSeries = new ObservableCollection<DataPoint>();
            InitializeCharts();
            
            SubscribeToEvents(_ordersProcessor);
            //_ordersProcessor.onDronesChanged += (drones) =>
            //{
            //    Dispatcher.Invoke((Action)delegate { RedrawDrones(drones); });
            //};

            //_ordersProcessor.onOrdersChanged += (orders) =>
            //{
            //    Dispatcher.Invoke((Action)delegate { RefreshOrders(orders); });
            //};
            //_ordersProcessor.onOrdersCountChanged += (ordersCount, iteration) =>
            //{
            //    Dispatcher.Invoke((Action)delegate { AddPointToChart(ordersCount, iteration); });
            //};

        }

        private void InitializeCharts()
        {
            
            SeriesMapping mapping = new SeriesMapping();
            mapping.ItemMappings.Add(new ItemMapping(nameof(DataPoint.XValue), DataPointMember.XValue));
            mapping.ItemMappings.Add(new ItemMapping(nameof(DataPoint.YValue), DataPointMember.YValue));
            ImprovedChart.DefaultSeriesDefinition = new SplineSeriesDefinition();
            ImprovedChart.ItemsSource = _improvedSeries;
            ImprovedChart.SeriesMappings.Add(mapping);



            SeriesMapping simpleMapping = new SeriesMapping();
            simpleMapping.ItemMappings.Add(new ItemMapping(nameof(DataPoint.XValue), DataPointMember.XValue));
            simpleMapping.ItemMappings.Add(new ItemMapping(nameof(DataPoint.YValue), DataPointMember.YValue));
            simpleAlgoChart.DefaultSeriesDefinition = new SplineSeriesDefinition();
            simpleAlgoChart.ItemsSource = _simpleSeries;
            simpleAlgoChart.SeriesMappings.Add(simpleMapping);

            SeriesMapping bestMapping = new SeriesMapping();
            bestMapping.ItemMappings.Add(new ItemMapping(nameof(DataPoint.XValue), DataPointMember.XValue));
            bestMapping.ItemMappings.Add(new ItemMapping(nameof(DataPoint.YValue), DataPointMember.YValue));
            bestChart.DefaultSeriesDefinition = new SplineSeriesDefinition();
            bestChart.ItemsSource = _bestSeries;
            bestChart.SeriesMappings.Add(bestMapping);
        }
        private void DrawGrid(int columns, int rows, int cellWidth = 10)
        {
            var stroke = System.Windows.Media.Brushes.Blue;
            var strokeThiknes = 1;
            mainCanvas.Width = columns*cellWidth;
            mainCanvas.Height = rows*cellWidth;
            //draw columns
            for (var i = 0; i <=columns; i++)
            {
                var line = new Line
                {
                    X1 = cellWidth*i,
                    Y1 = 0
                };
                line.X2 = line.X1;
                line.Y2 = cellWidth*rows;
                line.Stroke = stroke;
                line.StrokeThickness = strokeThiknes;
                mainCanvas.Children.Add(line);
            }
            for (var i = 0; i <=rows; i++)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = cellWidth*i,
                    Stroke = stroke,
                    StrokeThickness = strokeThiknes,
                    X2 = cellWidth*columns
                };
                line.Y2 = line.Y1;
                mainCanvas.Children.Add(line);
            }
        }

        private void DrawObjects(IEnumerable<IDrawableObject> drawableObjects , SolidColorBrush color, int shapeSize = 3, int gridCellSize = 10)
        {
            foreach (var drawable in drawableObjects)
            {
                var rectangle = new Rectangle()
                {
                    Width = shapeSize*gridCellSize,
                    Height = shapeSize*gridCellSize,
                    Fill = color,
                    Name = drawable.Name, 
                    
                };
                var label = new Label()
                {
                    Content = drawable.Name,
                    Name = drawable.Name
                };
                mainCanvas.Children.Add(label);
                Canvas.SetTop(label, drawable.R * gridCellSize);
                Canvas.SetLeft(label, drawable.C * gridCellSize);
                mainCanvas.Children.Add(rectangle);
                Canvas.SetTop(rectangle,drawable.R*gridCellSize);
                Canvas.SetLeft(rectangle, drawable.C*gridCellSize);
            }
        }

        public void RedrawDrones(List<Drone> drones)
        {
            ClearDrones(drones);
            DrawObjects(drones,Brushes.Red,2,gridCellSize);
        }

        private void ClearDrones(List<Drone> drones)
        {
            foreach (var drone in drones)
            {
                var element1 = (UIElement)LogicalTreeHelper.FindLogicalNode(mainCanvas, drone.Name);
                mainCanvas.Children.Remove(element1);
                var element2 = (UIElement)LogicalTreeHelper.FindLogicalNode(mainCanvas, drone.Name);
                mainCanvas.Children.Remove(element2);
            }
        }

        private void RefreshOrders(List<Order> orders)
        {
            foreach (var order in orders)
            {
                var uiElement = (UIElement) LogicalTreeHelper.FindLogicalNode(mainCanvas, order.Name);
                mainCanvas.Children.Remove(uiElement);
            }
        }

        private void AddPointToChart(ObservableCollection<DataPoint> series,int ordersCount,int iteration)
        {
            var dataPoint = new DataPoint(iteration, ordersCount);
            series.Add(dataPoint);
            
        }
        private async void Button_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(() => { _ordersProcessor.InmprovedAlgorithm(_dataModel.GeneralInfo.Steps, 10000); });
        }

        private void SubscribeToEvents(OrdersProcessor processor)
        {
            processor.onDronesChanged += (drones) =>
            {
                Dispatcher.Invoke((Action)delegate { RedrawDrones(drones); });
            };
            processor.onOrdersCountChanged += (ordersCount, iteration) =>
            {
                Dispatcher.Invoke((Action)delegate { AddPointToChart(_simpleSeries,ordersCount, iteration); });
            };
            processor.onSimulationFinished += (time) =>
            {
                Dispatcher.Invoke((Action)delegate { ShowMessageBox(time); });
            };
            processor.onOrdersCountChangedImproved += (ordersCount, iteration) =>
            {
                Dispatcher.Invoke((Action)delegate { AddPointToChart(_improvedSeries,ordersCount, iteration); });
            };
            processor.onOrdersCountChangedBest+= (ordersCount, iteration) =>
            {
                Dispatcher.Invoke((Action)delegate { AddPointToChart(_bestSeries, ordersCount, iteration); });
            };
        }

        private void ShowMessageBox(long time)
        {
            var result = MessageBox.Show($"Simulation finished in {time} miliseconds", "Finish", MessageBoxButton.YesNo, MessageBoxImage.Information);
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(() => { _ordersProcessor.Process(_dataModel.GeneralInfo.Steps, 10000); });
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(() => { _ordersProcessor.InmprovedAlgorithm(_dataModel.GeneralInfo.Steps,10000); });
        }

        private void ClearSimpleChart(object sender, RoutedEventArgs e)
        {
            _simpleSeries.Clear();
        }

        private void ClearImprovedChart(object sender, RoutedEventArgs e)
        {
            _improvedSeries.Clear();
        }

        private async void RunBestSimulation(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(() => { _ordersProcessor.InmprovedAlgorithmBestOrderFinding(100000, 10000); });
        }

        private void ShowAboutDialog(object sender, RoutedEventArgs e)
        {
            AboutDialog dialog = new AboutDialog();
            if (dialog.ShowDialog() == true)
            {
                
            }
        }

        private void ReloadData(string fileName)
        {
            var path = $@"C:\Users\Ihor\Documents\visual studio 2015\Projects\Drones\Drones\{fileName}.in";
            var reader = new FileOperations(path);
            _dataModel = reader.ReadFileAsync();
            _ordersProcessor.SetDataModel(_dataModel);
            _simpleSeries.Clear();
            _improvedSeries.Clear();
            _bestSeries.Clear();
        }

        private void LoadBusyDay(object sender, RoutedEventArgs e)
        {
            ReloadData(busyDay);
            ShowLoadedMessage();
        }

        private void LoadRedundancy(object sender, RoutedEventArgs e)
        {
            ReloadData(redundancy);
            ShowLoadedMessage();
        }

        private void LoadMother(object sender, RoutedEventArgs e)
        {
            ReloadData(mamka);
            ShowLoadedMessage();
        }

        private void ShowLoadedMessage()
        {
            var result = MessageBox.Show($"File loaded", "Message", MessageBoxButton.YesNo, MessageBoxImage.Information);
        }
    }
}
