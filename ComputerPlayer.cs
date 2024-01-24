using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mahjongNEA
{
    class ComputerPlayer : Player
    {
        public List<Tile> notAvailableTiles { get; private set; }
        public ComputerPlayer(int wind, int points) : base(wind, points)
        {
            InitializeComponent();
            notAvailableTiles = new List<Tile>();
        }

        public override void addTile(Tile t)
        {
            //t.concealTile();
            notAvailableTiles.Add(t);
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
                    notAvailableTiles.Add(t);
                }
                lastAction = new Action(0);
            }
            else if (a.typeOfAction == 1)
            {
                lastAction = new Action(0);
                List<Action> chowList = new List<Action>();
                List<Action> pongList = new List<Action>();
                List<Action> kongList = new List<Action>();
                for (int i = 0; i < ownTiles.Count - 1; i++)
                {
                    for (int k = i + 1; k < ownTiles.Count; k++)
                    {
                        if (Analysis.isChow(ownTiles[i], ownTiles[k], a.representingTile))
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
                if (pongList.Count > 0)
                {
                    lastAction = pongList[0];
                }
                
                //TODO: implement rob tile check
            }
            else if (a.typeOfAction == 0)
            {
                Tile t = ownTiles[0];
                //ownTiles.RemoveAt(0);
                lastAction = new Action(1, t);
            }
            return lastAction;
            //temp return to avoid crashing for testing
        }

        public override void acceptAction()
        {
            if (lastAction.typeOfAction == 1)
            {
                ownTiles.Remove(lastAction.representingTile);
                updateTileDisplay();
            }
            else if (lastAction.typeOfAction >= 2 && lastAction.typeOfAction <= 4)
            {
                foreach (Tile t in lastAction.allTiles)
                {
                    if (ownTiles.Contains(t))
                    {
                        t.unconcealTile();
                        ownTiles.Remove(t);
                        walledTiles.Add(t);
                    }
                }
                walledTiles.Add(lastAction.representingTile);
                updateTileDisplay();
            }
        }
    }
}
