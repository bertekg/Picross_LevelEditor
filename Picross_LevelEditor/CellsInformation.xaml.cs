using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Picross_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy CellsInformation.xaml
    /// </summary>
    public partial class CellsInformation : Window
    {
        GameLevel currGameLevel = new GameLevel();
        public CellsInformation(GameLevel gameLevel)
        {
            InitializeComponent();
            currGameLevel = gameLevel;
            tbLevelWidth.Text = currGameLevel.intLevelWidthX.ToString();
            tbLevelHeight.Text = currGameLevel.intLevelHeightY.ToString();
            tbLevelTotalCells.Text = (currGameLevel.intLevelWidthX * currGameLevel.intLevelHeightY).ToString();
            tbLevelFilledAllCellsCount.Text = currGameLevel.listLevelTilesOrder.Count.ToString();
            List<LevelTilesOrder> lInside = currGameLevel.listLevelTilesOrder.FindAll(item => item.tileLocationX < currGameLevel.intLevelWidthX && item.tileLocationY < currGameLevel.intLevelHeightY);
            List<LevelTilesOrder> lOutside = currGameLevel.listLevelTilesOrder.FindAll(item => item.tileLocationX >= currGameLevel.intLevelWidthX || item.tileLocationY >= currGameLevel.intLevelHeightY);
            tbLevelFilledViewCellsCount.Text = (lInside.Count).ToString();
            tbLevelFilledOutsideViewCellsCount.Text = lOutside.Count.ToString();
            tbLevelFilledViewInPrecent.Text = (((double)(lInside.Count) / (double)(currGameLevel.intLevelWidthX * currGameLevel.intLevelHeightY)) * 100.0).ToString() + "%";
            lvAllCells.ItemsSource = currGameLevel.listLevelTilesOrder;
            lvInsideCells.ItemsSource = lInside;
            lvOutsideCells.ItemsSource = lOutside;
            List<ColorDetail> lColDet = new List<ColorDetail>();
            for (byte i = 0; i < currGameLevel.listPossibleColorsOfTiles.Count; i++)
            {
                Color cTemp = Color.FromRgb(currGameLevel.listPossibleColorsOfTiles[i].colR, currGameLevel.listPossibleColorsOfTiles[i].colG, currGameLevel.listPossibleColorsOfTiles[i].colB);
                List<LevelTilesOrder> lTempColorPart = currGameLevel.listLevelTilesOrder.FindAll(item => item.colorId == i);
                double dProcentOfColor = (((double)lTempColorPart.Count) / (currGameLevel.listLevelTilesOrder.Count)) * 100.0;
                ColorDetail cdTemp = new ColorDetail(i, cTemp, lTempColorPart.Count, dProcentOfColor);
                lColDet.Add(cdTemp);
            }
            lvCellsWithColors.ItemsSource = lColDet;
        }
        public class ColorDetail
        {
            public byte colId { get; set; }
            public System.Windows.Media.Color colName { get; set; }
            public int countOfCol { get; set; }
            public double procToAll { get; set; }
            public ColorDetail()
            {
                colId = 0;
                colName = new Color();
                countOfCol = 0;
                procToAll = 0;
            }
            public ColorDetail(byte ColorId, System.Windows.Media.Color ColorName, int CountOfColor, double ProcentToAll)
            {
                colId = ColorId;
                colName = ColorName;
                countOfCol = CountOfColor;
                procToAll = ProcentToAll;
            }
        }
    }
}
