using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for newGameDialog.xaml
    /// </summary>
    public partial class newGameDialog : Window
    {
        private static Regex numbersOnlyRegex = new Regex("[0-9]+");
        public static int sPoints = 15000;
        public static int ePoints = 30000;
        public static int pWind = 0;
        public static int uWind = 0;
        public newGameDialog()
        {
            InitializeComponent();
            startingPoints.Text = sPoints.ToString();
            endingPoints.Text = ePoints.ToString();
            pWindSelector.SelectedIndex = pWind;
            uWindSelector.SelectedIndex = uWind;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !numbersOnlyRegex.IsMatch(e.Text);
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            pWind = pWindSelector.SelectedIndex;
            uWind = uWindSelector.SelectedIndex;
            int sp = Convert.ToInt32(startingPoints.Text);
            int ep = Convert.ToInt32(endingPoints.Text);
            if ((ep - sp) < 1000)
            {
                MessageBox.Show("Ending points has to be at least 1000 larger than starting points!");
            }
            else if (sp < 1000)
            {
                MessageBox.Show("Starting points must be more than 1000!");
            }
            else
            {
                DialogResult = true;
                sPoints = sp;
                ePoints = ep;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
