using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;

namespace mahjongNEA
{
    class UserPlayer : Player
    {
        private Tile selectedTile;
        private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
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
                selectedTile = null;
                Tile t = (Tile)e.Source;
                if (ownTurn)
                {
                    ewh.Set();
                    selectedTile = t;
                }
            }
            catch (Exception k)
            {
                MessageBox.Show(k.Message);
            }
        }
        //used for testing display, change later
        private void WaitForEvent(EventWaitHandle eventHandle)
        {
            //TODO: understand this
            var frame = new DispatcherFrame();
            new Thread(() =>
            {
                eventHandle.WaitOne();
                frame.Continue = false;
            }).Start();
            Dispatcher.PushFrame(frame);
        }

        public override Action getAction(Action a)
        {
            lastAction = null;
            if (a.typeOfAction == 0)
            {
                WaitForEvent(ewh);
                ewh.Reset();
                lastAction = new Action(1, selectedTile);
                selectedTile = null;
                return lastAction;
            }
            else if (a.typeOfAction == 1)
            {
                List<Action> actionList = new List<Action>();
                for (int i = 0; i < ownTiles.Count - 1; i++)
                {
                    for (int k = i + 1; k < ownTiles.Count; k++)
                    {
                        if (Analysis.isChow(ownTiles[i], ownTiles[k], a.representingTile))
                        {
                            actionList.Add(new Action(2, a.representingTile, new List<Tile>() { ownTiles[i], ownTiles[k], a.representingTile }));
                        }
                    }
                }
                Button chowButton = new Button();
                TextBlock tempTB = new TextBlock();
                tempTB.Text = "Chow";
                tempTB.FontSize = 12;
                chowButton.Content = tempTB;
                chowButton.Margin = new Thickness(2, 2, 2, 2);
                chowButton.Click += chowClick;
                actionButtons.Children.Add(chowButton);
                ewh.Reset();
                WaitForEvent(ewh);
            }
            return new Action(0);
            //temp return to avoid crashing for testing
        }

        private void chowClick(object sender, RoutedEventArgs e)
        {
            ewh.Set();
        }

        public override void acceptAction()
        {
            if (lastAction.typeOfAction == 1)
            {
                ownTiles.Remove(lastAction.representingTile);
                updateTileDisplay();
            }
        }
    }
}
