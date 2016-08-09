using MOBAManager.Management.Calendar;
using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System;

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

            Tournament deTest = new DoubleEliminationTournament("Test Championship", 2, tm.GetAllTeams(), hm.GetHeroDictionary(), 2);
            tournaments.Add(deTest);
            cm.AddTournamentDate(deTest.ID, 0);
        }
    }
}