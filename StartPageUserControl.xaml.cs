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
using System.IO;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for StartPageUserControl.xaml
    /// </summary>
    public partial class StartPageUserControl : UserControl
    {
        public bool tutorial;
        public StartPageUserControl()
        {
            InitializeComponent();
            string firstStartUp;
            using (StreamReader sr = new StreamReader("startupcheck.txt"))
            {
                firstStartUp = sr.ReadLine();
                if (firstStartUp == "true")
                {
                    startButton.Content = "Start Tutorial";
                    secondaryButton.Content = "Skip Tutorial";
                    firstStartUp = "false";
                }
            }
            using (StreamWriter sr = new StreamWriter("startupcheck.txt"))
            {
                sr.WriteLine("false");
            }
        }

        private void startButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((string)startButton.Content == "Start Tutorial")
            {
                tutorial = true;
            }
            else tutorial = false;
        }

        private void secondaryButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((string)startButton.Content == "Start Tutorial")
            {
                tutorial = false;
            }
            else tutorial = true;
        }
    }
}
