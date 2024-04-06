using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private string tileString;
        private string fullTileString;
        public Dictionary<string, int> faanPairs;

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
            tileString = "";
            foreach (Tile t in ts)
            {
                tileString += t.tileID;
            }
            fullTileString = "";
            List<Tile> tempFullTS = new List<Tile>();
            tempFullTS.AddRange(ts);
            foreach (Action a in walledTS)
            {
                tempFullTS.AddRange(a.allTiles);
            }
            Analysis.sortTiles(ref tempFullTS);
            foreach (Tile t in tempFullTS)
            {
                fullTileString += t.tileID;
            }
            faanPairs = new Dictionary<string, int>();
            setFaanPairs();
            foreach (int i in faanPairs.Values)
            {
                faan += i;
            }
        }

        private void setFaanPairs()
        {
            if (Analysis.countShanten(ts, walledTS.Count) != -1)
            {
                if (greatBonus()) faanPairs.Add("Great Bonus", 8); else if (smallBonus()) faanPairs.Add("Small Bonus", 3);
                return;
            }
            else
            {
                faanPairs.Add("Honours", honour());
                faanPairs.Add("Bonuses", windBonus());
                if (greatBonus()) faanPairs.Add("Great Bonus", 8);
                if (smallBonus()) faanPairs.Add("Small Bonus", 3);
                if (bonusSeries()) faanPairs.Add("Bonus Series", 2);
                if (thirteenOrphans()) faanPairs.Add("Thirteen Orphans", 13);
                if (allKongs()) faanPairs.Add("All Kongs", 13);
                if (greatWinds()) faanPairs.Add("Great Winds", 13);
                if (allHonours()) faanPairs.Add("All Honours", 10);
                if (orphans()) faanPairs.Add("Orphans", 10);
                if (nineGates()) faanPairs.Add("Nine Gates", 10);
                if (triplets()) faanPairs.Add("Triplets", 3);
                if (selftriplets()) faanPairs.Add("Self Triplets", 8);
                if (greatDragons()) faanPairs.Add("Great Dragons", 5);
                if (allOneSuit()) faanPairs.Add("All One Suit", 7);
                if (smallWinds()) faanPairs.Add("Small Winds", 3);
                if (smallDragons()) faanPairs.Add("Small Dragons", 3);
                if (mixedOneSuit()) faanPairs.Add("Mixed One Suit", 3);
                if (mixedOrphan()) faanPairs.Add("Mixed Orphan", 1);
                if (noBonuses()) faanPairs.Add("No Bonuses", 1);
                if (commonHand()) faanPairs.Add("Common Hand", 1);
                if (selfPick()) faanPairs.Add("Self Pick", 1);
            }
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
                if (a.typeOfAction == 3 || a.typeOfAction == 4)
                {
                    if (a.representingTile.dragon)
                    {
                        b++;
                    }
                    if (a.representingTile.honour && a.representingTile.rank - 2 == selfWind)
                    {
                        b++;
                    }
                    if (a.representingTile.honour && a.representingTile.rank - 2 == roundWind)
                    {
                        b++;
                    }
                }
            }
            Regex dragonRegex = new Regex(@"([6-8]z)\1\1");
            Regex windRegex = new Regex(@"(" + (selfWind + 2) + @"z|" + (roundWind + 2) + @"z){3}");
            MatchCollection md = dragonRegex.Matches(tileString);
            b += md.Count;
            md = windRegex.Matches(tileString);
            if (selfWind == roundWind) b += 2 * md.Count; else b += md.Count;
            return b;
        }
        private bool mixedOrphan() => !new Regex(@"([2-8][spm])").IsMatch(fullTileString);
        #endregion

        #region 2faan
        private bool bonusSeries()
        {
            List<int> f = new List<int>() { 1, 2, 3, 4 };
            List<int> n = new List<int>() { 1, 2, 3, 4 };
            if (bonuses.Count < 4) return false;
            else
            {
                foreach (Tile t in bonuses)
                {
                    if (t.suit == 'f') f.Remove(t.rank);
                    if (t.suit == 'n') n.Remove(t.rank);
                }
            }
            return f.Count == 0 || n.Count == 0;
        }
        #endregion

        #region 3faan
        private bool smallBonus() => bonuses.Count == 7;
        private bool smallWinds()
        {
            Regex removeWindsRegex = new Regex(@"(([2-5]z)\2\2\2?)");
            string tileStringCopy = $"{fullTileString}";
            tileStringCopy = removeWindsRegex.Replace(tileStringCopy, "", 3);
            removeWindsRegex = new Regex(@"(([2-5]z)\2)");
            tileStringCopy = removeWindsRegex.Replace(tileStringCopy, "", 1);
            return tileStringCopy.Length == 6 || tileStringCopy.Length == 8;
        }
        private bool triplets()
        {
            Regex removeTripletRegex = new Regex(@"((\d\w)\2\2)");
            string tileStringCopy = $"{tileString}";
            tileStringCopy = removeTripletRegex.Replace(tileStringCopy, "");
            bool b = tileStringCopy.Length == 4 && tileStringCopy[0] == tileStringCopy[2] && tileStringCopy[1] == tileStringCopy[3];
            foreach (Action a in walledTS)
            {
                b = b && (a.typeOfAction == 3 || a.typeOfAction == 4);
            }
            return b;
        }
        private bool mixedOneSuit()
        {
            string distinctSuits = "";
            foreach (Tile t in ts)
            {
                if (!distinctSuits.Contains(t.suit))
                {
                    distinctSuits += t.suit;
                }
            }
            foreach (Action a in walledTS)
            {
                foreach (Tile t in a.allTiles)
                {
                    if (!distinctSuits.Contains(t.suit))
                    {
                        distinctSuits += t.suit;
                    }
                }
            }
            return distinctSuits.EndsWith("z") && distinctSuits.Length == 2;
        }
        #endregion

        #region 5faan
        private bool smallDragons()
        {
            Regex removeDragonsRegex = new Regex(@"(([6-8]z)\2\2\2?)");
            string tileStringCopy = $"{fullTileString}";
            tileStringCopy = removeDragonsRegex.Replace(tileStringCopy, "", 2);
            removeDragonsRegex = new Regex(@"(([6-8]z)\2)");
            tileStringCopy = removeDragonsRegex.Replace(tileStringCopy, "", 1);
            return tileStringCopy.Length == 6 || tileStringCopy.Length == 8;
        }
        #endregion

        #region 7faan
        private bool allOneSuit()
        {
            string distinctSuits = "";
            foreach (Tile t in ts)
            {
                if (!distinctSuits.Contains(t.suit))
                {
                    distinctSuits += t.suit;
                }
            }
            foreach (Action a in walledTS)
            {
                foreach (Tile t in a.allTiles)
                {
                    if (!distinctSuits.Contains(t.suit))
                    {
                        distinctSuits += t.suit;
                    }
                }
            }
            return distinctSuits == "s" || distinctSuits == "p" || distinctSuits == "m";
        }
        #endregion

        #region 8faan
        private bool greatBonus() => bonuses.Count == 8;
        private bool selftriplets()
        {
            bool b = triplets();
            return b && closedHand();
        }
        private bool greatDragons() => new Regex(@"(([6-8]z)\2\2\2?)").Matches(fullTileString).Count == 3;
        #endregion

        #region 10faan
        private bool allHonours()
        {
            string distinctSuits = "";
            foreach (Tile t in ts)
            {
                if (!distinctSuits.Contains(t.suit))
                {
                    distinctSuits += t.suit;
                }
            }
            foreach (Action a in walledTS)
            {
                foreach (Tile t in a.allTiles)
                {
                    if (!distinctSuits.Contains(t.suit))
                    {
                        distinctSuits += t.suit;
                    }
                }
            }
            return distinctSuits == "z";
        }
        private bool orphans() => !new Regex(@"([2-8][spmz])").IsMatch(fullTileString);
        private bool nineGates()
        {
            Regex nineGatesRegex = new Regex(@"^1111?22?33?44?55?66?77?88?9999?$");
            string tileStringCopy = $"{tileString}";
            Regex removeSuitRegex = new Regex(@"([mspz])");
            tileStringCopy = removeSuitRegex.Replace(tileStringCopy, "");
            return nineGatesRegex.IsMatch(tileStringCopy) && tileStringCopy.Length == 14 && allOneSuit();
        }
        #endregion

        #region 13faan
        private bool thirteenOrphans()
        {
            Regex thirteenOrphanRegex = new Regex(@"[1m]{1,2}[9m]{1,2}[1p]{1,2}[9p]{1,2}[1s]{1,2}[9s]{1,2}[2z]{1,2}[3z]{1,2}[4z]{1,2}[5z]{1,2}[6z]{1,2}[7z]{1,2}[8z]{1,2}");
            return thirteenOrphanRegex.IsMatch(fullTileString) && fullTileString.Length == 28;
        }
        private bool allKongs() => fullTileString.Length == 36;
        private bool greatWinds()
        {
            Regex removeWindsRegex = new Regex(@"(([2-5]z)\2\2\2?)");
            string tileStringCopy = $"{fullTileString}";
            tileStringCopy = removeWindsRegex.Replace(fullTileString, "", 4);
            return tileStringCopy.Length == 4;
        }
        #endregion
    }
}