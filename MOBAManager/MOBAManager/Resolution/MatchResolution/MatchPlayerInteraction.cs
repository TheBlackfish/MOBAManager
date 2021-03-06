﻿using MOBAManager.Management.Players;
using System;
using System.Collections.Generic;

namespace MOBAManager.MatchResolution
{
    public partial class Match
    {
        #region Public methods
        /// <summary>
        /// Returns the full list of possible actions for the player interactions.
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string, int, int>> GetPlayerInteractions()
        {
            List<Tuple<string, int, int>> ret = new List<Tuple<string, int, int>>();

            ret.Add(new Tuple<string, int, int>("You guys decide.", 0, -1));

            ret.Add(new Tuple<string, int, int>("Select the best pick for these lineups.", 1, -1));

            ret.Add(new Tuple<string, int, int>("Select the best counter to their lineup.", 4, -1));

            foreach (Player p in team1.GetTeammates())
            {
                ret.Add(new Tuple<string, int, int>("Select whatever " + p.PlayerName + " is good at.", 2, p.ID));
            }

            foreach (Player p in team2.GetTeammates())
            {
                ret.Add(new Tuple<string, int, int>("Select whatever " + p.PlayerName + " is good at.", 2, p.ID));
            }

            ret.Add(new Tuple<string, int, int>("Pick the worst possible option.", 3, -1));

            ret.Add(new Tuple<string, int, int>("Just click the random button.", -1, -1));

            return ret;
        }

        /// <summary>
        /// Submits the user's suggestion for what their team should do.
        /// </summary>
        /// <param name="manuever">The type of selection to perform.</param>
        /// <param name="targetPlayerID">The target ID of a player to target, or -1 for no selection.</param>
        public void SubmitPlayerRecommendation(int manuever, int targetPlayerID)
        {
            ms.SetTeamDecisionProcess(playerTeam, manuever, targetPlayerID);
        }
        #endregion
    }
}