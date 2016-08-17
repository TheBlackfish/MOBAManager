using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MOBAManager.Management.Statistics
{
    sealed public class StatisticsManager
    {
        #region Private classes
        /// <summary>
        /// The private class for holding data about heros in our fake-database.
        /// </summary>
        sealed private class HeroStats
        {
            #region Public variables
            /// <summary>
            /// The name of the hero.
            /// </summary>
            private string _name;

            /// <summary>
            /// The number of times this hero has been picked in official matches.
            /// </summary>
            private int _picks;

            /// <summary>
            /// The number of times this hero has been banned in official matches.
            /// </summary>
            private int _bans;

            /// <summary>
            /// The number of wins this hero has.
            /// </summary>
            private int _wins;

            /// <summary>
            /// The number of losses this hero has.
            /// </summary>
            private int _losses;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new heroStats.
            /// </summary>
            /// <param name="s">The name of the hero.</param>
            public HeroStats(string s)
            {
                _name = s;
                _picks = 0;
                _bans = 0;
                _wins = 0;
                _losses = 0;
            }

            public HeroStats(XElement src)
            {
                _name = src.Attribute("name").Value;
                _picks = int.Parse(src.Element("picks").Value);
                _bans = int.Parse(src.Element("bans").Value);
                _wins = int.Parse(src.Element("wins").Value);
                _losses = int.Parse(src.Element("losses").Value);
            }
            #endregion

            #region Public properties
            public string Name
            {
                get { return _name; }
            }

            public int Picks
            {
                get { return _picks; }
            }

            public int Bans
            {
                get { return _bans; }
            }

            public void AddPickBan(bool wasPicked)
            {
                if (wasPicked)
                {
                    _picks++;
                }
                else
                {
                    _bans++;
                }
            }

            public void AddWinLoss(bool didWin)
            {
                if (didWin)
                {
                    _wins++;
                }
                else
                {
                    _losses++;
                }
            }

            /// <summary>
            /// Returns the winrate of the hero, or 0 if the hero has never been picked.
            /// </summary>
            public string Winrate
            {
                get
                {
                    if (_wins + _losses > 0)
                    {
                        double temp = (double)_wins / (_wins + _losses);
                        temp = Math.Round(temp, 2, MidpointRounding.AwayFromZero);
                        return temp.ToString("0.00");
                    }
                    else
                    {
                        return "-";
                    }
                }
            }

            public XElement ToXML()
            {
                XElement root = new XElement("hs");

                root.SetAttributeValue("name", _name);
                root.Add(new XElement("wins", _wins));
                root.Add(new XElement("losses", _losses));
                root.Add(new XElement("picks", _picks));
                root.Add(new XElement("bans", _bans));

                return root;
            }
            #endregion
        }

        /// <summary>
        /// The class for holding player stats.
        /// </summary>
        sealed private class PlayerStats
        {
            #region Public variables
            /// <summary>
            /// The name of the player.
            /// </summary>
            private string _name;

            /// <summary>
            /// The number of wins the player has.
            /// </summary>
            private int _wins;

            /// <summary>
            /// The number of losses the player has.
            /// </summary>
            private int _losses;

            private Dictionary<int, Tuple<int, int>> _heroStatistics;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new playerStats.
            /// </summary>
            /// <param name="s">The name of the player.</param>
            public PlayerStats(string s)
            {
                _name = s;
                _wins = 0;
                _losses = 0;
                _heroStatistics = new Dictionary<int, Tuple<int, int>>();
            }

            public PlayerStats(XElement src)
            {
                _name = src.Attribute("name").Value;
                _wins = int.Parse(src.Element("wins").Value);
                _losses = int.Parse(src.Element("losses").Value);
                _heroStatistics = new Dictionary<int, Tuple<int, int>>();
                foreach (XElement elem in src.Element("heroStats").Elements("stat"))
                {
                    int[] vals = elem.Value
                        .Split(new char[] { ',' })
                        .Select(i => int.Parse(i))
                        .ToArray();
                    _heroStatistics.Add(vals[0], new Tuple<int, int>(vals[1], vals[2]));
                }
            }
            #endregion

            #region Public properties
            public void AddWinLoss(int heroID, bool didWin)
            {
                AddWinLoss(didWin);
                if (_heroStatistics.ContainsKey(heroID))
                {
                    Tuple<int, int> historical = _heroStatistics[heroID];
                    _heroStatistics[heroID] = new Tuple<int, int>(historical.Item1 + (didWin ? 1 : 0), historical.Item2 + (didWin ? 0 : 1));
                }
                else
                {
                    _heroStatistics.Add(heroID, new Tuple<int, int>(didWin ? 1 : 0, didWin ? 0 : 1));
                }
            }

            public void AddWinLoss(bool didWin)
            {
                if (didWin)
                {
                    _wins++;
                }
                else
                {
                    _losses++;
                }
            }

            public string HeroTotalGames(int heroID)
            {
                if (_heroStatistics.ContainsKey(heroID))
                {
                    return (_heroStatistics[heroID].Item1 + _heroStatistics[heroID].Item2).ToString();
                }
                return "-";
            }

            public string HeroWinrate(int heroID)
            {
                if (_heroStatistics.ContainsKey(heroID))
                {
                    double temp = (double)_heroStatistics[heroID].Item1 / (_heroStatistics[heroID].Item1 + _heroStatistics[heroID].Item2);
                    temp = Math.Round(temp, 2, MidpointRounding.AwayFromZero);
                    return temp.ToString("0.00");
                }
                return "-";
            }

            public string Name
            {
                get
                {
                    return _name;
                }
            }

            public int[] TopHeroes()
            {
                int[] topHeroes = _heroStatistics.OrderByDescending(kvp => (double)kvp.Value.Item1 / (kvp.Value.Item1 + kvp.Value.Item2))
                    .Select(kvp => kvp.Key)
                    .Take(3)
                    .ToArray();
                return topHeroes;
            }

            /// <summary>
            /// Returns the winrate of the player, or 0 if they have played 0 games.
            /// </summary>
            public string Winrate
            {
                get
                {
                    if (_wins + _losses > 0)
                    {
                        double temp = (double)_wins / (_wins + _losses);
                        temp = Math.Round(temp, 2, MidpointRounding.AwayFromZero);
                        return temp.ToString("0.00");
                    }
                    else
                    {
                        return "-";
                    }
                }
            }

            public int TotalGames
            {
                get
                {
                    return _wins + _losses;
                }
            }

            public XElement ToXML()
            {
                XElement root = new XElement("ps");

                root.SetAttributeValue("name", _name);
                root.Add(new XElement("wins", _wins));
                root.Add(new XElement("losses", _losses));

                XElement heroStats = new XElement("heroStats");
                foreach (KeyValuePair<int, Tuple<int, int>> kvp in _heroStatistics)
                {
                    heroStats.Add(new XElement("stat", kvp.Key + "-" + kvp.Value.Item1 + "-" + kvp.Value.Item2));
                }
                root.Add(heroStats);

                return root;
            }
            #endregion
        }

        /// <summary>
        /// The class that holds a team's stats.
        /// </summary>
        sealed private class TeamStats
        {
            #region Public variables
            /// <summary>
            /// The name of the team.
            /// </summary>
            private string _name;

            /// <summary>
            /// The number of wins the team has.
            /// </summary>
            private int _wins;

            /// <summary>
            /// The number of losses the team has.
            /// </summary>
            private int _losses;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new teamStats.
            /// </summary>
            /// <param name="s">The name of the team.</param>
            public TeamStats(string s)
            {
                _name = s;
                _wins = 0;
                _losses = 0;
            }

            public TeamStats(XElement src)
            {
                _name = src.Attribute("name").Value;
                _wins = int.Parse(src.Element("wins").Value);
                _losses = int.Parse(src.Element("losses").Value);
            }
            #endregion

            #region Public methods
            public string Name
            {
                get
                {
                    return _name;
                }
            }

            /// <summary>
            /// Returns the winrate of the team, or 0 if they haven't played in any matches.
            /// </summary>
            public string Winrate
            {
                get
                {
                    if (_wins + _losses > 0)
                    {
                        double temp = (double)_wins / (_wins + _losses);
                        temp = Math.Round(temp, 2, MidpointRounding.AwayFromZero);
                        return temp.ToString("0.00");
                    }
                    else
                    {
                        return "-";
                    }
                }
            }

            public int TotalGames
            {
                get
                {
                    return _wins + _losses;
                }
            }

            public void AddWinLoss(bool didWin)
            {
                if (didWin)
                {
                    _wins++;
                }
                else
                {
                    _losses++;
                }
            }

            public XElement ToXML()
            {
                XElement root = new XElement("ts");

                root.SetAttributeValue("name", _name);
                root.Add(new XElement("wins", _wins));
                root.Add(new XElement("losses", _losses));

                return root;
            }
            #endregion
        }
        #endregion

        #region Private variables
        /// <summary>
        /// The dictionary holding herostats. Each herostat's key is that hero's ID.
        /// </summary>
        private Dictionary<int, HeroStats> heroDict;

        /// <summary>
        /// The dictionary holding playerstats. Each playerstat's key is that player's ID.
        /// </summary>
        private Dictionary<int, PlayerStats> playerDict;

        /// <summary>
        /// The dictionary holding teamstats. Each teamstat's key is that player's ID.
        /// </summary>
        private Dictionary<int, TeamStats> teamDict;
        #endregion

        #region Public methods
        /// <summary>
        /// Processes many stat bundles into the manager.
        /// </summary>
        /// <param name="bundles">The list containing all of the bundles to process.</param>
        /// <param name="hm">The hero manager of the current game.</param>
        /// <param name="pm">The player manager of the current game.</param>
        /// <param name="tm">The team manager of the current game.</param>
        public void ProcessManyBundles(List<StatsBundle> bundles, HeroManager hm, PlayerManager pm, TeamManager tm)
        {
            foreach(StatsBundle sb in bundles)
            {
                ProcessBundle(sb, hm, pm, tm);
            }
        }

        /// <summary>
        /// Processes a stat bundle and adds all relevant information to the various stat dictionaries.
        /// </summary>
        /// <param name="bundle">The bundle to process.</param>
        /// <param name="hm">The hero manager of the current game.</param>
        /// <param name="pm">The player manager of the current game.</param>
        /// <param name="tm">The team manager of the current game.</param>
        public void ProcessBundle(StatsBundle bundle, HeroManager hm, PlayerManager pm, TeamManager tm)
        {
            Dictionary<int, bool> pb = bundle.GetHeroPickBans();
            Dictionary<int, bool> hw = bundle.GetHeroWins();
            Dictionary<int, bool> pw = bundle.GetPlayerWins();
            Dictionary<int, bool> tw = bundle.GetTeamWins();
            List<Tuple<int, int>> phc = bundle.GetPlayerHeroPicks();

            foreach(KeyValuePair<int, bool> kvp in pb.Select(kvp => kvp).ToList())
            {
                if (!heroDict.ContainsKey(kvp.Key))
                {
                    HeroStats newHero = new HeroStats(hm.GetHeroName(kvp.Key));
                    heroDict.Add(kvp.Key, newHero);
                }

                heroDict[kvp.Key].AddPickBan(kvp.Value);
            }

            foreach(KeyValuePair<int, bool> kvp in hw.Select(kvp => kvp).ToList())
            {
                if (!heroDict.ContainsKey(kvp.Key))
                {
                    HeroStats newHero = new HeroStats(hm.GetHeroName(kvp.Key));
                    heroDict.Add(kvp.Key, newHero);
                }

                heroDict[kvp.Key].AddWinLoss(kvp.Value);
            }

            foreach (KeyValuePair<int, bool> kvp in pw.Select(kvp => kvp).ToList())
            {
                if (kvp.Key >= 0)
                {
                    if (!playerDict.ContainsKey(kvp.Key))
                    {
                        PlayerStats newPlayer = new PlayerStats(pm.GetPlayerName(kvp.Key));
                        playerDict.Add(kvp.Key, newPlayer);
                    }

                    playerDict[kvp.Key].AddWinLoss(phc.Where(t => t.Item1 == kvp.Key)
                                                    .Select(t => t.Item2)
                                                    .First(), kvp.Value);
                }
            }

            foreach (KeyValuePair<int, bool> kvp in tw.Select(kvp => kvp).ToList())
            {
                if (kvp.Key >= 0)
                {
                    if (!teamDict.ContainsKey(kvp.Key))
                    {
                        TeamStats newTeam = new TeamStats(tm.GetTeamName(kvp.Key));
                        teamDict.Add(kvp.Key, newTeam);
                    }

                    teamDict[kvp.Key].AddWinLoss(kvp.Value);
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
        public BindingSource GetHeroStats()
        {
            BindingSource bs = new BindingSource();

            List<HeroStats> hStats = heroDict.Select(kvp => kvp.Value)
                .OrderByDescending(hs => hs.Winrate)
                .ThenByDescending(hs => hs.Picks + hs.Bans)
                .ToList();

            foreach (HeroStats hs in hStats)
            {
                Tuple<string, string, int, int> res = new Tuple<string, string, int, int>(hs.Name, hs.Winrate, hs.Bans, hs.Picks);
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
        public BindingSource GetPlayerStats()
        {
            BindingSource bs = new BindingSource();

            List<PlayerStats> pStats = playerDict.Select(kvp => kvp.Value)
                .OrderByDescending(ps => ps.Winrate)
                .ThenByDescending(ps => ps.TotalGames)
                .ToList();

            foreach (PlayerStats ps in pStats)
            {
                int[] notableHeroes = ps.TopHeroes();
                string nhstr = "";
                foreach (int i in notableHeroes)
                {
                    if (nhstr.Length > 0)
                    {
                        nhstr += ", ";
                    }
                    nhstr += heroDict[i].Name;
                }
                Tuple<string, string, int, string> res = new Tuple<string, string, int, string>(ps.Name, ps.Winrate, ps.TotalGames, nhstr);
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
        public BindingSource GetTeamStats()
        {
            BindingSource bs = new BindingSource();

            List<TeamStats> tStats = teamDict.Select(kvp => kvp.Value)
                .OrderByDescending(ps => ps.Winrate)
                .ThenByDescending(ps => ps.TotalGames)
                .ToList();

            foreach (TeamStats ts in tStats)
            {
                Tuple<string, string, int> res = new Tuple<string, string, int>(ts.Name, ts.Winrate, ts.TotalGames);
                bs.Add(res);
            }

            return bs;
        }

        public XElement ToXML()
        {
            XElement root = new XElement("stats");

            XElement heroRoot = new XElement("heroStats");
            foreach (HeroStats hs in heroDict.Select(kvp => kvp.Value).ToList()) 
            {
                heroRoot.Add(hs.ToXML());
            }
            root.Add(heroRoot);

            XElement playerRoot = new XElement("playerStats");
            foreach (PlayerStats ps in playerDict.Select(kvp => kvp.Value).ToList())
            {
                playerRoot.Add(ps.ToXML());
            }
            root.Add(playerRoot);

            XElement teamRoot = new XElement("teamStats");
            foreach (TeamStats ts in teamDict.Select(kvp => kvp.Value).ToList())
            {
                teamRoot.Add(ts.ToXML());
            }
            root.Add(teamRoot);

            return root;
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
            heroDict = new Dictionary<int, HeroStats>();
            foreach (KeyValuePair<int, Hero> kvp in heroIDs)
            {
                heroDict.Add(kvp.Key, new HeroStats(kvp.Value.HeroName));
            }

            playerDict = new Dictionary<int, PlayerStats>();
            foreach (KeyValuePair<int, Player> kvp in playerIDs)
            {
                playerDict.Add(kvp.Key, new PlayerStats(kvp.Value.PlayerName));
            }

            teamDict = new Dictionary<int, TeamStats>();
            foreach (KeyValuePair<int, Team> kvp in teamIDs)
            {
                teamDict.Add(kvp.Key, new TeamStats(kvp.Value.TeamName));
            }
        }

        public StatisticsManager(HeroManager hm, PlayerManager pm, TeamManager tm, XElement src)
        {
            heroDict = new Dictionary<int, HeroStats>();
            foreach (XElement elem in src.Element("heroStats").Elements("hs"))
            {
                HeroStats hs = new HeroStats(elem);
                heroDict.Add(hm.GetHeroID(hs.Name), hs);
            }

            playerDict = new Dictionary<int, PlayerStats>();
            foreach (XElement elem in src.Element("playerStats").Elements("ps"))
            {
                PlayerStats ps = new PlayerStats(elem);
                playerDict.Add(pm.GetPlayerID(ps.Name), ps);
            }

            teamDict = new Dictionary<int, TeamStats>();
            foreach (XElement elem in src.Element("teamStats").Elements("ts"))
            {
                TeamStats ts = new TeamStats(elem);
                teamDict.Add(tm.GetTeamID(ts.Name), ts);
            }
        }
        #endregion
    }
}
