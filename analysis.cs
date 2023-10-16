﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;


namespace mahjongNEA
{
    class Analysis
    {
        public List<Tile> tiles;
        private int shanten;
        private int pairCount;
        private int taatsuCount;
        private int groupCount;
        private int specialCount;

        public Analysis(List<Tile> tiles)
        {
            this.tiles = tiles;
            specialCount = differentSpecialTiles();
        }

        private int differentSpecialTiles()
        {
            int s = 0;
            List<Tile> temp = new List<Tile>();
            foreach (Tile t in tiles)
            {
                bool c = false;
                if (t.special)
                {
                    foreach (Tile x in temp)
                    {
                        if (x == t)
                        {
                            c = true;
                            break;
                        }
                    }
                    if (!c)
                    {
                        temp.Add(t);
                        s++;
                    }
                }
            }
            return s;
        }
    }
}
