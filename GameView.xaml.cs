using System;
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
        private List<Tile> discardedTiles = new List<Tile>();
        public GameView(int prevailingWind, int playerWind, int startingPoints, int endingPoints)
        {
            InitializeComponent();
            this.playerWind = playerWind;
            this.prevailingWind = prevailingWind;
            this.startingPoints = startingPoints;
            this.endingPoints = endingPoints;
            players = new Player[4];
            players[playerWind % 4] = new UserPlayer(playerWind, startingPoints, userActionButtons);
            players[(playerWind + 1) % 4] = new ComputerPlayer((playerWind + 1) % 4, startingPoints);
            players[(playerWind + 2) % 4] = new ComputerPlayer((playerWind + 2) % 4, startingPoints);
            players[(playerWind + 3) % 4] = new ComputerPlayer((playerWind + 3) % 4, startingPoints);
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

        public void toggleSort()
        {
            foreach (Player p in players)
            {
                p.sortTiles();
            }
        }

        public void gameLoop()
        {
            Player currentPlayer;
            bool end = false;
            int roundNumber = 0;
            int playerIndex = prevailingWind;
            int maxChoice;
            Dictionary<Player, Action> playerActions = new Dictionary<Player, Action>();
            //oldGameLoop
            /*
             * do
            {
                Array.ForEach(players, e => e.ownTurn = false);
                players[(playerIndex + 1) % 4].ownTurn = true;
                currentPlayer.acceptAction();
                firstDraw = true;
                if (lastAction.typeOfAction == 1)
                {
                    discardedTiles.Add(lastAction.representingTile);
                    lastAction.representingTile.unconcealTile();
                    lastAction.representingTile.unhover();
                    lastAction.representingTile.interactive = false;
                    lastAction.representingTile.Margin = new Thickness(5, 5, 5, 5);
                    discardPanel.Children.Add(lastAction.representingTile);
                }
                if (lastAction.typeOfAction == 4 || lastAction.typeOfAction == 3 || lastAction.typeOfAction == 2)
                {
                    if (lastAction.typeOfAction == 4)
                    {
                        do
                        {
                            drawnTile = availableTiles[rng.Next(availableTiles.Count)];
                            currentPlayer.addTile(drawnTile);
                            availableTiles.Remove(drawnTile);
                        } while (drawnTile.bonus);
                    }
                    lastAction = currentPlayer.getAction(new Action(0));
                }
                playerActions.Clear();
                firstDraw = true;
                playerActions.Add(players[(playerIndex + 1) % 4], players[(playerIndex + 1) % 4].getAction(lastAction));
                playerActions.Add(players[(playerIndex + 2) % 4], players[(playerIndex + 2) % 4].getAction(lastAction));
                playerActions.Add(players[(playerIndex + 3) % 4], players[(playerIndex + 3) % 4].getAction(lastAction));
                maxChoice = -1;
                foreach (Action a in playerActions.Values)
                {
                    maxChoice = Math.Max(a.typeOfAction, maxChoice);
                }
                currentPlayer = players[playerIndex];
                if (maxChoice == 5)
                {
                    //TODO: implement win round by player
                    end = true;
                }
                else if (maxChoice == 3 || maxChoice == 4)
                {
                    Player p = playerActions.First(e => e.Value.typeOfAction == 4 || e.Value.typeOfAction == 3).Key;
                    playerIndex = Array.IndexOf(players, p);
                    MessageBox.Show($"{lastAction.typeOfAction} {lastAction.representingTile.tileID}");
                }
                else
                {
                    playerIndex = (playerIndex + 1) % 4;
                    if (maxChoice == 2 && playerActions[players[playerIndex]].typeOfAction == 2)
                    {
                        MessageBox.Show($"{lastAction.typeOfAction} {lastAction.representingTile.tileID}");
                    }
                    else
                    {
                        MessageBox.Show($"{lastAction.typeOfAction} {lastAction.representingTile.tileID}");
                    }
                }
                currentPlayer = players[playerIndex];
                lastAction = playerActions[currentPlayer];
            } while (!end);
            */
            //oldGameLoop 2
            /*
             * do
            {
                currentPlayer = players[playerIndex];
                Array.ForEach(players, e => e.ownTurn = false);
                currentPlayer.ownTurn = true;
                if (availableTiles.Count == 0)
                {
                    //handle end turn
                    MessageBox.Show("ended");
                    end = true;
                    break;
                }
                if (lastAction.typeOfAction == 0)
                {
                    roundNumber++;
                    lastAction = currentPlayer.getAction(lastAction);
                    currentPlayer.acceptAction();
                }
                if (lastAction.typeOfAction == 1)
                {
                    discardedTiles.Add(lastAction.representingTile);
                    lastAction.representingTile.unconcealTile();
                    lastAction.representingTile.unhover();
                    lastAction.representingTile.interactive = false;
                    lastAction.representingTile.Margin = new Thickness(5, 5, 5, 5);
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
                if (maxChoice == 5)
                {
                    //prepare for end turn
                    end = true;
                    break;
                }
                else if (maxChoice == 3 || maxChoice == 4)
                {
                    Player p = playerActions.First(e => e.Value.typeOfAction == 4 || e.Value.typeOfAction == 3).Key;
                    playerIndex = Array.IndexOf(players, p);
                }
                else if (maxChoice == 1)
                {
                    playerIndex = (playerIndex + 1) % 4;
                }
                else if (lastAction.typeOfAction < 5 && lastAction.typeOfAction > 1)
                {
                    if (lastAction.typeOfAction == 4)
                    {
                        do
                        {
                            drawnTile = availableTiles[rng.Next(availableTiles.Count)];
                            currentPlayer.addTile(drawnTile);
                            availableTiles.Remove(drawnTile);
                        } while (drawnTile.bonus);
                    }
                    lastAction = new Action(0);
                    continue;
                }
                currentPlayer = players[playerIndex];
                if (lastAction.typeOfAction == 1)
                {
                    MessageBox.Show($"{lastAction.typeOfAction} {lastAction.representingTile.tileID}");
                }
                lastAction = playerActions[currentPlayer];
                if (discardedTiles.Contains(lastAction.representingTile))
                {
                    discardedTiles.Remove(lastAction.representingTile);
                    discardPanel.Children.Remove(lastAction.representingTile);
                }
                currentPlayer.acceptAction();
            } while (!end);
            */

            #region new new gameloop start
            currentPlayer = players[playerIndex];
            lastAction = new Action(0);
            drawTile(currentPlayer);
            do
            {
                currentPlayer = players[playerIndex];
                Array.ForEach(players, e => e.ownTurn = false);
                currentPlayer.ownTurn = true;
                //end turn handling
                if (availableTiles.Count == 0)
                {
                    //handle end turn
                    MessageBox.Show("ended");
                    end = true;
                    break;
                }

                if (lastAction.typeOfAction == 0)
                {
                    roundNumber++;
                    lastAction = currentPlayer.getAction(lastAction);
                    currentPlayer.acceptAction();
                }
                if (lastAction.typeOfAction == 1)
                {
                    discardedTiles.Add(lastAction.representingTile);
                    lastAction.representingTile.unconcealTile();
                    lastAction.representingTile.unhover();
                    lastAction.representingTile.interactive = false;
                    lastAction.representingTile.Margin = new Thickness(5, 5, 5, 5);
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
                switch (maxChoice)
                {
                    case 5:
                        end = true;
                        //handle end
                        break;
                    case 4:
                        currentPlayer = playerActions.First(e => e.Value.typeOfAction == 4 || e.Value.typeOfAction == 3).Key;
                        playerIndex = Array.IndexOf(players, currentPlayer);
                        lastAction = playerActions[currentPlayer];
                        if (discardedTiles.Contains(lastAction.representingTile))
                        {
                            discardedTiles.Remove(lastAction.representingTile);
                            discardPanel.Children.Remove(lastAction.representingTile);
                        }
                        currentPlayer.acceptAction();
                        drawTile(currentPlayer);
                        lastAction = new Action(0);
                        break;
                    case 3:
                        currentPlayer = playerActions.First(e => e.Value.typeOfAction == 4 || e.Value.typeOfAction == 3).Key;
                        playerIndex = Array.IndexOf(players, currentPlayer);
                        lastAction = playerActions[currentPlayer];
                        if (discardedTiles.Contains(lastAction.representingTile))
                        {
                            discardedTiles.Remove(lastAction.representingTile);
                            discardPanel.Children.Remove(lastAction.representingTile);
                        }
                        currentPlayer.acceptAction();
                        lastAction = new Action(0);
                        break;
                    case 2:
                        playerIndex = (playerIndex + 1) % 4;
                        currentPlayer = players[playerIndex];
                        if (playerActions[currentPlayer].typeOfAction == 2)
                        {
                            lastAction = playerActions[currentPlayer];
                            if (discardedTiles.Contains(lastAction.representingTile))
                            {
                                discardedTiles.Remove(lastAction.representingTile);
                                discardPanel.Children.Remove(lastAction.representingTile);
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
                        break;
                    default:
                        MessageBox.Show("this line shouldn't run, check switch case in gameview.cs");
                        break;
                }
                if (end) break;
            } while (!end);
            #endregion
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
    }
}