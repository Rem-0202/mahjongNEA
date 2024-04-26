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

        private bool glowing;
        public int rank { get; private set; }
        public char suit { get; private set; }
        public int suitID { get; private set; }
        public string tileID { get; private set; }
        public bool terminal { get; private set; }
        public bool honour { get; private set; }
        public bool special { get; private set; }
        public bool bonus { get; private set; }
        public bool concealed { get; private set; }
        public bool dragon { get; private set; }

        private static int uniqueTileNumber = 0;

        private int tileNumber;

        public bool interactive;
        public bool hovered;
        public Tile()
        {
            InitializeComponent();
        }

        public Tile(int r, char s)
        {
            InitializeComponent();
            interactive = false;
            glowing = false;
            rank = r;
            suit = s;
            tileID = r.ToString() + s;
            terminal = (suit == 'm' || suit == 'p' || suit == 's') && (rank == 1 || rank == 9);
            honour = suit == 'z';
            dragon = honour && rank > 5 && rank < 9;
            suitID = suitOrder.IndexOf(suit);
            special = terminal || honour;
            bonus = suit == 'f' || suit == 'n';
            concealed = false;
            setImage();
            Height = tileImage.Height + 4;
            Width = tileImage.Width * 1.1;
            tileNumber = uniqueTileNumber;
            uniqueTileNumber++;
        }

        public Tile(int r, char s, bool _)
        {
            InitializeComponent();
            interactive = false;
            rank = r;
            suit = s;
            tileID = r.ToString() + s;
            terminal = (suit == 'm' || suit == 'p' || suit == 's') && (rank == 1 || rank == 9);
            honour = suit == 'z';
            dragon = honour && rank > 5 && rank < 9;
            suitID = suitOrder.IndexOf(suit);
            special = terminal || honour;
            bonus = suit == 'f' || suit == 'n';
            concealed = false;
            setImage();
            Height = tileImage.Height + 4;
            Width = tileImage.Width * 1.1;
            tileNumber = -1;
        }

        public static bool operator ==(Tile a, Tile b) => a.tileID == b.tileID;

        public static bool operator !=(Tile a, Tile b) => a.tileID != b.tileID;

        public static bool operator >(Tile a, Tile b) => a.suitID > b.suitID || (a.suitID == b.suitID && a.rank > b.rank);

        public static bool operator <(Tile a, Tile b) => a.suitID < b.suitID || (a.suitID == b.suitID && a.rank < b.rank);

        public void concealTile()
        {
            concealed = true;
            interactive = false;
            setImage();
        }

        public void unconcealTile()
        {
            concealed = false;
            setImage();
        }

        public void setImage()
        {
            string imageName = concealed ? "concealed" : tileID;
            BitmapImage bitmap = new BitmapImage(new Uri($"tiles\\{imageName}.jpg", UriKind.Relative));
            tileImage.Height = bitmap.Height * 1.9;
            tileImage.Width = bitmap.Width * 1.9;
            tileBorder.Height = tileImage.Height + 4;
            tileBorder.Width = tileImage.Width + 4;
            tileImage.Source = bitmap;
        }

        private void tileImage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!concealed && interactive)
            {
                Height += tileImage.ActualHeight / 4;
                tileBorder.Margin = new Thickness(0, 0, 0, tileImage.ActualHeight / 4);
                hovered = true;
            }
        }

        private void tileImage_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!concealed && interactive)
            {
                Height -= tileImage.ActualHeight / 4;
                tileBorder.Margin = new Thickness(0, 0, 0, 0);
                hovered = false;
            }
        }

        public void unhover()
        {
            if (hovered)
            {
                Height -= tileImage.ActualHeight / 4;
                tileBorder.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        public void setRotated()
        {
            setImage();
            LayoutTransform = new RotateTransform(270);
            Height = tileImage.Height + 4;
            Width = tileImage.Width * 1.1;
        }

        public static Tile stringToTile(string s)
        {
            return new Tile(Convert.ToInt32(s[0].ToString()), s[1], false);
        }

        public void glow()
        {
            Height += 10;
            Width += 10;
            tileBorder.BorderThickness = new Thickness(3);
            tileBorder.BorderBrush = Brushes.Yellow;
            glowing = true;
        }

        public void unGlow()
        {
            if (glowing)
            {
                Height -= 10;
                Width -= 10;
                tileBorder.BorderThickness = new Thickness(0);
                tileBorder.BorderBrush = Brushes.Transparent;
                glowing = false;
            }
        }
    }
}
