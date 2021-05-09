using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace Picross_LevelEditor
{
    [System.Serializable]
    public class MultiLevelMenager
    {
        public string stringAllLevelsEngName { get; set; }
        public string stringAllLevelsPolName { get; set; }
        public int intTotalLevelWidthX { get; set; }
        public int intTotalLevelHeightY { get; set; }
        public byte byteSingleWidthX { get; set; }
        public byte byteSingleHeightY { get; set; }
        public int intCountPartsWidthX { get; set; }
        public int intCountPartsHeightY { get; set; }
        public byte byteLastWidthX { get; set; }
        public byte byteLastHeightY { get; set; }
        public List<InofAboutToPlay> listLevelToPlay { get; set; }
        private Dictionary<Point,GameLevel> directoryGameLevels { get; set; }
        private Dictionary<Point, bool> directiryLevelToPlay { get; set; }
        public void FillInfoAboutToPlay()
        {
            listLevelToPlay = new List<InofAboutToPlay>();
            for (int i = 0; i < intCountPartsWidthX; i++)
            {
                for(int j = 0; j < intCountPartsHeightY; j++)
                {
                    InofAboutToPlay tempIATP = new InofAboutToPlay();
                    Point pTemp = new Point(i, j);
                    tempIATP.indexX = i;
                    tempIATP.indexY = j;
                    tempIATP.isToPlay = directiryLevelToPlay[pTemp];
                    listLevelToPlay.Add(tempIATP);
                }
            }
        }
        public void MakeNewDirectory()
        {
            directoryGameLevels.Clear();
            directiryLevelToPlay.Clear();
            for (int i = 0; i < intCountPartsWidthX; i++)
            {
                for (int j = 0; j < intCountPartsHeightY; j++)
                {
                    GameLevel tempGameLevel = new GameLevel();
                    if(i < intCountPartsWidthX-1)
                    {
                        tempGameLevel.intLevelWidthX = byteSingleWidthX;
                    }
                    else
                    {
                        tempGameLevel.intLevelWidthX = byteLastWidthX;
                    }
                    if (i < intCountPartsHeightY - 1)
                    {
                        tempGameLevel.intLevelHeightY = byteSingleHeightY;
                    }
                    else
                    {
                        tempGameLevel.intLevelHeightY = byteLastHeightY;
                    }
                    directoryGameLevels.Add(new Point(i, j), tempGameLevel);
                    directiryLevelToPlay.Add(new Point(i, j), true);
                }
            }
        }
        public Dictionary<Point, GameLevel> GetDirectoryListGameLevel()
        {
            return directoryGameLevels;
        }
        public void SetGameLevel(Point point, GameLevel gl)
        {
            directoryGameLevels[point] = gl;
        }
        public GameLevel GetGameLevel(Point point)
        {
            return directoryGameLevels[point];
        }
        public void SetDirectoryListGameLevel(Dictionary<Point, GameLevel> toSet)
        {
            directoryGameLevels = toSet;
        }
        public Dictionary<Point, bool> GetDirectiryListLevelToPlay()
        {
            return directiryLevelToPlay;
        }
        public void SetDirectiryListLevelToPlay(Dictionary<Point, bool> toSet)
        {
            directiryLevelToPlay = toSet;
        }
        public MultiLevelMenager()
        {
            stringAllLevelsEngName = "";
            stringAllLevelsPolName = "";
            intTotalLevelWidthX = 140;
            intTotalLevelHeightY = 90;
            byteSingleWidthX = 25;
            byteSingleHeightY = 20;
            intCountPartsWidthX = 0;
            intCountPartsHeightY = 0;
            byteLastWidthX = 0;
            byteLastHeightY = 0;
            directoryGameLevels = new Dictionary<Point, GameLevel>();
            directiryLevelToPlay = new Dictionary<Point, bool>();
        }
    }
    public class InofAboutToPlay
    {
        public int indexX;
        public int indexY;
        public bool isToPlay;
    }
}
