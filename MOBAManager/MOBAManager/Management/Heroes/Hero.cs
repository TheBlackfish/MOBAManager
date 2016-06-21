using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management
{
    class Hero
    {
        private int _id;
        private string _name;
        private int powerLevel = 0;
        private Dictionary<int, int> synergies;
        private Dictionary<int, int> counters;

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
        /// <returns></returns>
        public int calculatePerformance(List<int> friendlies, List<int> enemies)
        {
            int ret = this.powerLevel;
            foreach (var friendly in friendlies)
            {
                int toAdd;
                if (this.synergies.TryGetValue(friendly, out toAdd))
                {
                    ret += toAdd;
                }
            }

            foreach (var enemy in enemies)
            {
                int toAdd;
                if (this.synergies.TryGetValue(enemy, out toAdd))
                {
                    ret += toAdd;
                }
            }

            return 0;
        }

        /// <summary>
        /// Sets a synergistic value between the supplied hero and this one for the value provided.
        /// </summary>
        /// <param name="hero">The hero to synergize with.</param>
        /// <param name="val">The value of the synergy.</param>
        public void setSynergy(int hero, int val)
        {
            this.synergies.Add(hero, val);
        }

        /// <summary>
        /// Sets a counter between the supplied hero and this one for the value provided.
        /// </summary>
        /// <param name="hero">The hero to counter.</param>
        /// <param name="val">The value of the counter.</param>
        public void setCounter(int hero, int val)
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
        }
        #endregion
    }
}
