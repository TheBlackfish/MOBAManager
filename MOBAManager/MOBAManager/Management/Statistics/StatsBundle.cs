using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Statistics
{
    sealed public class StatsBundle
    {
        #region Private variables
        /// <summary>
        /// The dictionary containing data for picked and banned heroes.
        /// </summary>
        private Dictionary<int, bool> heroPicks;

        /// <summary>
        /// The dictionary containing data for hero wins and losses.
        /// </summary>
        private Dictionary<int, bool> heroWins;

        /// <summary>
        /// The dictionary containing data for player wins and losses.
        /// </summary>
        private Dictionary<int, bool> playerWins;

        /// <summary>
        /// The dictionary containing data for team wins and losses.
        /// </summary>
        private Dictionary<int, bool> teamWins;

        private List<Tuple<int, int>> playerHeroCombinations;
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the hero picks/bans dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, bool> GetHeroPickBans()
        {
            return heroPicks;
        }

        /// <summary>
        /// Returns the hero win/loss dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, bool> GetHeroWins()
        {
            return heroWins;
        }

        /// <summary>
        /// Returns the player win/loss dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, bool> GetPlayerWins()
        {
            return playerWins;
        }

        /// <summary>
        /// Returns the team win/loss dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, bool> GetTeamWins()
        {
            return teamWins;
        }

        public List<Tuple<int, int>> GetPlayerHeroPicks()
        {
            return playerHeroCombinations;
        }

        /// <summary>
        /// Adds the data for a hero being either picked or banned to the bundle, replacing any previously set value if necessary.
        /// </summary>
        /// <param name="ID">The ID of the hero.</param>
        /// <param name="wasPicked">If true, the hero was picked. If false, the hero was banned.</param>
        public void AddHeroPickBan(int ID, bool wasPicked)
        {
            if (heroPicks.ContainsKey(ID))
            {
                heroPicks[ID] = wasPicked;
            }
            else
            {
                heroPicks.Add(ID, wasPicked);
            }
        }

        /// <summary>
        /// Adds the data for a hero either winning or losing to the bundle, replacing any previously set value if necessary.
        /// </summary>
        /// <param name="ID">The ID of the hero.</param>
        /// <param name="won">If true, the hero won. If false, the hero lost.</param>
        public void AddHeroWin(int ID, bool won)
        {
            if (heroWins.ContainsKey(ID))
            {
                heroWins[ID] = won;
            }
            else
            {
                heroWins.Add(ID, won);
            }
        }

        /// <summary>
        /// Adds the data for a player either winning or losing to the bundle, replacing any previously set value if necessary.
        /// </summary>
        /// <param name="ID">The ID of the player.</param>
        /// <param name="won">If true, the player won. If false, the player lost.</param>
        public void AddPlayerWin(int ID, bool won)
        {
            if (playerWins.ContainsKey(ID))
            {
                playerWins[ID] = won;
            }
            else
            {
                playerWins.Add(ID, won);
            }
        }

        public void AddPlayerHeroCombination(int playerID, int heroID)
        {
            playerHeroCombinations.Add(new Tuple<int, int>(playerID, heroID));
        }

        /// <summary>
        /// Adds the data for a team either winning or losing to the bundle, replacing any previously set value if necessary.
        /// </summary>
        /// <param name="ID">The ID of the team.</param>
        /// <param name="won">If true, the team won. If false, the team lost.</param>
        public void AddTeamWin(int ID, bool won)
        {
            if (teamWins.ContainsKey(ID))
            {
                teamWins[ID] = won;
            }
            else
            {
                teamWins.Add(ID, won);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new StatsBundle.
        /// </summary>
        public StatsBundle()
        {
            heroPicks = new Dictionary<int, bool>();
            heroWins = new Dictionary<int, bool>();
            playerWins = new Dictionary<int, bool>();
            teamWins = new Dictionary<int, bool>();

            playerHeroCombinations = new List<Tuple<int, int>>();
        }
        #endregion
    }
}
