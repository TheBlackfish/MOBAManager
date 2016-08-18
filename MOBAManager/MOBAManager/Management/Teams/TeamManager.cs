using MOBAManager.Management.Players;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Teams
{
    sealed public partial class TeamManager
    {
        #region Variables
        /// <summary>
        /// The player manager that this team manager uses for players.
        /// </summary>
        private PlayerManager pm;
        
        /// <summary>
        /// The dictionary containing all of the teams.
        /// </summary>
        private Dictionary<int, Team> teams;
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a team based on its ID.
        /// </summary>
        /// <param name="id">The ID of the team being sought.</param>
        /// <returns></returns>
        public Team GetTeamByID(int id)
        {
            List<Team> results = teams.Where(kvp => kvp.Key == id).Select(kvp => kvp.Value).ToList();
            if (results.Count == 1)
            {
                return results[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the team's ID that matches the name provided, or -1 if no team has that name.
        /// </summary>
        /// <param name="name">The name of the team to retrieve the ID for.</param>
        /// <returns></returns>
        public int GetTeamID(string name)
        {
            foreach (Team t in teams.Select(kvp => kvp.Value).ToList())
            {
                if (t.TeamName == name)
                {
                    return t.ID;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the name of the team with the ID specified.
        /// </summary>
        /// <param name="id">The ID of the team.</param>
        /// <returns></returns>
        public string GetTeamName(int id)
        {
            return teams[id].TeamName;
        }

        /// <summary>
        /// Gets all of the teams in a list.
        /// </summary>
        /// <returns>The list containing all teams.</returns>
        public List<Team> GetAllTeams()
        {
            return teams.Select(kvp => kvp.Value).ToList();
        }

        /// <summary>
        /// Returns all of the teams in a ID-aligned dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Team> GetTeamDictionary()
        {
            return teams;
        }

        /// <summary>
        /// Seeds all of the teams currently not legal with the players provided.
        /// </summary>
        /// <param name="population">The list of players eligible to </param>
        public void PopulateTeams(List<Player> population)
        {
            List<Team> tl = GetAllTeams();

            foreach (Team t in tl)
            {
                while (!t.IsLegalTeam())
                {
                    Player curPlayer = population[RNG.Roll(population.Count)];
                    t.AddMember(curPlayer);
                    population = population.Where(n => n.ID != curPlayer.ID).ToList();
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new team manager with the default teams created.
        /// <param name="pm">The player manager to use with this team manager.</param>
        /// </summary>
        public TeamManager(PlayerManager pm)
        {
            teams = new Dictionary<int, Team>();
            this.pm = pm;
            CreateTeams();   
        }
        #endregion
    }
}
