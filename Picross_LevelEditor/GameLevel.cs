using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picross_LevelEditor
{
    [System.Serializable]
    public class GameLevel
    {
        //public Dictionary<string, string> dictonaryLevelNames  { get; set; }
        public string stringEngName { get; set; }
        public string stringPolName { get; set; }
        public byte intLevelWidthX { get; set; }
        public byte intLevelHeightY { get; set; }
        public OnlyRGB colorTilesNeutral { get; set; }
        public OnlyRGB colorBackground { get; set; }
        public OnlyRGB colorMarker { get; set; }
        public List<OnlyRGB> listPossibleColorsOfTiles { get; set; }
        public List<LevelTilesOrder> listLevelTilesOrder { get; set; }
        public List<List<HintData>> listVerticlaNumbersHints { get; set; }
        public List<List<HintData>> listHorizontalNumberHints { get; set; }
        public GameLevel()
        {
            //dictonaryLevelNames = new Dictionary<string, string>();
            stringEngName = "";
            stringPolName = "";
            intLevelWidthX = 5;
            intLevelHeightY = 5;
            colorTilesNeutral = new OnlyRGB();
            colorBackground = new OnlyRGB();
            colorMarker = new OnlyRGB();
            listPossibleColorsOfTiles = new List<OnlyRGB>();
            listLevelTilesOrder = new List<LevelTilesOrder>();
            listVerticlaNumbersHints = new List<List<HintData>>();
            listHorizontalNumberHints = new List<List<HintData>>();
            colorTilesNeutral = new OnlyRGB(255, 255, 255);
            colorBackground = new OnlyRGB(200, 200, 200);
            colorMarker = new OnlyRGB(255, 0, 0);
            listPossibleColorsOfTiles.Add(new OnlyRGB(0, 0, 0));
        }
    }
}
