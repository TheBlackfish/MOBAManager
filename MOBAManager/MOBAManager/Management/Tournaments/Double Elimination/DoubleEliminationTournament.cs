using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MOBAManager.Management.Tournaments
{
    class DoubleEliminationTournament : Tournament
    {
        /// <summary>
        /// Returns "Double Elimination".
        /// </summary>
        /// <returns></returns>
        protected override string GetTournamentType()
        {
            return "Double Elimination";
        }

        #region Public methods
        /// <summary>
        /// Creates all of the matches for the double elimination tournament.
        /// </summary>
        public override void SetupMatches()
        {
            bool firstLowerRoundPlacement = true;
            int currentRound = 0;
            int currentID = 0;

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
                TourneyMatchup tm = new TourneyMatchup(currentID, 1, GetTeamInSlot, GetTeamInSlot, new int[] { currentRound, lowerBound }, lowerBound, upperBound);
                currentID++;
                lowerBound++;
                upperBound--;
                currentWinnerFunctions.Add(tm.GetWinner);
                currentLoserFunctions.Add(tm.GetLoser);
                upcomingMatchups.Add(tm);
            }

            int tempLowerBound = lowerBound;
            currentRound++;

            //while (winner func delegates != 1 && loser func delegates != 1)
            while (currentWinnerFunctions.Count > 1 || currentLoserFunctions.Count > 1)
            {
                //create match using current loser functions, grabbing mirrored positions
                while (currentLoserFunctions.Count > 1)
                {
                    int[] cellPos = new int[] { -1, -1 };

                    if (firstLowerRoundPlacement)
                    {
                        cellPos = new int[] { currentRound, lowerBound };
                        lowerBound++;
                    }
                    else
                    {
                        cellPos = new int[] { currentRound, ((TourneyMatchup)currentLoserFunctions[0].Target).cellPosition[1] };
                        if (cellPos[1] < tempLowerBound)
                        {
                            cellPos = new int[] { cellPos[0], tempLowerBound };
                        }
                    }

                    TourneyMatchup tm = new TourneyMatchup(currentID, 1, currentLoserFunctions[0], currentLoserFunctions[currentLoserFunctions.Count - 1], cellPos);
                    currentID++;
                    currentLoserFunctions = currentLoserFunctions.Skip(1).Take(currentLoserFunctions.Count - 2).ToList();
                    upcomingMatchups.Add(tm);
                    upcomingLoserFunctions.Add(tm.GetWinner);
                }
                firstLowerRoundPlacement = false;

                //if winner func delegates > 1
                while (currentWinnerFunctions.Count > 1)
                {
                    TourneyMatchup tm = new TourneyMatchup(currentID, 1, currentWinnerFunctions[0], currentWinnerFunctions[1], new int[] { currentRound, ((TourneyMatchup)currentWinnerFunctions[0].Target).cellPosition[1] });
                    currentID++;
                    currentWinnerFunctions = currentWinnerFunctions.Skip(2).ToList();
                    upcomingMatchups.Add(tm);
                    upcomingWinnerFunctions.Add(tm.GetWinner);
                    upcomingLoserFunctions.Add(tm.GetLoser);
                }

                currentWinnerFunctions = currentWinnerFunctions.Concat(upcomingWinnerFunctions).ToList();
                currentLoserFunctions = currentLoserFunctions.Concat(upcomingLoserFunctions).ToList();
                upcomingWinnerFunctions = new List<Func<int, Team>>();
                upcomingLoserFunctions = new List<Func<int, Team>>();

                currentRound++;
            }

            //Create final match using last remaining winner func and loser func
            TourneyMatchup finalMatch = new TourneyMatchup(currentID, 3, currentWinnerFunctions[0], currentLoserFunctions[0], new int[] { currentRound, 0 });
            upcomingMatchups.Add(finalMatch);
        }

        /// <summary>
        /// Creates all of the matches for the double elimination tournament and then spreads them over the number of days specified.
        /// </summary>
        /// <param name="overDays"></param>
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
        /// Returns a list of all teams participating in this tournament, ranked by last round played with number of wins as the tiebreaker.
        /// </summary>
        /// <returns></returns>
        public override List<Team> GetRankedResults()
        {
            List<Tuple<Team, int, int>> teamsByWins = new List<Tuple<Team, int, int>>();
            foreach (Team t in includedTeams)
            {
                teamsByWins.Add(new Tuple<Team, int, int>(t, 0, -1));
            }

            foreach (TourneyMatchup tm in resolvedMatchups)
            {
                Team winner = tm.GetWinner(0);
                Team loser = tm.GetLoser(0);
                for (int i = 0; i < teamsByWins.Count; i++)
                {
                    if (teamsByWins[i].Item1.ID == winner.ID)
                    {
                        teamsByWins[i] = new Tuple<Team, int, int>(winner, teamsByWins[i].Item2 + 1, teamsByWins[i].Item3);
                    }
                    else if (teamsByWins[i].Item1.ID == loser.ID)
                    {
                        teamsByWins[i] = new Tuple<Team, int, int>(loser, teamsByWins[i].Item2, tm.cellPosition[0] > teamsByWins[i].Item3 ? tm.cellPosition[0] : teamsByWins[i].Item3);
                    }
                }
            }

            if (IsComplete())
            {
                TourneyMatchup championship = resolvedMatchups.Last();
                Team winner = championship.GetWinner(0);
                for (int i = 0; i < teamsByWins.Count; i++)
                {
                    if (teamsByWins[i].Item1.ID == winner.ID)
                    {
                        teamsByWins[i] = new Tuple<Team, int, int>(winner, teamsByWins[i].Item2, int.MaxValue);
                    }
                }
            }

            return teamsByWins.OrderByDescending(t => t.Item3).ThenByDescending(t => t.Item2).Select(t => t.Item1).ToList();
        }
        #endregion

        /// <summary>
        /// Creates a new Double Elimination tournament.
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
        /// Creates a new Double Elimination tournament that spans multiple days.
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

        /// <summary>
        /// Creates a new Double Elimination tournament that uses invites instead of fixed teams.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="ID">The ID of the tournament</param>
        /// <param name="numberOfTeams">The minimum number of teams participating in the tournament.</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        public DoubleEliminationTournament(string name, int ID, int numberOfTeams, Dictionary<int, Hero> heroes)
            : base(name, ID, numberOfTeams, heroes)
        {
            //blank
        }

        /// <summary>
        /// Creates a new Double Elimination tournament that uses invites instead of fixed teams and spans multiple days.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="ID">The ID of the tournament</param>
        /// <param name="numberOfTeams">The minimum number of teams participating in the tournament.</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        /// <param name="numberOfDays">The number of days this tournament spans</param>
        public DoubleEliminationTournament(string name, int ID, int numberOfTeams, Dictionary<int, Hero> heroes, int numberOfDays)
            : base(name, ID, numberOfTeams, heroes, numberOfDays)
        {
            //blank
        }

        /// <summary>
        /// Creates a new Double Elimination tournament from the XElement provided.
        /// </summary>
        /// <param name="tm">The TeamManager that relates to this tournament.</param>
        /// <param name="hm">The HeroManager that relates to this tournament.</param>
        /// <param name="src">The XElement to build from.</param>
        public DoubleEliminationTournament(TeamManager tm, HeroManager hm, XElement src)
            : base(tm, hm, src)
        {
            //blank
        }
    }
}