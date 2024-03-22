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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for helpWindow.xaml
    /// </summary>
    public partial class helpWindow : Window
    {
        public helpWindow()
        {
            InitializeComponent();
        }

        private void collapseButton(object sender, RoutedEventArgs e)
        {
            leftListView.Visibility = leftListView.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadFlowDocumentToRTB(string fileName)
        {
            //load stuff
            FileStream fStream;
            if (File.Exists(fileName))
            {
                fStream = new FileStream(fileName, FileMode.Open);
                FlowDocument flowDocument = XamlReader.Load(fStream) as FlowDocument;
                fStream.Close();
                RTB.Document = flowDocument;
            }
        }

        private void tileNamesHelp(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "Tile Names: ";
            LoadFlowDocumentToRTB(@"helpDocs\tileNamesHelp.xaml");
            winningHandsCollapse();
        }

        private void roundProgressionHelp(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "Round Progression: ";
            LoadFlowDocumentToRTB(@"helpDocs\roundProgression.xaml");
            winningHandsCollapse();
        }

        private void roundsDisruptions(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "Disruption to Rounds";
            LoadFlowDocumentToRTB(@"helpDocs\roundDisruptions.xaml");
            winningHandsCollapse();
        }

        private void winningHandsDrop(object sender, RoutedEventArgs e)
        {
            if (winningHandsListItem.Content.ToString() == "Winning hands ⮝")
            {
                winningHandsCollapse();
            }
            else
            {
                winningHandsDropDown();
            }
        }

        private void winningHandsDropDown()
        {
            winningHandsItems1.Visibility = Visibility.Visible;
            winningHandsItems2.Visibility = Visibility.Visible;
            winningHandsItems3.Visibility = Visibility.Visible;
            winningHandsItems5.Visibility = Visibility.Visible;
            winningHandsItems6.Visibility = Visibility.Visible;
            winningHandsItems7.Visibility = Visibility.Visible;
            winningHandsItems8.Visibility = Visibility.Visible;
            winningHandsItems10.Visibility = Visibility.Visible;
            winningHandsItems13.Visibility = Visibility.Visible;
            winningHandsListItem.Content = "Winning hands ⮝";
        }

        private void winningHandsCollapse()
        {
            winningHandsItems1.Visibility = Visibility.Hidden;
            winningHandsItems2.Visibility = Visibility.Hidden;
            winningHandsItems3.Visibility = Visibility.Hidden;
            winningHandsItems5.Visibility = Visibility.Hidden;
            winningHandsItems6.Visibility = Visibility.Hidden;
            winningHandsItems7.Visibility = Visibility.Hidden;
            winningHandsItems8.Visibility = Visibility.Hidden;
            winningHandsItems10.Visibility = Visibility.Hidden;
            winningHandsItems13.Visibility = Visibility.Hidden;
            winningHandsListItem.Content = "Winning hands ⮟";
        }

        private void WinningHandsItems1_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "1 Faan Winning Hands";
            LoadFlowDocumentToRTB(@"faans\faan1.xaml");
        }

        private void WinningHandsItems2_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "2 Faan Winning Hands";
            LoadFlowDocumentToRTB(@"faans\faan2.xaml");
        }

        private void WinningHandsItems3_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "3 Faan Winning Hands";
            LoadFlowDocumentToRTB(@"faans\faan3.xaml");
        }

        private void WinningHandsItems5_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "5 Faan Winning Hands";
            LoadFlowDocumentToRTB(@"faans\faan5.xaml");
        }

        private void WinningHandsItems6_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "6 Faan Winning Hands";
            LoadFlowDocumentToRTB(@"faans\faan6.xaml");
        }

        private void WinningHandsItems7_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "7 Faan Winning Hands";
            LoadFlowDocumentToRTB(@"faans\faan7.xaml");
        }

        private void WinningHandsItems8_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "8 Faan Winning Hands";
            LoadFlowDocumentToRTB(@"faans\faan8.xaml");
        }

        private void WinningHandsItems10_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "10 Faan Winning Hands";
            LoadFlowDocumentToRTB(@"faans\faan10.xaml");
        }

        private void WinningHandsItems13_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "13 Faan Winning Hands (Maximum Faan)";
            LoadFlowDocumentToRTB(@"faans\faan13.xaml");
        }

        private void WinningHandsListItem_Selected(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "Winning Hands";
            LoadFlowDocumentToRTB(@"helpDocs\winningHands.xaml");
        }
    }
}
