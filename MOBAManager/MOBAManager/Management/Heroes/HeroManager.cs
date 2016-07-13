using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Heroes
{
    public partial class HeroManager
    {
        #region Variables
        /// <summary>
        /// The dictionary containing all of the heroes.
        /// </summary>
        private Dictionary<int, Hero> allHeroes;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a list of all of the heroes in the game.
        /// </summary>
        /// <returns>The list of all heroes.</returns>
        public List<Hero> getAllHeroes()
        {
            return allHeroes.Select(kvp => kvp.Value).ToList();
        }

        /// <summary>
        /// Gets the dictionary of heroes with their IDs as the key.s
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Hero> getHeroDictionary()
        {
            return allHeroes;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the HeroManager object.
        /// </summary>
        public HeroManager()
        {
            allHeroes = new Dictionary<int, Hero>();
            createHeroes();
        }
        #endregion
    }
}
