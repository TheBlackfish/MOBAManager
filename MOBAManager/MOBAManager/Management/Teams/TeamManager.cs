using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Teams
{
    partial class TeamManager
    {
        #region Variables
        /// <summary>
        /// The dictionary containing all of the teams.
        /// </summary>
        private Dictionary<int, Team> teams;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets all of the teams in a list.
        /// </summary>
        /// <returns>The list containing all teams.</returns>
        public List<Team> getAllTeams()
        {
            return teams.Select(kvp => kvp.Value).ToList();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new team manager with the default teams created.
        /// </summary>
        public TeamManager()
        {
            createTeams();   
        }
        #endregion
    }
}
