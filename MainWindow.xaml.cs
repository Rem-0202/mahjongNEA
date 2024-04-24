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
using System.Windows.Threading;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameView g;
        public StartPageUserControl s;
        public static bool autoSort = true;
        public MainWindow()
        {
            InitializeComponent();
            s = new StartPageUserControl();
            s.PreviewMouseUp += frame_MouseUp;
            displayGrid.Children.Add(s);
        }

        protected void WaitForEvent(EventWaitHandle eventHandle)
        {
            var frame = new DispatcherFrame();
            new Thread(() =>
            {
                eventHandle.WaitOne();
                frame.Continue = false;
            }).Start();
            Dispatcher.PushFrame(frame);
        }

        private void frame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
            ewh.Reset();
            TutorialUserControl t = new TutorialUserControl(ewh);
            if (s.tutorial == true)
            {
                displayGrid.Children.Add(t);
                bool end = false;
                while (!end)
                {
                    WaitForEvent(ewh);
                    switch (new newGameDialog().ShowDialog())
                    {
                        case true:
                            end = true;
                            g = new GameView(newGameDialog.pWind, newGameDialog.uWind, newGameDialog.sPoints, newGameDialog.ePoints);
                            displayGrid.Children.Clear();
                            displayGrid.Children.Add(g);
                            restartButton.Visibility = Visibility.Visible;
                            restartButton.IsEnabled = true;
                            ewh.Reset();
                            break;
                        case false:
                            ewh.Reset();
                            break;
                        default:
                            ewh.Reset();
                            break;
                    }
                }
            }
            else
            {
                switch (new newGameDialog().ShowDialog())
                {
                    case true:
                        g = new GameView(newGameDialog.pWind, newGameDialog.uWind, newGameDialog.sPoints, newGameDialog.ePoints);
                        displayGrid.Children.Clear();
                        displayGrid.Children.Add(g);
                        restartButton.Visibility = Visibility.Visible;
                        restartButton.IsEnabled = true;
                        ewh.Reset();
                        break;
                    case false:
                        ewh.Reset();
                        break;
                    default:
                        ewh.Reset();
                        break;
                }
            }
        }

        private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            helpWindow h = new helpWindow();
            h.ShowDialog();
        }

        private void autoSortChecked(object sender, RoutedEventArgs e)
        {
            autoSort = true;
            g?.toggleSort();
        }

        private void autoSortUnchecked(object sender, RoutedEventArgs e)
        {
            autoSort = false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (new newGameDialog().ShowDialog())
            {
                case true:
                    g = new GameView(newGameDialog.pWind, newGameDialog.uWind, newGameDialog.sPoints, newGameDialog.ePoints);
                    displayGrid.Children.Clear();
                    displayGrid.Children.Add(g);
                    restartButton.Visibility = Visibility.Visible;
                    restartButton.IsEnabled = true;
                    exposeTileToggle.IsChecked = false;
                    break;
                case false:
                    break;
                default:
                    break;
            }
        }

        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            g = new GameView(newGameDialog.pWind, newGameDialog.uWind, newGameDialog.sPoints, newGameDialog.ePoints);
            displayGrid.Children.Clear();
            displayGrid.Children.Add(g);
            exposeTileToggle.Checked -= ExposeTileMenuItem_Checked;
            exposeTileToggle.IsChecked = false;
            exposeTileToggle.Checked += ExposeTileMenuItem_Checked;
        }

        private void ExposeTileMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            g?.exposeTiles();
        }

        private void ExposeTileMenuItem_unChecked(object sender, RoutedEventArgs e)
        {
            g?.unExposeTiles();
        }

        private void MenuItem_RedoTutorial_Click(object sender, RoutedEventArgs e)
        {
            EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
            ExternalTutorialWindow etw = new ExternalTutorialWindow(ewh);
            etw.Show();
            WaitForEvent(ewh);
            etw.Close();
        }
    }
}
