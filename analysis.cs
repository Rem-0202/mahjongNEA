﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace mahjongNEA
{
    static class Analysis
    {
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

        private static void sortTiles(ref List<Tile> ts)
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


        private static int countGroups(ref List<Tile> ts, int g)
        {
            List<Tile> temp = new List<Tile>();
            temp.AddRange(ts);
            if (ts.Count >= 3)
            {
                for (int i = 0; i < ts.Count - 2; i++)
                {
                    for (int k = i + 1; k < ts.Count - 1 && k < i + 5; k++)
                    {
                        for (int j = k + 1; j < ts.Count && j < k + 5; j++)
                        {
                            if (isGroup(ts[i], ts[k], ts[j]))
                            {
                                g += 1;
                                temp.Remove(ts[i]);
                                temp.Remove(ts[j]);
                                temp.Remove(ts[k]);
                                ts.Clear();
                                ts.AddRange(temp);
                                return countGroups(ref ts, g);
                            }
                        }
                    }
                }
            }
            return g;
        }

        //private static int countPairs(ref List<Tile> ts, int p)
        //{
        //    sortTiles(ref ts);
        //    List<Tile> tempList = new List<Tile>();
        //    tempList.AddRange(ts);
        //    for (int i = 0; i < ts.Count - 2; i++)
        //    {
        //        for (int k = i + 1; k < ts.Count - 1 && k < i + 5; k++)
        //        {
        //            if (ts[i] == ts[k])
        //            {
        //                p += 1;
        //                tempList.Remove(ts[i]);
        //                tempList.Remove(ts[k]);
        //                ts.Clear();
        //                ts.AddRange(tempList);
        //                return countPairs(ref ts, p);
        //            }
        //        }
        //    }
        //    return p;
        //}

        private static int[] countProto(List<Tile> ts)
        {
            int[] temp = new int[2] { 0, 0 };
            for (int i = 0; i < ts.Count - 1; i++)
            {
                if (ts[i] == ts[i + 1])
                {
                    temp[0]++;
                    i++;
                }
                else if (isTaatsu(ts[i], ts[i + 1]))
                {
                    temp[1]++;
                    i++;
                }
            }
            return temp;
        }

        private static int countShanten(List<Tile> ts, int g)
        {
            {
                //List<Tile> ownTiles = new List<Tile>();
                //ownTiles.AddRange(ts);
                //int uniqueSpecialCount;
                //if (ownTiles.Count >= 13)
                //{
                //    uniqueSpecialCount = differentSpecialTiles(ownTiles);
                //}
                //else uniqueSpecialCount = -100;
                //int specialCount = specialTiles(ownTiles);
                //int g = countGroups(ref ownTiles, s);
                ////int p = countPairs(ref ownTiles, 0);
                //int[] pt = countProto(ref ownTiles);
                //int p = pt[0];
                //int t = pt[1];
                //return Math.Min(8 - g - p - t, 13 - uniqueSpecialCount - (specialCount > uniqueSpecialCount ? 1 : 0));
            }
            List<Tile> ownTiles = new List<Tile>();
            ownTiles.AddRange(ts);
            sortTiles(ref ownTiles);
            int uniqueSpecialCount = differentSpecialTiles(ownTiles);
            int specialCount = specialTiles(ownTiles);
            g = countGroups(ref ownTiles, g);
            int[] temp = countProto(ownTiles);
            int p = temp[0];
            int t = temp[1];
            int s = 8 - 2 * g - p - t;
            if (ownTiles.Count >= 13)
            {
                s = Math.Min(s, 13 - uniqueSpecialCount - (specialCount > uniqueSpecialCount ? 1 : 0));
            }
            return s;
        }

        public static Action chooseDiscard(List<Tile> ts, int s, Dictionary<string, int> tileCount)
        {
            List<Tile> originalCopy = new List<Tile>();
            originalCopy.AddRange(ts);
            int oShanten = countShanten(originalCopy, s);
            int nShanten;
            int[] neededTileCount = new int[ts.Count];
            for (int i = 0; i < ts.Count; i++)
            {
                foreach (string t in tileCount.Keys)
                {
                    nShanten = 100;
                    originalCopy.Clear();
                    originalCopy.AddRange(ts);
                    originalCopy.Remove(ts[i]);
                    originalCopy.Add(Tile.stringToTile(t));
                    nShanten = countShanten(originalCopy, s);
                    if (nShanten < oShanten)
                    {
                        neededTileCount[i] += tileCount[t];
                    }
                }
            }
            int maxTile = 0;
            for (int i = 1; i < neededTileCount.Length; i++)
            {
                if (neededTileCount[i] > neededTileCount[maxTile])
                {
                    maxTile = i;
                }
            }
            return new Action(1, ts[maxTile]);
        }

        // Standard shanten: 8-2g-t-p
        // Kokushi shanten: 13 - diffterm - termCount>diffTerm?1:0
        // Pair shanten: 6-p
        // Kokushi from 13, 7 pairs from 6, standard from 8, find best as priority
        // PRIORITY: groups => pairs => taatsu
        // pass info to computerplayer class which handles decision
    }
}
