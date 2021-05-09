using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picross_LevelEditor
{
    public class OnlyRGB
    {
        public byte colR { get; set; }
        public byte colG { get; set; }
        public byte colB { get; set; }
        public OnlyRGB()
        {
            colR = 0;
            colG = 0;
            colB = 0;
        }
        public OnlyRGB(byte ColorR, byte ColorG, byte ColorB)
        {
            colR = ColorR;
            colG = ColorG;
            colB = ColorB;
        }
    }
}
