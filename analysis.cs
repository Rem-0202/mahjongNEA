using System;
using System.CodeDom.Compiler;
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
                if (t.special)
                {
                    foreach (Tile x in temp)
                    {
                        if (!temp.Contains(x))
                        {
                            s++;
                            temp.Add(x);
                        }
                    }
                }
            }
            return s;
        }

        public static bool isChow(Tile a, Tile b, Tile c)
        {
            int[] ranks = { a.rank, b.rank, c.rank };
            if (a.honour || a.bonus || b.honour || b.bonus || c.honour || c.bonus)
            {
                return false;
            }
            else
            {
                if (a.suit == b.suit && a.suit == c.suit)
                {
                    int min = Math.Min(Math.Min(a.rank, b.rank), c.rank);
                    return ranks.Contains(min + 1) && ranks.Contains(min + 2);
                }
                else return false;
            }
        }

        public static bool isPong(Tile a, Tile b, Tile c)
        {
            return a == b && b == c;
        }

        public static bool isKong(Tile a, Tile b, Tile c, Tile d)
        {
            return a == b && b == c && c == d;
        }

        public static bool isGroup(Tile a, Tile b, Tile c)
        {
            return isChow(a, b, c) || isPong(a, b, c);
        }

        //public static int shantenNumber()
        //{

        //}
    }
}
