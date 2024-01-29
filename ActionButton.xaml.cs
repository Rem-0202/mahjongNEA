using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public bool clicked;
        private List<Action> actions;
        public EventWaitHandle ewh;
        public ActionButton(List<Action> allActions, string typeOfAction, ref EventWaitHandle ewh)
        {
            InitializeComponent();
            clicked = false;
            this.ewh = ewh;
            actions = allActions;
            Margin = new Thickness(2, 2, 2, 2);
            actionButton.Content = typeOfAction;
            for (int i = 0; i < allActions.Count; i++)
            {
                actionSelectGrid.ColumnDefinitions.Add(new ColumnDefinition());
                StackPanel s = new StackPanel();
                s.Background = Brushes.Transparent;
                s.Margin = new Thickness(1, 20, 10, 3);
                s.Orientation = Orientation.Horizontal;
                s.MouseDown += tileGroup_MouseDown;
                s.MouseEnter += stackPanel_MouseEnter;
                s.MouseLeave += stackPanel_MouseLeave;
                foreach (Tile t in allActions[i].allTiles)
                {
                    Tile x = new Tile(t.rank, t.suit);
                    s.Children.Add(x);
                }
                actionSelectGrid.Children.Add(s);
                Grid.SetColumn(s, i);
            }
        }

        public ActionButton(ref EventWaitHandle ewh)
        {
            InitializeComponent();
            this.ewh = ewh;
            action = new Action(0);
            actionButton.Click -= actionButton_Click;
            actionButton.Click += actionButton_Click_Skip;
            actionButton.Content = "Skip";
        }

        private void actionButton_Click(object sender, RoutedEventArgs e)
        {
            actionPopup.IsOpen = true;
        }

        private void actionButton_Click_Skip(object sender, RoutedEventArgs e)
        {
            clicked = true;
            ewh.Set();
        }

        private void tileGroup_MouseDown(object sender, MouseEventArgs e)
        {
            UIElement u = sender as UIElement;
            action = actions[Grid.GetColumn(u)];
            clicked = true;
            ewh.Set();
        }

        private void stackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            StackPanel u = sender as StackPanel;
            u.Height += u.ActualHeight / 4;
            u.Margin = new Thickness(1, 0, 10, u.ActualHeight / 4);
        }

        private void stackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            StackPanel u = sender as StackPanel;
            u.Height -= u.ActualHeight / 4;
            u.Margin = new Thickness(1, 20, 10, 3);
        }
    }
}
