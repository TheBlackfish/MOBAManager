using MOBAManager.Management.Players;
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

        /// <summary>
        /// Seeds all of the teams currently not legal with the players provided.
        /// </summary>
        /// <param name="population">The list of players eligible to </param>
        public void populateTeams(List<Player> population)
        {
            Random rnd = new Random();
            List<Team> tl = getAllTeams();

            foreach (Team t in tl)
            {
                while (!t.isLegalTeam())
                {
                    Player curPlayer = population[rnd.Next(population.Count)];
                    t.addMember(curPlayer);
                    population = population.Where(n => n.ID != curPlayer.ID).ToList();
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new team manager with the default teams created.
        /// </summary>
        public TeamManager()
        {
            teams = new Dictionary<int, Team>();
            createTeams();   
        }
        #endregion
    }
}
