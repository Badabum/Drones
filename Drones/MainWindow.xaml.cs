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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Drones.Common;
using Drones.Models;
using GHQualification.Models;

namespace Drones
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            const string busyDay = "busy_day";
            const string redundancy = "redundancy";
            const string mamka = "mother_of_all_warehouses";
            InitializeComponent();
            var gridCellSize = 3;
            var path = $@"C:\Users\Ihor\Documents\visual studio 2015\Projects\Drones\Drones\{busyDay}.in";
            var reader = new FileOperations(path);
            var _dataModel = reader.ReadFileAsync();
            DrawGrid(_dataModel.GeneralInfo.Columns,_dataModel.GeneralInfo.Rows,gridCellSize);
            DrawObjects(_dataModel.Warehouses, Brushes.Green,3, gridCellSize);
            DrawObjects(_dataModel.Orders, Brushes.Yellow,1,gridCellSize);
            
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
                    Fill = color
                };
                mainCanvas.Children.Add(rectangle);
                Canvas.SetTop(rectangle,drawable.R*gridCellSize);
                Canvas.SetLeft(rectangle, drawable.C*gridCellSize);
            }
        }


        

    }
}
