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
using Picross_LevelEditor.Languages;
using System.IO;
using Picross_LevelEditor.Properties;

namespace Picross_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy ExportToGraphicFiles.xaml
    /// </summary>
    public partial class ExportToGraphicFiles : Window
    {
        public GameLevel currGameLevel = new GameLevel();
        public ExportToGraphicFiles(GameLevel gameLevel, string sProjectName)
        {
            InitializeComponent();
            currGameLevel = gameLevel;
            cbAddLevelSizeToName.Content = "Add size of lavel to name: [File name]__" + currGameLevel.intLevelWidthX + "x" + currGameLevel.intLevelHeightY +".*";
            tbFilesName.Text = sProjectName;
            tbFolderPath.Text = Settings.Default.sExportGraphPath;
            cbAddLevelSizeToName.IsChecked = Settings.Default.bExportGraphAddSizeToName;
            cbUsePng.IsChecked = Settings.Default.bExportGraphUsePng;
            cbPngUseInterlace.IsChecked = Settings.Default.bExportGraphPngInter;
            cbUseBmp.IsChecked = Settings.Default.bExportGraphUseBmp;
            cbUseTif.IsChecked = Settings.Default.bExportGraphUseTif;
            cbTifInZip.IsChecked = Settings.Default.bExportGraphTifWithZip;
            cbUseGif.IsChecked = Settings.Default.bExportGraphUseGif;
            cbUseJpg.IsChecked = Settings.Default.bExportGraphUseJpg;
            iudJpgQualityLevel.Value = Settings.Default.byteExportGraphJpgQuality;
            cbUseWpg.IsChecked = Settings.Default.bExportGraphUseWpg;
            iudWpgQualityLevel.Value = Settings.Default.byteExportGraphWpgQuality;
            iudOutputFilesScale.Value = Settings.Default.iExportGraphOutputScale;
        }
        private void BFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog winFolder = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if ((bool)winFolder.ShowDialog())
            {
                tbFolderPath.Text = winFolder.SelectedPath;
            }
        }
        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            bool bSaveCorrect = false;
            string sName = tbFilesName.Text;
            string sPath = tbFolderPath.Text;
            if (sName == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Level name cannot be empty", "Empty level name", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (sPath == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Path cannot be empty", "Empty path", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!Directory.Exists(sPath))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Inputed path not exist on PC", "Wrong path", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if(!iudOutputFilesScale.Value.HasValue)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("No value in scale of output file. Please specifie correct value", "No value of scale", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                List<SupportFilesSave> lSupFilsSave = new List<SupportFilesSave>();
                if(cbUsePng.IsChecked.Value)
                {
                    lSupFilsSave.Add(SupportFilesSave.PNG);
                }
                if(cbUseBmp.IsChecked.Value)
                {
                    lSupFilsSave.Add(SupportFilesSave.BMP);
                }
                if (cbUseTif.IsChecked.Value)
                {
                    lSupFilsSave.Add(SupportFilesSave.TIFF);
                }
                if (cbUseGif.IsChecked.Value)
                {
                    lSupFilsSave.Add(SupportFilesSave.GIF);
                }
                if (cbUseJpg.IsChecked.Value)
                {
                    lSupFilsSave.Add(SupportFilesSave.JEPG);
                }
                if (cbUseWpg.IsChecked.Value)
                {
                    lSupFilsSave.Add(SupportFilesSave.WPG);
                }
                if(lSupFilsSave.Count == 0)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("No anny file to save was selected, please select minimum one file type", "Wrong selection file type", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if(lSupFilsSave.Contains(SupportFilesSave.JEPG) && !iudJpgQualityLevel.Value.HasValue)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Quality level of JPEG is empty. Please set correct value.", "Wrong value of JPEG quality level", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (lSupFilsSave.Contains(SupportFilesSave.WPG) && !iudWpgQualityLevel.Value.HasValue)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Quality level of WPG is empty. Please set correct value.", "Wrong value of WPG quality level", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    List<string> lsFullNamesOfFile = new List<string>();
                    if (cbAddLevelSizeToName.IsChecked.Value)
                    {
                        sName += "_" + currGameLevel.intLevelWidthX.ToString() + "x" + currGameLevel.intLevelHeightY.ToString();
                    }
                    for(int i = 0; i < lSupFilsSave.Count; i++)
                    {
                        switch(lSupFilsSave[i])
                        {
                            case SupportFilesSave.PNG:
                                lsFullNamesOfFile.Add(System.IO.Path.Combine(sPath, sName + ".png"));
                                break;
                            case SupportFilesSave.BMP:
                                lsFullNamesOfFile.Add(System.IO.Path.Combine(sPath, sName + ".bmp"));
                                break;
                            case SupportFilesSave.TIFF:
                                lsFullNamesOfFile.Add(System.IO.Path.Combine(sPath, sName + ".tif"));
                                break;
                            case SupportFilesSave.GIF:
                                lsFullNamesOfFile.Add(System.IO.Path.Combine(sPath, sName + ".gif"));
                                break;
                            case SupportFilesSave.JEPG:
                                lsFullNamesOfFile.Add(System.IO.Path.Combine(sPath, sName + ".jpg"));
                                break;
                            case SupportFilesSave.WPG:
                                lsFullNamesOfFile.Add(System.IO.Path.Combine(sPath, sName + ".wdp"));
                                break;
                            default:
                                lsFullNamesOfFile.Add(System.IO.Path.Combine(sPath, sName));
                                break;
                        }
                        
                    }
                    
                    List<string> lsOfExistingFiles = new List<string>();
                    for(int index = 0; index < lsFullNamesOfFile.Count; index++)
                    {
                        if(File.Exists(lsFullNamesOfFile[index]))
                        {
                            lsOfExistingFiles.Add(lsFullNamesOfFile[index]);
                        }
                    }
                    bool bExecuteSave = true;
                    if (lsOfExistingFiles.Count > 0 && lsOfExistingFiles.Count != lsFullNamesOfFile.Count)
                    {
                        string sMessage = "Some files exist on PC. Number of files [" + lsOfExistingFiles.Count.ToString() + "], list of files:\n";
                        for (int index = 0; index < lsOfExistingFiles.Count; index++)
                        {
                            sMessage += lsOfExistingFiles[index] + "\n";
                        }
                        sMessage += "\nDid you want overwite this files?\n[Yes] - All fille will be saved\n[No] - Only no exist will be saved\n[Cancel] - Any files will be saved";
                        MessageBoxResult ans = Xceed.Wpf.Toolkit.MessageBox.Show(sMessage, "Files exist on PC", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if(ans == MessageBoxResult.No)
                        {
                            for (int index = 0; index < lsOfExistingFiles.Count; index++)
                            {
                                lSupFilsSave.RemoveAt(lsFullNamesOfFile.IndexOf(lsOfExistingFiles[index]));                                
                                lsFullNamesOfFile.Remove(lsOfExistingFiles[index]);
                            }
                        }
                        else if(ans == MessageBoxResult.Cancel)
                        {
                            bExecuteSave = false;
                        }
                    }
                    else if(lsOfExistingFiles.Count > 0 && lsOfExistingFiles.Count == lsFullNamesOfFile.Count)
                    {
                        string sMessage = "All files exist on PC.\nDid you want overwite this files?";
                        MessageBoxResult ans = Xceed.Wpf.Toolkit.MessageBox.Show(sMessage, "Files exist on PC", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (ans == MessageBoxResult.No)
                        {
                            bExecuteSave = false;
                        }
                    }
                    if (bExecuteSave)
                    {
                        int scaleValue = iudOutputFilesScale.Value.Value;
                        WriteableBitmap wbitmap = new WriteableBitmap(scaleValue * (int)currGameLevel.intLevelWidthX, scaleValue * (int)currGameLevel.intLevelHeightY, 96, 96, PixelFormats.Bgra32, null);
                        /// Tutaj toto jest: http://csharphelper.com/blog/2015/07/set-the-pixels-in-a-wpf-bitmap-in-c/
                        byte[] pixels1d = new byte[((int)(currGameLevel.intLevelHeightY * currGameLevel.intLevelWidthX * 4)) * scaleValue * scaleValue];
                        int index = 0;
                        for (int row = 0; row < currGameLevel.intLevelHeightY; row++)
                        {
                            for (int yscale = 0; yscale < scaleValue; yscale++)
                            {
                                for (int col = 0; col < currGameLevel.intLevelWidthX; col++)
                                {
                                    for (int xscale = 0; xscale < scaleValue; xscale++)
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
                            }
                        }
                        Int32Rect rect = new Int32Rect(0, 0, scaleValue * (int)currGameLevel.intLevelWidthX, scaleValue * (int)currGameLevel.intLevelHeightY);
                        int stride = scaleValue * ((int)(4 * currGameLevel.intLevelWidthX));
                        wbitmap.WritePixels(rect, pixels1d, stride, 0);
                        for (int i = 0; i < lSupFilsSave.Count; i++)
                        {
                            using (FileStream stream5 = new FileStream(lsFullNamesOfFile[i], FileMode.Create))
                            {
                                switch (lSupFilsSave[i])
                                {
                                    case SupportFilesSave.PNG:
                                        PngBitmapEncoder encoderPng = new PngBitmapEncoder();
                                        encoderPng.Interlace = PngInterlaceOption.On;
                                        encoderPng.Frames.Add(BitmapFrame.Create(wbitmap));
                                        encoderPng.Save(stream5);
                                        break;
                                    case SupportFilesSave.BMP:
                                        BmpBitmapEncoder encoderBMP = new BmpBitmapEncoder();
                                        encoderBMP.Frames.Add(BitmapFrame.Create(wbitmap));
                                        encoderBMP.Save(stream5);
                                        break;
                                    case SupportFilesSave.TIFF:
                                        TiffBitmapEncoder encoderTif = new TiffBitmapEncoder();
                                        if (cbTifInZip.IsChecked.Value)
                                        {
                                            encoderTif.Compression = TiffCompressOption.Zip;
                                        }
                                        encoderTif.Frames.Add(BitmapFrame.Create(wbitmap));
                                        encoderTif.Save(stream5);
                                        break;
                                    case SupportFilesSave.GIF:
                                        GifBitmapEncoder encoderGIF = new GifBitmapEncoder();
                                        encoderGIF.Frames.Add(BitmapFrame.Create(wbitmap));
                                        encoderGIF.Save(stream5);
                                        break;
                                    case SupportFilesSave.JEPG:
                                        JpegBitmapEncoder encoderJpeg = new JpegBitmapEncoder();
                                        encoderJpeg.QualityLevel = iudJpgQualityLevel.Value.Value;
                                        encoderJpeg.Frames.Add(BitmapFrame.Create(wbitmap));
                                        encoderJpeg.Save(stream5);
                                        break;
                                    case SupportFilesSave.WPG:
                                        WmpBitmapEncoder encoderWpg = new WmpBitmapEncoder();
                                        encoderWpg.QualityLevel = iudWpgQualityLevel.Value.Value;
                                        encoderWpg.Frames.Add(BitmapFrame.Create(wbitmap));
                                        encoderWpg.Save(stream5);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        bSaveCorrect = true;
                    }
                    
                }
            }
            if(bSaveCorrect)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Save of graphic was done correct", "Conifrmation after save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        enum SupportFilesSave {NoSelect, PNG, BMP, TIFF, GIF, JEPG, WPG};
        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void CbUsePng_ChangeCheck(object sender, RoutedEventArgs e)
        {
            cbPngUseInterlace.IsEnabled = cbUsePng.IsChecked.Value;
        }
        private void CbUseTif_ChangeCheck(object sender, RoutedEventArgs e)
        {
            cbTifInZip.IsEnabled = cbUseTif.IsChecked.Value;
        }
        private void CbUseJpg_ChangeCheck(object sender, RoutedEventArgs e)
        {
            iudJpgQualityLevel.IsEnabled = cbUseJpg.IsChecked.Value;
        }
        private void CbUseWpg_ChangeCheck(object sender, RoutedEventArgs e)
        {
            iudWpgQualityLevel.IsEnabled = cbUseWpg.IsChecked.Value;
        }
        private void IudOutputFilesScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(iudOutputFilesScale.Value.HasValue && tbOutputFilesSize != null)
            {
                tbOutputFilesSize.Text = ((int)currGameLevel.intLevelWidthX * iudOutputFilesScale.Value.Value).ToString() + "x" + ((int)currGameLevel.intLevelHeightY * iudOutputFilesScale.Value.Value).ToString();
            }            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.sExportGraphPath = tbFolderPath.Text;
            Settings.Default.bExportGraphAddSizeToName = cbAddLevelSizeToName.IsChecked.Value;
            Settings.Default.bExportGraphUsePng = cbUsePng.IsChecked.Value;
            Settings.Default.bExportGraphPngInter = cbPngUseInterlace.IsChecked.Value;
            Settings.Default.bExportGraphUseBmp = cbUseBmp.IsChecked.Value;
            Settings.Default.bExportGraphUseTif = cbUseTif.IsChecked.Value;
            Settings.Default.bExportGraphTifWithZip = cbTifInZip.IsChecked.Value;
            Settings.Default.bExportGraphUseGif = cbUseGif.IsChecked.Value;
            Settings.Default.bExportGraphUseJpg = cbUseJpg.IsChecked.Value;
            if(iudJpgQualityLevel.Value.HasValue)
            {
                Settings.Default.byteExportGraphJpgQuality = iudJpgQualityLevel.Value.Value;
            }
            Settings.Default.bExportGraphUseWpg = cbUseWpg.IsChecked.Value;
            if (iudWpgQualityLevel.Value.HasValue)
            {
                Settings.Default.byteExportGraphWpgQuality = iudWpgQualityLevel.Value.Value;
            }
            Settings.Default.iExportGraphOutputScale = iudOutputFilesScale.Value.Value;
            Settings.Default.Save();
        }
    }
}
