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
using System.Windows.Shapes;

namespace Picross_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy PreviewLevel.xaml
    /// </summary>
    public partial class PreviewLevel : Window
    {
        GameLevel currGameLevel = new GameLevel();
        int offsetY;
        public PreviewLevel(GameLevel previewGameLevel)
        {
            InitializeComponent();
            currGameLevel = previewGameLevel;
            gStartBackgroundGrid.Background = new SolidColorBrush(Color.FromRgb(currGameLevel.colorBackground.colR, currGameLevel.colorBackground.colG, currGameLevel.colorBackground.colB));
            gEndBackgroundGrid.Background = new SolidColorBrush(Color.FromRgb(currGameLevel.colorBackground.colR, currGameLevel.colorBackground.colG, currGameLevel.colorBackground.colB));
            offsetY = currGameLevel.intLevelHeightY % 5;
            AddHintsToGrid();
            UpdatesGridsLayoutOfPicture();            
            AddCellsToGrids();
            UpadteFinalPicture();
            tbFinish_NameLevel.Text = currGameLevel.stringEngName + " [" + currGameLevel.intLevelWidthX.ToString() + "," + currGameLevel.intLevelHeightY.ToString() + "]";
        }
        private void AddHintsToGrid()
        {
            gStartHintsHorizontal.ColumnDefinitions.Clear();
            gEndHintsHorizontal.ColumnDefinitions.Clear();
            gStartHintsHorizontal.RowDefinitions.Clear();
            gEndHintsHorizontal.RowDefinitions.Clear();
            gStartHintsVertical.ColumnDefinitions.Clear();
            gEndHintsVertical.ColumnDefinitions.Clear();
            gStartHintsVertical.RowDefinitions.Clear();
            gEndHintsVertical.RowDefinitions.Clear();
            ColumnDefinition gridColDefTemp;
            double spacegrid;
            
            for (int i = 0; i < currGameLevel.intLevelWidthX; i++)
            {
                if(i % 5 == 0 && i != 0)
                {
                    spacegrid = 15;
                }
                else
                {
                    spacegrid = 5;
                }
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(spacegrid);
                gStartHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(spacegrid);
                gEndHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                gStartHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                gEndHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                if (i == currGameLevel.intLevelWidthX - 1)
                {
                    gridColDefTemp = new ColumnDefinition();
                    gridColDefTemp.Width = new GridLength(5);
                    gStartHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                    gridColDefTemp = new ColumnDefinition();
                    gridColDefTemp.Width = new GridLength(5);
                    gEndHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                }
            }
            int maxSizeHintsHorizontalCounts = currGameLevel.listHorizontalNumberHints.OrderByDescending(list => list.Count()).First().Count;
            RowDefinition gridRowDefTemp;
            for (int i = 0; i < maxSizeHintsHorizontalCounts; i++)
            {
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                gStartHintsHorizontal.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                gEndHintsHorizontal.RowDefinitions.Add(gridRowDefTemp);
            }
            gStartHintsHorizontal.Children.Clear();
            gEndHintsHorizontal.Children.Clear();
            for (int i = 0; i < currGameLevel.intLevelWidthX; i++)
            {
                for(int j = 0; j < currGameLevel.listHorizontalNumberHints[i].Count; j++)
                {
                    TextBlock textBlockHint = new TextBlock();
                    textBlockHint.Text = currGameLevel.listHorizontalNumberHints[i][j].length.ToString();
                    Grid.SetColumn(textBlockHint, 2 * i + 1);
                    Grid.SetRow(textBlockHint, maxSizeHintsHorizontalCounts - currGameLevel.listHorizontalNumberHints[i].Count + j);
                    byte byteResult = currGameLevel.listHorizontalNumberHints[i][j].color;
                    textBlockHint.Foreground = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteResult].colR, currGameLevel.listPossibleColorsOfTiles[byteResult].colG, currGameLevel.listPossibleColorsOfTiles[byteResult].colB));                    
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gStartHintsHorizontal.Children.Add(textBlockHint);
                    textBlockHint = new TextBlock();
                    textBlockHint.Text = currGameLevel.listHorizontalNumberHints[i][j].length.ToString();
                    Grid.SetColumn(textBlockHint, 2 * i + 1);
                    Grid.SetRow(textBlockHint, maxSizeHintsHorizontalCounts - currGameLevel.listHorizontalNumberHints[i].Count + j);
                    byteResult = currGameLevel.listHorizontalNumberHints[i][j].color;
                    textBlockHint.Foreground = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteResult].colR, currGameLevel.listPossibleColorsOfTiles[byteResult].colG, currGameLevel.listPossibleColorsOfTiles[byteResult].colB));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gEndHintsHorizontal.Children.Add(textBlockHint);
                }
            }
            int maxSizeHintsVerticalCounts = currGameLevel.listVerticlaNumbersHints.OrderByDescending(list => list.Count()).First().Count;
            for (int i = 0; i < maxSizeHintsVerticalCounts; i++)
            {
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                gStartHintsVertical.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                gEndHintsVertical.ColumnDefinitions.Add(gridColDefTemp);
            }
            for (int i = 0; i < currGameLevel.intLevelHeightY; i++)
            {
                if ((i - offsetY) % 5 == 0 && i != 0)
                {
                    spacegrid = 15;
                }
                else
                {
                    spacegrid = 5;
                }
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(spacegrid);
                gStartHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(spacegrid);
                gEndHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                gStartHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                gEndHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                if (i == currGameLevel.intLevelHeightY - 1)
                {
                    gridRowDefTemp = new RowDefinition();
                    gridRowDefTemp.Height = new GridLength(5);
                    gStartHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                    gridRowDefTemp = new RowDefinition();
                    gridRowDefTemp.Height = new GridLength(5);
                    gEndHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                }
            }
            gStartHintsVertical.Children.Clear();
            gEndHintsVertical.Children.Clear();
            for (int j = 0; j < currGameLevel.intLevelHeightY; j++)
            {
                for (int i = 0; i< currGameLevel.listVerticlaNumbersHints[j].Count; i++)
                {
                    TextBlock textBlockHint = new TextBlock();
                    textBlockHint.Text = currGameLevel.listVerticlaNumbersHints[j][i].length.ToString();
                    Grid.SetColumn(textBlockHint, maxSizeHintsVerticalCounts - currGameLevel.listVerticlaNumbersHints[j].Count + i);
                    Grid.SetRow(textBlockHint, 2 * ((int)currGameLevel.intLevelHeightY - j - 1) + 1);
                    byte byteResult = currGameLevel.listVerticlaNumbersHints[j][i].color;
                    textBlockHint.Foreground = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteResult].colR, currGameLevel.listPossibleColorsOfTiles[byteResult].colG, currGameLevel.listPossibleColorsOfTiles[byteResult].colB));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gStartHintsVertical.Children.Add(textBlockHint);
                    textBlockHint = new TextBlock();
                    textBlockHint.Text = currGameLevel.listVerticlaNumbersHints[j][i].length.ToString();
                    Grid.SetColumn(textBlockHint, maxSizeHintsVerticalCounts - currGameLevel.listVerticlaNumbersHints[j].Count + i);
                    Grid.SetRow(textBlockHint, 2 * (currGameLevel.intLevelHeightY - j - 1) + 1);
                    byteResult = currGameLevel.listVerticlaNumbersHints[j][i].color;
                    textBlockHint.Foreground = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteResult].colR, currGameLevel.listPossibleColorsOfTiles[byteResult].colG, currGameLevel.listPossibleColorsOfTiles[byteResult].colB));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gEndHintsVertical.Children.Add(textBlockHint);
                }
            }
        }
        private void UpdatesGridsLayoutOfPicture()
        {
            gStartPicture.RowDefinitions.Clear();
            gStartPicture.ColumnDefinitions.Clear();
            gEndPic.RowDefinitions.Clear();
            gEndPic.ColumnDefinitions.Clear();
            ColumnDefinition gridColTemp;
            double spacegrid;
            for (int i = 0; i < currGameLevel.intLevelWidthX; i++)
            {
                if (i % 5 == 0 && i != 0)
                {
                    spacegrid = 15;
                }
                else
                {
                    spacegrid = 5;
                }
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(spacegrid);
                gStartPicture.ColumnDefinitions.Add(gridColTemp);
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(spacegrid);
                gEndPic.ColumnDefinitions.Add(gridColTemp);
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(50);
                gStartPicture.ColumnDefinitions.Add(gridColTemp);
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(50);
                gEndPic.ColumnDefinitions.Add(gridColTemp);
                if (i == currGameLevel.intLevelWidthX - 1)
                {
                    gridColTemp = new ColumnDefinition();
                    gridColTemp.Width = new GridLength(5);
                    gStartPicture.ColumnDefinitions.Add(gridColTemp);
                    gridColTemp = new ColumnDefinition();
                    gridColTemp.Width = new GridLength(5);
                    gEndPic.ColumnDefinitions.Add(gridColTemp);
                }
            }
            RowDefinition gridRowTemp;
            for (int i = 0; i < currGameLevel.intLevelHeightY; i++)
            {
                if ((i - offsetY) % 5 == 0 && i != 0)
                {
                    spacegrid = 15;
                }
                else
                {
                    spacegrid = 5;
                }
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(spacegrid);
                gStartPicture.RowDefinitions.Add(gridRowTemp);
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(spacegrid);
                gEndPic.RowDefinitions.Add(gridRowTemp);
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(50);
                gStartPicture.RowDefinitions.Add(gridRowTemp);
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(50);
                gEndPic.RowDefinitions.Add(gridRowTemp);
                if (i == currGameLevel.intLevelHeightY - 1)
                {
                    gridRowTemp = new RowDefinition();
                    gridRowTemp.Height = new GridLength(5);
                    gStartPicture.RowDefinitions.Add(gridRowTemp);
                    gridRowTemp = new RowDefinition();
                    gridRowTemp.Height = new GridLength(5);
                    gEndPic.RowDefinitions.Add(gridRowTemp);
                }
            }
        }
        private void AddCellsToGrids()
        {
            gStartPicture.Children.Clear();
            gEndPic.Children.Clear();
            Rectangle rectangleMark = new Rectangle();
            rectangleMark.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectangleMark.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(rectangleMark, 0);
            Grid.SetRow(rectangleMark, 2 * (currGameLevel.intLevelHeightY - 1));
            Grid.SetColumnSpan(rectangleMark, 3);
            Grid.SetRowSpan(rectangleMark, 3);
            rectangleMark.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorMarker.colR, currGameLevel.colorMarker.colG, currGameLevel.colorMarker.colB));
            gStartPicture.Children.Add(rectangleMark);
            rectangleMark = new Rectangle();
            rectangleMark.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectangleMark.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(rectangleMark, 0);
            Grid.SetRow(rectangleMark, 2 * (currGameLevel.intLevelHeightY - 1));
            Grid.SetColumnSpan(rectangleMark, 3);
            Grid.SetRowSpan(rectangleMark, 3);
            rectangleMark.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorMarker.colR, currGameLevel.colorMarker.colG, currGameLevel.colorMarker.colB));
            gEndPic.Children.Add(rectangleMark);
            Rectangle rectangleTempStart;
            Rectangle rectangleTempEnd;
            for (int i = 0; i < currGameLevel.intLevelWidthX; i++)
            {
                for (int j = 0; j < currGameLevel.intLevelHeightY; j++)
                {
                    rectangleTempStart = new Rectangle();
                    rectangleTempEnd = new Rectangle();
                    rectangleTempStart.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTempEnd.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTempStart.VerticalAlignment = VerticalAlignment.Stretch;
                    rectangleTempEnd.VerticalAlignment = VerticalAlignment.Stretch;
                    Grid.SetColumn(rectangleTempStart, 2*i + 1);
                    Grid.SetColumn(rectangleTempEnd, 2* i + 1);
                    Grid.SetRow(rectangleTempStart, 2 * ((currGameLevel.intLevelHeightY - 1) - j) + 1);
                    Grid.SetRow(rectangleTempEnd, 2 * ((currGameLevel.intLevelHeightY - 1) - j) + 1);
                    rectangleTempStart.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB));
                    if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == i && item.tileLocationY == j)))
                    {
                        byte byteResult = currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == i && item.tileLocationY == j)).colorId;
                        rectangleTempEnd.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteResult].colR, currGameLevel.listPossibleColorsOfTiles[byteResult].colG, currGameLevel.listPossibleColorsOfTiles[byteResult].colB));
                    }
                    else
                    {
                        rectangleTempEnd.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB));
                    }
                    gStartPicture.Children.Add(rectangleTempStart);
                    gEndPic.Children.Add(rectangleTempEnd);                    
                }
            }            
        }
        private void UpadteFinalPicture()
        {
            WriteableBitmap wbitmap = new WriteableBitmap(currGameLevel.intLevelWidthX, currGameLevel.intLevelHeightY, 96, 96, PixelFormats.Bgra32, null);
            /// Tutaj toto jest: http://csharphelper.com/blog/2015/07/set-the-pixels-in-a-wpf-bitmap-in-c/
            byte[] pixels1d = new byte[currGameLevel.intLevelHeightY * currGameLevel.intLevelWidthX * 4];
            int index = 0;
            for (int row = 0; row < currGameLevel.intLevelHeightY; row++)
            {
                for (int col = 0; col < currGameLevel.intLevelWidthX; col++)
                {
                    if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == col && item.tileLocationY == currGameLevel.intLevelHeightY - row - 1)))
                    {
                        byte byteResult = currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == col && item.tileLocationY == currGameLevel.intLevelHeightY - row - 1)).colorId;
                        pixels1d[index++] = currGameLevel.listPossibleColorsOfTiles[byteResult].colB;
                        pixels1d[index++] = currGameLevel.listPossibleColorsOfTiles[byteResult].colG;
                        pixels1d[index++] = currGameLevel.listPossibleColorsOfTiles[byteResult].colR;
                    }
                    else
                    {
                        pixels1d[index++] = currGameLevel.colorTilesNeutral.colB;
                        pixels1d[index++] = currGameLevel.colorTilesNeutral.colG;
                        pixels1d[index++] = currGameLevel.colorTilesNeutral.colR;
                    }
                    pixels1d[index++] = 255;
                }
            }
            Int32Rect rect = new Int32Rect(0, 0, currGameLevel.intLevelWidthX, currGameLevel.intLevelHeightY);
            int stride = 4 * currGameLevel.intLevelWidthX;
            wbitmap.WritePixels(rect, pixels1d, stride, 0);
            iFinal.Source = wbitmap;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Escape || e.Key == Key.Enter || e.Key == Key.Back)
            {
                this.Close();
            }
        }
    }
}
