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
    /// Interaction logic for ActionButton.xaml
    /// </summary>
    public partial class ActionButton : UserControl
    {
        public Action action { get; set; }
        public ActionButton(List<Action> allActions, string typeOfAction)
        {
            InitializeComponent();
            Margin = new Thickness(2, 2, 2, 2);
            actionButton.Content = typeOfAction;
            for (int i = 0; i < allActions.Count; i++)
            {
                actionSelectGrid.ColumnDefinitions.Add(new ColumnDefinition());
                StackPanel s = new StackPanel();
                foreach (Tile t in allActions[i].allTiles)
                {
                    s.Children.Add(t);
                }
                actionSelectGrid.Children.Add(s);
                Grid.SetRow(s, 2 * i);
            }
        }

        private void actionButton_Click(object sender, RoutedEventArgs e)
        {
            actionPopup.IsOpen = true;
        }

        private void tileGroup_MouseDown(object sender, MouseEventArgs e)
        {

        }
    }
}
