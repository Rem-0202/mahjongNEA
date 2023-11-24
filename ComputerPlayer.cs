using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mahjongNEA
{
    class ComputerPlayer : Player
    {
        public List<Tile> notAvailableTiles { get; private set; }
        public ComputerPlayer(int wind, int points) : base(wind, points)
        {
            InitializeComponent();
            notAvailableTiles = new List<Tile>();
        }

        public override void addTile(Tile t)
        {
            t.concealTile();
            notAvailableTiles.Add(t);
            base.addTile(t);
        }

        public override void sortTiles()
        {
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

        public override Action getAction(Action a)
        {
            if (a.allTiles != null && a.allTiles.Count > 0)
            {
                foreach (Tile t in a.allTiles)
                {
                    notAvailableTiles.Add(t);
                }
            }
            return new Action(0);
            //temp return to avoid crashing for testing
        }
    }
}
