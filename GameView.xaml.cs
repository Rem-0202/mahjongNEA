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
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random rng = new Random();
            test.Children.Add(new Tile(rng.Next(1, 10), 's'));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < test.Children.Count; i++)
            {
                tiles.Add((Tile)test.Children[i]);
            }
            Tile temp = new Tile();
            bool sorted = false;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < tiles.Count - 1; i++)
                {
                    if (tiles[i] > tiles[i + 1])
                    {
                        sorted = false;
                        temp = tiles[i];
                        tiles[i] = tiles[i + 1];
                        tiles[i + 1] = temp;
                    }
                }
            }
            test.Children.Clear();
            foreach (Tile t in tiles)
            {
                test.Children.Add(t);
            }
        }
    }
}
