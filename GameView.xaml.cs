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
        public static Random rng = new Random();
        public int prevailingWind { get; private set; }
        public int playerWind { get; private set; }
        public List<Tile> availableTiles { get; private set; }
        public Player[] players { get; private set; }
        public int startingPoints { get; private set; }
        public int endingPoints { get; private set; }
        public Player currentPlayer { get; private set; }
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

        public void temptest()
        {
            players[playerWind].ownTurn = true;
            Action ta = new Action(0);
            ta = players[playerWind].getAction(ta);
            MessageBox.Show(ta.representingTile.tileID);
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
            bool end = false;
            int playerIndex = prevailingWind;
            currentPlayer = players[playerIndex];
            Tile drawnTile;
            Dictionary<Player, Action> playerActions = new Dictionary<Player, Action>();
            do
            {
                drawnTile = availableTiles[rng.Next(availableTiles.Count)];
                currentPlayer.addTile(drawnTile);
                availableTiles.Remove(drawnTile);
            } while (drawnTile.bonus);
            Array.ForEach(players, e => e.ownTurn = false);
            currentPlayer.ownTurn = true;
            Action lastAction = currentPlayer.getAction(new Action(0));
            do
            {
                currentPlayer = players[playerIndex];
                Array.ForEach(players, e => e.ownTurn = false);
                currentPlayer.ownTurn = true;
                if (lastAction.typeOfAction != 3 || lastAction.typeOfAction != 2)
                {
                    do
                    {
                        drawnTile = availableTiles[rng.Next(availableTiles.Count)];
                        currentPlayer.addTile(drawnTile);
                        availableTiles.Remove(drawnTile);
                    } while (drawnTile.bonus);
                }
                if (lastAction.typeOfAction == 4 || lastAction.typeOfAction == 3 || lastAction.typeOfAction == 2)
                {
                    lastAction = currentPlayer.getAction(new Action(0));
                }
                currentPlayer.acceptAction();
                playerActions.Clear();
                playerActions.Add(players[(playerIndex + 1) % 4], players[(playerIndex + 1) % 4].getAction(lastAction));
                playerActions.Add(players[(playerIndex + 2) % 4], players[(playerIndex + 2) % 4].getAction(lastAction));
                playerActions.Add(players[(playerIndex + 3) % 4], players[(playerIndex + 3) % 4].getAction(lastAction));
                int maxChoice = -1;
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
                    lastAction = playerActions[p];
                    playerIndex = Array.IndexOf(players, p);
                    //TODO: implement pong kong display
                }
                else
                {
                    if (playerActions[currentPlayer].typeOfAction == 2)
                    {
                        //TODO: implement chow display
                        lastAction = playerActions[currentPlayer];
                    }
                    else
                    {
                        //TODO: implement discard display
                        playerIndex = (playerIndex + 1) % 4;
                    }
                }
            } while (!end);
        }
    }
}