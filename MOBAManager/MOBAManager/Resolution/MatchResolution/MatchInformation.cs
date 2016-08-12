using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Statistics;
using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.MatchResolution
{
    public partial class Match
    {
        #region Public methods
        /// <summary>
        /// Returns a tuple of strings containing the formatted timers.
        /// </summary>
        /// <returns></returns>
        public Tuple<string, string, string, string> GetTimerDisplayInformation()
        {
            return ms.GetFormattedTimers();
        }

        /// <summary>
        /// Returns a multiline string containing all of the information about the selected team.
        /// </summary>
        /// <param name="team">The team to select. 1 or 2 are the only valid parameters.</param>
        /// <returns></returns>
        public string GetTeamDisplayInformation(int team)
        {
            Team t = team1;
            if (team == 2)
            {
                t = team2;
            }

            string ret = t.TeamName + Environment.NewLine + "-----";
            foreach (Player p in t.GetTeammates())
            {
                ret += Environment.NewLine + p.PlayerName;
            }
            return ret;
        }

        /// <summary>
        /// Returns the name of the team in the specified slot.
        /// </summary>
        /// <param name="team">The slot to pick from. 1 or 2.</param>
        /// <returns></returns>
        public string GetTeamName(int team)
        {
            if (team == 1)
            {
                return team1.TeamName;
            }
            else if (team == 2)
            {
                return team2.TeamName;
            }
            return "";
        }

        /// <summary>
        /// Gets the name of the team that is currently not player-controlled.
        /// </summary>
        /// <returns></returns>
        public string GetAITeamName()
        {
            if (IsThreaded)
            {
                if (playerTeam == 1)
                {
                    return team2.TeamName;
                }
                else
                {
                    return team1.TeamName;
                }
            }

            return "ERROR";
        }              

        /// <summary>
        /// Returns a formatted string that returns the full final line-up of a team.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public string GetLineupDisplayInformation(int team)
        {
            string ret = team1.TeamName;
            if (team == 2)
            {
                ret = team2.TeamName;
            }

            ret += Environment.NewLine + "-----" + Environment.NewLine;

            ret += ms.GetFormattedTeamLineup(team);

            return ret;
        }

        /// <summary>
        /// Control method that gets formatted information about a team's bans or picks.
        /// </summary>
        /// <param name="team">1 or 2, the team to get information for.</param>
        /// <param name="getPicks">If set to true, this will return the specified team's picks. Otherwise this will return the team's bans.</param>
        public string GetTeamPBDisplayInformation(int team, bool getPicks)
        {
            if (getPicks)
            {
                if (team == 1)
                {
                    return GetFormattedTeam1Picks();
                }
                return GetFormattedTeam2Picks();
            }
            if (team == 1)
            {
                return GetFormattedTeam1Bans();
            }
            return GetFormattedTeam2Bans();
        }

        /// <summary>
        /// Returns a multiline string containing all of Team 1's bans.
        /// </summary>
        /// <returns></returns>
        private string GetFormattedTeam1Bans()
        {
            List<Hero> selections = ms.GetTeamBans(1);
            return FormatHeroNames(selections);
        }

        /// <summary>
        /// Returns a multiline string containing all of Team 2's bans.
        /// </summary>
        /// <returns></returns>
        private string GetFormattedTeam2Bans()
        {
            List<Hero> selections = ms.GetTeamBans(2);
            return FormatHeroNames(selections);
        }

        /// <summary>
        /// Returns a multiline string containing all of Team 1's picks.
        /// </summary>
        /// <returns></returns>
        private string GetFormattedTeam1Picks()
        {
            List<Hero> selections = ms.GetTeamSelections(1);
            return FormatHeroNames(selections);
        }

        /// <summary>
        /// Returns a multiline string containing all of Team 2's picks.
        /// </summary>
        /// <returns></returns>
        private string GetFormattedTeam2Picks()
        {
            List<Hero> selections = ms.GetTeamSelections(2);
            return FormatHeroNames(selections);
        }

        /// <summary>
        /// Gets a descriptive text describing the events of the game after it has been fully resolved.
        /// The descriptive text changes verbage based on how far apart the skill levels were between teams.
        /// </summary>
        /// <returns></returns>
        public string GetMatchSummary()
        {
            int winningTeam = Winner;
            int losingTeam = 3 - winningTeam;
            if (Winner == -1)
            {
                return "The match is still being decided.";
            }

            double diff = ms.GetTeamLineupSkill(winningTeam) - ms.GetTeamLineupSkill(3 - winningTeam);

            string sum = GetTeamName(winningTeam);

            if (diff >= 2000)
            {
                sum += " utterly decimated ";
            }
            else if (diff >= 1000)
            {
                sum += " handily beat ";
            }
            else if (diff >= 400)
            {
                sum += " edged out ";
            }
            else if (diff >= -400)
            {
                sum += " squeaked a victory from ";
            }
            else if (diff >= -1000)
            {
                sum += " outplayed ";
            }
            else if (diff >= -2000)
            {
                sum += " overcame the odds to beat ";
            }
            else
            {
                sum += " utterly humiliated ";
            }

            return sum + GetTeamName(3 - winningTeam) + ".";
        }

        /// <summary>
        /// Generates and returns a stats bundle with information pertaining to this match.
        /// </summary>
        /// <returns></returns>
        public StatsBundle GetStats()
        {
            StatsBundle bundle = new StatsBundle();

            bundle = UpdateBundleWithTeamStats(bundle);
            bundle = UpdateBundleWithPlayerStats(bundle);
            bundle = UpdateBundleWithHeroStats(bundle);
            bundle = UpdateBundleWithPlayerHeroCombinations(bundle);

            return bundle;
        }

        /// <summary>
        /// Updates and returns the stats bundle provided with the correct team statistics for this match.
        /// </summary>
        /// <param name="bundle">The bundle to update.</param>
        /// <returns></returns>
        private StatsBundle UpdateBundleWithTeamStats(StatsBundle bundle)
        {
            if (Winner != -1)
            {
                bundle.AddTeamWin(team1.ID, Winner == 1);
                bundle.AddTeamWin(team2.ID, Winner == 2);
            }

            return bundle;
        }

        /// <summary>
        /// Updates and returns the stats bundle provided with the correct player statistics for this match.
        /// </summary>
        /// <param name="bundle">The bundle to update.</param>
        /// <returns></returns>
        private StatsBundle UpdateBundleWithPlayerStats(StatsBundle bundle)
        {
            if (Winner != -1)
            {
                foreach (Player p in team1.GetTeammates())
                {
                    bundle.AddPlayerWin(p.ID, Winner == 1);
                }

                foreach (Player p in team2.GetTeammates())
                {
                    bundle.AddPlayerWin(p.ID, Winner == 2);
                }
            }

            return bundle;
        }

        /// <summary>
        /// Updates and returns the stats bundle provided with the correct hero statistics for this match.
        /// </summary>
        /// <param name="bundle">The bundle to update.</param>
        /// <returns></returns>
        private StatsBundle UpdateBundleWithHeroStats(StatsBundle bundle)
        {
            if (Winner != -1)
            {
                foreach (Hero h in ms.GetTeamSelections(1))
                {
                    bundle.AddHeroWin(h.ID, Winner == 1);
                    bundle.AddHeroPickBan(h.ID, true);
                }

                foreach (Hero h in ms.GetTeamSelections(2))
                {
                    bundle.AddHeroWin(h.ID, Winner == 2);
                    bundle.AddHeroPickBan(h.ID, true);
                }

                foreach (Hero h in ms.GetTeamBans(1))
                {
                    bundle.AddHeroPickBan(h.ID, false);
                }

                foreach (Hero h in ms.GetTeamBans(2))
                {
                    bundle.AddHeroPickBan(h.ID, false);
                }
            }

            return bundle;
        }

        private StatsBundle UpdateBundleWithPlayerHeroCombinations(StatsBundle bundle)
        {
            foreach (Tuple<int, int> tpl in ms.GetPlayerHeroCombinationsForTeam(1).Concat(ms.GetPlayerHeroCombinationsForTeam(2)))
            {
                bundle.AddPlayerHeroCombination(tpl.Item1, tpl.Item2);
            }
            return bundle;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Turns a list of heroes in a multiline string with their names.
        /// </summary>
        /// <param name="lh">The list of heroes to get a formatted string for.</param>
        /// <returns></returns>
        private static string FormatHeroNames(List<Hero> lh)
        {
            if (lh.Count > 0)
            {
                string ret = "";
                foreach (Hero h in lh)
                {
                    ret += h.HeroName;
                    if (h != lh[lh.Count - 1])
                    {
                        ret += Environment.NewLine;
                    }
                }
                return ret;
            }
            return "";
        }
        #endregion
    }
}