﻿using System;
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
        public int faan;
        private string tileString;
        private string fullTileString;
        public List<Tile> tempFullTS;
        public Dictionary<string, int> faanPairs;

        public HandCheck(List<Tile> ts, List<Action> walledTS, List<Tile> bonuses, bool selfDrawn, int roundWind, int selfWind)
        {
            List<Tile> tempTS = new List<Tile>();
            tempTS.AddRange(ts);
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
            tempFullTS = new List<Tile>();
            tempFullTS.AddRange(ts);
            foreach (Action a in walledTS)
            {
                tempFullTS.AddRange(a.allTiles);
            }
            Analysis.sortTiles(ref tempTS);
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
            //calling all meethods to calculate the separate faan values
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
                else if (smallBonus()) faanPairs.Add("Small Bonus", 3);
                else if (bonusSeries()) faanPairs.Add("Bonus Series", 2);
                if (thirteenOrphans()) faanPairs.Add("Thirteen Orphans", 13);
                else if (mixedOrphan()) faanPairs.Add("Mixed Orphan", 1);
                if (orphans()) faanPairs.Add("Orphans", 10);
                if (allKongs()) faanPairs.Add("All Kongs", 13);
                else if (triplets())
                {
                    if (closedHand()) faanPairs.Add("Self Triplets", 8);
                    else faanPairs.Add("Triplets", 3);
                }
                else if (closedHand()) faanPairs.Add("Closed Hand", 1);
                if (greatWinds()) faanPairs.Add("Great Winds", 13);
                else if (smallWinds()) faanPairs.Add("Small Winds", 3);
                if (allOneSuit())
                {
                    if (nineGates()) faanPairs.Add("Nine Gates", 10);
                    else if (fullTileString.Contains('z')) faanPairs.Add("All Honours", 10);
                    else faanPairs.Add("All One Suit", 7);
                }
                if (greatDragons()) faanPairs.Add("Great Dragons", 5);
                else if (smallDragons()) faanPairs.Add("Small Dragons", 3);
                if (mixedOneSuit()) faanPairs.Add("Mixed One Suit", 3);
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
            //the only hand that cannot be checked by simple regex or iteration
            //needs four melds of chow and one pair
            foreach (Action a in walledTS)
            {
                if (a.typeOfAction != 2)
                {
                    return false;
                }
            }
            Regex removeHonour = new Regex(@"([2-8]z)\1");
            MatchCollection mc = removeHonour.Matches(fullTileString);
            if (mc.Count > 1 || (mc.Count == 0 && fullTileString.Contains('z'))) return false;
            removeHonour = new Regex(@"([2-8]z)\1\1");
            mc = removeHonour.Matches(fullTileString);
            if (mc.Count != 0) return false;
            //cannot contain honour melds or more than one honour pair
            List<Tile> tempTS = new List<Tile>();
            tempTS.AddRange(ts);
            Dictionary<string, int> numTiles = new Dictionary<string, int>();
            foreach (Tile t in tempTS)
            {
                if (!numTiles.ContainsKey(t.tileID)) numTiles.Add(t.tileID, 1); else numTiles[t.tileID]++;
            }
            string current;
            string previous1;
            string previous2;
            int i = numTiles.Count;
            //minimally removes melds of chow to prevent removing groups of three tiles spanning two different melds
            //alternatively could use decomposition of tiles
            //  if decomposition of tiles is used might bring performance issue unless it is also used for all other checks and in analysis
            do
            {
                i = Math.Min(numTiles.Count - 1, i - 1);
                while (numTiles.Values.ElementAt(i) % 2 != 0)
                {
                    current = numTiles.Keys.ElementAt(i);
                    if (!(current[0] == '2' || current[0] == '1'))
                    {
                        previous1 = (Convert.ToInt32(current[0].ToString()) - 1).ToString() + current[1];
                        previous2 = (Convert.ToInt32(current[0].ToString()) - 2).ToString() + current[1];
                        if (numTiles.ContainsKey(previous1) && numTiles.ContainsKey(previous2))
                        {
                            numTiles[current]--;
                            numTiles[previous1]--;
                            numTiles[previous2]--;
                            if (numTiles[current] == 0) numTiles.Remove(current);
                            if (numTiles[previous1] == 0) numTiles.Remove(previous1);
                            if (numTiles[previous2] == 0) numTiles.Remove(previous2);
                        }
                        else break;
                    }
                    else break;
                    if (numTiles.Count - 1 < i) break;
                }
            } while (i > 0);
            i = numTiles.Count;
            if (numTiles.Count == 1 && numTiles[numTiles.Keys.ElementAt(numTiles.Count - 1)] == 2) return true;
            //remove remaining melds until two tiles left
            do
            {
                i = Math.Min(numTiles.Count - 1, i - 1);
                while (numTiles.Count > i && numTiles.Count != 1)
                {
                    current = numTiles.Keys.ElementAt(i);
                    if (!(current[0] == '2' || current[0] == '1'))
                    {
                        previous1 = (Convert.ToInt32(current[0].ToString()) - 1).ToString() + current[1];
                        previous2 = (Convert.ToInt32(current[0].ToString()) - 2).ToString() + current[1];
                        if (numTiles.ContainsKey(previous1) && numTiles.ContainsKey(previous2))
                        {
                            numTiles[current]--;
                            numTiles[previous1]--;
                            numTiles[previous2]--;
                            if (numTiles[current] == 0) numTiles.Remove(current);
                            if (numTiles[previous1] == 0) numTiles.Remove(previous1);
                            if (numTiles[previous2] == 0) numTiles.Remove(previous2);
                        }
                        else break;
                    }
                    else break;
                }
            } while (i > 0);
            //check for the remaining pair
            return numTiles.Count == 1 && numTiles.Values.ElementAt(0) == 2;
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
            List<int> f = new List<int>() { 2, 3, 4, 5 };
            List<int> n = new List<int>() { 2, 3, 4, 5 };
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
        private bool greatDragons() => new Regex(@"(([6-8]z)\2\2\2?)").Matches(fullTileString).Count == 3;
        #endregion

        #region 10faan
        private bool orphans() => !new Regex(@"([2-8][spmz])").IsMatch(fullTileString);
        private bool nineGates()
        {
            Regex nineGatesRegex = new Regex(@"^1111?22?33?44?55?66?77?88?9999?$");
            string tileStringCopy = $"{tileString}";
            Regex removeSuitRegex = new Regex(@"([mspz])");
            tileStringCopy = removeSuitRegex.Replace(tileStringCopy, "");
            return nineGatesRegex.IsMatch(tileStringCopy) && tileStringCopy.Length == 14;
        }
        #endregion

        #region 13faan
        private bool thirteenOrphans()
        {
            int uniqueSpecialCount = Analysis.differentSpecialTiles(ts);
            int specialCount = Analysis.specialTiles(ts);
            int s = 13 - uniqueSpecialCount - (specialCount > uniqueSpecialCount ? 1 : 0);
            return s == -1;
        }
        private bool allKongs() => fullTileString.Length == 36;
        private bool greatWinds()
        {
            return new Regex(@"(([2-5]z)\2\2\2?)").Replace(fullTileString, "", 4).Length == 4;
        }
        #endregion
    }
}