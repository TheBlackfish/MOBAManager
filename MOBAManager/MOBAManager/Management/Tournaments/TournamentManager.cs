using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Tournaments
{
    public partial class TournamentManager
    {
        /// <summary>
        /// The list of all tournaments in this manager.
        /// </summary>
        private List<Tournament> tournaments;

        /// <summary>
        /// Advances the current day of all active tournaments.
        /// </summary>
        public void advanceDay()
        {
            foreach(Tournament t in tournaments.Where(t => t.enabled).ToList())
            {
                t.advanceDay();
            }
        }

        /// <summary>
        /// Enables the tournament with the specified ID.
        /// </summary>
        /// <param name="ID">The ID of the tournament to enable.</param>
        public void EnableTournament(int ID)
        {
            Tournament target = GetTournamentByID(ID);
            if (target != null)
            {
                target.enabled = true;
            }
        }

        /// <summary>
        /// Returns the tournament with the specified ID.
        /// </summary>
        /// <param name="ID">The ID of the tournament to return.</param>
        /// <returns></returns>
        public Tournament GetTournamentByID(int ID)
        {
            List<Tournament> candidates = tournaments.Where(t => t.ID == ID).ToList();
            if (candidates.Count == 1)
            {
                return candidates[0];
            }
            return null;
        }

        /// <summary>
        /// Returns a list of all tournaments that are enabled.
        /// </summary>
        /// <returns></returns>
        public List<Tournament> GetEnabledTournaments()
        {
            return tournaments.Where(t => t.enabled).ToList();
        }

        /// <summary>
        /// Returns a list of all incomplete tournaments.
        /// </summary>
        /// <returns></returns>
        public List<Tournament> GetIncompleteTournaments()
        {
            return tournaments.Where(t => !t.isComplete()).OrderBy(t => t.name).ToList();
        }

        /// <summary>
        /// Creates a new tournament manager.
        /// </summary>
        public TournamentManager()
        {
            tournaments = new List<Tournament>();
        }
    }
}
