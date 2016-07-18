using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MOBAManager.Management.Statistics
{
    public class StatisticsManager
    {
        #region Private classes
        /// <summary>
        /// The private class for holding data about heros in our fake-database.
        /// </summary>
        private class heroStats
        {
            #region Public variables
            /// <summary>
            /// The name of the hero.
            /// </summary>
            public string name;

            /// <summary>
            /// The number of times this hero has been picked in official matches.
            /// </summary>
            public int picks;

            /// <summary>
            /// The number of times this hero has been banned in official matches.
            /// </summary>
            public int bans;

            /// <summary>
            /// The number of wins this hero has.
            /// </summary>
            public int wins;

            /// <summary>
            /// The number of losses this hero has.
            /// </summary>
            public int losses;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new heroStats.
            /// </summary>
            /// <param name="s">The name of the hero.</param>
            public heroStats(string s)
            {
                name = s;
                picks = 0;
                bans = 0;
                wins = 0;
                losses = 0;
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Returns the winrate of the hero, or 0 if the hero has never been picked.
            /// </summary>
            public double winrate
            {
                get
                {
                    if (wins + losses > 0)
                    {
                        return wins / (wins + losses);
                    }
                    else
                    {
                        return 0.0;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// The class for holding player stats.
        /// </summary>
        private class playerStats
        {
            #region Public variables
            /// <summary>
            /// The name of the player.
            /// </summary>
            public string name;

            /// <summary>
            /// The number of wins the player has.
            /// </summary>
            public int wins;

            /// <summary>
            /// The number of losses the player has.
            /// </summary>
            public int losses;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new playerStats.
            /// </summary>
            /// <param name="s">The name of the player.</param>
            public playerStats(string s)
            {
                name = s;
                wins = 0;
                losses = 0;
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Returns the winrate of the player, or 0 if they have played 0 games.
            /// </summary>
            public double winrate
            {
                get
                {
                    if (wins + losses > 0)
                    {
                        return wins / (wins + losses);
                    }
                    else
                    {
                        return 0.0;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// The class that holds a team's stats.
        /// </summary>
        private class teamStats
        {
            #region Public variables
            /// <summary>
            /// The name of the team.
            /// </summary>
            public string name;

            /// <summary>
            /// The number of wins the team has.
            /// </summary>
            public int wins;

            /// <summary>
            /// The number of losses the team has.
            /// </summary>
            public int losses;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new teamStats.
            /// </summary>
            /// <param name="s">The name of the team.</param>
            public teamStats(string s)
            {
                name = s;
                wins = 0;
                losses = 0;
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Returns the winrate of the team, or 0 if they haven't played in any matches.
            /// </summary>
            public double winrate
            {
                get
                {
                    if (wins + losses > 0)
                    {
                        return wins / (wins + losses);
                    }
                    else
                    {
                        return 0.0;
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Private variables
        /// <summary>
        /// The dictionary holding herostats. Each herostat's key is that hero's ID.
        /// </summary>
        private Dictionary<int, heroStats> heroDict;

        /// <summary>
        /// The dictionary holding playerstats. Each playerstat's key is that player's ID.
        /// </summary>
        private Dictionary<int, playerStats> playerDict;

        /// <summary>
        /// The dictionary holding teamstats. Each teamstat's key is that player's ID.
        /// </summary>
        private Dictionary<int, teamStats> teamDict;
        #endregion

        #region Public methods
        /// <summary>
        /// Processes many stat bundles into the manager.
        /// </summary>
        /// <param name="bundles">The list containing all of the bundles to process.</param>
        /// <param name="hm">The hero manager of the current game.</param>
        /// <param name="pm">The player manager of the current game.</param>
        /// <param name="tm">The team manager of the current game.</param>
        public void processManyBundles(List<StatsBundle> bundles, HeroManager hm, PlayerManager pm, TeamManager tm)
        {
            foreach(StatsBundle sb in bundles)
            {
                processBundle(sb, hm, pm, tm);
            }
        }

        /// <summary>
        /// Processes a stat bundle and adds all relevant information to the various stat dictionaries.
        /// </summary>
        /// <param name="bundle">The bundle to process.</param>
        /// <param name="hm">The hero manager of the current game.</param>
        /// <param name="pm">The player manager of the current game.</param>
        /// <param name="tm">The team manager of the current game.</param>
        public void processBundle(StatsBundle bundle, HeroManager hm, PlayerManager pm, TeamManager tm)
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
                    heroStats newHero = new heroStats(hm.getHeroName(id));
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
                    heroStats newHero = new heroStats(hm.getHeroName(id));
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
                    playerStats newPlayer = new playerStats(pm.getPlayerName(id));
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
                    teamStats newTeam = new teamStats(tm.getTeamName(id));
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

        /// <summary>
        /// Returns a binding source representing hero data for data grid views that contains Tuples with the following fields:
        /// <para>A string with the name of the hero</para>
        /// <para>A double with the hero's winrate</para>
        /// <para>An int with the hero's ban rate</para>
        /// <para>An int with the hero's pick rate</para>
        /// </summary>
        /// <returns></returns>
        public BindingSource getHeroStats()
        {
            BindingSource bs = new BindingSource();

            List<heroStats> hStats = heroDict.Select(kvp => kvp.Value).OrderByDescending(hs => hs.winrate).ThenByDescending(hs => hs.picks + hs.bans).ToList();

            foreach (heroStats hs in hStats)
            {
                Tuple<string, double, int, int> res = new Tuple<string, double, int, int>(hs.name, hs.winrate, hs.bans, hs.picks);
                bs.Add(res);
            }

            return bs;
        }

        /// <summary>
        /// Returns a binding source representing player data for data grid views that contains Tuples with the following fields:
        /// <para>A string with the name of the player</para>
        /// <para>A double with the players's winrate</para>
        /// <para>An int with the player's total played matches</para>
        /// </summary>
        /// <returns></returns>
        public BindingSource getPlayerStats()
        {
            BindingSource bs = new BindingSource();

            List<playerStats> pStats = playerDict.Select(kvp => kvp.Value).OrderByDescending(ps => ps.winrate).ThenByDescending(ps => ps.wins + ps.losses).ToList();

            foreach (playerStats ps in pStats)
            {
                Tuple<string, double, int> res = new Tuple<string, double, int>(ps.name, ps.winrate, ps.wins + ps.losses);
                bs.Add(res);
            }

            return bs;
        }

        /// <summary>
        /// Returns a binding source representing team data for data grid views that contains Tuples with the following fields:
        /// <para>A string with the name of the team</para>
        /// <para>A double with the team's winrate</para>
        /// <para>An int with the team's total played matches</para>
        /// </summary>
        /// <returns></returns>
        public BindingSource getTeamStats()
        {
            BindingSource bs = new BindingSource();

            List<teamStats> tStats = teamDict.Select(kvp => kvp.Value).OrderByDescending(ps => ps.winrate).ThenByDescending(ps => ps.wins + ps.losses).ToList();

            foreach (teamStats ts in tStats)
            {
                Tuple<string, double, int> res = new Tuple<string, double, int>(ts.name, ts.winrate, ts.wins + ts.losses);
                bs.Add(res);
            }

            return bs;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new statistics manager and fills it with all currently known data.
        /// </summary>
        /// <param name="heroIDs">The dictionary of all heroes in the game.</param>
        /// <param name="playerIDs">The dictionary of all players in the game.</param>
        /// <param name="teamIDs">The dictionary of all teams in the game.</param>
        public StatisticsManager(Dictionary<int, Hero> heroIDs, Dictionary<int, Player> playerIDs, Dictionary<int, Team> teamIDs)
        {
            heroDict = new Dictionary<int, heroStats>();
            foreach (KeyValuePair<int, Hero> kvp in heroIDs)
            {
                heroDict.Add(kvp.Key, new heroStats(kvp.Value.HeroName));
            }

            playerDict = new Dictionary<int, playerStats>();
            foreach (KeyValuePair<int, Player> kvp in playerIDs)
            {
                playerDict.Add(kvp.Key, new playerStats(kvp.Value.playerName));
            }

            teamDict = new Dictionary<int, teamStats>();
            foreach (KeyValuePair<int, Team> kvp in teamIDs)
            {
                teamDict.Add(kvp.Key, new teamStats(kvp.Value.teamName));
            }
        }
        #endregion
    }
}
