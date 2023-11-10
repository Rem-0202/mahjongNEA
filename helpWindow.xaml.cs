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
            LoadFlowDocumentToRTB(@"../../tileNamesHelp.xaml");
        }

        private void roundProgressionHelp(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "Round Progression: ";
            LoadFlowDocumentToRTB(@"../../roundProgression.xaml");
        }

        private void roundsDisruptions(object sender, RoutedEventArgs e)
        {
            helpTitle.Text = "Disruption to Rounds";
            LoadFlowDocumentToRTB(@"../../roundDisruptions.xaml");
        }
    }
}
