using MOBAManager.Management.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Teams
{
    sealed public class Team
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
