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
        public List<Tile> ownTiles { get; protected set; }
        public List<Tile> walledTiles { get; protected set; }
        public Tile drawnTile { get; private set; }
        public int wind { get; private set; }
        public bool sortOn;

        public Player(int w)
        {
            InitializeComponent();
            ownTiles = new List<Tile>();
            walledTiles = new List<Tile>();
            wind = w;
        }

        public virtual void addTile(Tile t)
        {
            
        }

        public void updateTileDisplay()
        {
            ownTileDisplay.Children.Clear();
            walledTileDisplay.Children.Clear();
            foreach (Tile t in ownTiles)
            {
                ownTileDisplay.Children.Add(t);
            }
            foreach (Tile t in walledTiles)
            {
                walledTileDisplay.Children.Add(t);
            }
        }

        //rename later, doesn't need any code here
        protected virtual void temp(object sender, MouseButtonEventArgs e)
        {

        }

        public virtual void sortTiles()
        {
            if (!MainWindow.autoSort) return;
            bool sorted = false;
            Tile temp;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < ownTiles.Count-1; i++)
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
    }
}
