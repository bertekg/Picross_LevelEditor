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

namespace Picross_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy MultiLevelTabView.xaml
    /// </summary>
    public partial class MultiLevelTabView : UserControl
    {
        public MultiLevelMenager multiLevelMena = new MultiLevelMenager();
        public MultiLevelTabView()
        {
            InitializeComponent();
            MainWindow mainWin = (MainWindow)Application.Current.MainWindow;
            multiLevelMena = mainWin.currMultiLevelMenager;
            UpdateAllLayout();
        }
        public SelectionMode smCurrent = SelectionMode.Edit;
        public enum SelectionMode { None, Edit, ActiveDeactive};
        private void UpdateAllLayout()
        {
            tbAllLevelNameEng.Text = multiLevelMena.stringAllLevelsEngName;
            tbAllLevelNamePol.Text = multiLevelMena.stringAllLevelsPolName;
            iudTotalLevelWidthX.Value = multiLevelMena.intTotalLevelWidthX;
            iudTotalLevelHeightY.Value = multiLevelMena.intTotalLevelHeightY;
            budSingleLevelWidthXBasic.Value = multiLevelMena.byteSingleWidthX;
            budSingleLevelHeightYBasic.Value = multiLevelMena.byteSingleHeightY;
            CalcSingleWidth();
            CalcSingleHeight();
            SetForAllPlay();
            UpateSelectionModeOnGui();
            UpdatePartGrid();
        }
        private void SetForAllPlay()
        {
            for (int j = 0; j < multiLevelMena.intCountPartsHeightY; j++)
            {
                for (int i = 0; i < multiLevelMena.intCountPartsWidthX; i++)
                {
                    Point pTemp = new Point(i, j);
                    //multiLevelMena.listLevelToPlay.Add(pTemp, true);
                }
            }
        }
        private void UpdatePartGrid()
        {
            gPlacePart.Children.Clear();
            gPlacePart.RowDefinitions.Clear();
            gPlacePart.ColumnDefinitions.Clear();
            int currVal = 0;
            for(int i = 0; i < multiLevelMena.intCountPartsHeightY; i++)
            {
                if(i > 0)
                {
                    currVal = 10 * multiLevelMena.byteSingleHeightY;
                }
                else
                {
                    currVal = 10 * multiLevelMena.byteLastHeightY;
                }
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(currVal);
                gPlacePart.RowDefinitions.Add(gridRow);
            }
            for (int i = 0; i < multiLevelMena.intCountPartsWidthX; i++)
            {
                if (i < multiLevelMena.intCountPartsWidthX - 1)
                {
                    currVal = 10 * multiLevelMena.byteSingleWidthX;
                }
                else
                {
                    currVal = 10 * multiLevelMena.byteLastWidthX;
                }
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(currVal);
                gPlacePart.ColumnDefinitions.Add(gridCol);
            }
            SolidColorBrush scbGreen = new SolidColorBrush(Colors.Green);
            SolidColorBrush scbRed = new SolidColorBrush(Colors.Red);
            Thickness thicknessTemp = new Thickness(1);
            SolidColorBrush scbBlue = new SolidColorBrush(Colors.Blue);
            Dictionary<Point, bool> temp = multiLevelMena.GetDirectiryListLevelToPlay();
            for (int j = 0; j < multiLevelMena.intCountPartsHeightY; j++)
            {
                for (int i = 0; i < multiLevelMena.intCountPartsWidthX; i++)
                {
                    Border borderTemp = new Border();
                    Point pTemp = new Point(i, j);
                    
                    if (temp[pTemp])
                    {
                        borderTemp.BorderBrush = scbGreen;
                    }
                    else
                    {
                        borderTemp.BorderBrush = scbRed;
                    }
                    borderTemp.BorderThickness = thicknessTemp;
                    Viewbox tempViewBox = new Viewbox();
                    tempViewBox.Stretch = Stretch.Uniform;
                    Image tempImage = new Image();
                    tempImage.Stretch = Stretch.Uniform; tempImage.HorizontalAlignment = HorizontalAlignment.Stretch; tempImage.VerticalAlignment = VerticalAlignment.Stretch;
                    RenderOptions.SetBitmapScalingMode(tempImage, BitmapScalingMode.NearestNeighbor);
                    tempImage.Tag = "[" + i.ToString() + "][" + j.ToString() + "]";
                    GameLevel currGameLevel = multiLevelMena.GetGameLevel(pTemp);
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
                    tempImage.Source = wbitmap;
                    tempImage.MouseDown += TempRect_MouseDown;
                    borderTemp.Tag = tempImage.Tag;
                    tempViewBox.Child = tempImage;
                    Grid.SetColumn(borderTemp, i);
                    Grid.SetRow(borderTemp, j);//(multiLevelMena.intCountPartsHeightY - 1) - j);
                    borderTemp.Child = tempViewBox;
                    gPlacePart.Children.Add(borderTemp);
                }
            }
        }
        private void TempRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image TempRec = (Image)sender;
            string fromTag = (string)TempRec.Tag;
            int intX = Convert.ToInt16(fromTag.Substring(1, fromTag.IndexOf("]") - 1));
            int intY = Convert.ToInt16(fromTag.Substring(fromTag.IndexOf("[",1) + 1, fromTag.Length - fromTag.IndexOf("[", 1) - 2));
            Point pCurr = new Point(intX, intY);
            //MessageBox.Show(fromTag);
            if (smCurrent == SelectionMode.Edit)
            {
                int iIndexOfThisPart = -1;
                for(int i = 1; i < tcMain.Items.Count; i++)
                {
                    TabItem tiTem = (TabItem)tcMain.Items[i];
                    if(tiTem.Tag.ToString() == fromTag)
                    {
                        iIndexOfThisPart = i;
                        break;
                    }
                }
                if(iIndexOfThisPart == -1)
                {
                    TabItem tiTemp = new TabItem();
                    tiTemp.Tag = fromTag;
                    StackPanel spNewHeader = new StackPanel();
                    spNewHeader.Orientation = Orientation.Horizontal;
                    TextBlock tb = new TextBlock();
                    tb.Text = "X: " + intX.ToString() + ", Y: " + intY.ToString();
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    spNewHeader.Children.Add(tb);
                    Button bCloseIt = new Button();
                    bCloseIt.Tag = fromTag;
                    bCloseIt.Content = "X";
                    bCloseIt.Margin = new Thickness(5, 0, 5, 0);
                    bCloseIt.Click += BCloseIt_Click;
                    //bCloseIt.MouseEnter += BCloseIt_MouseEnter; ;
                    spNewHeader.Children.Add(bCloseIt);
                    tiTemp.Header = spNewHeader;
                    EditLevelControl elcTemp = new EditLevelControl(intX, intY);
                    tiTemp.Content = elcTemp;
                    tcMain.Items.Add(tiTemp);
                    Dispatcher.BeginInvoke((Action)(() => tcMain.SelectedIndex = tcMain.Items.Count - 1));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => tcMain.SelectedIndex = iIndexOfThisPart));
                    //tcMain.SelectedIndex = iIndexOfThisPart;
                }
            }
            else if(smCurrent == SelectionMode.ActiveDeactive)
            {
                int index = intY * multiLevelMena.intCountPartsWidthX + intX;
                Border bTemp = (Border)gPlacePart.Children[index];
                Dictionary<Point, bool> temp = multiLevelMena.GetDirectiryListLevelToPlay();
                temp[pCurr] = !temp[pCurr];
                if (temp[pCurr])
                {
                    bTemp.BorderBrush = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    bTemp.BorderBrush = new SolidColorBrush(Colors.Red);
                }
               // gPlacePart.Children[index] = bTemp;
            }
        }

        private void BCloseIt_Click(object sender, RoutedEventArgs e)
        {
            Button TempRec = (Button)sender;
            string fromTag = (string)TempRec.Tag;
            int iIndexOfThisPart = 0;
            for (int i = 1; i < tcMain.Items.Count; i++)
            {
                TabItem tiTem = (TabItem)tcMain.Items[i];
                if (tiTem.Tag.ToString() == fromTag)
                {
                    iIndexOfThisPart = i;
                    break;
                }
            }
            if (iIndexOfThisPart > 0)
            {
                tcMain.Items.RemoveAt(iIndexOfThisPart);
            }
        }
        private void UpateSelectionModeOnGui()
        {
            SolidColorBrush scbWhite = new SolidColorBrush(Colors.White);
            SolidColorBrush scbLime = new SolidColorBrush(Colors.Lime);
            if(smCurrent == SelectionMode.Edit)
            {
                borderEdit.BorderBrush = scbLime;
                borderActivDeactive.BorderBrush = scbWhite;
            }
            else if (smCurrent == SelectionMode.ActiveDeactive)
            {
                borderEdit.BorderBrush = scbWhite;
                borderActivDeactive.BorderBrush = scbLime;
            }
            else
            {
                borderEdit.BorderBrush = scbWhite;
                borderActivDeactive.BorderBrush = scbWhite;
            }
        }
        private void CalcSingleWidth()
        {
            multiLevelMena.intCountPartsWidthX = multiLevelMena.intTotalLevelWidthX / multiLevelMena.byteSingleWidthX;
            multiLevelMena.byteLastWidthX = multiLevelMena.byteSingleWidthX;
            if (multiLevelMena.intTotalLevelWidthX % multiLevelMena.byteSingleWidthX > 0)
            {
                multiLevelMena.intCountPartsWidthX++;
                multiLevelMena.byteLastWidthX = (byte)(multiLevelMena.intTotalLevelWidthX % multiLevelMena.byteSingleWidthX);
            }
            tbCurrTotalLevelWidthX.Text = multiLevelMena.intTotalLevelWidthX.ToString();
            tbCurrSingleLevelWidthX.Text = multiLevelMena.byteSingleWidthX.ToString();
            tbCurrSingleLevelWidthXCount.Text = multiLevelMena.intCountPartsWidthX.ToString();
            tbCurrLastSingleWidthX.Text = multiLevelMena.byteLastWidthX.ToString();
        }
        private void CalcSingleHeight()
        {
            multiLevelMena.intCountPartsHeightY = multiLevelMena.intTotalLevelHeightY / multiLevelMena.byteSingleHeightY;
            multiLevelMena.byteLastHeightY = multiLevelMena.byteSingleHeightY;
            if (multiLevelMena.intTotalLevelHeightY % multiLevelMena.byteSingleHeightY > 0)
            {
                multiLevelMena.intCountPartsHeightY++;
                multiLevelMena.byteLastHeightY = (byte)(multiLevelMena.intTotalLevelHeightY % multiLevelMena.byteSingleHeightY);
            }
            tbCurrTotalLevelHeightY.Text = multiLevelMena.intTotalLevelHeightY.ToString();
            tbCurrSingleLevelHeightY.Text = multiLevelMena.byteSingleHeightY.ToString();
            tbCurrSingleLevelHeightYCount.Text = multiLevelMena.intCountPartsHeightY.ToString();
            tbCurrLastSingleHeightY.Text = multiLevelMena.byteLastHeightY.ToString();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            smCurrent = SelectionMode.Edit;
            UpateSelectionModeOnGui();
        }
        private void ButtonActivDeactive_Click(object sender, RoutedEventArgs e)
        {
            smCurrent = SelectionMode.ActiveDeactive;
            UpateSelectionModeOnGui();
        }
        private void bGenereteNewLevel_Click(object sender, RoutedEventArgs e)
        {
            if (iudTotalLevelWidthX.Value.HasValue && iudTotalLevelHeightY.Value.HasValue && budSingleLevelWidthXBasic.Value.HasValue && budSingleLevelHeightYBasic.Value.HasValue)
            {
                multiLevelMena.intTotalLevelWidthX = iudTotalLevelWidthX.Value.Value;
                multiLevelMena.intTotalLevelHeightY = iudTotalLevelHeightY.Value.Value;
                multiLevelMena.byteSingleWidthX = budSingleLevelWidthXBasic.Value.Value;
                multiLevelMena.byteSingleHeightY = budSingleLevelHeightYBasic.Value.Value;
                CalcSingleWidth();
                CalcSingleHeight();
                multiLevelMena.MakeNewDirectory();
                UpdatePartGrid();
            }
        }
        private void buttonGenerateView_Click(object sender, RoutedEventArgs e)
        {
            UpdatePartGrid();
        }
    }
}
