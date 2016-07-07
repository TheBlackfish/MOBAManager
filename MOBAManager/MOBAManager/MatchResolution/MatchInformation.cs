﻿using MOBAManager.Management.Heroes;
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
        /// Returns the name of the team in the specified slot.
        /// </summary>
        /// <param name="team">The slot to pick from. 1 or 2.</param>
        /// <returns></returns>
        public string getTeamName(int team)
        {
            if (team == 1)
            {
                return team1.teamName;
            }
            else if (team == 2)
            {
                return team2.teamName;
            }
            return "";
        }
              

        /// <summary>
        /// Returns a formatted string that returns the full final line-up of a team.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public string getFormattedLineup(int team)
        {
            string ret = team1.teamName;
            if (team == 2)
            {
                ret = team2.teamName;
            }

            ret += Environment.NewLine + "-----" + Environment.NewLine;

            ret += ms.getFormattedTeamLineup(team);

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

    partial class MatchAI
    {
        /// <summary>
        /// Gets a Tuple of strings containing the times left in each category formatted correctly.
        /// </summary>
        /// <returns></returns>
        public Tuple<string, string, string, string> getFormattedTimers()
        {
            string[] placeholder = new string[4];
            double min = 0;

            double seconds = Math.Truncate(bonusTimeMaximum / 1000) - Math.Truncate(team1BonusTimeCounter / 1000);
            while (seconds > 60)
            {
                min++;
                seconds -= 60;
            }
            placeholder[0] = min.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();

            seconds = Math.Truncate(regularTimeMaximum / 1000) - Math.Truncate(team1RegularTimeCounter / 1000);
            min = 0;
            while (seconds > 60)
            {
                min++;
                seconds -= 60;
            }
            placeholder[1] = min.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();

            seconds = Math.Truncate(bonusTimeMaximum / 1000) - Math.Truncate(team2BonusTimeCounter / 1000);
            min = 0;
            while (seconds > 60)
            {
                min++;
                seconds -= 60;
            }
            placeholder[2] = min.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();

            seconds = Math.Truncate(regularTimeMaximum / 1000) - Math.Truncate(team2RegularTimeCounter / 1000);
            min = 0;
            while (seconds > 60)
            {
                min++;
                seconds -= 60;
            }
            placeholder[3] = min.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();

            return new Tuple<string, string, string, string>(placeholder[0], placeholder[1], placeholder[2], placeholder[3]);
        }
    }
}