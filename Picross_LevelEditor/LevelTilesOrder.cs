using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picross_LevelEditor
{
    public class LevelTilesOrder
    {
        public byte tileLocationX { get; set; }
        public byte tileLocationY { get; set; }
        public byte colorId { get; set; }
        public LevelTilesOrder()
        {
            tileLocationX = 0;
            tileLocationY = 0;
            colorId = 0;
        }
        public LevelTilesOrder(byte TileLocX, byte TileLocY, byte ColorId)
        {
            tileLocationX = TileLocX;
            tileLocationY = TileLocY;
            colorId = ColorId;
        }
    }
}
