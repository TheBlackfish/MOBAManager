using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using MOBAManager.MatchResolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management
{
    class GameManager
    {
        #region Variables
        /// <summary>
        /// The hero manager of the current game.
        /// </summary>
        private HeroManager heroManager;

        /// <summary>
        /// The player manager of the current game.
        /// </summary>
        private PlayerManager playerManager;

        /// <summary>
        /// The team manager of the current game.
        /// </summary>
        private TeamManager teamManager;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the game manager and all of its subordinate managers.
        /// </summary>
        public GameManager()
        {
            heroManager = new HeroManager();

            playerManager = new PlayerManager();

            teamManager = new TeamManager();

            teamManager.populateTeams(playerManager.getAllPlayers());
        }
        #endregion
    }
}
