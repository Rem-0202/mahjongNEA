using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mahjongNEA
{
    public class Action
    {
        public int typeOfAction { get; private set; }
        /* 
         * 
         * 0 = nothing
         * 1 = discard
         * 2 = chow
         * 3 = pong
         * 4 = kong
         * 5 = win
         * 
         */
        public Tile representingTile { get; private set; }
        public List<Tile> allTiles { get; private set; }
        public Action(int t, Tile r)
        {
            typeOfAction = t;
            representingTile = r;
            allTiles = new List<Tile>() { r };
        }

        public Action(int t)
        {
            typeOfAction = t;
        }

        public Action(int t, Tile r, List<Tile> ts)
        {
            typeOfAction = t;
            representingTile = r;
            allTiles = ts;
        }
    }
}
