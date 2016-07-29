using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.MatchResolution
{
    public partial class MatchAI
    {
        #region Public methods
        /// <summary>
        /// Sets a team's selection process.
        /// </summary>
        /// <param name="team">The team slot to set.</param>
        /// <param name="manuever">The type of selection to perform.</param>
        /// <param name="targetPlayerID">The target ID of a player to target, or -1 for no selection.</param>
        public void SetTeamDecisionProcess(int team, int manuever, int targetPlayerID)
        {
            if (team == 1)
            {
                team1SelectionMode = manuever;
                team1SelectionPlayerTarget = targetPlayerID;
            }
            else if (team == 2)
            {
                team2SelectionMode = manuever;
                team2SelectionPlayerTarget = targetPlayerID;
            }

            if (team == match.GetCurrentActingTeam)
            {
                SetCurrentSelectionDelay(false);
            }
        }
        #endregion
    }
}
