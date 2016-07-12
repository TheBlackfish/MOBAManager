using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Statistics
{
    class StatisticsManager
    {
        private class heroStats
        {
            public int picks;
            public int bans;
            public int wins;
            public int losses;

            public heroStats()
            {
                picks = 0;
                bans = 0;
                wins = 0;
                losses = 0;
            }
        }

        private class playerStats
        {
            public int wins;
            public int losses;

            public playerStats()
            {
                wins = 0;
                losses = 0;
            }
        }

        private class teamStats
        {
            public int wins;
            public int losses;

            public teamStats()
            {
                wins = 0;
                losses = 0;
            }
        }

        private Dictionary<int, heroStats> heroDict;
        private Dictionary<int, playerStats> playerDict;
        private Dictionary<int, teamStats> teamDict;

        public void processManyBundles(List<StatsBundle> bundles)
        {
            foreach(StatsBundle sb in bundles)
            {
                processBundle(sb);
            }
        }

        public void processBundle(StatsBundle bundle)
        {
            Dictionary<int, bool> pb = bundle.getHeroPickBans();
            Dictionary<int, bool> hw = bundle.getHeroWins();
            Dictionary<int, bool> pw = bundle.getPlayerWins();
            Dictionary<int, bool> tw = bundle.getTeamWins();

            foreach(KeyValuePair<int, bool> kvp in pb.Select(kvp => kvp).ToList())
            {
                int id = kvp.Key;
                bool pbStatus = kvp.Value;

                if (!heroDict.ContainsKey(id))
                {
                    heroStats newHero = new heroStats();
                    heroDict.Add(id, newHero);
                }

                if (pbStatus)
                {
                    heroDict[id].picks++;
                }
                else
                {
                    heroDict[id].bans++;
                }
            }

            foreach(KeyValuePair<int, bool> kvp in hw.Select(kvp => kvp).ToList())
            {
                int id = kvp.Key;
                bool hwStatus = kvp.Value;

                if (!heroDict.ContainsKey(id))
                {
                    heroStats newHero = new heroStats();
                    heroDict.Add(id, newHero);
                }

                if (hwStatus)
                {
                    heroDict[id].wins++;
                }
                else
                {
                    heroDict[id].losses++;
                }
            }

            foreach (KeyValuePair<int, bool> kvp in pw.Select(kvp => kvp).ToList())
            {
                int id = kvp.Key;
                bool pwStatus = kvp.Value;

                if (!playerDict.ContainsKey(id))
                {
                    playerStats newPlayer = new playerStats();
                    playerDict.Add(id, newPlayer);
                }

                if (pwStatus)
                {
                    playerDict[id].wins++;
                }
                else
                {
                    playerDict[id].losses++;
                }
            }

            foreach (KeyValuePair<int, bool> kvp in tw.Select(kvp => kvp).ToList())
            {
                int id = kvp.Key;
                bool twStatus = kvp.Value;

                if (!teamDict.ContainsKey(id))
                {
                    teamStats newTeam = new teamStats();
                    teamDict.Add(id, newTeam);
                }

                if (twStatus)
                {
                    teamDict[id].wins++;
                }
                else
                {
                    teamDict[id].losses++;
                }
            }
        }
    }
}
