using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Players
{
    partial class PlayerManager
    {
        #region Variables
        /// <summary>
        /// The dictionary containing all of the heroes.
        /// </summary>
        private Dictionary<int, Player> allPlayers;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a list of all of the heroes in the game.
        /// </summary>
        /// <returns>The list of all heroes.</returns>
        public List<Player> getAllPlayers()
        {
            return allPlayers.Select(kvp => kvp.Value).ToList();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the HeroManager object.
        /// </summary>
        public PlayerManager()
        {
            createPlayers();
        }
        #endregion
    }
}
