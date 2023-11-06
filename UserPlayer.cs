using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mahjongNEA
{
    class UserPlayer : Player
    {
        public UserPlayer(int wind) : base(wind) 
        {
            InitializeComponent();
        }

        public override void addTile(Tile t)
        {
            t.interactive = true;
            ownTiles.Add(t);
            updateTileDisplay();
        }
    }
}
