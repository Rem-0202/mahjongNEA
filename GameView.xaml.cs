using System;
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
        private int score;
        public static Random rng = new Random();
        public int prevailingWind { get; private set; }
        public int playerWind { get; private set; }
        public List<Tile> availableTiles { get; private set; }
        public Player[] players { get; private set; }
        public int startingPoints { get; private set; }
        public int endingPoints { get; private set; }
        private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
        private bool exposedTile = false;
        private List<Tile> discardedTiles = new List<Tile>();
        private string tempStartup;
        private int discardedPlayerIndex;
        public GameView(int prevailingWind, int playerWind, int startingPoints, int endingPoints)
        {
            InitializeComponent();
            this.playerWind = playerWind;
            this.prevailingWind = prevailingWind;
            this.startingPoints = startingPoints;
            this.endingPoints = endingPoints;
            using (StreamReader sr = new StreamReader("startupcheck.txt"))
            {
                tempStartup = sr.ReadLine();
                username = sr.ReadLine();
            }
            usernameBox.Text = username;
        }

        private void usernameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !usernameRegex.IsMatch(e.Text);
        }

        private void setNames()
        {
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
                usernameGrid.Children.Clear();
                main.Children.Remove(usernameGrid);
                setUpGame();
                gameLoop();
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
                    tempPlayers[Array.IndexOf(players, p)] = new ComputerPlayer(p.wind, p.points, p.pWind, p.name);
                }
                else if (p is UserPlayer)
                {
                    tempPlayers[Array.IndexOf(players, p)] = new UserPlayer(p.wind, p.points, userActionButtons, p.pWind, p.name);
                }
            }
            if (exposedTile) exposeTiles(); else unExposeTiles();
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
            exposedTile = true;
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
            exposedTile = false;
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
            Player currentPlayer;
            bool endTurn = false;
            bool endGame = false;
            int roundNumber = 0;
            int playerIndex = prevailingWind;
            int maxChoice;
            DispatcherTimer dt = new DispatcherTimer();
            do
            {
                Dictionary<Player, Action> playerActions = new Dictionary<Player, Action>();
                currentPlayer = players[playerIndex];
                lastAction = new Action(0);
                drawTile(currentPlayer);
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
                    if (lastAction.typeOfAction == 0)
                    {
                        roundNumber++;
                        lastAction = currentPlayer.getAction(lastAction);
                        if (lastAction.typeOfAction == 5)
                        {
                            endTurn = true;
                            HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, true, prevailingWind, currentPlayer.wind);
                            score = Analysis.faanToScore(h.faan, true);
                            WinWindow ww = new WinWindow(prevailingWind, playerIndex, h.tempFullTS, h.faanPairs, score, currentPlayer.actionsDone, currentPlayer.name);
                            currentPlayer.exposeTile();
                            currentPlayer.changePointsByAmount(score);
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
                            currentPlayer.acceptAction();
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
                        lastAction.representingTile.Margin = new Thickness(6);
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
                                roundNumber++;
                                lastAction = currentPlayer.getAction(lastAction);
                                if (lastAction.typeOfAction == 5)
                                {
                                    endTurn = true;
                                    currentPlayer.exposeTile();
                                    HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, true, prevailingWind, currentPlayer.wind);
                                    score = Analysis.faanToScore(h.faan, true);
                                    WinWindow ww = new WinWindow(prevailingWind, currentPlayer.wind, h.tempFullTS, h.faanPairs, score, currentPlayer.actionsDone, currentPlayer.name);
                                    ww.ShowDialog();
                                    currentPlayer.changePointsByAmount(score);
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
                            currentPlayer.exposeTile();
                            discardedTiles.Remove(lastAction.representingTile);
                            discardPanel.Children.Remove(lastAction.representingTile);
                            currentPlayer.addTile(lastAction.representingTile);
                            HandCheck h = new HandCheck(currentPlayer.ownTiles, currentPlayer.actionsDone, currentPlayer.bonusTiles, false, prevailingWind, currentPlayer.wind);
                            score = Analysis.faanToScore(h.faan, false);
                            WinWindow ww = new WinWindow(prevailingWind, currentPlayer.wind, h.tempFullTS, h.faanPairs, score, currentPlayer.actionsDone, currentPlayer.name);
                            currentPlayer.changePointsByAmount(score);

                            //temp:
                            foreach (Player p in players)
                            {
                                if (p != currentPlayer)
                                {
                                    p.changePointsByAmount(-score / 3);
                                }
                            }
                            //tempend


                            //DEDUCT FROM OTHER PLAYERS
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
                    setUpGame();
                }
            } while (!endGame);
            //end game screen
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