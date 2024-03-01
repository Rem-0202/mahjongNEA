using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mahjongNEA
{
    public class HandCheck
    {
        private List<Tile> ts;
        private List<Action> walledTS;
        private List<Tile> bonuses;
        private bool selfDrawn;
        private int roundWind;
        private int selfWind;
        private int faan;

        public HandCheck(List<Tile> ts, List<Action> walledTS, List<Tile> bonuses, bool selfDrawn, int roundWind, int selfWind)
        {
            List<Tile> tempTS = new List<Tile>();
            tempTS.AddRange(ts);
            Analysis.sortTiles(ref tempTS);
            this.ts = tempTS;
            this.walledTS = walledTS;
            this.bonuses = bonuses;
            this.selfDrawn = selfDrawn;
            this.roundWind = roundWind;
            this.selfWind = selfWind;
            faan = 0;
        }

        #region 1faan
        private bool noBonuses() => bonuses.Count == 0;
        private bool closedHand() => walledTS.Count == 0;
        private int windBonus()
        {
            int b = 0;
            foreach (Tile t in bonuses)
            {
                if (t.bonus && t.rank - 2 == selfWind)
                {
                    b++;
                }
            }
            return b;
        }
        private bool commonHand()
        {
            bool b = true;
            foreach (Action a in walledTS)
            {
                if (a.typeOfAction != 2)
                {
                    b = false;
                }
            }
            bool removed = true;
            List<Tile> tempTS = new List<Tile>();
            tempTS.AddRange(ts);
            while (removed)
            {
                removed = false;
                for (int i = 0; i < ts.Count - 2; i++)
                {
                    for (int j = i; i < ts.Count - 1; i++)
                    {
                        for (int k = j; i < ts.Count; i++)
                        {
                            if (Analysis.isChow(tempTS[i], tempTS[j], tempTS[k]))
                            {
                                removed = true;
                                tempTS.RemoveAt(i);
                                tempTS.RemoveAt(j);
                                tempTS.RemoveAt(k);
                            }
                        }
                    }
                }
            }
            return tempTS.Count == 2 && tempTS[0] == tempTS[1] && b;
        }
        private bool selfPick() => selfDrawn;
        private int honour()
        {
            int b = 0;
            foreach (Action a in walledTS)
            {
                if (a.typeOfAction == 3)
                {
                    
                }
            }
        }
        private bool isHonourGroup_Dragon(Tile a, Tile b, Tile c) => Analysis.isPong(a, b, c) && a.dragon;
        private bool isHonourGroup_SelfWind(Tile a, Tile b, Tile c) => Analysis.isPong(a, b, c) && a.rank - 2 == selfWind;
        private bool isHonourGroup_RoundWind(Tile a, Tile b, Tile c) => Analysis.isPong(a, b, c) && a.rank - 2 == roundWind;
        private bool isHonourGroup_Dragon(Tile a, Tile b, Tile c, Tile d) => Analysis.isKong(a, b, c, d) && a.dragon;
        private bool isHonourGroup_SelfWind(Tile a, Tile b, Tile c) => Analysis.isPong(a, b, c) && a.rank - 2 == selfWind;
        private bool isHonourGroup_RoundWind(Tile a, Tile b, Tile c) => Analysis.isPong(a, b, c) && a.rank - 2 == roundWind;
        private int honourGroupFaan(Tile a, Tile b, Tile c)
        {
            int f = 0;
            if (isHonourGroup_Dragon(a, b, c)) return 1;
            return Convert.ToInt32(isHonourGroup_SelfWind(a, b, c)) + Convert.ToInt32(isHonourGroup_RoundWind(a, b, c));
        }
        #endregion
    }
}
