using MOBAManager.Management.Calendar;
using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System;
using System.Linq;

namespace MOBAManager.Management.Tournaments
{
    public partial class TournamentManager
    {
        /// <summary>
        /// Creates all of the generic tournaments for the game.
        /// </summary>
        /// <param name="cm">The calendar manager to store dates in.</param>
        /// <param name="tm">The team manager to get teams from.</param>
        /// <param name="hm">The hero manager to get heroes from.</param>
        public void CreateTournaments(CalendarManager cm, TeamManager tm, HeroManager hm)
        { 
            Tournament summerSolstice = new SingleEliminationTournament("Summer Solstice Championship", 0, tm.GetAllTeams(), hm.GetHeroDictionary(), 3);
            tournaments.Add(summerSolstice);
            cm.AddTournamentDates(summerSolstice.ID, cm.GetSummerSolsticeOffset() - 2, 3);

            Tournament winterSolstice = new SingleEliminationTournament("Winter Solstice Championship", 1, tm.GetAllTeams(), hm.GetHeroDictionary(), 3);
            tournaments.Add(winterSolstice);
            cm.AddTournamentDates(winterSolstice.ID, cm.GetWinterSolsticeOffset() - 2, 3);

            Tournament deTestA = new RoundRobinTournament("Test Championship Group A", 2, tm.GetAllTeams().Take(8).ToList(), hm.GetHeroDictionary(), 2);
            tournaments.Add(deTestA);
            cm.AddTournamentDates(deTestA.ID, 0, 2);

            Tournament deTestB = new RoundRobinTournament("Test Championship Group B", 3, tm.GetAllTeams().Skip(8).ToList(), hm.GetHeroDictionary(), 2);
            tournaments.Add(deTestB);
            cm.AddTournamentDates(deTestB.ID, 0, 2);

            Tournament deTestC = new SingleEliminationTournament("Test Championship Finals", 4, 8, hm.GetHeroDictionary());
            deTestC.addInviteFunction(deTestA.GetTopRankedTeams, 4);
            deTestC.addInviteFunction(deTestB.GetTopRankedTeams, 4);
            tournaments.Add(deTestC);
            cm.AddTournamentDate(deTestC.ID, 3);
        }
    }
}