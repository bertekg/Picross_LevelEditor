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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Picross_LevelEditor.Properties;
using System.Globalization;
using System.Threading;

namespace Picross_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool bAfterInit = false;
        string projectName = "";
        string projectPath = "";
        public MainWindow()
        {
            ChangeCultureInfo();
            MakeNewSampleSimpleLevel();
            MakeNewSampleMultiLevel();
            InitializeComponent();
            SetLangIndicator();
            SetWindowsVisableElement();
            bAfterInit = true;            
            //pMainContent.Content = page1;
        }
        private void ChangeCultureInfo()
        {
            string sReadSelectedLang = Settings.Default.ciSelectedLanguage.ToString();
            if (sReadSelectedLang == "") sReadSelectedLang = Thread.CurrentThread.CurrentCulture.ToString();
            CultureInfo returnCulture = new CultureInfo("en-US");
            if (sReadSelectedLang.Contains("pl-PL")) returnCulture = CultureInfo.GetCultureInfo("pl-PL");
            else if (sReadSelectedLang.Contains("en-US")) returnCulture = CultureInfo.GetCultureInfo("en-US");
            else returnCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = returnCulture;
            Thread.CurrentThread.CurrentUICulture = returnCulture;
            Settings.Default.ciSelectedLanguage = returnCulture;
            Settings.Default.Save();
        }
        private void SetWindowsVisableElement()
        {
            miShowGridLines.IsChecked = Settings.Default.bGridLineShow;
            miShowCellsTextInfo.IsChecked = Settings.Default.bToolTipAvilable;
            if(Settings.Default.bIsSimple)
            {
                gMainContent.Children.Clear();
                gMainContent.Children.Add(new EditLevelControl());
                miSimple.IsChecked = true;
                miMultiple.IsChecked = false;
            }
            else
            {
                gMainContent.Children.Clear();
                gMainContent.Children.Add(new MultiLevelTabView());
                miSimple.IsChecked = false;
                miMultiple.IsChecked = true;
            }
        }
        public MultiLevelMenager currMultiLevelMenager = new MultiLevelMenager();
        public GameLevel currGameLevel = new GameLevel();
        private void MakeNewSampleSimpleLevel()
        {
            if(true)
            {
                currGameLevel = new GameLevel();
                currGameLevel.intLevelWidthX = 5;
                currGameLevel.intLevelHeightY = 5;
                //currGameLevel.dictonaryLevelNames.Add("EN", "");
                currGameLevel.colorTilesNeutral = new OnlyRGB(255, 255, 255);
                currGameLevel.colorMarker = new OnlyRGB(255, 165, 0);
                currGameLevel.colorBackground = new OnlyRGB(192, 192, 192);
                currGameLevel.listPossibleColorsOfTiles.Add(new OnlyRGB(0, 0, 0));
                if(bAfterInit)
                {
                    gMainContent.Children.Clear();
                    gMainContent.Children.Add(new EditLevelControl());
                }
            }
        }
        private void MakeNewSampleMultiLevel()
        {
            currMultiLevelMenager = new MultiLevelMenager();
            currMultiLevelMenager.stringAllLevelsEngName = "New level";
            currMultiLevelMenager.stringAllLevelsPolName = "Nowy poziom";
            currMultiLevelMenager.intTotalLevelWidthX = 15;
            currMultiLevelMenager.intTotalLevelHeightY = 10;
            currMultiLevelMenager.byteSingleWidthX = 5;
            currMultiLevelMenager.byteSingleHeightY = 5;
            Dictionary<Point, GameLevel> listGameLevels = new Dictionary<Point, GameLevel>();
            Dictionary<Point, bool> listLevelToPlay = new Dictionary<Point, bool>();
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    listGameLevels.Add(new Point(i, j), new GameLevel());
                    listLevelToPlay.Add(new Point(i, j), true);
                }                
            }
            currMultiLevelMenager.SetDirectoryListGameLevel(listGameLevels);
            currMultiLevelMenager.SetDirectiryListLevelToPlay(listLevelToPlay);
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
        
        private void miNewProject_Click(object sender, RoutedEventArgs e)
        {
            if(Settings.Default.bIsSimple)
            {
                MakeNewSampleSimpleLevel();
                gMainContent.Children.Clear();
                gMainContent.Children.Add(new EditLevelControl());
            }
            else
            {
                MakeNewSampleMultiLevel();
                gMainContent.Children.Clear();
                gMainContent.Children.Add(new MultiLevelTabView());
            }
        }
        private void miOpenProject_Click(object sender, RoutedEventArgs e)
        {
            if(Settings.Default.bIsSimple == true)
            {
                System.Windows.Forms.OpenFileDialog textDialogOpen = new System.Windows.Forms.OpenFileDialog();
                textDialogOpen.Filter = Lang.sGameLevel + " | *.xml";
                System.Windows.Forms.DialogResult result = textDialogOpen.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(textDialogOpen.FileName);
                        string xmlString = xmlDocument.OuterXml;
                        using (StringReader read = new StringReader(xmlString))
                        {
                            Type outType = typeof(GameLevel);

                            XmlSerializer serializer = new XmlSerializer(outType);
                            using (XmlReader reader = new XmlTextReader(read))
                            {
                                currGameLevel = (GameLevel)serializer.Deserialize(reader);
                                reader.Close();
                            }
                            read.Close();
                        }
                        projectName = System.IO.Path.GetFileNameWithoutExtension(textDialogOpen.FileName);
                        projectPath = System.IO.Path.GetDirectoryName(textDialogOpen.FileName);
                        this.Title = Lang.sPicross_LevelEditor + " [" + projectName + "]";
                        sbiProjectPath.Text = projectPath;
                        gMainContent.Children.Clear();
                        gMainContent.Children.Add(new EditLevelControl());
                        Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sInfoOpenLevelConfirmationMessage, Lang.sInfoOpenLevelConfirmationTittle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    catch (Exception ex)
                    {
                        Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorOpenLevelMessage, Lang.sErrorOpenLevelTittle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void miSaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (projectName == "" || projectPath == "")
            {
                SaveAs();
            }
            else
            {
                bool bRetAfterSave = SaveProject(projectName, projectPath);
                if (bRetAfterSave == true)
                {
                    this.Title = Lang.sPicross_LevelEditor + " [" + projectName + "]";
                    sbiProjectPath.Text = projectPath;
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sInfoSaveLevelMessage, Lang.sInfoSaveLevelTittle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorSaveLevelMessage, Lang.sErrorSaveLevelTittle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void miSaveAsProject_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }
        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void miDeleteAllCells_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbrAnswere = MessageBox.Show("Are you sure you want delete all cells from current level?", "Confirmation of delete cells",MessageBoxButton.YesNoCancel,MessageBoxImage.Question);
            if(mbrAnswere == MessageBoxResult.Yes)
            {
                currGameLevel.listLevelTilesOrder.Clear();
                //DrawAllLevelCells();
            }
        }
        private void miShowCellsTextInfo_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.bToolTipAvilable = miShowCellsTextInfo.IsChecked;
            Settings.Default.Save();
            //DrawAllLevelCells();
        }
        private void miShowGridLines_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.bGridLineShow = miShowGridLines.IsChecked;
            Settings.Default.Save();
            //gMainPlaceGrid.ShowGridLines = Settings.Default.bGridLineShow;
        }
        private void miSetDefoultView_Click(object sender, RoutedEventArgs e)
        {
            miShowCellsTextInfo.IsChecked = true;
            Settings.Default.bToolTipAvilable = true;            
            //DrawAllLevelCells();
            miShowGridLines.IsChecked = true;
            //gMainPlaceGrid.ShowGridLines = true;
            Settings.Default.bGridLineShow = true;
            Settings.Default.Save();
        }
        private void miLangPol_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ciSelectedLanguage = new CultureInfo("pl-PL");
            Settings.Default.Save();
            Xceed.Wpf.Toolkit.MessageBox.Show("Wybrałeś język Polski.\nProszę zamknać i ponownie uruchomić program.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            SetLangIndicator();
        }
        private void miLangEng_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ciSelectedLanguage = new CultureInfo("en-US");
            Settings.Default.Save();
            Xceed.Wpf.Toolkit.MessageBox.Show("You select English language.\nPlease close and once again open program.", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            SetLangIndicator();
        }
        private void SetLangIndicator()
        {
            string langStr = Thread.CurrentThread.CurrentCulture.ToString();
            if (langStr.Contains("pl-PL"))
            {
                miLangPol.IsChecked = true; miLangEng.IsChecked = false;
            }
            else if (langStr.Contains("en-US"))
            {
                miLangPol.IsChecked = false; miLangEng.IsChecked = true;
            }
            else
            {
                miLangPol.IsChecked = false; miLangEng.IsChecked = true;
            }
        }
        private void miOpenManual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string langStr = Thread.CurrentThread.CurrentCulture.ToString();
                if (langStr.Contains("pl-PL"))
                {
                    System.Diagnostics.Process.Start("Manual\\UserManual_Pol.pdf");
                }
                else
                {
                    System.Diagnostics.Process.Start("Manual\\UserManual_Eng.pdf");
                }             
            }
            catch
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sManualFileMissingMessage, Lang.sManualFileMissingTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void miAboutProgram_Click(object sender, RoutedEventArgs e)
        {
            ShowVersionInformation();
        }
        private void ShowVersionInformation()
        {
            AboutProgram apWindow = new AboutProgram();
            apWindow.ShowDialog();
            /*
            string sProgramVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string sReleseData = "2019.07.03";
            string insaidInfo = Lang.sPicross_LevelEditor + "\n" + Lang.sVersionNumber + ": " + sProgramVersion + "\n" + Lang.sReleaseDate + ": " + sReleseData;
            Xceed.Wpf.Toolkit.MessageBox.Show(insaidInfo, Lang.sAboutProgram, MessageBoxButton.OK, MessageBoxImage.Information);
            */
            /*
            Xceed.Wpf.Toolkit.MessageBox verionInfoMessageBox = new Xceed.Wpf.Toolkit.MessageBox();
            verionInfoMessageBox.ImageSource = new BitmapImage(new Uri(String.Format("/FlipBlock3D_LevelEditor;component/Graphics/ProgramIcon_128.png"), UriKind.RelativeOrAbsolute));
            verionInfoMessageBox.Caption = "About Program";
            verionInfoMessageBox.Text = insaidInfo;
            verionInfoMessageBox.ShowDialog();
            */
        }
        //byte byteLeftEditMode = 1, byteRightEditMode = 0;       
        
        
       
        
       
       
       
        private void miImportFromPicture_Click(object sender, RoutedEventArgs e)
        {
            ImportFromPicture ifpWindow = new ImportFromPicture(ImportFromPicture.OpenImportMode.Direct);
            ifpWindow.ShowDialog();
            if(ifpWindow.boolFinishOK)
            {
                if(ifpWindow.boolIsSingle)
                {
                    currGameLevel = ifpWindow.currentGameLevel;
                    gMainContent.Children.Clear();
                    gMainContent.Children.Add(new EditLevelControl());
                }
                else
                {
                    currMultiLevelMenager = ifpWindow.currentMultiLevelMenager;
                    gMainContent.Children.Clear();
                    gMainContent.Children.Add(new MultiLevelTabView());
                }
            }
        }

        private void miPreviewLevel_Click(object sender, RoutedEventArgs e)
        {
            //currGameLevel.stringEngName = tbEnglishLevelName.Text;
            //currGameLevel.stringPolName = tbPolishLevelName.Text;
            CalcTilesHintData();
            PreviewLevel ifpWindow = new PreviewLevel(currGameLevel);
            ifpWindow.ShowDialog();
        }
        private void SaveAs()
        {
            System.Windows.Forms.SaveFileDialog textDialogSave = new System.Windows.Forms.SaveFileDialog();
            textDialogSave.Filter = Lang.sGameLevel + " | *.xml";
            System.Windows.Forms.DialogResult result = textDialogSave.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                bool bRetAfterSave = SaveProject(System.IO.Path.GetFileNameWithoutExtension(textDialogSave.FileName), System.IO.Path.GetDirectoryName(textDialogSave.FileName));
                if (bRetAfterSave == true)
                {
                    this.Title = Lang.sPicross_LevelEditor + " [" + projectName + "]";
                    sbiProjectPath.Text = projectPath;
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sInfoSaveLevelMessage, Lang.sInfoSaveLevelTittle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorSaveLevelMessage, Lang.sErrorSaveLevelTittle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void MiImportFromPicturePdf_Click(object sender, RoutedEventArgs e)
        {
            ImportFromPicture ifpWindow = new ImportFromPicture(ImportFromPicture.OpenImportMode.BigFromPdf);
            ifpWindow.ShowDialog();
            if (ifpWindow.boolFinishOK)
            {
                currGameLevel = ifpWindow.currentGameLevel;
                gMainContent.Children.Clear();
                gMainContent.Children.Add(new EditLevelControl());
            }
        }
        private bool SaveProject(string pName, string pPath)
        {
            bool correctSave = false;
            if (pName == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorIncorrectNameMessage, Lang.sErrorIncorrectNameTittle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (pPath == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorIncorrectPathMessage, Lang.sErrorIncorrectPathTittle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!Directory.Exists(pPath))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorPathNoExistMessage, Lang.sErrorPathNoExistTittle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if(Settings.Default.bIsSimple)
                {
                    //currGameLevel.stringEngName = tbEnglishLevelName.Text;
                    //currGameLevel.stringPolName = tbPolishLevelName.Text;
                    CalcTilesHintData();
                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        XmlSerializer serializer = new XmlSerializer(typeof(GameLevel));
                        string fileLoc = System.IO.Path.Combine(pPath, pName + ".xml");
                        using (MemoryStream stream = new MemoryStream())
                        {
                            serializer.Serialize(stream, currGameLevel);
                            stream.Position = 0;
                            xmlDocument.Load(stream);
                            xmlDocument.Save(fileLoc);
                            stream.Close();
                        }
                        projectName = pName;
                        projectPath = pPath;
                        correctSave = true;
                    }
                    catch (IOException e)
                    {
                        if (e.Source != null)
                            MessageBox.Show("IOException source: " + e.Source.ToString());
                        return correctSave;
                    }
                }
                else
                {
                    try
                    {
                        currMultiLevelMenager.FillInfoAboutToPlay();
                        XmlDocument xmlDocument = new XmlDocument();
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiLevelMenager));
                        string fileLoc = System.IO.Path.Combine(pPath, pName + ".xml");
                        using (MemoryStream stream = new MemoryStream())
                        {
                            serializer.Serialize(stream, currMultiLevelMenager);
                            stream.Position = 0;
                            xmlDocument.Load(stream);
                            xmlDocument.Save(fileLoc);
                            stream.Close();
                        }
                        Dictionary<Point, GameLevel> tt = currMultiLevelMenager.GetDirectoryListGameLevel();
                        for (int i = 0; i < currMultiLevelMenager.intCountPartsWidthX; i++)
                        {
                            for (int j = 0; j < currMultiLevelMenager.intCountPartsHeightY; j++)
                            {
                                Point pTemp = new Point(i, j);
                                try
                                {
                                    XmlDocument xmlDocumentTemp = new XmlDocument();
                                    XmlSerializer serializerTemp = new XmlSerializer(typeof(GameLevel));
                                    string fileLocTemp = System.IO.Path.Combine(pPath, pName + "_" + i.ToString() + "x" + j.ToString() + ".xml");
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        serializerTemp.Serialize(stream, tt[pTemp]);
                                        stream.Position = 0;
                                        xmlDocumentTemp.Load(stream);
                                        xmlDocumentTemp.Save(fileLocTemp);
                                        stream.Close();
                                    }
                                }
                                catch (IOException e)
                                {
                                    if (e.Source != null)
                                        MessageBox.Show("IOException source: " + e.Source.ToString());
                                    return correctSave;
                                }
                            }
                        }
                        projectName = pName;
                        projectPath = pPath;
                        correctSave = true;
                    }
                    catch (IOException e)
                    {
                        if (e.Source != null)
                            MessageBox.Show("IOException source: " + e.Source.ToString());
                        return correctSave;
                    }
                }
            }
            return correctSave;
        }
        private void miExportToPng_Click(object sender, RoutedEventArgs e)
        {
            ExportToGraphicFiles expToGraphicFile = new ExportToGraphicFiles(currGameLevel, projectName);
            expToGraphicFile.ShowDialog();
        }

        private void miSimple_Click(object sender, RoutedEventArgs e)
        {
            miMultiple.IsChecked = false;
            gMainContent.Children.Clear();
            gMainContent.Children.Add(new EditLevelControl());
            Settings.Default.bIsSimple = true;
            Settings.Default.Save();
            //vPlaceAllCells.Visibility = Visibility.Visible;
            //tbMultiplePanel.Visibility = Visibility.Collapsed;
        }

        private void miMultiple_Click(object sender, RoutedEventArgs e)
        {
            miSimple.IsChecked = false;
            gMainContent.Children.Clear();
            gMainContent.Children.Add(new MultiLevelTabView());
            Settings.Default.bIsSimple = false;
            Settings.Default.Save();
            //vPlaceAllCells.Visibility = Visibility.Collapsed;
            // tbMultiplePanel.Visibility = Visibility.Visible;
        }

        private void MiCellsInformation_Click(object sender, RoutedEventArgs e)
        {
            CellsInformation ciWindow = new CellsInformation(currGameLevel);
            ciWindow.ShowDialog();
        }
    }
}