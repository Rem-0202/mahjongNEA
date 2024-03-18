﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;


//TODO:
//1) gameview ask each player for action with input action(last action)
//2) each player returns their wanted action
//3) accept actions based on wanted action list
//4) call acccepted player action to perform action


namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public bool messyDiscard;
        private Action lastAction;
        public static Random rng = new Random();
        public int prevailingWind { get; private set; }
        public int playerWind { get; private set; }
        public List<Tile> availableTiles { get; private set; }
        public Player[] players { get; private set; }
        public int startingPoints { get; private set; }
        public int endingPoints { get; private set; }
        private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
        private List<Tile> discardedTiles = new List<Tile>();
        private bool lastDiscard = false;
        public GameView(int prevailingWind, int playerWind, int startingPoints, int endingPoints)
        {
            InitializeComponent();
            this.playerWind = playerWind;
            this.prevailingWind = prevailingWind;
            this.startingPoints = startingPoints;
            this.endingPoints = endingPoints;
            players = new Player[4];
            players[playerWind % 4] = new UserPlayer(playerWind, startingPoints, userActionButtons, prevailingWind);
            players[(playerWind + 1) % 4] = new ComputerPlayer((playerWind + 1) % 4, startingPoints, prevailingWind);
            players[(playerWind + 2) % 4] = new ComputerPlayer((playerWind + 2) % 4, startingPoints, prevailingWind);
            players[(playerWind + 3) % 4] = new ComputerPlayer((playerWind + 3) % 4, startingPoints, prevailingWind);
            setUpGame();
            //code for showing discarded tiles, change later when implemented discard and add removing from discarded pile
            //TODO: add discarded pile list, unlink tile with grid and link grid with discarded tile list
            //TODO: change so that random placement + stack on top each other
            //for (int column = 0; column < 12; column++)
            //{
            //    for (int row = 0; row < 7; row++)
            //    {
            //        Tile n = availableTiles[rng.Next(availableTiles.Count)];
            //        n.LayoutTransform = new RotateTransform(rng.NextDouble() * 360);
            //        Grid.SetRow(n, row);
            //        Grid.SetColumn(n, column);
            //        availableTiles.Remove(n);
            //        messyDiscardGrid.Children.Add(n);
            //    }
            //}
        }

        private void setUpGame()
        {
            discardedTiles.Clear();
            discardPanel.Children.Clear();
            userPlayerGrid.Children.Clear();
            rightPlayerGrid.Children.Clear();
            leftPlayerGrid.Children.Clear();
            topPlayerGrid.Children.Clear();
            Player[] tempPlayers = new Player[4];
            foreach (Player p in players)
            {
                if (p is ComputerPlayer)
                {
                    tempPlayers[Array.IndexOf(players, p)] = new ComputerPlayer(p.wind, p.points, p.pWind);
                }
                else if (p is UserPlayer)
                {
                    tempPlayers[Array.IndexOf(players, p)] = new UserPlayer(p.wind, p.points, userActionButtons, p.pWind);
                }
            }
            players = tempPlayers;
            userPlayerGrid.Children.Add(players[playerWind % 4]);
            rightPlayerGrid.Children.Add(players[(playerWind + 1) % 4]);
            topPlayerGrid.Children.Add(players[(playerWind + 2) % 4]);
            leftPlayerGrid.Children.Add(players[(playerWind + 3) % 4]);
            players[(playerWind + 1) % 4].LayoutTransform = new RotateTransform(270.0);
            players[(playerWind + 2) % 4].LayoutTransform = new RotateTransform(180.0);
            players[(playerWind + 2) % 4].flipTiles();
            players[(playerWind + 3) % 4].LayoutTransform = new RotateTransform(90.0);
            availableTiles = new List<Tile>();
            for (int i = 1; i <= 9; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    availableTiles.Add(new Tile(i, 'm'));
                    availableTiles.Add(new Tile(i, 's'));
                    availableTiles.Add(new Tile(i, 'p'));
                }
            }
            for (int i = 2; i <= 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    availableTiles.Add(new Tile(i, 'z'));
                }
            }
            for (int i = 2; i <= 5; i++)
            {
                availableTiles.Add(new Tile(i, 'n'));
                availableTiles.Add(new Tile(i, 'f'));
            }
            Tile k;
            foreach (Player p in players)
            {
                for (int i = 0; i < 13; i++)
                {
                    k = availableTiles[rng.Next(availableTiles.Count)];
                    p.addTile(k);
                    availableTiles.Remove(k);
                    if (k.bonus) i--;
                }
            }
        }

        public void toggleSort()
        {
            foreach (Player p in players)
            {
                p.sortTiles();
            }
        }

        public void toggleExposeTiles()
        {
            foreach (Player p in players)
            {
                p.toggleExposeTile();
            }
        }

        public void gameLoop()
        {
            Player currentPlayer;
            bool endTurn = false;
            bool endGame = false;
            int roundNumber = 0;
            int playerIndex = prevailingWind;
            int maxChoice;
            do
            {
                Dictionary<Player, Action> playerActions = new Dictionary<Player, Action>();
                currentPlayer = players[playerIndex];
                lastAction = new Action(0);
                drawTile(currentPlayer);
                DispatcherTimer dt = new DispatcherTimer();
                dt.Interval = TimeSpan.FromSeconds(1);
                dt.Tick += timer_Tick;
                do
                {
                    currentPlayer = players[playerIndex];
                    Array.ForEach(players, (e) => e.unglow());
                    currentPlayer.glow();
                    Array.ForEach(players, e => e.ownTurn = false);
                    Array.ForEach(players, e => e.nextTurn = false);
                    currentPlayer.ownTurn = true;
                    players[(playerIndex + 1) % 4].nextTurn = true;
                    //end turn handling
                    if (lastAction.typeOfAction == 0)
                    {
                        roundNumber++;
                        lastAction = currentPlayer.getAction(lastAction);
                        if (lastAction.typeOfAction == 5)
                        {
                            endTurn = true;
                            HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, true, prevailingWind, currentPlayer.wind);
                            WinWindow ww = new WinWindow(prevailingWind, playerIndex, currentPlayer.ownTiles, h.faanPairs, 1000);
                            ww.ShowDialog();
                            break;
                        }
                        if (lastAction.typeOfAction == 4)
                        {
                            lastAction = currentPlayer.getAction(new Action(0));
                        }
                        currentPlayer.acceptAction();
                    }
                    if (lastAction.typeOfAction == 1)
                    {
                        if (discardPanel.Children.Count > 0)
                        {
                            Tile temp = (Tile)discardPanel.Children[discardPanel.Children.Count - 1];
                            temp.unGlow();
                        }
                        discardedTiles.Add(lastAction.representingTile);
                        lastAction.representingTile.unconcealTile();
                        lastAction.representingTile.unhover();
                        lastAction.representingTile.interactive = false;
                        lastAction.representingTile.Margin = new Thickness(6, 6, 6, 6);
                        lastAction.representingTile.glow();
                        discardPanel.Children.Add(lastAction.representingTile);
                    }
                    playerActions.Clear();
                    playerActions.Add(players[(playerIndex + 1) % 4], players[(playerIndex + 1) % 4].getAction(lastAction));
                    playerActions.Add(players[(playerIndex + 2) % 4], players[(playerIndex + 2) % 4].getAction(lastAction));
                    playerActions.Add(players[(playerIndex + 3) % 4], players[(playerIndex + 3) % 4].getAction(lastAction));
                    maxChoice = -1;
                    foreach (Action a in playerActions.Values)
                    {
                        maxChoice = Math.Max(a.typeOfAction, maxChoice);
                    }
                    if (availableTiles.Count == 0)
                    {
                        lastDiscard = true;
                        if (maxChoice == 4)
                        {
                            //handle end turn
                            MessageBox.Show("No tiles left. Ending turn.");
                            endTurn = true;
                            break;
                        }
                        else
                        {
                            if (lastAction.typeOfAction == 0)
                            {
                                roundNumber++;
                                lastAction = currentPlayer.getAction(lastAction);
                                if (lastAction.typeOfAction == 5)
                                {
                                    endTurn = true;
                                    if (currentPlayer is ComputerPlayer)
                                    {
                                        currentPlayer.toggleExposeTile();
                                    }
                                    HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, true, prevailingWind, currentPlayer.wind);
                                    WinWindow ww = new WinWindow(prevailingWind, currentPlayer.wind, currentPlayer.ownTiles, h.faanPairs, 1000);
                                    ww.ShowDialog();
                                    break;
                                }
                                currentPlayer.acceptAction();
                                if (lastAction.typeOfAction == 1)
                                {
                                    if (discardPanel.Children.Count > 0)
                                    {
                                        Tile temp = (Tile)discardPanel.Children[discardPanel.Children.Count - 1];
                                        temp.unGlow();
                                    }
                                    discardedTiles.Add(lastAction.representingTile);
                                    lastAction.representingTile.unconcealTile();
                                    lastAction.representingTile.unhover();
                                    lastAction.representingTile.interactive = false;
                                    lastAction.representingTile.Margin = new Thickness(6, 6, 6, 6);
                                    lastAction.representingTile.glow();
                                    discardPanel.Children.Add(lastAction.representingTile);
                                }
                            }
                        }
                    }
                    switch (maxChoice)
                    {
                        case 5:
                            currentPlayer = playerActions.First(e => e.Value.typeOfAction == 5).Key;
                            endTurn = true;
                            if (currentPlayer is ComputerPlayer)
                            {
                                if (!currentPlayer.exposeAllTiles)
                                {
                                    currentPlayer.toggleExposeTile();
                                }
                            }
                            HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, false, prevailingWind, currentPlayer.wind);
                            WinWindow ww = new WinWindow(prevailingWind, currentPlayer.wind, currentPlayer.ownTiles, h.faanPairs, 1000);
                            ww.ShowDialog();
                            break;
                        case 4:
                            currentPlayer = playerActions.First(e => e.Value.typeOfAction == 4).Key;
                            Array.ForEach(players, (e) => e.unglow());
                            currentPlayer.glow();
                            playerIndex = Array.IndexOf(players, currentPlayer);
                            if (!(currentPlayer is UserPlayer))
                            {
                                dt.Start();
                                WaitForEvent(ewh);
                                ewh.Reset();
                                dt.Stop();
                            }
                            lastAction = playerActions[currentPlayer];
                            if (discardedTiles.Contains(lastAction.representingTile))
                            {
                                discardedTiles.Remove(lastAction.representingTile);
                                discardPanel.Children.Remove(lastAction.representingTile);
                                lastAction.representingTile.unGlow();
                            }
                            currentPlayer.acceptAction();
                            drawTile(currentPlayer);
                            lastAction = new Action(0);
                            break;
                        case 3:
                            currentPlayer = playerActions.First(e => e.Value.typeOfAction == 3).Key;
                            playerIndex = Array.IndexOf(players, currentPlayer);
                            Array.ForEach(players, (e) => e.unglow());
                            currentPlayer.glow();
                            if (!(currentPlayer is UserPlayer))
                            {
                                dt.Start();
                                WaitForEvent(ewh);
                                ewh.Reset();
                                dt.Stop();
                            }
                            lastAction = playerActions[currentPlayer];
                            if (discardedTiles.Contains(lastAction.representingTile))
                            {
                                discardedTiles.Remove(lastAction.representingTile);
                                discardPanel.Children.Remove(lastAction.representingTile);
                                lastAction.representingTile.unGlow();
                            }
                            currentPlayer.acceptAction();
                            lastAction = new Action(0);
                            break;
                        case 2:
                            playerIndex = (playerIndex + 1) % 4;
                            currentPlayer = players[playerIndex];
                            Array.ForEach(players, (e) => e.unglow());
                            currentPlayer.glow();
                            if (!(currentPlayer is UserPlayer))
                            {
                                dt.Start();
                                WaitForEvent(ewh);
                                ewh.Reset();
                                dt.Stop();
                            }
                            if (playerActions[currentPlayer].typeOfAction == 2)
                            {
                                lastAction = playerActions[currentPlayer];
                                if (discardedTiles.Contains(lastAction.representingTile))
                                {
                                    discardedTiles.Remove(lastAction.representingTile);
                                    discardPanel.Children.Remove(lastAction.representingTile);
                                    lastAction.representingTile.unGlow();
                                }
                                currentPlayer.acceptAction();
                                lastAction = new Action(0);
                            }
                            else
                            {
                                lastAction = new Action(0);
                            }
                            break;
                        case 0:
                            playerIndex = (playerIndex + 1) % 4;
                            currentPlayer = players[playerIndex];
                            lastAction = new Action(0);
                            drawTile(currentPlayer);
                            Array.ForEach(players, (e) => e.unglow());
                            currentPlayer.glow();
                            break;
                        default:
                            MessageBox.Show("this line shouldn't run, check switch case in gameview.cs");
                            break;
                    }
                    if (!(currentPlayer is UserPlayer))
                    {
                        dt.Start();
                        WaitForEvent(ewh);
                        ewh.Reset();
                        dt.Stop();
                    }
                } while (!endTurn);
                foreach (Player p in players)
                {
                    if (p.points >= endingPoints)
                    {
                        endGame = true;
                    }
                    else
                    {
                        setUpGame();
                    }
                }
            } while (!endGame);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ewh.Set();
        }

        private void drawTile(Player p)
        {
            Tile drawnTile;
            do
            {
                drawnTile = availableTiles[rng.Next(availableTiles.Count)];
                p.addTile(drawnTile);
                availableTiles.Remove(drawnTile);
            } while (drawnTile.bonus);
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
    }
}