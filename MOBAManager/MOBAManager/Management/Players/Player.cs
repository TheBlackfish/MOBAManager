using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Players
{
    sealed public class Player
    {
        #region Variables
        /// <summary>
        /// The ID of the player.
        /// </summary>
        private int _id;

        /// <summary>
        /// The name of the player.
        /// </summary>
        private string _name;

        /// <summary>
        /// The pure skill of the player. This represents the player just being amazing at the game.
        /// </summary>
        private double pureSkill;

        /// <summary>
        /// The dictionary of skill using different heroes for this player.
        /// </summary>
        private Dictionary<int, double> heroSkills;
        #endregion

        #region Accessors
        /// <summary>
        /// The public accessor for the player's ID.
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The public accessor for the player's name.
        /// </summary>
        public string PlayerName
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion

        #region Public Methods
        public void GainExperience(int heroID)
        {
            //Determine a random way to gain experience.
            int type = RNG.Roll(2);
            switch (type)
            {
                case 0:
                    GainHeroExperience(heroID);
                    break;
                case 1:
                    GainPSExperience();
                    break;
            }
        }

        public void GainPSExperience()
        {
            double RNGRoll = 10;
            if (pureSkill + 1 > RNGRoll)
            {
                RNGRoll = pureSkill + 1;
            }
            if (RNG.RollDouble(RNGRoll) > pureSkill)
            {
                pureSkill += 0.1;
            }
        }

        public void GainHeroExperience(int heroID)
        {
            double RNGRoll = 25;
            double hs = GetHeroSkill(heroID);
            if (hs + 1 > RNGRoll)
            {
                RNGRoll = hs + 1;
            }
            if (RNG.RollDouble(RNGRoll) > hs)
            {
                SetHeroSkill(heroID, hs + 0.1);
            }
        }

        /// <summary>
        /// Returns the amount of skill the player has with the provided hero.
        /// If the player does not have a set skill with a hero, this method will set it to STARTING_HEROSKILL first, then return it.
        /// </summary>
        /// <param name="heroID">The ID of the hero to get the skill for.</param>
        /// <returns>The player's skill level with the hero.</returns>
        public double GetHeroSkill(int heroID)
        {
            if (!heroSkills.ContainsKey(heroID))
            {
                heroSkills.Add(heroID, -4);
            }

            double skill = 0.0;
            heroSkills.TryGetValue(heroID, out skill);
            return skill + pureSkill; 
        }

        /// <summary>
        /// Sets the player's skill with a hero.
        /// </summary>
        /// <param name="heroID">The hero to provide a skill level for.</param>
        /// <param name="heroSkill">The skill level to set.</param>
        public void SetHeroSkill(int heroID, double heroSkill)
        {
            heroSkills[heroID] = heroSkill;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new player with the provided ID & name.
        /// </summary>
        /// <param name="id">The ID of the new player.</param>
        /// <param name="name">The name of the new player.</param>
        public Player(int id, string name) : this(id, name, 0)
        {
            //Empty for now.
        }

        /// <summary>
        /// Creates a new player with the provided ID & name.
        /// </summary>
        /// <param name="id">The ID of the new player.</param>
        /// <param name="name">The name of the new player.</param>
        /// <param name="skill">The skill level of the player.</param>
        public Player(int id, string name, int skill)
        {
            _id = id;
            _name = name;
            pureSkill = skill;
            heroSkills = new Dictionary<int, double>();
        }
        #endregion
    }
}
