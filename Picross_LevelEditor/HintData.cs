using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picross_LevelEditor
{
    public class HintData
    {
        public byte length { get; set; }
        public byte color { get; set; }
        public HintData()
        {
            length = 0;
            color = 0;
        }
        public HintData(byte TileLoc, byte ColorId)
        {
            length = TileLoc;
            color = ColorId;
        }
    }
}
