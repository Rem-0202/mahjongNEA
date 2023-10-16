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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>

    public partial class Tile : UserControl
    {
        public static List<char> suitOrder = new List<char>() { 'm', 'p', 's', 'z', 'f', 'n' };
        public int rank { get; private set; }
        public char suit { get; private set; }
        public string tileID { get; private set; }
        public bool terminal { get; private set; }
        public bool honour { get; private set; }
        public bool special { get; private set; }
        public bool bonus { get; private set; }
        public Tile()
        {
            InitializeComponent();
        }

        public Tile(int r, char s)
        {
            InitializeComponent();
            rank = r;
            suit = s;
            tileID = r.ToString() + s;
            terminal = (suit == 'm' || suit == 'p' || suit == 's') && (rank == 1 || rank == 9);
            honour = suit == 'z';
            special = terminal || honour;
            bonus = suit == 'f' || suit == 'n';
            name.Text = tileID;
            name.FontSize = 13;
        }

        public static bool operator ==(Tile a, Tile b) => a.tileID == b.tileID;

        public static bool operator !=(Tile a, Tile b) => a.tileID != b.tileID;

        public static bool operator >(Tile a, Tile b) => suitOrder.IndexOf(a.suit) == suitOrder.IndexOf(b.suit) ? a.rank > b.rank : suitOrder.IndexOf(a.suit) > suitOrder.IndexOf(b.suit);

        public static bool operator <(Tile a, Tile b) => suitOrder.IndexOf(a.suit) == suitOrder.IndexOf(b.suit) ? a.rank < b.rank : suitOrder.IndexOf(a.suit) < suitOrder.IndexOf(b.suit);
        

    }
}
