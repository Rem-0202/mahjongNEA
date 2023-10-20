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

        private void MenuItem_Temp_Click(object sender, RoutedEventArgs e)
        {
            tempStackPanel.Children.Add(new Tile(1, 'z'));
        }
    }
}
