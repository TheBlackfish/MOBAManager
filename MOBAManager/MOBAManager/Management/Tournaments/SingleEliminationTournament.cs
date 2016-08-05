using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBAManager.Management.Teams;
using MOBAManager.MatchResolution;
using MOBAManager.Management.Heroes;

namespace MOBAManager.Management.Tournaments
{
    class SingleEliminationTournament : Tournament
    {
        #region Public methods
        /// <summary>
        /// Creates all matches for the tournament.
        /// Starting with the initial X teams, each round gets X/2 matches. Each round beyond the first pulls from the previously created round, until only one match is made in a round.
        /// All of these matches only pull winners from previous matches. Losers are eliminated.
        /// </summary>
        public override void setupMatches()
        {
            //Create the first round by assigning each slot into a match.
            List<TourneyMatchup> currentRound = new List<TourneyMatchup>();

            int team1Tracker = 0;
            int team2Tracker = includedTeams.Count - 1;

            while (team1Tracker < team2Tracker)
            {
                currentRound.Add(new TourneyMatchup(3, getTeamInSlot, getTeamInSlot, team1Tracker, team2Tracker));
                team1Tracker++;
                team2Tracker--;
            }

            //Create rounds until we have one with only one match
            while (currentRound.Count > 1)
            {
                List<TourneyMatchup> nextRound = new List<TourneyMatchup>();
                for (int i = 0; (i + 1) < currentRound.Count; i = i + 2)
                {
                    nextRound.Add(new TourneyMatchup(3, currentRound[i].GetWinner, currentRound[i + 1].GetWinner));
                }
                
                foreach(TourneyMatchup tm in currentRound)
                {
                    upcomingMatchups.Add(tm);
                }

                currentRound = nextRound;
            }

            foreach (TourneyMatchup tm in currentRound)
            {
                upcomingMatchups.Add(tm);
            }
        }

        /// <summary>
        /// Splits all created matches evenly over the number of days specified.
        /// </summary>
        /// <param name="overDays">The number of days the tournament occurs over.</param>
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
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Single Elimination tournament.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="id">The ID of the tournament</param>
        /// <param name="teams">All of the teams participating in the tournament</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        public SingleEliminationTournament(string name, int id, List<Team> teams, Dictionary<int, Hero> heroes)
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
        public SingleEliminationTournament(string name, int id, List<Team> teams, Dictionary<int, Hero> heroes, int numberOfDays)
            : base(name, id, teams, heroes, numberOfDays)
        {
            //blank because why not
        }
        #endregion
    }
}
