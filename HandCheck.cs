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

        private bool noBonuses() => bonuses.Count == 0;
        private bool closedHand() => walledTS.Count == 0;
        private bool windBonus()
        {
            bool b = false;
            foreach (Tile t in bonuses)
            {
                if (t.bonus && t.rank - 2 == selfWind)
                {
                    faan++;
                    b = true;
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
    }
}
