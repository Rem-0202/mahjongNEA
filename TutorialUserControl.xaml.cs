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
    /// Interaction logic for TutorialUserControl.xaml
    /// </summary>
    public partial class TutorialUserControl : UserControl
    {
        public bool cancel;
        private EventWaitHandle ewh;
        private int currentTutorialIndex;
        public TutorialUserControl(EventWaitHandle ewh)
        {
            InitializeComponent();
            this.ewh = ewh;
            cancel = false;
            currentTutorialIndex = 0;
        }

        private void nextPageButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancel = true;
            ewh.Set();
        }

        private void previousPageButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
