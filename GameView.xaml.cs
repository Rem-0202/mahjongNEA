﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public bool messyDiscard;
        public string username;
        private Action lastAction;
        private static Regex usernameRegex = new Regex(@"^[a-zA-Z]$");
        private int startingPlayerWind;
        private int score;
        public static Random rng = new Random();
        public int prevailingWind { get; private set; }
        public int playerWind { get; private set; }
        public List<Tile> availableTiles { get; private set; }
        public Player[] players { get; private set; }
        public int startingPoints { get; private set; }
        public int endingPoints { get; private set; }
        private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
        private List<Tile> discardedTiles = new List<Tile>();
        private string tempStartup;
        private int discardedPlayerIndex;

        public event EventHandler<string> gameviewStateChanged;
        public event EventHandler<string> gameState;
        //the two custom events for MainWindow to update toggles and status bar accordingly


        public GameView(int prevailingWind, int playerWind, int startingPoints, int endingPoints)
        {
            InitializeComponent();
            //initialize variables needed for starting a game
            this.playerWind = playerWind;
            startingPlayerWind = playerWind;
            this.prevailingWind = prevailingWind;
            this.startingPoints = startingPoints;
            this.endingPoints = endingPoints;
            using (StreamReader sr = new StreamReader("startupcheck.txt"))
            {
                tempStartup = sr.ReadLine();
                username = sr.ReadLine();
            }
            usernameBox.Text = username;
            //reads and set previously saved username
        }

        private void usernameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !usernameRegex.IsMatch(e.Text);
        }

        private void setNames()
        {
            //checks if username is allowed and save username to file for use next time
            if (usernameBox.Text.Length > 0 && usernameBox.Text.Length < 15)
            {
                username = usernameBox.Text;
                using (StreamWriter sw = new StreamWriter("startupcheck.txt"))
                {
                    sw.WriteLine(tempStartup);
                    sw.WriteLine(username);
                }
                players = new Player[4];
                players[playerWind % 4] = new UserPlayer(playerWind, startingPoints, userActionButtons, prevailingWind, username);
                players[(playerWind + 1) % 4] = new ComputerPlayer((playerWind + 1) % 4, startingPoints, prevailingWind, "CPU 1");
                players[(playerWind + 2) % 4] = new ComputerPlayer((playerWind + 2) % 4, startingPoints, prevailingWind, "CPU 2");
                players[(playerWind + 3) % 4] = new ComputerPlayer((playerWind + 3) % 4, startingPoints, prevailingWind, "CPU 3");
                //initialize players and names
                usernameGrid.Children.Clear();
                main.Children.Remove(usernameGrid);
                setUpGame();
                gameLoop();
                //starts the game loop
            }
            else
            {
                MessageBox.Show("Username must be between 1 to 15 characters long!");
            }
        }

        private void usernameBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = usernameBox.Text.Trim() == "";
            }
            if (e.Key == Key.Enter)
            {
                setNames();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            setNames();
        }

        private void usernameBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
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
                    tempPlayers[p.wind] = new ComputerPlayer(p.wind, p.points, p.pWind, p.name);
                }
                else if (p is UserPlayer)
                {
                    tempPlayers[p.wind] = new UserPlayer(playerWind, p.points, userActionButtons, p.pWind, p.name);
                }
            }
            unExposeTiles();
            players = tempPlayers;
            //resets players with their winds
            userPlayerGrid.Children.Add(players[playerWind % 4]);
            rightPlayerGrid.Children.Add(players[(playerWind + 1) % 4]);
            topPlayerGrid.Children.Add(players[(playerWind + 2) % 4]);
            leftPlayerGrid.Children.Add(players[(playerWind + 3) % 4]);
            players[(playerWind + 1) % 4].LayoutTransform = new RotateTransform(270.0);
            players[(playerWind + 2) % 4].LayoutTransform = new RotateTransform(180.0);
            players[(playerWind + 2) % 4].flipTiles();
            players[(playerWind + 3) % 4].LayoutTransform = new RotateTransform(90.0);
            //places players in their seats, user player should always be at the bottom facing the user 
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
            //initialize all the tiles needed in one match
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
            gameviewStateChanged.Invoke(this, "endSetup");
        }

        public void toggleSort()
        {
            if (players != null)
            {
                foreach (Player p in players)
                {
                    p.sortTiles();
                }
            }
        }

        public void exposeTiles()
        {
            if (players != null)
            {
                foreach (Player p in players)
                {
                    p.exposeTile();
                }
            }
        }

        public void unExposeTiles()
        {
            if (players != null)
            {
                foreach (Player p in players)
                {
                    p.unExposeTile();
                }
            }
        }

        public void gameLoop()
        {
            /*
             * Overall working of the gameloop:
             *   requests the current player for an action
             *   requests all other three users with the previous action
             *   accepts based on the type of the action (win > kong > pong > chow > discard)
             * 
             * The game loop also handles most of the visuals of the game
             *   visuals update according to actions performed by players
             *   show other windows upon end of turn or game
             * 
             */
            //declare necessary variables for a game
            Player currentPlayer;
            bool endTurn = false;
            bool endGame = false;
            int playerIndex;
            Dictionary<Player, Action> playerActions;
            int maxChoice;
            DispatcherTimer dt = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            dt.Tick += timer_Tick;
            //repeats the game until any player has a negative score or reaches ending score
            do
            {
                playerIndex = prevailingWind;
                playerActions = new Dictionary<Player, Action>();
                currentPlayer = players[playerIndex];
                lastAction = new Action(0);
                drawTile(currentPlayer);
                //main loop of a game
                do
                {
                    currentPlayer = players[playerIndex];
                    Array.ForEach(players, (e) => e.unglow());
                    currentPlayer.glow();
                    Array.ForEach(players, e => e.ownTurn = false);
                    Array.ForEach(players, e => e.nextTurn = false);
                    currentPlayer.ownTurn = true;
                    players[(playerIndex + 1) % 4].nextTurn = true;
                    if (lastAction.typeOfAction == 0)
                    {
                        lastAction = currentPlayer.getAction(lastAction);
                        if (lastAction.typeOfAction == 5)
                        {
                            endTurn = true;
                            HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, true, prevailingWind, currentPlayer.wind);
                            score = Analysis.faanToScore(h.faan, true);
                            WinWindow ww = new WinWindow(prevailingWind, playerIndex, h.tempFullTS, h.faanPairs, score, currentPlayer.name);
                            currentPlayer.exposeTile();
                            currentPlayer.changePointsByAmount(score);
                            gameState.Invoke(this, $"{currentPlayer.name} Won");
                            foreach (Player p in players)
                            {
                                if (p != currentPlayer)
                                {
                                    p.changePointsByAmount(-score / 3);
                                }
                            }
                            ww.ShowDialog();
                            break;
                        }
                        if (lastAction.typeOfAction == 4)
                        {
                            gameState.Invoke(this, $"{currentPlayer.name} declared a Kong");
                            currentPlayer.acceptAction();
                            lastAction = currentPlayer.getAction(new Action(0));
                        }
                        currentPlayer.acceptAction();
                        gameState.Invoke(this, $"{currentPlayer.name} discarded a tile");
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
                        lastAction.representingTile.Margin = new Thickness(6);
                        lastAction.representingTile.glow();
                        discardPanel.Children.Add(lastAction.representingTile);
                        discardedPlayerIndex = playerIndex;
                        gameState.Invoke(this, $"{currentPlayer.name} discarded a tile");
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
                        if (maxChoice == 4)
                        {
                            MessageBox.Show("No tiles left. Ending turn.");
                            endTurn = true;
                            break;
                        }
                        else
                        {
                            if (lastAction.typeOfAction == 0)
                            {
                                lastAction = currentPlayer.getAction(lastAction);
                                if (lastAction.typeOfAction == 5)
                                {
                                    endTurn = true;
                                    currentPlayer.exposeTile();
                                    HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, true, prevailingWind, currentPlayer.wind);
                                    score = Analysis.faanToScore(h.faan, true);
                                    WinWindow ww = new WinWindow(prevailingWind, currentPlayer.wind, h.tempFullTS, h.faanPairs, score, currentPlayer.name);
                                    ww.ShowDialog();
                                    currentPlayer.changePointsByAmount(score);
                                    gameState.Invoke(this, $"{currentPlayer.name} Won");
                                    foreach (Player p in players)
                                    {
                                        if (p != currentPlayer)
                                        {
                                            p.changePointsByAmount(-score / 3);
                                        }
                                    }
                                    break;
                                }
                                currentPlayer.acceptAction();
                                if (lastAction.typeOfAction == 1)
                                {
                                    gameState.Invoke(this, $"{currentPlayer.name} discarded a tile");
                                    if (discardPanel.Children.Count > 0)
                                    {
                                        Tile temp = (Tile)discardPanel.Children[discardPanel.Children.Count - 1];
                                        temp.unGlow();
                                    }
                                    discardedTiles.Add(lastAction.representingTile);
                                    lastAction.representingTile.unconcealTile();
                                    lastAction.representingTile.unhover();
                                    lastAction.representingTile.interactive = false;
                                    lastAction.representingTile.Margin = new Thickness(6);
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
                            discardedTiles.Remove(lastAction.representingTile);
                            discardPanel.Children.Remove(lastAction.representingTile);
                            currentPlayer.addTile(lastAction.representingTile);
                            currentPlayer.exposeTile();
                            HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, false, prevailingWind, currentPlayer.wind);
                            score = Analysis.faanToScore(h.faan, false);
                            WinWindow ww = new WinWindow(prevailingWind, currentPlayer.wind, h.tempFullTS, h.faanPairs, score, currentPlayer.name);
                            currentPlayer.changePointsByAmount(score);
                            players[discardedPlayerIndex].changePointsByAmount(-score);
                            gameState.Invoke(this, $"{currentPlayer.name} Won");
                            ww.ShowDialog();
                            break;
                        case 4:
                            currentPlayer = playerActions.First(e => e.Value.typeOfAction == 4).Key;
                            gameState.Invoke(this, $"{currentPlayer.name} declared a Kong");
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
                            gameState.Invoke(this, $"{currentPlayer.name} declared a Pong");
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
                            discardedPlayerIndex = playerIndex;
                            playerIndex = (playerIndex + 1) % 4;
                            currentPlayer = players[playerIndex];
                            gameState.Invoke(this, $"{currentPlayer.name} declared a Chow");
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
                            discardedPlayerIndex = playerIndex;
                            playerIndex = (playerIndex + 1) % 4;
                            currentPlayer = players[playerIndex];
                            lastAction = new Action(0);
                            drawTile(currentPlayer);
                            Array.ForEach(players, (e) => e.unglow());
                            currentPlayer.glow();
                            break;
                        default:
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
                endTurn = false;
                foreach (Player p in players)
                {
                    if (p.points >= endingPoints || p.points < 0)
                    {
                        endGame = true;
                    }
                }
                if (!endGame)
                {
                    gameviewStateChanged.Invoke(this, "endTurn");
                    userActionButtons.Children.Clear();
                    foreach (Player p in players)
                    {
                        p.wind = (p.wind + 1) % 4;
                    }
                    playerWind = (playerWind + 1) % 4;
                    if (playerWind == startingPlayerWind)
                    {
                        endGame = true;
                        break;
                    }
                    setUpGame();
                }
            } while (!endGame);
            EndWindow ew = new EndWindow(players);
            ew.ShowDialog();
            gameviewStateChanged.Invoke(this, "endGame");
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            usernameBox.Focus();
            usernameBox.SelectAll();
        }
    }
}