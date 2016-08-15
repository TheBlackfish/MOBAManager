using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Heroes
{
    sealed public partial class HeroManager
    {
        #region Variables
        /// <summary>
        /// The dictionary containing all of the heroes.
        /// </summary>
        private Dictionary<int, Hero> allHeroes;
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the name of the hero specified
        /// </summary>
        /// <param name="id">The ID of the hero to get the name for.</param>
        /// <returns></returns>
        public string GetHeroName(int id)
        {
            return allHeroes[id].HeroName;
        }

        public Hero GetHeroByID(int id)
        {
            return allHeroes[id];
        }

        /// <summary>
        /// Gets a list of all of the heroes in the game.
        /// </summary>
        /// <returns>The list of all heroes.</returns>
        public List<Hero> GetAllHeroes()
        {
            return allHeroes.Select(kvp => kvp.Value).ToList();
        }

        /// <summary>
        /// Gets a list of tuples containing pairs of hero IDs and the corresponding names of the heroes matching those IDs.
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, string>> GetIDSortedNames()
        {
            List<Tuple<int, string>> ret = new List<Tuple<int, string>>();

            foreach(Hero h in allHeroes.Select(kvp => kvp.Value).ToList())
            {
                ret.Add(new Tuple<int, string>(h.ID, h.HeroName));
            }

            return ret;
        }

        /// <summary>
        /// Gets the dictionary of heroes with their IDs as the key.s
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Hero> GetHeroDictionary()
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
            CreateHeroes();
        }
        #endregion
    }
}
