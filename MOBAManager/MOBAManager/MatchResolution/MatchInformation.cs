using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;

namespace MOBAManager.MatchResolution
{
    public partial class Match
    {
        #region Public methods
        /// <summary>
        /// Returns a tuple of strings containing the formatted timers.
        /// </summary>
        /// <returns></returns>
        public Tuple<string, string, string, string> getFormattedTimers()
        {
            return ms.getFormattedTimers();
        }

        /// <summary>
        /// Returns a multiline string containing all of the information about the selected team.
        /// </summary>
        /// <param name="team">The team to select. 1 or 2 are the only valid parameters.</param>
        /// <returns></returns>
        public string getTeamInformation(int team)
        {
            Team t = team1;
            if (team == 2)
            {
                t = team2;
            }

            string ret = t.teamName + Environment.NewLine + "-----";
            foreach (Player p in t.getTeammates())
            {
                ret += Environment.NewLine + p.playerName;
            }
            return ret;
        }

        /// <summary>
        /// Returns a multiline string containing all of Team 1's bans.
        /// </summary>
        /// <returns></returns>
        public string getFormattedTeam1Bans()
        {
            List<Hero> selections = ms.getTeamBans(1);
            return formatHeroNames(selections);
        }

        /// <summary>
        /// Returns a multiline string containing all of Team 2's bans.
        /// </summary>
        /// <returns></returns>
        public string getFormattedTeam2Bans()
        {
            List<Hero> selections = ms.getTeamBans(2);
            return formatHeroNames(selections);
        }

        /// <summary>
        /// Returns a multiline string containing all of Team 1's picks.
        /// </summary>
        /// <returns></returns>
        public string getFormattedTeam1Picks()
        {
            List<Hero> selections = ms.getTeamSelections(1);
            return formatHeroNames(selections);
        }

        /// <summary>
        /// Returns a multiline string containing all of Team 2's picks.
        /// </summary>
        /// <returns></returns>
        public string getFormattedTeam2Picks()
        {
            List<Hero> selections = ms.getTeamSelections(2);
            return formatHeroNames(selections);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Turns a list of heroes in a multiline string with their names.
        /// </summary>
        /// <param name="lh">The list of heroes to get a formatted string for.</param>
        /// <returns></returns>
        private string formatHeroNames(List<Hero> lh)
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