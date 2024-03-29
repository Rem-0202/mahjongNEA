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
using System.Windows.Shapes;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for WinWindow.xaml
    /// </summary>
    public partial class WinWindow : Window
    {
        private static string windNames = "東E南S西W北N";
        public WinWindow(int pw, int uw, List<Tile> ts, Dictionary<string, int> faanPairs, int points, List<Action> walledGroups)
        {
            InitializeComponent();
            pWindTB.Text = $"{windNames[pw * 2]}{windNames[pw * 2 + 1]}";
            uWindTB.Text = $"{windNames[uw * 2]}{windNames[uw * 2 + 1]}";
            scoreTB.Text = points.ToString();
            foreach (Tile t in ts)
            {
                tileDisplay.Children.Add(Tile.stringToTile(t.tileID));
            }
            foreach (Action a in walledGroups)
            {
                foreach (Tile t in a.allTiles)
                {
                    tileDisplay.Children.Add(new Tile(t.rank, t.suit, false));
                }
            }
            for (int i = 0; i < faanPairs.Keys.Count; i++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = faanPairs.Keys.ElementAt(i);
                Grid.SetColumn(tb, i > 6 ? 3 : 0);
                Grid.SetRow(tb, i % 7);
                faanDisplayGrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = faanPairs.Values.ElementAt(i).ToString();
                Grid.SetColumn(tb, i > 6 ? 4 : 1);
                Grid.SetRow(tb, i % 7);
                faanDisplayGrid.Children.Add(tb);
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
