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
        private EventWaitHandle actionEWH = new EventWaitHandle(false, EventResetMode.ManualReset);

        private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
        public StackPanel actionButtons;

        public UserPlayer(int wind, int points, UIElement actionButtons, int pWind, string n) : base(wind, points, pWind, n)
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
            catch { }
        }

        public override Action getAction(Action a)
        {
            lastAction = null;
            bool bonusWin = bonusTiles.Count == 8 || bonusTiles.Count == 7 && lastTileBonus;
            if (a.typeOfAction == 0)
            {
                List<Tile> tempTS = new List<Tile>();
                tempTS.AddRange(ownTiles);
                if (Analysis.countShanten(tempTS, walledGroupCount) == -1 || bonusWin)
                {
                    ActionButton skipButton = new ActionButton(ref actionEWH, false);
                    ActionButton winButton = new ActionButton(ref actionEWH, true);
                    actionButtons.Children.Add(skipButton);
                    actionButtons.Children.Add(winButton);
                    actionEWH.Reset();
                    WaitForEvent(actionEWH);
                    actionEWH.Reset();
                    if (winButton.clicked)
                    {
                        return new Action(5);
                    }
                    actionButtons.Children.Clear();
                }
                for (int i = 0; i < ownTiles.Count - 3; i++)
                {
                    for (int j = i + 1; j < ownTiles.Count - 2; j++)
                    {
                        for (int k = j + 1; k < ownTiles.Count - 1; k++)
                        {
                            for (int l = k + 1; l < ownTiles.Count; l++)
                            {
                                if (Analysis.isKong(ownTiles[i], ownTiles[j], ownTiles[k], ownTiles[l]))
                                {
                                    List<Action> kongList = new List<Action>
                                    {
                                        new Action(4, ownTiles[i], new List<Tile>() { ownTiles[i], ownTiles[k], ownTiles[j], ownTiles[l] })
                                    };
                                    ActionButton kongButton = new ActionButton(kongList, "Kong", ref actionEWH);
                                    ActionButton skipButton = new ActionButton(ref actionEWH, false);
                                    i = j = k = l = 9999;
                                    actionButtons.Children.Add(skipButton);
                                    actionButtons.Children.Add(kongButton);
                                    actionEWH.Reset();
                                    WaitForEvent(actionEWH);
                                    actionEWH.Reset();
                                    if (kongButton.clicked)
                                    {
                                        return kongButton.action;
                                    }
                                    actionButtons.Children.Clear();
                                }
                            }
                        }
                    }
                }
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
                List<Tile> tempTS = new List<Tile>();
                tempTS.AddRange(ownTiles);
                tempTS.Add(a.representingTile);
                bool win = Analysis.countShanten(tempTS, walledGroupCount) == -1 || bonusWin;
                for (int i = 0; i < ownTiles.Count - 1; i++)
                {
                    for (int k = i + 1; k < ownTiles.Count; k++)
                    {
                        if (Analysis.isChow(ownTiles[i], ownTiles[k], a.representingTile) && nextTurn)
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
                if (chowList.Count != 0 || pongList.Count != 0 || kongList.Count != 0 || win)
                {
                    ActionButton skipButton = new ActionButton(ref actionEWH, false);
                    ActionButton winButton = new ActionButton(ref actionEWH, true);
                    ActionButton chowButton = new ActionButton(ref actionEWH, false);
                    ActionButton pongButton = new ActionButton(ref actionEWH, false);
                    ActionButton kongButton = new ActionButton(ref actionEWH, false);
                    actionButtons.Children.Add(skipButton);
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
                    if (win)
                    {
                        actionButtons.Children.Add(winButton);
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
                    else if (winButton.clicked)
                    {
                        lastAction = new Action(5);
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