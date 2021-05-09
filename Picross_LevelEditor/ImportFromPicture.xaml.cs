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
using Microsoft.Win32;

namespace Picross_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy ImportFromPicture.xaml
    /// </summary>
    public partial class ImportFromPicture : Window
    {
        bool bAfterInit = false;
        bool bAfterLoad = true;
        public enum OpenImportMode { NoSelect, Direct, BigFromPdf};
        public ImportFromPicture(OpenImportMode oim)
        {
            InitializeComponent();
            bAfterInit = true;
            if(oim == OpenImportMode.BigFromPdf)
            {
                tcSelectedMode.SelectedIndex = 1;
            }
        }
        public GameLevel currentGameLevel = new GameLevel();
        public MultiLevelMenager currentMultiLevelMenager = new MultiLevelMenager();
        public bool boolIsSingle = true;
        GameLevel temporaryGameLevel = new GameLevel();
        MultiLevelMenager tempMLM = new MultiLevelMenager();
        public bool boolFinishOK = false;
        byte byteSelectedNeutralColor = 0;
        private void ButtonLoadPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialogForImage = new OpenFileDialog();
            openFileDialogForImage.DefaultExt = ".png";
            openFileDialogForImage.Filter = "PNG (*.png)|*.png|All files (*.*)|*.*";
            openFileDialogForImage.Multiselect = false;
            Nullable<bool> result = openFileDialogForImage.ShowDialog();
            if (result == true)
            {                
                if(radioButton_SingleMode.IsChecked.Value)
                {
                    bAfterLoad = false;
                    temporaryGameLevel = new GameLevel();
                    BitmapImage imageSource = new BitmapImage(new Uri(openFileDialogForImage.FileName));
                    string stringDefoultName = System.IO.Path.GetFileNameWithoutExtension(openFileDialogForImage.FileName);
                    temporaryGameLevel.stringEngName = stringDefoultName;
                    //temporaryGameLevel.dictonaryLevelNames.Add("EN", stringDefoultName);
                    tbDefoultName.Text = stringDefoultName;
                    temporaryGameLevel.intLevelWidthX = (byte)imageSource.PixelWidth;
                    tbWidth.Text = temporaryGameLevel.intLevelWidthX.ToString();
                    temporaryGameLevel.intLevelHeightY = (byte)imageSource.PixelHeight;
                    tbHeight.Text = temporaryGameLevel.intLevelHeightY.ToString();

                    gridOnImage.ColumnDefinitions.Clear();
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = new GridLength(20);
                    gridOnImage.ColumnDefinitions.Add(cd);

                    gridOnImage.RowDefinitions.Clear();
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength(20);
                    gridOnImage.RowDefinitions.Add(rd);

                    int stride = imageSource.PixelWidth * 4;
                    int size = imageSource.PixelHeight * stride;
                    byte[] pixels = new byte[size];
                    imageSource.CopyPixels(pixels, stride, 0);
                    int index;
                    for (int i = 0; i < imageSource.PixelWidth; i++)
                    {
                        for (int j = 0; j < imageSource.PixelHeight; j++)
                        {
                            index = j * stride + 4 * i;
                            SolidColorBrush scbSet = new SolidColorBrush(Color.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]));
                            OnlyRGB tempRGB = new OnlyRGB(scbSet.Color.R, scbSet.Color.G, scbSet.Color.B);
                            if (!temporaryGameLevel.listPossibleColorsOfTiles.Any(item => ((item.colR == scbSet.Color.R) && (item.colG == scbSet.Color.G) && (item.colB == scbSet.Color.B))))
                            {
                                temporaryGameLevel.listPossibleColorsOfTiles.Add(tempRGB);
                            }
                            else
                            {
                                tempRGB = temporaryGameLevel.listPossibleColorsOfTiles.First(item => ((item.colR == scbSet.Color.R) && (item.colG == scbSet.Color.G) && (item.colB == scbSet.Color.B)));
                            }
                            temporaryGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)i, (byte)(imageSource.PixelHeight - 1 - j), (byte)temporaryGameLevel.listPossibleColorsOfTiles.IndexOf(tempRGB)));
                        }
                    }

                    WriteableBitmap wbitmap = new WriteableBitmap(temporaryGameLevel.intLevelWidthX, temporaryGameLevel.intLevelHeightY, 96, 96, PixelFormats.Bgra32, null);
                    /// Tutaj toto jest: http://csharphelper.com/blog/2015/07/set-the-pixels-in-a-wpf-bitmap-in-c/
                    byte[] pixels1d = new byte[temporaryGameLevel.intLevelHeightY * temporaryGameLevel.intLevelWidthX * 4];
                    index = 0;
                    for (int row = 0; row < temporaryGameLevel.intLevelHeightY; row++)
                    {
                        for (int col = 0; col < temporaryGameLevel.intLevelWidthX; col++)
                        {
                            if (temporaryGameLevel.listLevelTilesOrder.Any(item => (item.tileLocationX == col && item.tileLocationY == temporaryGameLevel.intLevelHeightY - row - 1)))
                            {
                                byte byteResult = temporaryGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == col && item.tileLocationY == temporaryGameLevel.intLevelHeightY - row - 1)).colorId;
                                pixels1d[index++] = temporaryGameLevel.listPossibleColorsOfTiles[byteResult].colB;
                                pixels1d[index++] = temporaryGameLevel.listPossibleColorsOfTiles[byteResult].colG;
                                pixels1d[index++] = temporaryGameLevel.listPossibleColorsOfTiles[byteResult].colR;
                            }
                            else
                            {
                                pixels1d[index++] = temporaryGameLevel.colorTilesNeutral.colB;
                                pixels1d[index++] = temporaryGameLevel.colorTilesNeutral.colG;
                                pixels1d[index++] = temporaryGameLevel.colorTilesNeutral.colR;
                            }
                            pixels1d[index++] = 255;
                        }
                    }
                    Int32Rect rect = new Int32Rect(0, 0, temporaryGameLevel.intLevelWidthX, temporaryGameLevel.intLevelHeightY);
                    stride = 4 * temporaryGameLevel.intLevelWidthX;
                    wbitmap.WritePixels(rect, pixels1d, stride, 0);
                    Image tempImage = new Image();
                    tempImage.Stretch = Stretch.Uniform; tempImage.HorizontalAlignment = HorizontalAlignment.Stretch; tempImage.VerticalAlignment = VerticalAlignment.Stretch;
                    RenderOptions.SetBitmapScalingMode(tempImage, BitmapScalingMode.NearestNeighbor);
                    tempImage.Source = wbitmap;
                    gridOnImage.Children.Clear();
                    gridOnImage.Children.Add(tempImage);
                    tbCountColor.Text = temporaryGameLevel.listPossibleColorsOfTiles.Count.ToString();
                    for (int i = 0; i < temporaryGameLevel.listPossibleColorsOfTiles.Count; i++)
                    {
                        gridListOfColors.RowDefinitions.Add(new RowDefinition());
                        RadioButton rbTemp = new RadioButton();
                        if (i == 0)
                        {
                            byteSelectedNeutralColor = 0;
                            rbTemp.IsChecked = true;
                        }
                        rbTemp.Tag = i; Grid.SetColumn(rbTemp, 1); Grid.SetRow(rbTemp, i);
                        rbTemp.Content = "Neutral"; rbTemp.VerticalAlignment = VerticalAlignment.Center; rbTemp.GroupName = "IsNeutral";
                        rbTemp.Checked += RbTemp_Checked;
                        Xceed.Wpf.Toolkit.ColorPicker cpTemp = new Xceed.Wpf.Toolkit.ColorPicker();
                        cpTemp.Tag = i; Grid.SetColumn(cpTemp, 0); Grid.SetRow(cpTemp, i);
                        cpTemp.SelectedColorChanged += CpTemp_SelectedColorChanged;
                        cpTemp.DisplayColorAndName = true; cpTemp.SelectedColor = Color.FromRgb(temporaryGameLevel.listPossibleColorsOfTiles[i].colR, temporaryGameLevel.listPossibleColorsOfTiles[i].colG, temporaryGameLevel.listPossibleColorsOfTiles[i].colB);
                        gridListOfColors.Children.Add(cpTemp);
                        gridListOfColors.Children.Add(rbTemp);
                    }
                    bAfterLoad = true;
                }
                else
                {
                    if(byteUpDown_MultiBasicWidth.Value.HasValue && byteUpDown_MultiBasicHeight.Value.HasValue)
                    {                        
                        bAfterLoad = false;
                        tempMLM = new MultiLevelMenager();
                        temporaryGameLevel = new GameLevel();
                        BitmapImage imageSource = new BitmapImage(new Uri(openFileDialogForImage.FileName));
                        string stringDefoultName = System.IO.Path.GetFileNameWithoutExtension(openFileDialogForImage.FileName);
                        tempMLM.stringAllLevelsEngName = stringDefoultName;
                        tbDefoultName.Text = stringDefoultName;
                        tempMLM.intTotalLevelWidthX = (byte)imageSource.PixelWidth;
                        tbWidth.Text = tempMLM.intTotalLevelWidthX.ToString();
                        tempMLM.intTotalLevelHeightY = (byte)imageSource.PixelHeight;
                        tbHeight.Text = tempMLM.intTotalLevelHeightY.ToString();
                        CalcBasic();
                        int stride = imageSource.PixelWidth * 4;
                        int size = imageSource.PixelHeight * stride;
                        byte[] pixels = new byte[size];
                        imageSource.CopyPixels(pixels, stride, 0);
                        int index;
                        for (int i = 0; i < imageSource.PixelWidth; i++)
                        {
                            for (int j = 0; j < imageSource.PixelHeight; j++)
                            {
                                index = j * stride + 4 * i;
                                SolidColorBrush scbSet = new SolidColorBrush(Color.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]));
                                OnlyRGB tempRGB = new OnlyRGB(scbSet.Color.R, scbSet.Color.G, scbSet.Color.B);
                                if (!temporaryGameLevel.listPossibleColorsOfTiles.Any(item => ((item.colR == scbSet.Color.R) && (item.colG == scbSet.Color.G) && (item.colB == scbSet.Color.B))))
                                {
                                    temporaryGameLevel.listPossibleColorsOfTiles.Add(tempRGB);
                                }
                                else
                                {
                                    tempRGB = temporaryGameLevel.listPossibleColorsOfTiles.First(item => ((item.colR == scbSet.Color.R) && (item.colG == scbSet.Color.G) && (item.colB == scbSet.Color.B)));
                                }
                                temporaryGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)i, (byte)(imageSource.PixelHeight - 1 - j), (byte)temporaryGameLevel.listPossibleColorsOfTiles.IndexOf(tempRGB)));
                            }
                        }
                        CalcAdvence();
                        bAfterLoad = true;
                    }
                    else
                    {
                        MessageBox.Show("Some input inccorrect!");
                    }
                }
            }
        }
        private void CalcBasic()
        {
            tempMLM.byteSingleWidthX = byteUpDown_MultiBasicWidth.Value.Value;
            tempMLM.byteSingleHeightY = byteUpDown_MultiBasicHeight.Value.Value;
            tempMLM.intCountPartsWidthX = tempMLM.intTotalLevelWidthX / tempMLM.byteSingleWidthX;
            tempMLM.byteLastWidthX = tempMLM.byteSingleWidthX;
            if (tempMLM.intTotalLevelWidthX % tempMLM.byteSingleWidthX > 0)
            {
                tempMLM.intCountPartsWidthX++;
                tempMLM.byteLastWidthX = (byte)(tempMLM.intTotalLevelWidthX % tempMLM.byteSingleWidthX);
            }
            tbCurrTotalLevelWidthX.Text = tempMLM.intTotalLevelWidthX.ToString();
            tbCurrSingleLevelWidthX.Text = tempMLM.byteSingleWidthX.ToString();
            tbCurrSingleLevelWidthXCount.Text = tempMLM.intCountPartsWidthX.ToString();
            tbCurrLastSingleWidthX.Text = tempMLM.byteLastWidthX.ToString();
            tempMLM.intCountPartsHeightY = tempMLM.intTotalLevelHeightY / tempMLM.byteSingleHeightY;
            tempMLM.byteLastHeightY = tempMLM.byteSingleHeightY;
            if (tempMLM.intTotalLevelHeightY % tempMLM.byteSingleHeightY > 0)
            {
                tempMLM.intCountPartsHeightY++;
                tempMLM.byteLastHeightY = (byte)(tempMLM.intTotalLevelHeightY % tempMLM.byteSingleHeightY);
            }
            tbCurrTotalLevelHeightY.Text = tempMLM.intTotalLevelHeightY.ToString();
            tbCurrSingleLevelHeightY.Text = tempMLM.byteSingleHeightY.ToString();
            tbCurrSingleLevelHeightYCount.Text = tempMLM.intCountPartsHeightY.ToString();
            tbCurrLastSingleHeightY.Text = tempMLM.byteLastHeightY.ToString();
            gridOnImage.ColumnDefinitions.Clear();
            for (int i = 0; i < tempMLM.intCountPartsWidthX; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                if (i < tempMLM.intCountPartsWidthX - 1)
                {
                    cd.Width = new GridLength(10 * tempMLM.byteSingleWidthX);
                }
                else
                {
                    cd.Width = new GridLength(10 * tempMLM.byteLastWidthX);
                }
                gridOnImage.ColumnDefinitions.Add(cd);
            }
            gridOnImage.RowDefinitions.Clear();
            for (int i = 0; i < tempMLM.intCountPartsHeightY; i++)
            {
                RowDefinition rd = new RowDefinition();
                if (i > 0)
                {
                    rd.Height = new GridLength(10 * tempMLM.byteSingleHeightY);
                }
                else
                {
                    rd.Height = new GridLength(10 * tempMLM.byteLastHeightY);
                }
                gridOnImage.RowDefinitions.Add(rd);
            }
        }
        private void CalcAdvence()
        {
            tempMLM.MakeNewDirectory();
            int index;
            int stride;
            gridOnImage.Children.Clear();
            byte tempSingleWidth, tempSingleHeight;
            int startPixelX, startPixelY;
            for (int i = 0; i < tempMLM.intCountPartsWidthX; i++)
            {
                for (int j = 0; j < tempMLM.intCountPartsHeightY; j++)
                {
                    if (i < tempMLM.intCountPartsWidthX - 1)
                    {
                        tempSingleWidth = tempMLM.byteSingleWidthX;
                    }
                    else
                    {
                        tempSingleWidth = tempMLM.byteLastWidthX;
                    }
                    if (j > 0)
                    {
                        tempSingleHeight = tempMLM.byteSingleHeightY;
                    }
                    else
                    {
                        tempSingleHeight = tempMLM.byteLastHeightY;
                    }
                    WriteableBitmap wbitmap = new WriteableBitmap(tempSingleWidth, tempSingleHeight, 96, 96, PixelFormats.Bgra32, null);
                    /// Tutaj toto jest: http://csharphelper.com/blog/2015/07/set-the-pixels-in-a-wpf-bitmap-in-c/
                    byte[] pixels1d = new byte[tempSingleWidth * tempSingleHeight * 4];
                    startPixelX = i * tempMLM.byteSingleWidthX;
                    if (j > 0)
                    {
                        startPixelY = tempMLM.byteLastHeightY + (j - 1) * tempMLM.byteSingleHeightY;
                    }
                    else
                    {
                        startPixelY = 0;
                    }
                    index = 0;
                    int pixelIndexX, pixelIndexY;
                    GameLevel partGameLevel;
                    partGameLevel = new GameLevel();
                    partGameLevel.listPossibleColorsOfTiles = temporaryGameLevel.listPossibleColorsOfTiles;
                    partGameLevel.intLevelWidthX = tempSingleWidth;
                    partGameLevel.intLevelHeightY = tempSingleHeight;
                    for (int row = 0; row < tempSingleHeight; row++)
                    {
                        for (int col = 0; col < tempSingleWidth; col++)
                        {
                            
                            pixelIndexX = startPixelX + col;
                            pixelIndexY = tempMLM.intTotalLevelHeightY - (row + startPixelY) - 1;
                            
                            byte byteResult = temporaryGameLevel.listLevelTilesOrder.Find(item => (item.tileLocationX == pixelIndexX && item.tileLocationY == pixelIndexY)).colorId;
                            partGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)col, (byte)(partGameLevel.intLevelHeightY - 1 - row), byteResult));
                            pixels1d[index++] = temporaryGameLevel.listPossibleColorsOfTiles[byteResult].colB;
                            pixels1d[index++] = temporaryGameLevel.listPossibleColorsOfTiles[byteResult].colG;
                            pixels1d[index++] = temporaryGameLevel.listPossibleColorsOfTiles[byteResult].colR;
                            pixels1d[index++] = 255;
                        }
                    }
                    tempMLM.SetGameLevel(new Point(i, j), partGameLevel);
                    Int32Rect rect = new Int32Rect(0, 0, tempSingleWidth, tempSingleHeight);
                    stride = 4 * tempSingleWidth;
                    wbitmap.WritePixels(rect, pixels1d, stride, 0);
                    Image tempImage = new Image();
                    tempImage.Stretch = Stretch.Uniform; tempImage.HorizontalAlignment = HorizontalAlignment.Stretch; tempImage.VerticalAlignment = VerticalAlignment.Stretch;
                    RenderOptions.SetBitmapScalingMode(tempImage, BitmapScalingMode.NearestNeighbor);
                    tempImage.Source = wbitmap;
                    Grid.SetColumn(tempImage, i);
                    Grid.SetRow(tempImage, j);
                    gridOnImage.Children.Add(tempImage);
                }
            }
            tbCountColor.Text = temporaryGameLevel.listPossibleColorsOfTiles.Count.ToString();
            for (int i = 0; i < temporaryGameLevel.listPossibleColorsOfTiles.Count; i++)
            {
                gridListOfColors.RowDefinitions.Add(new RowDefinition());
                RadioButton rbTemp = new RadioButton();
                if (i == 0)
                {
                    byteSelectedNeutralColor = 0;
                    rbTemp.IsChecked = true;
                }
                rbTemp.Tag = i; Grid.SetColumn(rbTemp, 1); Grid.SetRow(rbTemp, i);
                rbTemp.Content = "Neutral"; rbTemp.VerticalAlignment = VerticalAlignment.Center; rbTemp.GroupName = "IsNeutral";
                rbTemp.Checked += RbTemp_Checked;
                Xceed.Wpf.Toolkit.ColorPicker cpTemp = new Xceed.Wpf.Toolkit.ColorPicker();
                cpTemp.Tag = i; Grid.SetColumn(cpTemp, 0); Grid.SetRow(cpTemp, i);
                cpTemp.SelectedColorChanged += CpTemp_SelectedColorChanged;
                cpTemp.DisplayColorAndName = true; cpTemp.SelectedColor = Color.FromRgb(temporaryGameLevel.listPossibleColorsOfTiles[i].colR, temporaryGameLevel.listPossibleColorsOfTiles[i].colG, temporaryGameLevel.listPossibleColorsOfTiles[i].colB);
                gridListOfColors.Children.Add(cpTemp);
                gridListOfColors.Children.Add(rbTemp);
            }
            tempMLM.FillInfoAboutToPlay();
        }
        private void RbTemp_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rbTemp = (RadioButton)sender;
            byteSelectedNeutralColor = Convert.ToByte(rbTemp.Tag);
        }

        private void CpTemp_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if(bAfterLoad)
            {
                Xceed.Wpf.Toolkit.ColorPicker cpTemp = (Xceed.Wpf.Toolkit.ColorPicker)sender;
                byte byteColorId = Convert.ToByte(cpTemp.Tag.ToString());
                temporaryGameLevel.listPossibleColorsOfTiles[byteColorId] = new OnlyRGB(cpTemp.SelectedColor.Value.R, cpTemp.SelectedColor.Value.G, cpTemp.SelectedColor.Value.B);
                for (int i = 0; i < temporaryGameLevel.listLevelTilesOrder.Count; i++)
                {
                    SolidColorBrush scbSet = new SolidColorBrush(Color.FromRgb(temporaryGameLevel.listPossibleColorsOfTiles[(int)temporaryGameLevel.listLevelTilesOrder[i].colorId].colR, temporaryGameLevel.listPossibleColorsOfTiles[(int)temporaryGameLevel.listLevelTilesOrder[i].colorId].colG, temporaryGameLevel.listPossibleColorsOfTiles[(int)temporaryGameLevel.listLevelTilesOrder[i].colorId].colB));
                    Rectangle imageTemp = new Rectangle();
                    imageTemp.Fill = scbSet;
                    Grid.SetColumn(imageTemp, (int)temporaryGameLevel.listLevelTilesOrder[i].tileLocationX);
                    Grid.SetRow(imageTemp, temporaryGameLevel.intLevelHeightY - 1 - (int)temporaryGameLevel.listLevelTilesOrder[i].tileLocationY);
                    gridOnImage.Children.Add(imageTemp);
                }
            }
        }           
        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            if(radioButton_SingleMode.IsChecked.Value)
            {
                //currentGameLevel.dictonaryLevelNames = temporaryGameLevel.dictonaryLevelNames;
                currentGameLevel.stringEngName = temporaryGameLevel.stringEngName;
                currentGameLevel.intLevelWidthX = temporaryGameLevel.intLevelWidthX;
                currentGameLevel.intLevelHeightY = temporaryGameLevel.intLevelHeightY;
                for (int i = 0; i < temporaryGameLevel.listPossibleColorsOfTiles.Count; i++)
                {
                    if (byteSelectedNeutralColor == i)
                    {
                        currentGameLevel.colorTilesNeutral = temporaryGameLevel.listPossibleColorsOfTiles[i];
                    }
                    else
                    {
                        currentGameLevel.listPossibleColorsOfTiles.Add(temporaryGameLevel.listPossibleColorsOfTiles[i]);
                    }
                }
                for (int i = 0; i < temporaryGameLevel.listLevelTilesOrder.Count; i++)
                {
                    if (temporaryGameLevel.listLevelTilesOrder[i].colorId < byteSelectedNeutralColor)
                    {
                        currentGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder(temporaryGameLevel.listLevelTilesOrder[i].tileLocationX, temporaryGameLevel.listLevelTilesOrder[i].tileLocationY, temporaryGameLevel.listLevelTilesOrder[i].colorId));
                    }
                    else if (temporaryGameLevel.listLevelTilesOrder[i].colorId > byteSelectedNeutralColor)
                    {
                        currentGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder(temporaryGameLevel.listLevelTilesOrder[i].tileLocationX, temporaryGameLevel.listLevelTilesOrder[i].tileLocationY, (byte)(temporaryGameLevel.listLevelTilesOrder[i].colorId - 1)));
                    }
                }
                boolFinishOK = true;
                boolIsSingle = true;
                this.Close();
            }
            else
            {
                currentMultiLevelMenager.stringAllLevelsEngName = tempMLM.stringAllLevelsEngName;
                currentMultiLevelMenager.byteLastHeightY = tempMLM.byteLastHeightY;
                currentMultiLevelMenager.byteLastWidthX = tempMLM.byteLastWidthX;
                currentMultiLevelMenager.byteSingleHeightY = tempMLM.byteSingleHeightY;
                currentMultiLevelMenager.byteSingleWidthX = tempMLM.byteSingleWidthX;
                currentMultiLevelMenager.intCountPartsHeightY = tempMLM.intCountPartsHeightY;
                currentMultiLevelMenager.intCountPartsWidthX = tempMLM.intCountPartsWidthX;
                currentMultiLevelMenager.intTotalLevelHeightY = tempMLM.intTotalLevelHeightY;
                currentMultiLevelMenager.intTotalLevelWidthX = tempMLM.intTotalLevelWidthX;
                currentMultiLevelMenager.SetDirectoryListGameLevel(tempMLM.GetDirectoryListGameLevel());
                var tttttt = tempMLM.GetDirectiryListLevelToPlay();
                currentMultiLevelMenager.SetDirectiryListLevelToPlay(tttttt);
                var oooo = tempMLM.GetDirectoryListGameLevel();
                currentMultiLevelMenager.SetDirectoryListGameLevel(oooo);
                currentMultiLevelMenager.FillInfoAboutToPlay();
                boolFinishOK = true;
                boolIsSingle = false;
                this.Close();
            }
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbAllFeatures_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if(bAfterInit)
            {
                gridOnImage.ShowGridLines = cbShowGridLines.IsChecked.Value;
            }            
        }

        private void ButtonLoadPictureBig_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialogForImage = new OpenFileDialog();
            openFileDialogForImage.DefaultExt = ".png";
            openFileDialogForImage.Filter = "PNG (*.png)|*.png|All files (*.*)|*.*";
            openFileDialogForImage.Multiselect = false;
            Nullable<bool> result = openFileDialogForImage.ShowDialog();
            if (result == true)
            {
                BitmapImage imageSource = new BitmapImage(new Uri(openFileDialogForImage.FileName));
                iLoadBigFromPdf.Source = imageSource;
                wpConvertion.IsEnabled = true;
                string stringDefoultName = System.IO.Path.GetFileNameWithoutExtension(openFileDialogForImage.FileName);
                temporaryGameLevel.stringEngName = stringDefoultName;
                //temporaryGameLevel.dictonaryLevelNames.Add("EN", stringDefoultName);
                tbFileNameBigFromPdf.Text = stringDefoultName;
                tbWidthBigFromPdf.Text = imageSource.PixelWidth.ToString();
                tbHeightBigFromPdf.Text = imageSource.PixelHeight.ToString();
            }
        }
        private void ButtonConvertLevelData_Click(object sender, RoutedEventArgs e)
        {
            temporaryGameLevel = new GameLevel();
            gridListOfColorsBigFromPdf.Children.Clear();
            gridListOfColorsBigFromPdf.RowDefinitions.Clear();
            gridOnImageBig.Children.Clear();
            gridOnImageBig.ColumnDefinitions.Clear();
            gridOnImageBig.RowDefinitions.Clear();
            if (budPictureWidthX.Value.HasValue && budPictureHeightY.Value.HasValue)
            {
                bAfterLoad = false;
                byte levelX = budPictureWidthX.Value.Value;
                temporaryGameLevel.intLevelWidthX = levelX;
                byte levelY = budPictureHeightY.Value.Value;
                temporaryGameLevel.intLevelHeightY = levelY;                
                for (byte i = 0; i < levelX; i++)
                {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = new GridLength(20);
                    gridOnImageBig.ColumnDefinitions.Add(cd);
                }                
                for (byte i = 0; i < levelY; i++)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength(20);
                    gridOnImageBig.RowDefinitions.Add(rd);
                }
                BitmapImage imageSource = (BitmapImage)iLoadBigFromPdf.Source;
                double offsetX = ((double)(imageSource.PixelWidth)) / ((double)levelX);
                double offsetY = ((double)(imageSource.PixelHeight)) / ((double)levelY);
                int stride = imageSource.PixelWidth * 4;
                int size = imageSource.PixelHeight * stride;
                byte[] pixels = new byte[size];
                imageSource.CopyPixels(pixels, stride, 0);
                for (int i = 0; i < levelX; i++)
                {
                    for (int j = 0; j < levelY; j++)
                    {
                        int index = ((int)(offsetY * (j+0.5))) * stride + 4 * ((int)(offsetX * (i+0.5)));
                        SolidColorBrush scbSet = new SolidColorBrush(Color.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]));
                        OnlyRGB tempRGB = new OnlyRGB(scbSet.Color.R, scbSet.Color.G, scbSet.Color.B);
                        if (!temporaryGameLevel.listPossibleColorsOfTiles.Any(item => ((item.colR == scbSet.Color.R) && (item.colG == scbSet.Color.G) && (item.colB == scbSet.Color.B))))
                        {
                            temporaryGameLevel.listPossibleColorsOfTiles.Add(tempRGB);
                        }
                        else
                        {
                            tempRGB = temporaryGameLevel.listPossibleColorsOfTiles.First(item => ((item.colR == scbSet.Color.R) && (item.colG == scbSet.Color.G) && (item.colB == scbSet.Color.B)));
                        }
                        temporaryGameLevel.listLevelTilesOrder.Add(new LevelTilesOrder((byte)i, (byte)(levelY - 1 - j), (byte)temporaryGameLevel.listPossibleColorsOfTiles.IndexOf(tempRGB)));
                        Rectangle imageTemp = new Rectangle();
                        imageTemp.Fill = scbSet;
                        Grid.SetColumn(imageTemp, i);
                        Grid.SetRow(imageTemp, j);
                        gridOnImageBig.Children.Add(imageTemp);
                    }
                }
                for (int i = 0; i < temporaryGameLevel.listPossibleColorsOfTiles.Count; i++)
                {
                    gridListOfColorsBigFromPdf.RowDefinitions.Add(new RowDefinition());
                    RadioButton rbTemp = new RadioButton();
                    if (i == 0)
                    {
                        byteSelectedNeutralColor = 0;
                        rbTemp.IsChecked = true;
                    }
                    rbTemp.Tag = i; Grid.SetColumn(rbTemp, 1); Grid.SetRow(rbTemp, i);
                    rbTemp.Content = "Neutral"; rbTemp.VerticalAlignment = VerticalAlignment.Center; rbTemp.GroupName = "IsNeutral";
                    rbTemp.Checked += RbTemp_Checked;
                    Xceed.Wpf.Toolkit.ColorPicker cpTemp = new Xceed.Wpf.Toolkit.ColorPicker();
                    cpTemp.Tag = i; Grid.SetColumn(cpTemp, 0); Grid.SetRow(cpTemp, i);
                    cpTemp.SelectedColorChanged += CpTemp_SelectedColorChanged;
                    cpTemp.DisplayColorAndName = true; cpTemp.SelectedColor = Color.FromRgb(temporaryGameLevel.listPossibleColorsOfTiles[i].colR, temporaryGameLevel.listPossibleColorsOfTiles[i].colG, temporaryGameLevel.listPossibleColorsOfTiles[i].colB);
                    gridListOfColorsBigFromPdf.Children.Add(cpTemp);
                    gridListOfColorsBigFromPdf.Children.Add(rbTemp);
                }
                bAfterLoad = true;
            }
            else
            {
                MessageBox.Show("Wron data input");
            }
        }
        private void button_ConvertMultiLevel_Click(object sender, RoutedEventArgs e)
        {
            bAfterLoad = false;
            CalcBasic();
            CalcAdvence();
            bAfterLoad = true;
        }
    }
}
