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
        private EventWaitHandle ewh;
        private string[] tutorialGifUriArray;
        private string[] titleArray;
        private int titleIndex;
        private int currentTutorialIndex;
        public TutorialUserControl(EventWaitHandle ewh)
        {
            InitializeComponent();
            this.ewh = ewh;
            currentTutorialIndex = 0;
            titleIndex = 0;
            tutorialGifUriArray = new string[4];
            titleArray = new string[2];
            titleArray[0] = "Vocabularies";
            titleArray[1] = "Rules";
            titleArray[2] = "Game Flow";
            titleArray[3] = "Distruptions";
            tutorialGifUriArray[0] = @"test.gif";
            tutorialGifUriArray[1] = @"test2.gif";
            TutorialMediaElement.LoadedBehavior = MediaState.Play;
            TutorialMediaElement.UnloadedBehavior = MediaState.Manual;
            TutorialMediaElement.Source = new Uri(tutorialGifUriArray[currentTutorialIndex], UriKind.Relative);
            TitleTextBlock.Text = titleArray[titleIndex];
        }

        private void nextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTutorialIndex < tutorialGifUriArray.Length - 1) currentTutorialIndex++;
            titleIndex = currentTutorialIndex;
            setMedia();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            ewh.Set();
        }

        private void setMedia()
        {
            if (currentTutorialIndex == tutorialGifUriArray.Length - 1)
            {
                endButton.Visibility = Visibility.Visible;
                endButton.IsEnabled = true;
                nextPageButton.Visibility = Visibility.Hidden;
                nextPageButton.IsEnabled = false;
            }
            else
            {
                endButton.Visibility = Visibility.Hidden;
                endButton.IsEnabled = false;
                nextPageButton.Visibility = Visibility.Visible;
                nextPageButton.IsEnabled = true;
            }
            if (currentTutorialIndex == 0)
            {
                previousPageButton.Visibility = Visibility.Hidden;
                previousPageButton.IsEnabled = false;
            }
            else
            {
                previousPageButton.Visibility = Visibility.Visible;
                previousPageButton.IsEnabled = true;
            }
            TutorialMediaElement.Source = new Uri(tutorialGifUriArray[currentTutorialIndex], UriKind.Relative);
            TutorialMediaElement.Position = new TimeSpan(0, 0, 1);
            TitleTextBlock.Text = titleArray[titleIndex];
        }

        private void previousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTutorialIndex > 0) currentTutorialIndex--;
            titleIndex = currentTutorialIndex;
            setMedia();
        }

        private void TutorialMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            TutorialMediaElement.Position = new TimeSpan(0, 0, 1);
        }
    }
}
