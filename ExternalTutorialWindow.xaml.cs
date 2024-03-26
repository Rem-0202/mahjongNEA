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
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for ExternalTutorialWindow.xaml
    /// </summary>
    public partial class ExternalTutorialWindow : Window
    {
        private EventWaitHandle ewh;
        public ExternalTutorialWindow(EventWaitHandle ewh)
        {
            InitializeComponent();
            this.ewh = ewh;
            start();
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

        private void start()
        {
            ewh.Reset();
            TutorialUserControl t = new TutorialUserControl(ewh);
            displayGrid.Children.Add(t);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ewh.Set();
        }
    }
}
