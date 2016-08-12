using MOBAManager.Management.Players;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Teams
{
    sealed public partial class Team
    {
        #region Static Variables
        /// <summary>
        /// The most amount of players a team can have.
        /// </summary>
        public static int MAX_CAPACITY = 5;
        #endregion

        #region Variables
        /// <summary>
        /// The ID of the team.
        /// </summary>
        private int _id;

        /// <summary>
        /// The name of the team.
        /// </summary>
        private string _name;

        /// <summary>
        /// The list containing all of the team's members.
        /// </summary>
        private List<Player> teammates;
        #endregion

        #region Accessors
        /// <summary>
        /// The ID of the team.
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The name of the team.
        /// </summary>
        public string TeamName
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Attempts to add the player to the team.
        /// Returns whether or not the operation was successful.
        /// </summary>
        /// <param name="p">The player to add to the team.</param>
        /// <returns>Whether or not the addition operation was successful.</returns>
        public bool AddMember(Player p)
        {
            if (teammates.Count < MAX_CAPACITY)
            {
                teammates.Add(p);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to remove the provided player from the team.
        /// Returns whether or not the operation was successful.
        /// </summary>
        /// <param name="p">The player to remove from the team.</param>
        /// <returns>Whether or not the removal operation was successful.</returns>
        public bool RemoveMember(Player p)
        {
            if (teammates.Contains(p))
            {
                teammates.Remove(p);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the total teamwork of the team.
        /// For each combination of players, the teamwork value is the lower value of the two's teamwork values for each other.
        /// </summary>
        public double GetTeamworkSkill()
        {
            double total = 0;
            for (int i = 0; i < teammates.Count; i++)
            {
                for (int j = i + 1; j < teammates.Count; j++)
                {
                    double valOne = teammates[i].GetTeamworkSkill(teammates[j].ID);
                    double valTwo = teammates[j].GetTeamworkSkill(teammates[i].ID);

                    total += (valOne <= valTwo ? valOne : valTwo);
                }
            }
            return total;
        }

        /// <summary>
        /// First, 5 random player combinations are altered by the value in shouldIncrease.
        /// Then, each combination of players has a chance to increase their teamwork values if they differ by more than the tolerance value.
        /// </summary>
        /// <param name="shouldIncrease">Whether or not the random combinations should increase or decrease in value.</param>
        public void AlterTeamworkSkill(bool shouldIncrease)
        {
            double change = shouldIncrease ? 0.1 : -0.1;
            for (int i = 0; i < 5; i++)
            {
                Player one = teammates[RNG.Roll(teammates.Count)];
                Player two = one;
                while (two.ID == one.ID)
                {
                    two = teammates[RNG.Roll(teammates.Count)];
                }
                one.GainFixedTeamworkExperience(two.ID, change);
            }

            for (int i = 0; i < teammates.Count; i++)
            {
                for (int j = i + 1; j < teammates.Count; j++)
                {
                    Player one = teammates[i];
                    Player two = teammates[j];

                    if (Math.Abs(one.GetTeamworkSkill(two.ID) - two.GetTeamworkSkill(one.ID)) < 0.2)
                    {
                        if (RNG.CoinFlip())
                        {
                            one.GainTeamworkExperience(two.ID);
                        }
                        else
                        {
                            two.GainTeamworkExperience(one.ID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The team is legal if it has members equal to the maximum capacity.
        /// </summary>
        /// <returns>If the team is legal or not.</returns>
        public bool IsLegalTeam()
        {
            return (teammates.Count == MAX_CAPACITY);
        }

        /// <summary>
        /// Gets the team members of this team.
        /// </summary>
        /// <returns>The list of players on this team.</returns>
        public List<Player> GetTeammates()
        {
            return teammates;
        }
        #endregion

        #region Constructors 
        /// <summary>
        /// Creates a new team with the provided ID and name.
        /// </summary>
        /// <param name="id">The ID of the team.</param>
        /// <param name="name">The name of the team.</param>
        public Team(int id, string name)
        {
            _id = id;
            _name = name;
            teammates = new List<Player>();
        }
        #endregion
    }
}
