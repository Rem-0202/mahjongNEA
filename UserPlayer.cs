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

namespace mahjongNEA
{
    class UserPlayer : Player
    {
        private Tile selectedTile;
        EventWaitHandle actionEWH = new EventWaitHandle(false, EventResetMode.ManualReset);

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
            if (a.typeOfAction == 0)
            {
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
                if ((chowList.Count != 0 && nextTurn) || pongList.Count != 0 || kongList.Count != 0)
                {
                    //WORK ON THIS MAINLY
                    //NOT FINISHED
                    ActionButton skipButton = new ActionButton(ref actionEWH);
                    actionButtons.Children.Add(skipButton);
                    ActionButton chowButton = new ActionButton(ref actionEWH);
                    ActionButton pongButton = new ActionButton(ref actionEWH); ;
                    ActionButton kongButton = new ActionButton(ref actionEWH); ;
                    if (chowList.Count != 0 && nextTurn)
                    {
                        chowButton = new ActionButton(chowList, "Chow", ref actionEWH);
                        actionButtons.Children.Add(chowButton);
                    }
                    if (pongList.Count != 0)
                    {
                        pongButton = new ActionButton(pongList, "Pong", ref actionEWH);
                        actionButtons.Children.Add(pongButton);
                    }
                    if (kongList.Count != 0)
                    {
                        kongButton = new ActionButton(kongList, "Kong", ref actionEWH);
                        actionButtons.Children.Add(kongButton);
                    }
                    actionEWH.Reset();
                    WaitForEvent(actionEWH);
                    if (chowButton.clicked)
                    {
                        lastAction = chowButton.action;
                    }
                    else if (pongButton.clicked)
                    {
                        lastAction = pongButton.action;
                    }
                    else if (kongButton.clicked)
                    {
                        lastAction = kongButton.action;
                    }
                    else if (skipButton.clicked)
                    {
                        lastAction = skipButton.action;
                    }
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
    }
}