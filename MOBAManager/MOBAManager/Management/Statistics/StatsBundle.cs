using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Statistics
{
    public class StatsBundle
    {
        private Dictionary<int, bool> heroPicks;
        private Dictionary<int, bool> heroWins;
        private Dictionary<int, bool> playerWins;
        private Dictionary<int, bool> teamWins;

        public Dictionary<int, bool> getHeroPickBans()
        {
            return heroPicks;
        }

        public Dictionary<int, bool> getHeroWins()
        {
            return heroWins;
        }

        public Dictionary<int, bool> getPlayerWins()
        {
            return playerWins;
        }

        public Dictionary<int, bool> getTeamWins()
        {
            return teamWins;
        }

        public void addHeroPickBan(int ID, bool wasPicked)
        {
            heroPicks.Add(ID, wasPicked);
        }

        public void addHeroWin(int ID, bool won)
        {
            heroWins.Add(ID, won);
        }

        public void addPlayerWin(int ID, bool won)
        {
            playerWins.Add(ID, won);
        }

        public void addTeamWin(int ID, bool won)
        {
            teamWins.Add(ID, won);
        }

        public StatsBundle()
        {
            heroPicks = new Dictionary<int, bool>();
            heroWins = new Dictionary<int, bool>();
            playerWins = new Dictionary<int, bool>();
            teamWins = new Dictionary<int, bool>();
        }
    }
}
