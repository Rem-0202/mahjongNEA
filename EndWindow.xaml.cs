using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for EndWindow.xaml
    /// </summary>
    public partial class EndWindow : Window
    {
        public EndWindow(Player[] players)
        {
            InitializeComponent();
            int[] ints = { 0, 1, 2, 3 };
            bool changed = false;
            int temp;
            while (!changed)
            {
                changed = true;
                for (int i = 0; i < players.Length - 1; i++)
                {
                    if (players[ints[i]].points < players[ints[i+1]].points)
                    {
                        temp = ints[i];
                        ints[i] = ints[i + 1];
                        ints[i + 1] = temp;
                        changed = false;
                    }
                }
            }
            name1.Text = players[ints[0]].name;
            name2.Text = players[ints[1]].name;
            name3.Text = players[ints[2]].name;
            name4.Text = players[ints[3]].name;
            points1.Text = players[ints[0]].points.ToString();
            points2.Text = players[ints[1]].points.ToString();
            points3.Text = players[ints[2]].points.ToString();
            points4.Text = players[ints[3]].points.ToString();
        }
    }
}
