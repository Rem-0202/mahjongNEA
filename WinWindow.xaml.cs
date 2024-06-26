﻿using System;
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
        private static string[] windNames = { "東 East", "南 South", "西 West", "北 North" };
        public WinWindow(int pw, int uw, List<Tile> ts, Dictionary<string, int> faanPairs, int points, string name)
        {
            InitializeComponent();
            nameBlock.Text = name;
            pWindTB.Text = $"{windNames[pw]}";
            uWindTB.Text = $"{windNames[uw]}";
            scoreTB.Text = points.ToString();
            List<string> tileIDs = new List<string>();
            foreach (Tile t in ts)
            {
                tileDisplay.Children.Add(Tile.stringToTile(t.tileID));
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
