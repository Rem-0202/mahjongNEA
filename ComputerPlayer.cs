using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mahjongNEA
{
    class ComputerPlayer : Player
    {
        public ComputerPlayer(int wind) : base(wind)
        {
            InitializeComponent();
            foreach (Tile t in ownTiles)
            {
                t.concealTile();
            }
            updateTileDisplay();
        }
    }
}
