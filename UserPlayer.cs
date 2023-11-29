using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mahjongNEA
{
    class UserPlayer : Player
    {
        private Tile selectedTile;
        public StackPanel actionButtons;

        public UserPlayer(int wind, int points, UIElement actionButtons) : base(wind, points)
        {
            InitializeComponent();
            this.actionButtons = (StackPanel)actionButtons;
        }

        public override void addTile(Tile t)
        {
            t.interactive = true;
            base.addTile(t);
        }

        protected override void OwnTileDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tile t = (Tile)e.Source;
            if (ownTurn)
            {
                selectedTile = t;
                ownTiles.Remove(t);
                updateTileDisplay();
            }
        }
        //used for testing display, change later

        public override Action getAction(Action a)
        {

            return new Action(1, selectedTile);
            //temp return to avoid crashing for testing
        }

        public override void acceptAction()
        {

        }
    }
}
