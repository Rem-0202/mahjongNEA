using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mahjongNEA
{
    class ComputerPlayer : Player
    {
        public ComputerPlayer(Tile[] startingTiles) : base()
        {
            InitializeComponent();
            ownTiles = startingTiles;
            foreach (Tile t in ownTiles)
            {
                t.concealTile();
                ownTileDisplay.Children.Add(t);
            }
        }
    }
}
