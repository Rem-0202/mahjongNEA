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

        public Player(int w)
        {
            InitializeComponent();
            ownTiles = new List<Tile>();
            walledTiles = new List<Tile>();
            wind = w;
        }

        public void addTile(Tile t)
        {
            ownTiles.Add(t);
            updateTileDisplay();
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
    }
}
