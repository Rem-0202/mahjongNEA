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
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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
            this.actionButtons.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(actionButtonStack_Click));
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
            if (a.typeOfAction == 0 && ownTurn)
            {
                ewh.Reset();
                WaitForEvent(ewh);
                ewh.Reset();
                lastAction = new Action(1, selectedTile);
                selectedTile = null;
            }
            else if (a.typeOfAction == 1)
            {
                lastAction = new Action(0);
                List<Action> chowList = new List<Action>();
                List<Action> pongList = new List<Action>();
                List<Action> kongList = new List<Action>();
                for (int i = 0; i < ownTiles.Count - 1; i++)
                {
                    for (int k = i + 1; k < ownTiles.Count; k++)
                    {
                        if (Analysis.isChow(ownTiles[i], ownTiles[k], a.representingTile))
                        {
                            chowList.Add(new Action(2, a.representingTile, new List<Tile>() { ownTiles[i], ownTiles[k], a.representingTile }));
                        }
                        if (Analysis.isPong(ownTiles[i], ownTiles[k], a.representingTile))
                        {
                            pongList.Add(new Action(3, a.representingTile, new List<Tile>() { ownTiles[i], ownTiles[k], a.representingTile }));
                        }
                        for (int j = k + 1; j < ownTiles.Count; j++)
                        {
                            if (Analysis.isKong(ownTiles[i], ownTiles[k], ownTiles[j], a.representingTile))
                            {
                                kongList.Add(new Action(4, a.representingTile, new List<Tile>() { ownTiles[i], ownTiles[k], ownTiles[j], a.representingTile }));
                            }
                        }
                    }
                }
                if (chowList.Count != 0 && nextTurn)
                {
                    foreach (Action x in chowList)
                    {
                        addActionButton(x);
                    }
                    WaitForEvent(ewh);
                    ewh.Reset();
                    actionButtons.Children.Clear();
                }
            }
            else
            {
                lastAction = new Action(0);
            }
            return lastAction;
            //temp return to avoid crashing for testing
        }

        public override void acceptAction()
        {
            if (lastAction != null)
            {
                switch (lastAction.typeOfAction)
                {
                    case 1:
                        ownTiles.Remove(lastAction.representingTile);
                        break;
                    case 2:
                        for (int i = 0; i < lastAction.allTiles.Count; i++)
                        {
                            Tile t = lastAction.allTiles[i];
                            ownTiles.Remove(t);
                            walledTiles.Add(t);
                            t.unhover();
                            t.unconcealTile();
                            t.interactive = false;
                            if (t == lastAction.representingTile)
                            {
                                t.LayoutTransform = new RotateTransform(270);
                                t.VerticalAlignment = VerticalAlignment.Bottom;
                            }
                            if (i == lastAction.allTiles.Count - 1)
                            {
                                t.Margin = new Thickness(t.Margin.Left, t.Margin.Top, t.Margin.Right + 10, t.Margin.Bottom);
                            }
                        }
                        break;
                }
                updateTileDisplay();
            }
        }

        private void addActionButton(Action a)
        {
            ActionButton chowButton = new ActionButton(a);
            actionButtons.Children.Add(chowButton);
        }

        public void actionButtonStack_Click(object sender, RoutedEventArgs e)
        {
            ActionButton b = e.Source as ActionButton;
            MessageBox.Show("g");
            lastAction = b.action;
            ewh.Set();
            e.Handled = true;
        }
    }
}