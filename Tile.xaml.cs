using System;
using System.Collections.Generic;
using System.IO;
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
        public int suitID { get; private set; }
        public string tileID { get; private set; }
        public bool terminal { get; private set; }
        public bool honour { get; private set; }
        public bool special { get; private set; }
        public bool bonus { get; private set; }
        public bool concealed { get; private set; }
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
            suitID = suitOrder.IndexOf(suit);
            special = terminal || honour;
            bonus = suit == 'f' || suit == 'n';
            concealed = false;
            setImage();
            Height = tileImage.Height;
            Width = tileImage.Width;
        }

        public static bool operator ==(Tile a, Tile b) => a.tileID == b.tileID;

        public static bool operator !=(Tile a, Tile b) => a.tileID != b.tileID;

        public static bool operator >(Tile a, Tile b) => a.suitID > b.suitID || (a.suitID == b.suitID && a.rank > b.rank);

        public static bool operator <(Tile a, Tile b) => a.suitID < b.suitID || (a.suitID == b.suitID && a.rank < b.rank);

        public void concealTile()
        {
            concealed = true;
            setImage();
        }

        public void setImage()
        {
            var bitmap = new BitmapImage();
            string imageName = concealed ? "concealed" : tileID;
            using (var stream = new FileStream($"../../{imageName}.jpg", FileMode.Open))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            tileImage.Height = bitmap.Height;
            tileImage.Width = bitmap.Width;
            tileImage.Source = bitmap;
        }

        private void tileImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Height += tileImage.ActualHeight / 4;
            tileImage.Margin = new Thickness(1, 0, 1, tileImage.ActualHeight/4);
        }

        private void tileImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Height -= tileImage.ActualHeight / 4;
            tileImage.Margin = new Thickness(1, 0, 1, 0);
        }
    }
}
