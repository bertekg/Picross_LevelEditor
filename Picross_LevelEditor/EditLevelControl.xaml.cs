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

using Picross_LevelEditor.Languages;
using Picross_LevelEditor.Properties;

namespace Picross_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy EditLevelControl.xaml
    /// </summary>
    public partial class EditLevelControl : UserControl
    {
        public GameLevel currGameLevel = new GameLevel();
        private bool bAfterInit = false;
        public EditLevelControl()
        {
            InitializeComponent();
            SetWindowsVisableElement();
            MainWindow mainWin = (MainWindow)Application.Current.MainWindow;
            currGameLevel = mainWin.currGameLevel;
            bAfterInit = true;
            UpdateAllGuiOfProgram();
        }
        public EditLevelControl(int i, int j)
        {
            InitializeComponent();
            SetWindowsVisableElement();
            MainWindow mainWin = (MainWindow)Application.Current.MainWindow;
            currGameLevel = mainWin.currMultiLevelMenager.GetGameLevel(new Point(i,j));
            bAfterInit = true;
            UpdateAllGuiOfProgram();
        }
        private void SetWindowsVisableElement()
        {
            gMainPlaceGrid.ShowGridLines = Settings.Default.bGridLineShow;
        }
        public void UpdateAllGuiOfProgram()
        {
            tbEnglishLevelName.Text = currGameLevel.stringEngName; //currGameLevel.dictonaryLevelNames["EN"];
            tbPolishLevelName.Text = currGameLevel.stringPolName;
            budLevelWidthX.Value = currGameLevel.intLevelWidthX;
            budLevelHeightY.Value = currGameLevel.intLevelHeightY;
            cpBackgroundColor.SelectedColor = Color.FromRgb(currGameLevel.colorBackground.colR, currGameLevel.colorBackground.colG, currGameLevel.colorBackground.colB);
            cpTilesNaturalColor.SelectedColor = Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB);
            cpMarkerColor.SelectedColor = Color.FromRgb(currGameLevel.colorMarker.colR, currGameLevel.colorMarker.colG, currGameLevel.colorMarker.colB);
            DrawColorPickerOptionMode();
            DrawAllLevelCells();
        }
        private void DrawAllLevelCells()
        {
            gMainPlaceGrid.Children.Clear();
            gMainPlaceGrid.RowDefinitions.Clear();
            gMainPlaceGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < currGameLevel.intLevelWidthX; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(20);
                gMainPlaceGrid.ColumnDefinitions.Add(gridCol);
            }
            for (int i = 0; i < currGameLevel.intLevelHeightY; i++)
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(20);
                gMainPlaceGrid.RowDefinitions.Add(gridRow);
            }
            for (int i = 0; i < currGameLevel.intLevelWidthX; i++)
            {
                for (int j = 0; j < currGameLevel.intLevelHeightY; j++)
                {
                    Rectangle rectangleTemp = new Rectangle();
                    Point newPoint = new Point(i, j);
                    rectangleTemp.Tag = newPoint;
                    rectangleTemp.MouseLeftButtonDown += RectangleTemp_MouseLeftButtonDown;
                    rectangleTemp.MouseRightButtonDown += RectangleTemp_MouseRightButtonDown;
                    rectangleTemp.MouseEnter += RectangleTemp_MouseEnter;
                    rectangleTemp.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTemp.VerticalAlignment = VerticalAlignment.Stretch;
                    Grid.SetColumn(rectangleTemp, i);
                    Grid.SetRow(rectangleTemp, (currGameLevel.intLevelHeightY - 1) - j);
                    if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == i && item.tileLocationY == j)))
                    {
                        byte byteResult = currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == i && item.tileLocationY == j)).colorId;
                        rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteResult].colR, currGameLevel.listPossibleColorsOfTiles[byteResult].colG, currGameLevel.listPossibleColorsOfTiles[byteResult].colB));
                        if (Settings.Default.bToolTipAvilable)
                        {
                            rectangleTemp.ToolTip = "X " + i.ToString() + "\nY " + j.ToString() + "\nC " + byteResult.ToString();
                        }
                    }
                    else
                    {
                        rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB));
                        if (Settings.Default.bToolTipAvilable)
                        {
                            rectangleTemp.ToolTip = "X " + i.ToString() + "\nY " + j.ToString() + "\nC -1";
                        }
                    }
                    gMainPlaceGrid.Children.Add(rectangleTemp);
                }
            }
        }
        private void DrawColorPickerOptionMode()
        {
            gUseColors.RowDefinitions.Clear();
            gUseColors.Children.Clear();
            for (int i = 0; i < currGameLevel.listPossibleColorsOfTiles.Count; i++)
            {
                RowDefinition rowDefAuto = new RowDefinition(); rowDefAuto.Height = GridLength.Auto;
                gUseColors.RowDefinitions.Add(rowDefAuto);
                Label labelNumberTemp = new Label();
                labelNumberTemp.Content = i.ToString();
                labelNumberTemp.VerticalAlignment = VerticalAlignment.Center;
                labelNumberTemp.HorizontalAlignment = HorizontalAlignment.Right;
                labelNumberTemp.SetValue(Grid.RowProperty, i);
                gUseColors.Children.Add(labelNumberTemp);
                Xceed.Wpf.Toolkit.ColorPicker xceedColorPickerTemp = new Xceed.Wpf.Toolkit.ColorPicker();
                xceedColorPickerTemp.Tag = i;
                xceedColorPickerTemp.SelectedColor = Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[i].colR, currGameLevel.listPossibleColorsOfTiles[i].colG, currGameLevel.listPossibleColorsOfTiles[i].colB);
                xceedColorPickerTemp.DisplayColorAndName = true;
                xceedColorPickerTemp.VerticalAlignment = VerticalAlignment.Center;
                xceedColorPickerTemp.SelectedColorChanged += ColorPicker_SelectedColorChanged;
                xceedColorPickerTemp.SetValue(Grid.RowProperty, i);
                xceedColorPickerTemp.SetValue(Grid.ColumnProperty, 1);
                gUseColors.Children.Add(xceedColorPickerTemp);
                Button buttonDeleteThisColorOption = new Button();
                buttonDeleteThisColorOption.Tag = i;
                buttonDeleteThisColorOption.Content = "X";
                buttonDeleteThisColorOption.VerticalAlignment = VerticalAlignment.Center;
                buttonDeleteThisColorOption.Margin = new Thickness(5);
                buttonDeleteThisColorOption.Padding = new Thickness(5);
                buttonDeleteThisColorOption.ToolTip = Lang.sDeleteThisColorOption;
                buttonDeleteThisColorOption.Click += ButtonDeleteThisColorOption_Click;
                buttonDeleteThisColorOption.SetValue(Grid.RowProperty, i);
                buttonDeleteThisColorOption.SetValue(Grid.ColumnProperty, 2);
                gUseColors.Children.Add(buttonDeleteThisColorOption);
            }
            DrawOptionColorMode();
        }
        private void DrawOptionColorMode()
        {
            wrapPanelColorsToSelect.Children.Clear();
            for (int i = 0; i < currGameLevel.listPossibleColorsOfTiles.Count; i++)
            {
                Border borderColorToSelectTemp = new Border();
                borderColorToSelectTemp.Tag = i + 1;
                borderColorToSelectTemp.BorderBrush = new SolidColorBrush(Colors.White);
                borderColorToSelectTemp.BorderThickness = new Thickness(5);
                borderColorToSelectTemp.MouseLeftButtonDown += EditModeChange_MouseLeftButtonDown;
                borderColorToSelectTemp.MouseRightButtonDown += EditModeChange_MouseRightButtonDown;
                Grid gridContent = new Grid();
                ColumnDefinition colDefAuto = new ColumnDefinition(); colDefAuto.Width = GridLength.Auto;
                gridContent.ColumnDefinitions.Add(colDefAuto);
                colDefAuto = new ColumnDefinition(); colDefAuto.Width = GridLength.Auto;
                gridContent.ColumnDefinitions.Add(colDefAuto);
                Label labelIdOfColor = new Label();
                labelIdOfColor.Content = i.ToString();
                labelIdOfColor.VerticalAlignment = VerticalAlignment.Center;
                gridContent.Children.Add(labelIdOfColor);
                Rectangle rectangleIndicateOfColor = new Rectangle();
                rectangleIndicateOfColor.Height = 32; rectangleIndicateOfColor.Width = 32;
                rectangleIndicateOfColor.SetValue(Grid.ColumnProperty, 1);
                rectangleIndicateOfColor.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[i].colR, currGameLevel.listPossibleColorsOfTiles[i].colG, currGameLevel.listPossibleColorsOfTiles[i].colB));
                gridContent.Children.Add(rectangleIndicateOfColor);
                borderColorToSelectTemp.Child = gridContent;
                wrapPanelColorsToSelect.Children.Add(borderColorToSelectTemp);
            }
            UpateEditModeOnGui();
        }
        private void RectangleTemp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = (Point)rectangleTemp.Tag;
            if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)))
            {
                if (byteLeftEditMode != 0)
                {
                    currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)).colorId = (byte)(byteLeftEditMode - 1);
                    rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colR, currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colG, currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colB));
                    if (Settings.Default.bToolTipAvilable)
                    {
                        rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteLeftEditMode - 1).ToString();
                    }
                    sender = rectangleTemp;
                }
                else
                {
                    currGameLevel.listLevelTilesOrder.Remove(currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)));
                    rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB));
                    if (Settings.Default.bToolTipAvilable)
                    {
                        rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteLeftEditMode - 1).ToString();
                    }
                    sender = rectangleTemp;
                }
            }
            else
            {
                if (byteLeftEditMode != 0)
                {
                    currGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)pTempTag.X, (byte)pTempTag.Y, (byte)(byteLeftEditMode - 1)));
                    rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colR, currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colG, currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colB));
                    if (Settings.Default.bToolTipAvilable)
                    {
                        rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteLeftEditMode - 1).ToString();
                    }
                    sender = rectangleTemp;
                }
            }
        }
        private void DrawAllLevelCells_Full()
        {
            gMainPlaceGrid.Children.Clear();
            gMainPlaceGrid.RowDefinitions.Clear();
            gMainPlaceGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < (2 * currGameLevel.intLevelWidthX) + 2; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                if (i == 0)
                {
                }
                else if (i % 2 == 1)
                {
                    if ((i - 1) % 5 == 0)
                    {
                        gridCol.Width = new GridLength(4);
                    }
                    else
                    {
                        gridCol.Width = new GridLength(2);
                    }
                }
                else
                {
                    gridCol.Width = new GridLength(50);
                }
                gMainPlaceGrid.ColumnDefinitions.Add(gridCol);
            }
            for (int i = 0; i < (2 * currGameLevel.intLevelHeightY) + 2; i++)
            {
                RowDefinition gridRow = new RowDefinition();
                if (i == 0)
                {
                }
                else if (i % 2 == 1)
                {
                    if ((i - 1) % 5 == 0)
                    {
                        gridRow.Height = new GridLength(4);
                    }
                    else
                    {
                        gridRow.Height = new GridLength(2);
                    }
                }
                else
                {
                    gridRow.Height = new GridLength(50);
                }
                gMainPlaceGrid.RowDefinitions.Add(gridRow);
            }
            for (int i = 0; i < currGameLevel.intLevelWidthX; i++)
            {
                for (int j = 0; j < currGameLevel.intLevelHeightY; j++)
                {
                    Grid gTemp = new Grid();
                    gTemp.MouseLeftButtonDown += GTemp_MouseLeftButtonDown;
                    Rectangle rTemp = new Rectangle();
                    rTemp.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rTemp.VerticalAlignment = VerticalAlignment.Stretch;
                    TextBlock tbTemp = new TextBlock();
                    Point newPoint = new Point(i, j);
                    gTemp.Width = 50; gTemp.Height = 50; gTemp.Tag = newPoint;
                    Grid.SetColumn(gTemp, 2 * i + 2);
                    Grid.SetRow(gTemp, 2 * ((currGameLevel.intLevelHeightY - 1) - j) + 2);
                    tbTemp.HorizontalAlignment = HorizontalAlignment.Center;
                    tbTemp.VerticalAlignment = VerticalAlignment.Center;

                    if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == newPoint.X && item.tileLocationY == newPoint.Y)))
                    {
                        byte byteResult = currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == newPoint.X && item.tileLocationY == newPoint.Y)).colorId;
                        rTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteResult].colR, currGameLevel.listPossibleColorsOfTiles[byteResult].colG, currGameLevel.listPossibleColorsOfTiles[byteResult].colB));
                        if ((currGameLevel.listPossibleColorsOfTiles[byteResult].colR * 0.299 + currGameLevel.listPossibleColorsOfTiles[byteResult].colG * 0.587 + currGameLevel.listPossibleColorsOfTiles[byteResult].colB * 0.114) > 186)
                        {
                            tbTemp.Foreground = new SolidColorBrush(Colors.Black);
                        }
                        else
                        {
                            tbTemp.Foreground = new SolidColorBrush(Colors.White);
                        }
                        if (Settings.Default.bToolTipAvilable)
                        {
                            gTemp.ToolTip = tbTemp.Text = "X " + i.ToString() + "\nY " + j.ToString() + "\nC " + byteResult.ToString();
                        }
                    }
                    else
                    {
                        rTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB));
                        if ((currGameLevel.colorTilesNeutral.colR * 0.299 + currGameLevel.colorTilesNeutral.colG * 0.587 + currGameLevel.colorTilesNeutral.colB * 0.114) > 186)
                        {
                            tbTemp.Foreground = new SolidColorBrush(Colors.Black);
                        }
                        else
                        {
                            tbTemp.Foreground = new SolidColorBrush(Colors.White);
                        }
                        if (Settings.Default.bToolTipAvilable)
                        {
                            gTemp.ToolTip = tbTemp.Text = "X " + i.ToString() + "\nY " + j.ToString() + "\nC -1";
                        }
                    }

                    //bTemp.MouseEnter += Button_MouseEnter;
                    gTemp.Children.Add(rTemp);
                    gTemp.Children.Add(tbTemp);
                    gMainPlaceGrid.Children.Add(gTemp);
                }
            }
            CalcTilesHintData();
            for (byte y = 0; y < currGameLevel.intLevelHeightY; y++)
            {
                if (currGameLevel.listVerticlaNumbersHints[y].Count > 0)
                {
                    WrapPanel wpTemp = new WrapPanel();
                    Grid.SetRow(wpTemp, 2 * ((currGameLevel.intLevelHeightY - 1) - y) + 2);
                    wpTemp.VerticalAlignment = VerticalAlignment.Center;
                    for (byte i = 0; i < currGameLevel.listVerticlaNumbersHints[y].Count; i++)
                    {
                        TextBlock tbTemp = new TextBlock();
                        tbTemp.Margin = new Thickness(4);
                        tbTemp.Text = currGameLevel.listVerticlaNumbersHints[y][i].length.ToString();
                        tbTemp.Foreground = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[currGameLevel.listVerticlaNumbersHints[y][i].color].colR, currGameLevel.listPossibleColorsOfTiles[currGameLevel.listVerticlaNumbersHints[y][i].color].colG, currGameLevel.listPossibleColorsOfTiles[currGameLevel.listVerticlaNumbersHints[y][i].color].colB));
                        wpTemp.Children.Add(tbTemp);
                    }
                    gMainPlaceGrid.Children.Add(wpTemp);
                }
            }

            for (byte x = 0; x < currGameLevel.intLevelWidthX; x++)
            {
                if (currGameLevel.listHorizontalNumberHints[x].Count > 0)
                {
                    WrapPanel wpTemp = new WrapPanel();
                    Grid.SetColumn(wpTemp, (2 * x) + 2);
                    wpTemp.HorizontalAlignment = HorizontalAlignment.Center;
                    wpTemp.Orientation = Orientation.Vertical;
                    for (byte i = 0; i < currGameLevel.listHorizontalNumberHints[x].Count; i++)
                    {
                        TextBlock tbTemp = new TextBlock();
                        tbTemp.Margin = new Thickness(4);
                        tbTemp.Text = currGameLevel.listHorizontalNumberHints[x][i].length.ToString();
                        tbTemp.Foreground = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[currGameLevel.listHorizontalNumberHints[x][i].color].colR, currGameLevel.listPossibleColorsOfTiles[currGameLevel.listHorizontalNumberHints[x][i].color].colG, currGameLevel.listPossibleColorsOfTiles[currGameLevel.listHorizontalNumberHints[x][i].color].colB));
                        wpTemp.Children.Add(tbTemp);
                    }
                    gMainPlaceGrid.Children.Add(wpTemp);
                }
            }
            //lvUsers.ItemsSource = null;
            //lvUsers.ItemsSource = currGameLevel.listLevelTilesOrder;
        }
        private void RectangleTemp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = (Point)rectangleTemp.Tag;
            if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)))
            {
                if (byteRightEditMode != 0)
                {
                    currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)).colorId = (byte)(byteRightEditMode - 1);
                    rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colR, currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colG, currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colB));
                    if (Settings.Default.bToolTipAvilable)
                    {
                        rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteRightEditMode - 1).ToString();
                    }
                    sender = rectangleTemp;
                }
                else
                {
                    currGameLevel.listLevelTilesOrder.Remove(currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)));
                    rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB));
                    if (Settings.Default.bToolTipAvilable)
                    {
                        rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteRightEditMode - 1).ToString();
                    }
                    sender = rectangleTemp;
                }
            }
            else
            {
                if (byteRightEditMode != 0)
                {
                    currGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)pTempTag.X, (byte)pTempTag.Y, (byte)(byteRightEditMode - 1)));
                    rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colR, currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colG, currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colB));
                    if (Settings.Default.bToolTipAvilable)
                    {
                        rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteRightEditMode - 1).ToString();
                    }
                    sender = rectangleTemp;
                }
            }
        }
        byte byteLeftEditMode = 1, byteRightEditMode = 0;
        private void RectangleTemp_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = (Point)rectangleTemp.Tag;
            // tbCurrentGrid.Text = "X:" + pTempTag.X.ToString() + ", Y:" + pTempTag.Y.ToString();
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)))
                {
                    if (byteLeftEditMode != 0)
                    {
                        currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)).colorId = (byte)(byteLeftEditMode - 1);
                        rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colR, currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colG, currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colB));
                        if (Settings.Default.bToolTipAvilable)
                        {
                            rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteLeftEditMode - 1).ToString();
                        }
                        sender = rectangleTemp;
                    }
                    else
                    {
                        currGameLevel.listLevelTilesOrder.Remove(currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)));
                        rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB));
                        if (Settings.Default.bToolTipAvilable)
                        {
                            rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteLeftEditMode - 1).ToString();
                        }
                        sender = rectangleTemp;
                    }
                }
                else
                {
                    if (byteLeftEditMode != 0)
                    {
                        currGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)pTempTag.X, (byte)pTempTag.Y, (byte)(byteLeftEditMode - 1)));
                        rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colR, currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colG, currGameLevel.listPossibleColorsOfTiles[byteLeftEditMode - 1].colB));
                        if (Settings.Default.bToolTipAvilable)
                        {
                            rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteLeftEditMode - 1).ToString();
                        }
                        sender = rectangleTemp;
                    }
                }
            }
            else if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)))
                {
                    if (byteRightEditMode != 0)
                    {
                        currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)).colorId = (byte)(byteRightEditMode - 1);
                        rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colR, currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colG, currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colB));
                        if (Settings.Default.bToolTipAvilable)
                        {
                            rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteRightEditMode - 1).ToString();
                        }
                        sender = rectangleTemp;
                    }
                    else
                    {
                        currGameLevel.listLevelTilesOrder.Remove(currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)));
                        rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.colorTilesNeutral.colR, currGameLevel.colorTilesNeutral.colG, currGameLevel.colorTilesNeutral.colB));
                        if (Settings.Default.bToolTipAvilable)
                        {
                            rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteRightEditMode - 1).ToString();
                        }
                        sender = rectangleTemp;
                    }
                }
                else
                {
                    if (byteRightEditMode != 0)
                    {
                        currGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)pTempTag.X, (byte)pTempTag.Y, (byte)(byteRightEditMode - 1)));
                        rectangleTemp.Fill = new SolidColorBrush(Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colR, currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colG, currGameLevel.listPossibleColorsOfTiles[byteRightEditMode - 1].colB));
                        rectangleTemp.ToolTip = "X " + pTempTag.X.ToString() + "\nY " + pTempTag.Y.ToString() + "\nC " + (byteRightEditMode - 1).ToString();
                        sender = rectangleTemp;
                    }
                }
            }
        }
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Xceed.Wpf.Toolkit.ColorPicker cpTemp = (Xceed.Wpf.Toolkit.ColorPicker)sender;
            byte byteIndexOfChangeColor = Convert.ToByte(cpTemp.Tag.ToString());
            currGameLevel.listPossibleColorsOfTiles[byteIndexOfChangeColor].colR = cpTemp.SelectedColor.Value.R;
            currGameLevel.listPossibleColorsOfTiles[byteIndexOfChangeColor].colG = cpTemp.SelectedColor.Value.G;
            currGameLevel.listPossibleColorsOfTiles[byteIndexOfChangeColor].colB = cpTemp.SelectedColor.Value.B;
            DrawOptionColorMode();
            DrawAllLevelCells();
        }
        private void buttonAddNewColorPicker_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            OnlyRGB temp = new OnlyRGB((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
            currGameLevel.listPossibleColorsOfTiles.Add(temp);
            DrawColorPickerOptionMode();
        }
        private void ButtonDeleteThisColorOption_Click(object sender, RoutedEventArgs e)
        {
            Button currentClickButton = (Button)sender;
            byte byteIdOfColor = Convert.ToByte(currentClickButton.Tag.ToString());
            MessageBoxResult mbsAnswer = MessageBox.Show(Lang.sQuestionBeforeDeleteColor + "\n" + Lang.sIdOfColor + ": " + currentClickButton.Tag.ToString(), Lang.sConfiramtionOfOperation, MessageBoxButton.YesNo);
            if (mbsAnswer == MessageBoxResult.Yes)
            {
                currGameLevel.listPossibleColorsOfTiles.RemoveAt(byteIdOfColor);
                currGameLevel.listLevelTilesOrder.RemoveAll(item => item.colorId == byteIdOfColor);
                for (int i = 0; i < currGameLevel.listLevelTilesOrder.Count; i++)
                {
                    if (currGameLevel.listLevelTilesOrder[i].colorId > byteIdOfColor)
                    {
                        currGameLevel.listLevelTilesOrder[i].colorId -= 1;
                    }
                }
                DrawColorPickerOptionMode();
                DrawAllLevelCells();
            }
        }
        private void EditModeChange_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border buttenClicked = (Border)sender;
            byte byteIndexOfSlectedBorder = Convert.ToByte(buttenClicked.Tag.ToString());
            if (byteLeftEditMode != byteIndexOfSlectedBorder && byteIndexOfSlectedBorder != byteRightEditMode)
            {
                byteLeftEditMode = byteIndexOfSlectedBorder;
                UpateEditModeOnGui();
            }
        }
        private void EditModeChange_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border buttenClicked = (Border)sender;
            byte byteIndexOfSlectedBorder = Convert.ToByte(buttenClicked.Tag.ToString());
            if (byteLeftEditMode != byteIndexOfSlectedBorder && byteLeftEditMode != byteRightEditMode)
            {
                byteRightEditMode = byteIndexOfSlectedBorder;
                UpateEditModeOnGui();
            }
        }
        private void UpateEditModeOnGui()
        {
            SolidColorBrush scbWhite = new SolidColorBrush(Colors.White);
            LinearGradientBrush lgbGreenYelowToWhite90 = new LinearGradientBrush(Colors.GreenYellow, Colors.Cyan, 90.0);
            LinearGradientBrush lgbPurpleToRed90 = new LinearGradientBrush(Colors.Purple, Colors.Red, 90.0);
            for (int i = 0; i < currGameLevel.listPossibleColorsOfTiles.Count; i++)
            {
                Border borderTemp = (Border)wrapPanelColorsToSelect.Children[i];
                if (i + 1 == byteLeftEditMode)
                {
                    borderTemp.BorderBrush = lgbGreenYelowToWhite90;
                }
                else if (i + 1 == byteRightEditMode)
                {
                    borderTemp.BorderBrush = lgbPurpleToRed90;
                }
                else
                {
                    borderTemp.BorderBrush = scbWhite;
                }
                wrapPanelColorsToSelect.Children[i] = borderTemp;
            }
            if (byteLeftEditMode == 0)
            {
                buttonDeleteTilleMode.BorderBrush = lgbGreenYelowToWhite90;
            }
            else if (byteRightEditMode == 0)
            {
                buttonDeleteTilleMode.BorderBrush = lgbPurpleToRed90;
            }
            else
            {
                buttonDeleteTilleMode.BorderBrush = scbWhite;
            }
        }
        private void GTemp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid bTemp = (Grid)sender;
            Point pTempTag = (Point)bTemp.Tag;
            if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)))
            {
                if (byteLeftEditMode != 0)
                {
                    currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)).colorId = (byte)(byteLeftEditMode - 1);
                }
                else
                {
                    currGameLevel.listLevelTilesOrder.Remove(currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pTempTag.X && item.tileLocationY == pTempTag.Y)));
                }
            }
            else
            {
                if (byteLeftEditMode != 0)
                {
                    currGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)pTempTag.X, (byte)pTempTag.Y, (byte)(byteLeftEditMode - 1)));
                }
            }
            DrawAllLevelCells();
        }
        private void CalcTilesHintData()
        {
            currGameLevel.listVerticlaNumbersHints.Clear();
            for (byte y = 0; y < currGameLevel.intLevelHeightY; y++)
            {
                byte prevCellId = 0;
                byte currentIdCombo = 0;
                List<HintData> lVerticalNumberHints = new List<HintData>();
                for (byte x = 0; x < currGameLevel.intLevelWidthX; x++)
                {
                    Point currPoint = new Point(x, y);
                    byte currCellId = 0;
                    if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == currPoint.X && item.tileLocationY == currPoint.Y)))
                    {
                        currCellId = (byte)(currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == currPoint.X && item.tileLocationY == currPoint.Y)).colorId + 1);
                        if (currCellId == prevCellId)
                        {
                            currentIdCombo++;
                        }
                        else if (prevCellId > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.color = (byte)(prevCellId - 1);
                            hdTemp.length = currentIdCombo;
                            lVerticalNumberHints.Add(hdTemp);
                            currentIdCombo = 1;
                        }
                        else
                        {
                            currentIdCombo = 1;
                        }
                        prevCellId = currCellId;
                    }
                    else
                    {
                        if (currentIdCombo > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.color = (byte)(prevCellId - 1);
                            hdTemp.length = currentIdCombo;
                            lVerticalNumberHints.Add(hdTemp);
                            currentIdCombo = 0;
                            prevCellId = 0;
                        }
                    }
                }
                if (currentIdCombo > 0)
                {
                    HintData hdTemp = new HintData();
                    hdTemp.color = (byte)(prevCellId - 1);
                    hdTemp.length = currentIdCombo;
                    lVerticalNumberHints.Add(hdTemp);
                    currentIdCombo = 0;
                    prevCellId = 0;
                }
                currGameLevel.listVerticlaNumbersHints.Add(lVerticalNumberHints);
            }

            currGameLevel.listHorizontalNumberHints.Clear();
            for (byte x = 0; x < currGameLevel.intLevelWidthX; x++)
            {
                byte prevCellId = 0;
                byte currentIdCombo = 0;
                List<HintData> lHorizontalNumberHints = new List<HintData>();
                for (byte y = currGameLevel.intLevelHeightY; y > 0; y--)
                {
                    Point currPoint = new Point(x, y - 1);
                    byte currCellId = 0;
                    if (currGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == currPoint.X && item.tileLocationY == currPoint.Y)))
                    {
                        currCellId = (byte)(currGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == currPoint.X && item.tileLocationY == currPoint.Y)).colorId + 1);
                        if (currCellId == prevCellId)
                        {
                            currentIdCombo++;
                        }
                        else if (prevCellId > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.color = (byte)(prevCellId - 1);
                            hdTemp.length = currentIdCombo;
                            lHorizontalNumberHints.Add(hdTemp);
                            currentIdCombo = 1;
                        }
                        else
                        {
                            currentIdCombo = 1;
                        }
                        prevCellId = currCellId;
                    }
                    else
                    {
                        if (currentIdCombo > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.color = (byte)(prevCellId - 1);
                            hdTemp.length = currentIdCombo;
                            lHorizontalNumberHints.Add(hdTemp);
                            currentIdCombo = 0;
                            prevCellId = 0;
                        }
                    }
                }
                if (currentIdCombo > 0)
                {
                    HintData hdTemp = new HintData();
                    hdTemp.color = (byte)(prevCellId - 1);
                    hdTemp.length = currentIdCombo;
                    lHorizontalNumberHints.Add(hdTemp);
                    currentIdCombo = 0;
                    prevCellId = 0;
                }
                currGameLevel.listHorizontalNumberHints.Add(lHorizontalNumberHints);
            }
        }
        private void CpTilesNaturalColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (bAfterInit)
            {
                currGameLevel.colorTilesNeutral.colR = cpTilesNaturalColor.SelectedColor.Value.R;
                currGameLevel.colorTilesNeutral.colG = cpTilesNaturalColor.SelectedColor.Value.G;
                currGameLevel.colorTilesNeutral.colB = cpTilesNaturalColor.SelectedColor.Value.B;
                DrawAllLevelCells();
            }
        }
        private void CpBackgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (bAfterInit)
            {
                currGameLevel.colorBackground.colR = cpBackgroundColor.SelectedColor.Value.R;
                currGameLevel.colorBackground.colG = cpBackgroundColor.SelectedColor.Value.G;
                currGameLevel.colorBackground.colB = cpBackgroundColor.SelectedColor.Value.B;
                gMainPlaceGrid.Background = new SolidColorBrush(Color.FromRgb(currGameLevel.colorBackground.colR, currGameLevel.colorBackground.colG, currGameLevel.colorBackground.colB));
            }
        }
        private void BudLevelWidthX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (bAfterInit)
            {
                currGameLevel.intLevelWidthX = budLevelWidthX.Value.Value;
                UpdateAllGuiOfProgram();
            }
        }
        private void BudLevelHeightY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (bAfterInit)
            {
                currGameLevel.intLevelHeightY = budLevelHeightY.Value.Value;
                UpdateAllGuiOfProgram();
            }
        }
        private void CpMarkerColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (bAfterInit)
            {
                currGameLevel.colorMarker.colR = cpMarkerColor.SelectedColor.Value.R;
                currGameLevel.colorMarker.colG = cpMarkerColor.SelectedColor.Value.G;
                currGameLevel.colorMarker.colB = cpMarkerColor.SelectedColor.Value.B;
            }
        }
    }
}
