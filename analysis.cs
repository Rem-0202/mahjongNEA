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
        private static int differentSpecialTiles(List<Tile> ownTiles)
        {
            int s = 0;
            List<Tile> temp = new List<Tile>();
            foreach (Tile t in ownTiles)
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

        private static int specialTiles(List<Tile> ownTiles)
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
            int s = 8;
            int t = 0;
            int p = 0;
            List<Tile> tempCopy = new List<Tile>();
            sortTiles(ref ts);
            int uniqueSpecialCount = differentSpecialTiles(ts);
            int specialCount = specialTiles(ts);
            if (ts.Count >= 13)
            {
                s = Math.Min(s, 13 - uniqueSpecialCount - (specialCount > uniqueSpecialCount ? 1 : 0));
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
                            }
                            if (ts[i] == ts[j] && ts[i] == ts[k])
                            {
                                i++;
                                j = 20;
                                k = 20;
                            }
                        }
                    }
                }
            }
            if (ts.Count > 1)
            {
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
            for (int i = 0; i < neededTileScore.Length; i++)
            {
                //defensive play, add slider to choose difficulty
                neededTileScore[i] -= (tileCount[ts[i].tileID] - 1) * safety;
            }
            return neededTileScore;
        }

        public static (int, int) getImprovingTileScores_OneTileLess(List<Tile> ts, int k, Dictionary<string, int> tileCount)
        {
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
            return (neededTileScore, lowest);
        }

        public static Action chooseDiscard(List<Tile> ts, int k, Dictionary<string, int> tileCount)
        {
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
            List<Tile> tempTS = new List<Tile>();
            tempTS.AddRange(ts);
            int[] improvingTileCount = new int[ats.Count + 1];
            int maxTileNum = -1;
            (int, int) noneActionImprovingTileScores = getImprovingTileScores_OneTileLess(tempTS, k, tileCount);
            (int, int) pongImprovingTileScores;
            int lowestShanten = 999;
            for (int i = 0; i < ats.Count; i++)
            {
                tempTS.Clear();
                tempTS.AddRange(ts);
                tempTS = useAction(tempTS, ats[i]);
                maxTileNum = -1;
                lowestShanten = Math.Min(lowestShanten, countShanten(tempTS, k + 1));
                if (ats[i].typeOfAction != 4)
                {
                    foreach (int j in getImprovingTileScores(tempTS, k + 1, tileCount))
                    {
                        maxTileNum = Math.Max(j, maxTileNum);
                    }
                    improvingTileCount[i] = maxTileNum;
                }
                else
                {
                    pongImprovingTileScores = getImprovingTileScores_OneTileLess(tempTS, k + 1, tileCount);
                    if (pongImprovingTileScores.Item2 == lowestShanten)
                    {
                        improvingTileCount[i] = pongImprovingTileScores.Item1;
                    }
                }
            }
            maxTileNum = 0;
            if (noneActionImprovingTileScores.Item2 == lowestShanten)
            {
                improvingTileCount[ats.Count] = noneActionImprovingTileScores.Item1;
            }
            for (int i = 0; i < improvingTileCount.Length - 1; i++)
            {
                maxTileNum = improvingTileCount[i] > improvingTileCount[i + 1] ? i : i + 1;
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

        // Standard shanten: 8-2g-t-p
        // Kokushi shanten: 13 - diffterm - termCount>diffTerm?1:0
        // Kokushi from 13, standard from 8, find best as priority
        // PRIORITY: groups => pairs => taatsu
    }
}
