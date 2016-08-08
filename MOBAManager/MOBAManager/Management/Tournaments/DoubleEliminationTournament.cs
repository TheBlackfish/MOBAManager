using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Tournaments
{
    class DoubleEliminationTournament : Tournament
    {
        /// <summary>
        /// Creates all of the matches for the double elimination tournament.
        /// </summary>
        public override void setupMatches()
        {
            //Set up current winner and loser func delegate lists
            List<Func<int, Team>> currentWinnerFunctions = new List<Func<int, Team>>();
            List<Func<int, Team>> currentLoserFunctions = new List<Func<int, Team>>();

            List<Func<int, Team>> upcomingWinnerFunctions = new List<Func<int, Team>>();
            List<Func<int, Team>> upcomingLoserFunctions = new List<Func<int, Team>>();

            //Create initial round of matches
            int lowerBound = 0;
            int upperBound = includedTeams.Count - 1;

            while (lowerBound < upperBound)
            {
                TourneyMatchup tm = new TourneyMatchup(1, getTeamInSlot, getTeamInSlot, lowerBound, upperBound);
                lowerBound++;
                upperBound--;
                currentWinnerFunctions.Add(tm.GetWinner);
                currentLoserFunctions.Add(tm.GetLoser);
                upcomingMatchups.Add(tm);
            }

            //while (winner func delegates != 1 && loser func delegates != 1)
            while (currentWinnerFunctions.Count > 1 || currentLoserFunctions.Count > 1)
            {
                //create match using current loser functions, grabbing mirrored positions
                while (currentLoserFunctions.Count > 1)
                {
                    TourneyMatchup tm = new TourneyMatchup(1, currentLoserFunctions[0], currentLoserFunctions[currentLoserFunctions.Count - 1]);
                    currentLoserFunctions = currentLoserFunctions.Skip(1).Take(currentLoserFunctions.Count - 2).ToList();
                    upcomingMatchups.Add(tm);
                    upcomingLoserFunctions.Add(tm.GetWinner);
                }

                //if winner func delegates > 1
                while (currentWinnerFunctions.Count > 1)
                {
                    TourneyMatchup tm = new TourneyMatchup(1, currentWinnerFunctions[0], currentWinnerFunctions[1]);
                    currentWinnerFunctions = currentWinnerFunctions.Skip(2).ToList();
                    upcomingMatchups.Add(tm);
                    upcomingWinnerFunctions.Add(tm.GetWinner);
                    upcomingLoserFunctions.Add(tm.GetLoser);
                }

                currentWinnerFunctions = currentWinnerFunctions.Concat(upcomingWinnerFunctions).ToList();
                currentLoserFunctions = currentLoserFunctions.Concat(upcomingLoserFunctions).ToList();
                upcomingWinnerFunctions = new List<Func<int, Team>>();
                upcomingLoserFunctions = new List<Func<int, Team>>();
            }

            //Create final match using last remaining winner func and loser func
            TourneyMatchup finalMatch = new TourneyMatchup(3, currentWinnerFunctions[0], currentLoserFunctions[0]);
            upcomingMatchups.Add(finalMatch);
        }

        /// <summary>
        /// Creates all of the matches for the double elimination tournament and then spreads them over the number of days specified.
        /// </summary>
        /// <param name="overDays"></param>
        public override void setupMatches(int overDays)
        {
            setupMatches();

            //Find out how many matches should occur each day, adding more to the first days if necessary.
            List<int> matchesPerDay = new List<int>();

            while ((matchesPerDay.Count + overDays) <= upcomingMatchups.Count)
            {
                for (int i = 0; i < overDays; i++)
                {
                    matchesPerDay.Add(i + 0);
                }
            }

            int remainingAmount = upcomingMatchups.Count - matchesPerDay.Count;
            if (remainingAmount > 0)
            {
                for (int i = 0; i < remainingAmount; i++)
                {
                    matchesPerDay.Add(i + 0);
                }
            }

            matchesPerDay = matchesPerDay.OrderBy(num => num).ToList();

            //Assign upcoming matchups to days equal to what each day should have.
            for (int i = 0; i < upcomingMatchups.Count; i++)
            {
                upcomingMatchups[i].DayOfMatch = matchesPerDay[i];
            }
        }

        /// <summary>
        /// Creates a new Single Elimination tournament.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="id">The ID of the tournament</param>
        /// <param name="teams">All of the teams participating in the tournament</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        public DoubleEliminationTournament(string name, int id, List<Team> teams, Dictionary<int, Hero> heroes)
            : base(name, id, teams, heroes)
        {
            //blank because why not
        }

        /// <summary>
        /// Creates a new Single Elimination tournament that spans multiple days.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="id">The ID of the tournament</param>
        /// <param name="teams">All of the teams participating in the tournament</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        /// <param name="numberOfDays">The number of days this tournament goes over.</param>
        public DoubleEliminationTournament(string name, int id, List<Team> teams, Dictionary<int, Hero> heroes, int numberOfDays)
            : base(name, id, teams, heroes, numberOfDays)
        {
            //blank because why not
        }
    }
}