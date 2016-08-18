using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBAManager.Management.Teams;
using MOBAManager.MatchResolution;
using MOBAManager.Management.Heroes;
using System.Xml.Linq;

namespace MOBAManager.Management.Tournaments
{
    class SingleEliminationTournament : Tournament
    {
        /// <summary>
        /// Returns "Single Elimination".
        /// </summary>
        /// <returns></returns>
        protected override string GetTournamentType()
        {
            return "Single Elimination";
        }

        #region Public methods
        /// <summary>
        /// Creates all matches for the tournament.
        /// Starting with the initial X teams, each round gets X/2 matches. Each round beyond the first pulls from the previously created round, until only one match is made in a round.
        /// All of these matches only pull winners from previous matches. Losers are eliminated.
        /// </summary>
        public override void SetupMatches()
        {
            //Create the first round by assigning each slot into a match.
            List<TourneyMatchup> currentRound = new List<TourneyMatchup>();

            int currentID = 0;
            int lowerBound = 0;
            int upperBound = includedTeams.Count - 1;

            while (lowerBound < upperBound)
            {
                currentRound.Add(new TourneyMatchup(currentID, 3, GetTeamInSlot, GetTeamInSlot, new int[] { 0, lowerBound }, lowerBound, upperBound));
                currentID++;
                lowerBound++;
                upperBound--;
            }

            int currentXPos = 1;

            //Create rounds until we have one with only one match
            while (currentRound.Count > 1)
            {
                List<TourneyMatchup> nextRound = new List<TourneyMatchup>();
                for (int i = 0; (i + 1) < currentRound.Count; i = i + 2)
                {
                    nextRound.Add(new TourneyMatchup(currentID, 3, currentRound[i].GetWinner, currentRound[i + 1].GetWinner, new int[] { currentXPos, currentRound[i].cellPosition[1] }));
                    currentID++;
                }

                foreach (TourneyMatchup tm in currentRound)
                {
                    upcomingMatchups.Add(tm);
                }

                currentRound = nextRound;
                currentXPos++;
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
        public override void SetupMatches(int overDays)
        {
            SetupMatches();

            if (overDays > 1)
            {
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
        }

        /// <summary>
        /// Returns a list of all teams participating in this tournament, ranked by number of wins.
        /// </summary>
        /// <returns></returns>
        override public List<Team> GetRankedResults()
        {
            List<Tuple<Team, int>> teamsByWins = new List<Tuple<Team, int>>();
            foreach (Team t in includedTeams)
            {
                teamsByWins.Add(new Tuple<Team, int>(t, 0));
            }

            foreach (TourneyMatchup tm in resolvedMatchups)
            {
                Team winner = tm.GetWinner(0);
                bool found = false;
                for (int i = 0; i < teamsByWins.Count && !found; i++)
                {
                    if (teamsByWins[i].Item1.ID == winner.ID)
                    {
                        teamsByWins[i] = new Tuple<Team, int>(winner, teamsByWins[i].Item2 + 1);
                        found = true;
                    }
                }
            }

            return teamsByWins.OrderByDescending(t => t.Item2).Select(t => t.Item1).ToList();
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

        /// <summary>
        /// Creates a new Single Elimination tournament that uses invites instead of fixed teams.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="ID">The ID of the tournament</param>
        /// <param name="numberOfTeams">The minimum number of teams participating in the tournament.</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        public SingleEliminationTournament(string name, int ID, int numberOfTeams, Dictionary<int, Hero> heroes)
            : base(name, ID, numberOfTeams, heroes)
        {
            //blank
        }

        /// <summary>
        /// Creates a new Single Elimination tournament that uses invites instead of fixed teams and spans multiple days.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="ID">The ID of the tournament</param>
        /// <param name="numberOfTeams">The minimum number of teams participating in the tournament.</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        /// <param name="numberOfDays">The number of days this tournament spans</param>
        public SingleEliminationTournament(string name, int ID, int numberOfTeams, Dictionary<int, Hero> heroes, int numberOfDays)
            : base(name, ID, numberOfTeams, heroes, numberOfDays)
        {
            //blank
        }

        /// <summary>
        /// Creates a new Single Elimination tournament from the XElement provided.
        /// </summary>
        /// <param name="tm">The TeamManager that relates to this tournament.</param>
        /// <param name="hm">The HeroManager that relates to this tournament.</param>
        /// <param name="src">The XElement to build from.</param>
        public SingleEliminationTournament(TeamManager tm, HeroManager hm, XElement src)
            : base(tm, hm, src)
        {
            //blank
        }
        #endregion
    }
}