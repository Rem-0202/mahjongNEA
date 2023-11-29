using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for Player.xaml   
    /// </summary>
    public partial class Player : UserControl
    {
        public bool ownTurn;
        public Action lastAction { get; protected set; }
        public List<Tile> ownTiles { get; protected set; }
        public List<Tile> walledTiles { get; protected set; }
        public List<Tile> bonusTiles { get; protected set; }
        public Tile drawnTile { get; private set; }
        public int wind { get; private set; }  //0 = 東(E)  1 = 南(S)  2 = 西(W)  3 = 北(N)
        public int points { get; private set; }

        public Player(int w, int points)
        {
            InitializeComponent();
            ownTiles = new List<Tile>();
            walledTiles = new List<Tile>();
            bonusTiles = new List<Tile>();
            this.points = points;
            wind = w;
        }

        public virtual void addTile(Tile t)
        {
            if (t.bonus)
            {
                t.interactive = false;
                t.unconcealTile();
                bonusTiles.Add(t);
            }
            else
            {
                ownTiles.Add(t);
            }
            sortTiles();
            updateTileDisplay();
        }

        public void updateTileDisplay()
        {
            ownTileDisplay.Children.Clear();
            walledTileDisplay.Children.Clear();
            bonusTileDisplay.Children.Clear();
            foreach (Tile t in ownTiles)
            {
                ownTileDisplay.Children.Add(t);
            }
            foreach (Tile t in walledTiles)
            {
                walledTileDisplay.Children.Add(t);
            }
            foreach (Tile t in bonusTiles)
            {
                bonusTileDisplay.Children.Add(t);
            }
        }

        public virtual void sortTiles()
        {
            if (!MainWindow.autoSort) return;
            bool sorted = false;
            Tile temp;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < ownTiles.Count - 1; i++)
                {
                    if (ownTiles[i] > ownTiles[i + 1])
                    {
                        sorted = false;
                        temp = ownTiles[i];
                        ownTiles[i] = ownTiles[i + 1];
                        ownTiles[i + 1] = temp;
                    }
                }
            }
            updateTileDisplay();
        }

        protected virtual void OwnTileDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void flipTiles()
        {
            walledTileDisplay.LayoutTransform = new RotateTransform(180.0);
            bonusTileDisplay.LayoutTransform = new RotateTransform(180.0);
        }

        public bool changePointsByAmount(int c)
        {
            points += c;
            return points <= 0;
        }

        public virtual Action getAction(Action a)
        {
            throw new NotImplementedException();
        }

        public virtual void acceptAction()
        {
            throw new NotImplementedException();
        }
    }
}
