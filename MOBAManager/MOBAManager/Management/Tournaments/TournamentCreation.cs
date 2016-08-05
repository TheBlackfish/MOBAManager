using MOBAManager.Management.Calendar;
using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;

namespace MOBAManager.Management.Tournaments
{
    public partial class TournamentManager
    {
        public void CreateTournaments(CalendarManager cm, TeamManager tm, HeroManager hm)
        {
            Tournament test = new SingleEliminationTournament("First Tournament", 0, tm.GetAllTeams(), hm.GetHeroDictionary(), 3);
            tournaments.Add(test);
            cm.AddTournamentDates(test._ID, 0, 3);
        }
    }
}