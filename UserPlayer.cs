using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            base.addTile(t);
        }

        protected override void OwnTileDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tile t = (Tile)e.Source;
            ownTiles.Remove(t);
            walledTiles.Add(t);
            updateTileDisplay();
        }
    }
}
