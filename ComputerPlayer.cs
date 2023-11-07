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
        public ComputerPlayer(int wind) : base(wind)
        {
            InitializeComponent();
        }

        public override void addTile(Tile t)
        {
            t.concealTile();
            ownTiles.Add(t);
            sortTiles();
        }

        //logic not needed, rename later
        protected override void temp(object sender, MouseButtonEventArgs e)
        {

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
    }
}
