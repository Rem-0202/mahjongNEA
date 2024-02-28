using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace mahjongNEA
{
    /// <summary>
    /// Interaction logic for Player.xaml   
    /// </summary>
    public partial class Player : UserControl
    {
        public bool ownTurn;
        protected static string windNames = "東E南S西W北N";
        protected Dictionary<string, int> tileCount;
        protected int walledGroupCount;
        public Action lastAction { get; protected set; }
        public List<Tile> ownTiles;
        public List<Tile> walledTiles { get; protected set; }
        public List<Tile> bonusTiles { get; protected set; }
        public int wind { get; private set; }  //0 = 東(E)  1 = 南(S)  2 = 西(W)  3 = 北(N)
        public int pWind { get; set; }
        public int points { get; private set; }

        public bool nextTurn;

        public bool exposeAllTiles = false;

        public Player(int w, int points, int pWind)
        {
            InitializeComponent();
            this.pWind = pWind;
            ownTiles = new List<Tile>();
            walledTiles = new List<Tile>();
            bonusTiles = new List<Tile>();
            this.points = points;
            wind = w;
            nextTurn = false;
            walledGroupCount = 0;
            windText.Text = $"{windNames[wind * 2]}{windNames[wind * 2 + 1]}";
            if (pWind == w)
            {
                windText.Foreground = Brushes.DarkRed;
            }
            scoreText.Text = points.ToString();
        }

        public virtual void addTile(Tile t)
        {
            if (t.bonus)
            {
                t.interactive = false;
                t.unconcealTile();
                bonusTiles.Add(t);
            }
            else
            {
                ownTiles.Add(t);
            }
            sortTiles();
            updateTileDisplay();
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

        public void toggleExposeTile()
        {
            exposeAllTiles = !exposeAllTiles;
            updateTileDisplay();
        }

        public virtual void updateTileDisplay()
        {
            ownTileDisplay.Children.Clear();
            walledTileDisplay.Children.Clear();
            bonusTileDisplay.Children.Clear();
            foreach (Tile t in ownTiles)
            {
                ownTileDisplay.Children.Add(t);
                if (exposeAllTiles)
                {
                    t.unconcealTile();
                }
            }
            foreach (Tile t in walledTiles)
            {
                walledTileDisplay.Children.Add(t);
            }
            foreach (Tile t in bonusTiles)
            {
                bonusTileDisplay.Children.Add(t);
            }
        }

        public virtual void sortTiles()
        {
            if (!MainWindow.autoSort) return;
            bool sorted = false;
            Tile temp;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < ownTiles.Count - 1; i++)
                {
                    if (ownTiles[i] > ownTiles[i + 1])
                    {
                        sorted = false;
                        temp = ownTiles[i];
                        ownTiles[i] = ownTiles[i + 1];
                        ownTiles[i + 1] = temp;
                    }
                }
            }
            updateTileDisplay();
        }

        protected virtual void OwnTileDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void flipTiles()
        {
            ownTileDisplay.HorizontalAlignment = HorizontalAlignment.Left;
            ownTileDisplay.LayoutTransform = new RotateTransform(180.0);
            walledTileDisplay.LayoutTransform = new RotateTransform(180.0);
            bonusTileDisplay.LayoutTransform = new RotateTransform(180.0);
            windText.LayoutTransform = new RotateTransform(180.0);
            scoreText.LayoutTransform = new RotateTransform(180.0);
        }

        public bool changePointsByAmount(int c)
        {
            points += c;
            scoreText.Text = points.ToString();
            return points <= 0;
        }

        public virtual Action getAction(Action a)
        {
            throw new NotImplementedException();
        }

        public void glow()
        {
            indicatorBar.BorderBrush = Brushes.Red;
            indicatorBar.BorderThickness = new Thickness(1.5);
        }

        public void unglow()
        {
            indicatorBar.BorderBrush = Brushes.Black;
            indicatorBar.BorderThickness = new Thickness(0.5);
        }

        public virtual void acceptAction()
        {
            if (lastAction != null)
            {
                switch (lastAction.typeOfAction)
                {
                    case 1:
                        ownTiles.Remove(lastAction.representingTile);
                        break;
                    case 2:
                        foreach (Tile t in lastAction.allTiles)
                        {
                            ownTiles.Remove(t);
                            walledTiles.Add(t);
                            t.unhover();
                            t.Margin = new Thickness(2, 2, 2, 0);
                            t.unconcealTile();
                            t.interactive = false;
                            t.VerticalAlignment = VerticalAlignment.Bottom;
                            if (t == lastAction.representingTile)
                            {
                                t.Margin = new Thickness(2, 2, 10, t.Margin.Bottom);
                                t.setRotated();
                            }
                        }
                        walledGroupCount++;
                        break;
                    case 3:
                        foreach (Tile t in lastAction.allTiles)
                        {
                            t.unhover();
                            t.Margin = new Thickness(2, 2, 2, 0);
                        }
                        lastAction.representingTile.Margin = new Thickness(2, 2, 10, lastAction.representingTile.Margin.Bottom);
                        lastAction.representingTile.setRotated();
                        foreach (Tile t in lastAction.allTiles)
                        {
                            ownTiles.Remove(t);
                            walledTiles.Add(t);
                            t.unconcealTile();
                            t.interactive = false;
                            t.VerticalAlignment = VerticalAlignment.Bottom;
                        }
                        walledGroupCount++;
                        break;
                    case 4:
                        foreach (Tile t in lastAction.allTiles)
                        {
                            t.unhover();
                            t.Margin = new Thickness(2, 2, 2, 0);
                        }
                        lastAction.representingTile.Margin = new Thickness(2, 2, 10, lastAction.representingTile.Margin.Bottom);
                        lastAction.representingTile.setRotated();
                        foreach (Tile t in lastAction.allTiles)
                        {
                            ownTiles.Remove(t);
                            walledTiles.Add(t);
                            t.unconcealTile();
                            t.interactive = false;
                            t.VerticalAlignment = VerticalAlignment.Bottom;
                        }
                        walledGroupCount++;
                        break;
                }
                updateTileDisplay();
            }
        }
    }
}
