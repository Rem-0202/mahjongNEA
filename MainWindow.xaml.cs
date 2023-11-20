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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameView g;
        public static bool autoSort = true;
        private static int prevailingWind;
        private static int playerWind;
        private static int startingPoints;
        private static int endingPoints;
        public MainWindow()
        {
            InitializeComponent();
        }
        Random rng = new Random();

        private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            helpWindow h = new helpWindow();
            h.Show();
        }

        private void autoSortChecked(object sender, RoutedEventArgs e)
        {
            autoSort = true;
            if (g != null)
            {
                g.toggleSort();
            }
        }

        private void autoSortUnchecked(object sender, RoutedEventArgs e)
        {
            autoSort = false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            newGameDialog ngd = new newGameDialog();
            bool? dresult = ngd.ShowDialog();
            switch (dresult)
            {
                case true:
                    prevailingWind = ngd.pWind;
                    playerWind = ngd.uWind;
                    startingPoints = ngd.sPoints;
                    endingPoints = ngd.ePoints;
                    g = new GameView(prevailingWind, playerWind, startingPoints, endingPoints);
                    displayGrid.Children.Clear();
                    displayGrid.Children.Add(g);
                    restartButton.Visibility = Visibility.Visible;
                    restartButton.IsEnabled = true;
                    break;
                case false:
                    break;
                default:
                    break;
            }
        }

        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            g = new GameView(prevailingWind, playerWind, startingPoints, endingPoints);
            displayGrid.Children.Clear();
            displayGrid.Children.Add(g);
        }
    }
}
