using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for Username.xaml
    /// </summary>
    public partial class Username : Window
    {
        public string username;
        public Username(string u)
        {
            InitializeComponent();
            username = u;
            usernameBox.Text = username;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            username = usernameBox.Text;
            DialogResult = true;
           
        }

        private void usernameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex(@"^\w+$").IsMatch(e.Text);
        }

        private void usernameBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }
    }
}
