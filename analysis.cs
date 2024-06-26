﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;


namespace mahjongNEA
{
    static class Analysis
    {
        public static int safety = 5;
        public static int differentSpecialTiles(List<Tile> ownTiles)
        {
            int s = 0;
            List<Tile> temp = new List<Tile>();
            foreach (Tile t in ownTiles)
            {
                if (t.special && !temp.Contains(t))
                {
                    s++;
                    temp.Add(t);
                }
            }
            return s;
        }

        public static int specialTiles(List<Tile> ownTiles)
        {
            int s = 0;
            foreach (Tile t in ownTiles)
            {
                if (t.special) s++;
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

        public static bool isTaatsu(Tile a, Tile b)
        {
            return a.suit == b.suit && (Math.Abs(a.rank - b.rank) == 1 || Math.Abs(a.rank - b.rank) == 2) && !a.honour && !b.honour;
        }

        public static void sortTiles(ref List<Tile> ts)
        {
            bool sorted = false;
            Tile temp;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < ts.Count - 1; i++)
                {
                    if (ts[i] > ts[i + 1])
                    {
                        sorted = false;
                        temp = ts[i];
                        ts[i] = ts[i + 1];
                        ts[i + 1] = temp;
                    }
                }
            }
        }

        public static int countShanten(List<Tile> ts, int g)
        {
            //shanten algorithm, formula for standard shanten: 8 - 2*g - p - t, needs at least 1 pair
            //shanten is the number of tiles needed to be changed in order to reach a complete hand, lower is better
            //shanten is -1 when complete hand
            //pair is a kind of taatsu
            int s = 8;
            int t = 0;
            int p = 0;
            //starting values

            List<Tile> tempCopy = new List<Tile>();
            sortTiles(ref ts);
            int uniqueSpecialCount = differentSpecialTiles(ts);
            int specialCount = specialTiles(ts);
            if (ts.Count >= 13)
            {
                s = Math.Min(s, 13 - uniqueSpecialCount - (specialCount > uniqueSpecialCount ? 1 : 0));
                //special case for 13 orphans
            }
            if (ts.Count > 3)
            {
                for (int i = 0; i < ts.Count - 2; i++)
                {
                    for (int j = i + 1; j < ts.Count - 1 && j < i + 5; j++)
                    {
                        for (int k = j + 1; k < ts.Count && k < j + 5; k++)
                        {
                            if (isGroup(ts[i], ts[j], ts[k]))
                            {
                                tempCopy.Clear();
                                tempCopy.AddRange(ts);
                                tempCopy.Remove(ts[i]);
                                tempCopy.Remove(ts[j]);
                                tempCopy.Remove(ts[k]);
                                s = Math.Min(s, countShanten(tempCopy, g + 1));
                                //recursion to remove melded tiles, +1 to groups
                            }
                            if (ts[i] == ts[j] && ts[i] == ts[k])
                            {
                                i++;
                                j = 20;
                                k = 20;
                                //skipping if removed
                            }
                        }
                    }
                }
            }
            if (ts.Count > 1)
            {
                //count pairs and taatsu
                for (int i = 0; i < ts.Count - 1; i++)
                {
                    if (ts[i] == ts[i + 1])
                    {
                        p++;
                        i++;
                    }
                    else if (i < ts.Count - 2 && ts[i] == ts[i + 2])
                    {
                        p++;
                        i += 2;
                    }
                    else if (isTaatsu(ts[i], ts[i + 1]))
                    {
                        t++;
                        i++;
                    }
                }
            }
            //exclude cases for when taatsu more than needed
            if (p + t > ts.Count / 3)
            {
                s = Math.Min(s, 8 - 2 * g - ts.Count / 3 - (p > 0 ? 1 : 0));
            }
            else
            {
                s = Math.Min(s, 8 - 2 * g - p - t);
            }
            return s;
        }

        public static int[] getImprovingTileScores(List<Tile> ts, int k, Dictionary<string, int> tileCount)
        {
            //calculate number of tiles able to improve the hand
            //simple iteration through all available tiles
            List<Tile> originalCopy = new List<Tile>();
            originalCopy.AddRange(ts);
            int oShanten = countShanten(originalCopy, k);
            int nShanten;
            int[] neededTileScore = new int[ts.Count];
            for (int i = 0; i < ts.Count; i++)
            {
                foreach (string rtile in tileCount.Keys)
                {
                    nShanten = 100;
                    originalCopy.Clear();
                    originalCopy.AddRange(ts);
                    originalCopy.Remove(ts[i]);
                    originalCopy.Add(Tile.stringToTile(rtile));
                    nShanten = countShanten(originalCopy, k);
                    if (nShanten < oShanten)
                    {
                        neededTileScore[i] += tileCount[rtile] * 10;
                    }
                }
            }

            //code for defensive play
            for (int i = 0; i < neededTileScore.Length; i++)
            {
                neededTileScore[i] -= (tileCount[ts[i].tileID] - 1) * safety;
            }
            return neededTileScore;
        }

        public static int[] getImprovingTileScores_OneTileLess(List<Tile> ts, int k, Dictionary<string, int> tileCount)
        {
            //separate case for when no tiles need to be discarded but number of tiles able to improve the hand needs to be calculated
            //used when action without drawing tiles
            Dictionary<string, int> tileShantens = new Dictionary<string, int>();
            foreach (string x in tileCount.Keys)
            {
                if (tileCount[x] > 0) tileShantens.Add(x, 0);
            }
            List<Tile> originalCopy = new List<Tile>();
            originalCopy.AddRange(ts);
            int nShanten;
            int neededTileScore = 0;
            foreach (string rtile in tileCount.Keys)
            {
                if (tileCount[rtile] < 0) continue;
                nShanten = 100;
                originalCopy.Clear();
                originalCopy.AddRange(ts);
                originalCopy.Add(Tile.stringToTile(rtile));
                nShanten = countShanten(originalCopy, k);
                tileShantens[rtile] = nShanten;
            }
            int lowest = 999;
            foreach (string s in tileShantens.Keys)
            {
                lowest = Math.Min(tileShantens[s], lowest);
            }
            foreach (string s in tileShantens.Keys)
            {
                if (tileShantens[s] == lowest)
                {
                    neededTileScore += tileCount[s] * 10;
                }
            }
            return new int[] { neededTileScore, lowest };
        }

        public static Action chooseDiscard(List<Tile> ts, int k, Dictionary<string, int> tileCount)
        {
            //chooses base on highest chance of getting tiles that can improve hand
            int[] neededTileScore = getImprovingTileScores(ts, k, tileCount);
            int maxTile = 0;
            for (int i = 1; i < neededTileScore.Length; i++)
            {
                if (neededTileScore[i] > neededTileScore[maxTile])
                {
                    maxTile = i;
                }
            }
            return new Action(1, ts[maxTile]);
        }

        public static Action chooseAction(List<Tile> ts, int k, List<Action> ats, Dictionary<string, int> tileCount)
        {
            //getting an extra group is always better than waiting for a self drawn meld if not considering specialized strategies
            //wouldn't improve shanten number if declaring the meld means breaking one or more other taatsu or groups
            //decision made by calculating shanten before and after the action
            //choice of no action performed added for cases if declaring the meld doesn't improve/worsen shanten
            List<Tile> tempTS = new List<Tile>();
            tempTS.AddRange(ts);
            int[] improvingTileCount = new int[ats.Count + 1];
            int[] noneActionImprovingTileScores = getImprovingTileScores_OneTileLess(tempTS, k, tileCount);
            int[] actionImprovingTileScores;
            int lowestShanten = 999;
            int maxTileNum;
            for (int i = 0; i < ats.Count; i++)
            {
                tempTS.Clear();
                tempTS.AddRange(ts);
                tempTS = useAction(tempTS, ats[i]);
                maxTileNum = -1;
                lowestShanten = Math.Min(lowestShanten, countShanten(tempTS, k + 1));
                if (ats[i].typeOfAction != 4)
                {
                    int[] gits = getImprovingTileScores(tempTS, k + 1, tileCount);
                    foreach (int j in gits)
                    {
                        maxTileNum = Math.Max(j, maxTileNum);
                    }
                    improvingTileCount[i] = maxTileNum;
                }
                else
                {
                    actionImprovingTileScores = getImprovingTileScores_OneTileLess(tempTS, k + 1, tileCount);
                    if (actionImprovingTileScores[1] <= lowestShanten)
                    {
                        improvingTileCount[i] = actionImprovingTileScores[0];
                    }
                }
            }
            maxTileNum = 0;
            if (noneActionImprovingTileScores[1] < lowestShanten)
            {
                return new Action(0);
            }
            for (int i = 1; i < improvingTileCount.Length; i++)
            {
                if (improvingTileCount[maxTileNum] < improvingTileCount[i])
                {
                    maxTileNum = i;
                }
            }
            ats.Add(new Action(0));
            return ats[maxTileNum];
        }

        public static List<Tile> useAction(List<Tile> ts, Action a)
        {
            if (a.typeOfAction == 0)
            {
                return ts;
            }
            else
            {
                foreach (Tile t in a.allTiles)
                {
                    if (ts.Contains(t))
                    {
                        ts.Remove(t);
                    }
                }
                return ts;
            }
        }

        public static int faanToScore(int faan, bool selfDrawn)
        {
            int[] converter = { 100, 200, 400, 800, 1600, 2400, 3200, 4800, 6400, 9600, 12800, 19200, 25600, 38400 };
            return converter[Math.Min(faan, 13)] * (selfDrawn ? 3 : 2) / 2;
        }

        // Standard shanten: 8-2g-t-p
        // Kokushi shanten: 13 - diffterm - termCount>diffTerm?1:0
        // Kokushi from 13, standard from 8, find best as priority
        // PRIORITY: groups => pairs => taatsu
    }
}
