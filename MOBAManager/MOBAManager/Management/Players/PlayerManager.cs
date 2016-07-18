using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Players
{
    public partial class PlayerManager
    {
        #region Variables
        /// <summary>
        /// The dictionary containing all of the heroes.
        /// </summary>
        private Dictionary<int, Player> allPlayers;
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the name of the player specified.
        /// </summary>
        /// <param name="id">The ID of the player to get the name of.</param>
        /// <returns></returns>
        public string getPlayerName(int id)
        {
            return allPlayers[id].playerName;
        }

        /// <summary>
        /// Gets a list of all of the heroes in the game.
        /// </summary>
        /// <returns>The list of all heroes.</returns>
        public List<Player> getAllPlayers()
        {
            return allPlayers.Select(kvp => kvp.Value).ToList();
        }

        /// <summary>
        /// Returns a list of all the players in an ID-aligned dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Player> getPlayerDictionary()
        {
            return allPlayers;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the HeroManager object.
        /// </summary>
        public PlayerManager()
        {
            allPlayers = new Dictionary<int, Player>();
            createPlayers();
        }
        #endregion
    }
}
