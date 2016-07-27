using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Heroes
{
    sealed public class Hero
    {
        #region Private Variables
        /// <summary>
        /// The ID of the hero.
        /// </summary>
        private int _id;

        /// <summary>
        /// The name of the hero.
        /// </summary>
        private string _name;

        /// <summary>
        /// The base power level of the hero. This reflects how good the hero is in a vacuum.
        /// </summary>
        private int powerLevel = 0;

        /// <summary>
        /// The synergistic combos with this hero. This represents certain heros performing better with other heros on their team.
        /// </summary>
        private Dictionary<int, int> synergies;

        /// <summary>
        /// The counters to this hero. This represents certain heros being able to shut down this hero when on the opposing team, or this hero doing that much better.
        /// </summary>
        private Dictionary<int, int> counters;
        #endregion

        #region Accessors
        /// <summary>
        /// The unique ID of this hero.
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The name of the hero.
        /// </summary>
        public string HeroName
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the expected performance of the hero given the current situation in the match.
        /// </summary>
        /// <param name="friendlies">The list of all other heroes on the current team selecting.</param>
        /// <param name="enemies">The list of all other heroes on the enemy team.</param>
        /// <returns>The skill level of the hero given the circumstances provided.</returns>
        public int CalculatePerformance(List<int> friendlies, List<int> enemies)
        {
            int ret = this.powerLevel;
            foreach (int friendly in friendlies)
            {
                int toAdd;
                if (this.synergies.TryGetValue(friendly, out toAdd))
                {
                    ret += toAdd;
                }
            }

            foreach (int enemy in enemies)
            {
                int toAdd;
                if (this.synergies.TryGetValue(enemy, out toAdd))
                {
                    ret += toAdd;
                }
            }

            return ret;
        }

        /// <summary>
        /// Sets a synergistic value between the supplied hero and this one for the value provided.
        /// </summary>
        /// <param name="hero">The hero to synergize with.</param>
        /// <param name="val">The value of the synergy.</param>
        public void SetSynergy(int hero, int val)
        {
            this.synergies.Add(hero, val);
        }

        /// <summary>
        /// Sets a counter between the supplied hero and this one for the value provided.
        /// </summary>
        /// <param name="hero">The hero to counter.</param>
        /// <param name="val">The value of the counter.</param>
        public void SetCounter(int hero, int val)
        {
            this.counters.Add(hero, val);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new hero with the supplied name & ID.
        /// </summary>
        /// <param name="id">The ID of this hero.</param>
        /// <param name="name">The name of this hero.</param>
        public Hero(int id, string name) : this(id, name, 0)
        {
            //Empty because of reasons
        }

        /// <summary>
        /// Creates a new hero with the supplied name & ID.
        /// </summary>
        /// <param name="id">The ID of this hero.</param>
        /// <param name="name">The name of this hero.</param>
        /// <param name="pl">The initial power level of the hero.</param>
        public Hero(int id, string name, int pl)
        {
            _id = id;
            _name = name;
            powerLevel = pl;
            synergies = new Dictionary<int, int>();
            counters = new Dictionary<int, int>();
        }
        #endregion
    }
}
