using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace mahjongNEA
{
    class ComputerPlayer : Player
    {
        private Dictionary<string, int> tileCount;
        public ComputerPlayer(int wind, int points) : base(wind, points)
        {
            InitializeComponent();
            tileCount = new Dictionary<string, int>();
            for (int i = 1; i <= 9; i++)
            {
                tileCount.Add($"{i.ToString()}m", 4);
                tileCount.Add($"{i.ToString()}p", 4);
                tileCount.Add($"{i.ToString()}s", 4);
            }
            for (int i = 2; i <= 8; i++)
            {
                tileCount.Add($"{i.ToString()}z", 4);
            }
        }

        public override void addTile(Tile t)
        {
            //t.concealTile();
            if (!t.bonus)
            {
                tileCount[t.tileID]--;
            }
            base.addTile(t);
        }

        public override void sortTiles()
        {
            bool sorted = false;
            Tile temp;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < ownTiles.Count - 1; i++)
                {
                    if (ownTiles[i] > ownTiles[i + 1])
                    {
                        sorted = false;
                        temp = ownTiles[i];
                        ownTiles[i] = ownTiles[i + 1];
                        ownTiles[i + 1] = temp;
                    }
                }
            }
            updateTileDisplay();

        }

        public override Action getAction(Action a)
        {
            if (a.typeOfAction >= 2)
            {
                foreach (Tile t in a.allTiles)
                {
                    tileCount[t.tileID]--;
                }
                lastAction = new Action(0);
            }
            else if (a.typeOfAction == 1)
            {
                tileCount[a.representingTile.tileID]--;
                lastAction = new Action(0);
                List<Action> chowList = new List<Action>();
                List<Action> pongList = new List<Action>();
                List<Action> kongList = new List<Action>();
                for (int i = 0; i < ownTiles.Count - 1; i++)
                {
                    for (int k = i + 1; k < ownTiles.Count; k++)
                    {
                        if (Analysis.isChow(ownTiles[i], ownTiles[k], a.representingTile) && nextTurn)
                        {
                            chowList.Add(new Action(2, a.representingTile, new List<Tile>() { ownTiles[i], ownTiles[k], a.representingTile }));
                        }
                        if (Analysis.isPong(ownTiles[i], ownTiles[k], a.representingTile))
                        {
                            pongList.Add(new Action(3, a.representingTile, new List<Tile>() { ownTiles[i], ownTiles[k], a.representingTile }));
                        }
                        for (int j = k + 1; j < ownTiles.Count; j++)
                        {
                            if (Analysis.isKong(ownTiles[i], ownTiles[k], ownTiles[j], a.representingTile))
                            {
                                kongList.Add(new Action(4, a.representingTile, new List<Tile>() { ownTiles[i], ownTiles[k], ownTiles[j], a.representingTile }));
                            }
                        }
                    }
                }
                List<Action> tempActionList = new List<Action>();
                tempActionList.AddRange(chowList);
                tempActionList.AddRange(pongList);
                tempActionList.AddRange(kongList);
                Random rng = new Random();
                if (tempActionList.Count != 0)
                {
                    lastAction = tempActionList[rng.Next(tempActionList.Count)];
                    foreach (Tile x in lastAction.allTiles)
                    {
                        tileCount[x.tileID]--;
                    }
                }
                //TODO: implement rob tile check
            }
            else if (a.typeOfAction == 0)
            {
                lastAction = Analysis.chooseDiscard(ownTiles, walledGroupCount, tileCount);
                tileCount[lastAction.representingTile.tileID]--;
            }
            return lastAction;
            //temp return to avoid crashing for testing
        }
    }
}
