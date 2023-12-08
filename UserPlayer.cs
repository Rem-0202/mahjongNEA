using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;

namespace mahjongNEA
{
    class UserPlayer : Player
    {
        private Tile selectedTile;
        private bool clickedTile = false;
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
            try
            {
                Tile t = (Tile)e.Source;
                if (ownTurn)
                {
                    clickedTile = true;
                    selectedTile = t;
                }
            }
            catch (Exception k)
            {
                MessageBox.Show(k.Message);
            }
        }
        //used for testing display, change later

        private void waitForClick()
        {
            while (!clickedTile)
            {
                Thread.Sleep(200);
            }
            lastAction = new Action(1, selectedTile);
            selectedTile = null;
            clickedTile = false;
        }

        public override Action getAction(Action a)
        {
            if (a.typeOfAction == 0)
            {
                Thread t = new Thread(waitForClick);
                t.Start();
                return lastAction;
            }
            else if (a.typeOfAction == 1)
            {

            }
            return new Action(0);
            //temp return to avoid crashing for testing
        }

        public override void acceptAction()
        {
            ownTurn = true;
        }
    }
}
