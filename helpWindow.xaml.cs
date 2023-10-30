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

        void LoadXamlPackage(string fileName)
        {
            TextRange range;
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
            LoadXamlPackage(@"tileNamesHelp.xaml");
        }
    }
}
