using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Tournaments
{
    public partial class TournamentManager
    {
        private List<Tournament> tournaments;

        public void advanceDay()
        {
            foreach(Tournament t in tournaments.Where(t => t.Enabled).ToList())
            {
                t.advanceDay();
            }
        }

        public void ActivateTournament(int ID)
        {
            Tournament target = GetTournamentByID(ID);
            if (target != null)
            {
                target.Enabled = true;
            }
        }

        public Tournament GetTournamentByID(int ID)
        {
            List<Tournament> candidates = tournaments.Where(t => t._ID == ID).ToList();
            if (candidates.Count == 1)
            {
                return candidates[0];
            }
            return null;
        }

        public List<Tournament> GetActiveTournaments()
        {
            return tournaments.Where(t => t.Enabled).ToList();
        }

        public TournamentManager()
        {
            tournaments = new List<Tournament>();
        }
    }
}
