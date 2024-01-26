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
        public ActionButton(Action a)
        {
            InitializeComponent();
            Margin = new Thickness(2, 2, 2, 2);
            switch (a.typeOfAction)
            {
                case 2:
                    actionButton.Content = "Chow";
                    break;
                case 3:
                    actionButton.Content = "Pong";
                    break;
                case 4:
                    actionButton.Content = "Kong";
                    break;
            }
            action = a;
        }

        private void actionButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
        }
    }
}
