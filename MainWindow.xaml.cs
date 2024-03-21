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
using System.Windows.Threading;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameView g;
        public StartPageUserControl s;
        public static bool autoSort = true;
        public MainWindow()
        {
            InitializeComponent();
            s = new StartPageUserControl();
            s.PreviewMouseDown += frame_MouseDown;
            displayGrid.Children.Add(s);
        }

        private void frame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (s.tutorial != null)
            {
                if (s.tutorial == true)
                {
                    helpWindow h = new helpWindow();
                    h.ShowDialog();
                }
                displayGrid.Children.Clear();
                switch (new newGameDialog().ShowDialog())
                {
                    case true:
                        g = new GameView(newGameDialog.pWind, newGameDialog.uWind, newGameDialog.sPoints, newGameDialog.ePoints);
                        displayGrid.Children.Clear();
                        displayGrid.Children.Add(g);
                        restartButton.Visibility = Visibility.Visible;
                        restartButton.IsEnabled = true;
                        g.gameLoop();
                        break;
                    case false:
                        //s = new
                        break;
                    default:
                        break;
                }
            }
        }

        private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            helpWindow h = new helpWindow();
            h.ShowDialog();
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
            switch (new newGameDialog().ShowDialog())
            {
                case true:
                    g = new GameView(newGameDialog.pWind, newGameDialog.uWind, newGameDialog.sPoints, newGameDialog.ePoints);
                    displayGrid.Children.Clear();
                    displayGrid.Children.Add(g);
                    restartButton.Visibility = Visibility.Visible;
                    restartButton.IsEnabled = true;
                    g.gameLoop();
                    break;
                case false:
                    break;
                default:
                    break;
            }
        }

        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            g = new GameView(newGameDialog.pWind, newGameDialog.uWind, newGameDialog.sPoints, newGameDialog.ePoints);
            displayGrid.Children.Clear();
            displayGrid.Children.Add(g);
        }

        //temp new game for easier debug
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            exposeTileToggle.IsChecked = false;
            g = new GameView(1, 0, newGameDialog.sPoints, newGameDialog.ePoints);
            displayGrid.Children.Clear();
            displayGrid.Children.Add(g);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (g != null)
            {
                g.gameLoop();
            }
        }

        private void ExposeTileMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (g != null)
            {
                g.toggleExposeTiles();
            }
        }
    }
}
