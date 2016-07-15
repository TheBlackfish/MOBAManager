using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Statistics;
using MOBAManager.Management.Teams;
using MOBAManager.MatchResolution;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management
{
    public class GameManager
    {
        #region Temporarily Public Variables
        /// <summary>
        /// The hero manager of the current game.
        /// </summary>
        public HeroManager heroManager;

        /// <summary>
        /// The player manager of the current game.
        /// </summary>
        public PlayerManager playerManager;

        /// <summary>
        /// The statistics manager of the current game.
        /// </summary>
        public StatisticsManager statsManager;

        /// <summary>
        /// The team manager of the current game.
        /// </summary>
        public TeamManager teamManager;
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the next match the player is involved in.
        /// Placeholder function returns a generic match for now.
        /// </summary>
        /// <returns></returns>
        public Match getNextPlayerMatch()
        {
            Match m = new Match(true, teamManager.getAllTeams()[0], teamManager.getAllTeams()[1], 1, heroManager.getHeroDictionary());
            return m;
        }

        /// <summary>
        /// Generic placeholder method that generates a new match. If the player's team is involved, it is created as a threaded match.
        /// </summary>
        /// <returns></returns>
        public Match getNextMatch()
        {
            int team1Index = -1;
            int team2Index = -1;

            while (team1Index == -1)
            {
                team1Index = RNG.roll(teamManager.getAllTeams().Count);
            }

            while (team2Index == -1 || team1Index == team2Index)
            {
                team2Index = RNG.roll(teamManager.getAllTeams().Count);
            }

            if (team1Index == 0 || team2Index == 0)
            {
                return new Match(true, teamManager.getAllTeams()[team1Index], teamManager.getAllTeams()[team2Index], (team1Index == 0) ? 1 : 2, heroManager.getHeroDictionary());
            }
            else
            {
                return new Match(teamManager.getAllTeams()[team1Index], teamManager.getAllTeams()[team2Index], heroManager.getHeroDictionary());
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the game manager and all of its subordinate managers.
        /// </summary>
        public GameManager()
        {
            heroManager = new HeroManager();

            playerManager = new PlayerManager();

            statsManager = new StatisticsManager();

            teamManager = new TeamManager();

            teamManager.populateTeams(playerManager.getAllPlayers());
        }
        #endregion
    }
}
