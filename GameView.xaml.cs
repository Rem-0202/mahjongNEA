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

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public static Random rng = new Random();
        public int prevailingWind { get; private set; }
        public int playerWind { get; private set; }
        public List<Tile> availableTiles { get; private set; }
        public Player[] players { get; private set; }
        public GameView(int prevailingWind, int playerWind)
        {
            InitializeComponent();
            this.playerWind = playerWind;
            this.prevailingWind = prevailingWind;
            players = new Player[4];
            players[playerWind-1] = new UserPlayer(playerWind);
            players[playerWind % 4] = new ComputerPlayer((playerWind + 1) % 4);
            players[(playerWind + 1 ) % 4] = new ComputerPlayer((playerWind + 2) % 4);
            players[(playerWind + 2 ) % 4] = new ComputerPlayer((playerWind + 3) % 4);
            userPlayerGrid.Children.Add(players[playerWind - 1]);
            rightPlayerGrid.Children.Add(players[playerWind % 4]);
            topPlayerGrid.Children.Add(players[(playerWind + 1) % 4]);
            leftPlayerGrid.Children.Add(players[(playerWind + 2) % 4]);
            players[playerWind % 4].LayoutTransform = new RotateTransform(270.0);
            players[(playerWind + 1) % 4].LayoutTransform = new RotateTransform(180.0);
            players[(playerWind + 2) % 4].LayoutTransform = new RotateTransform(90.0);
            availableTiles = new List<Tile>();
            for (int i = 1; i <= 9; i++)
            {
                availableTiles.Add(new Tile(i, 'm'));
                availableTiles.Add(new Tile(i, 'm'));
                availableTiles.Add(new Tile(i, 'm'));
                availableTiles.Add(new Tile(i, 'm'));
                availableTiles.Add(new Tile(i, 's'));
                availableTiles.Add(new Tile(i, 's'));
                availableTiles.Add(new Tile(i, 's'));
                availableTiles.Add(new Tile(i, 's'));
                availableTiles.Add(new Tile(i, 'p'));
                availableTiles.Add(new Tile(i, 'p'));
                availableTiles.Add(new Tile(i, 'p'));
                availableTiles.Add(new Tile(i, 'p'));
            }
            for (int i = 2; i <= 8; i++)
            {
                availableTiles.Add(new Tile(i, 'z'));
                availableTiles.Add(new Tile(i, 'z'));
                availableTiles.Add(new Tile(i, 'z'));
                availableTiles.Add(new Tile(i, 'z'));
            }
            for (int i = 2; i <= 5; i++)
            {
                availableTiles.Add(new Tile(i, 'n'));
                availableTiles.Add(new Tile(i, 'f'));
            }
            int k;
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 13; i++)
                {
                    k = rng.Next(availableTiles.Count);
                    players[j].addTile(availableTiles[k]);
                    availableTiles.RemoveAt(k);
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
    }
}