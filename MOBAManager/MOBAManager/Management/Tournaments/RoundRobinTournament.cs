using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBAManager.Management.Teams;
using MOBAManager.Utility;
using MOBAManager.Management.Heroes;

namespace MOBAManager.Management.Tournaments
{
    class RoundRobinTournament : Tournament
    {
        #region Public methods
        /// <summary>
        /// Returns the ranked list of all teams participating in this tournament.
        /// </summary>
        /// <returns></returns>
        public override List<Team> GetRankedResults()
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

        /// <summary>
        /// Creates matches by attempting to create distinct rounds consisting of exclusive pairings.
        /// </summary>
        public override void SetupMatches()
        {
            List<List<Tuple<int, int>>> allRounds = new List<List<Tuple<int, int>>>();

            //Create initial round seedings
            for (int i = 1; i < includedTeams.Count; i++)
            {
                List<Tuple<int, int>> round = new List<Tuple<int, int>>();
                round.Add(new Tuple<int, int>(0, i));

                if (allRounds.Count == 0)
                {
                    allRounds.Add(round);
                }
                else
                {
                    if (RNG.CoinFlip())
                    {
                        allRounds.Add(round);
                    }
                    else
                    {
                        allRounds.Insert(0, round);
                    }
                }
            }

            //Go through all remaining pairings and place them into rounds in which their numbers are not in there yet.
            for (int i = 1; i < includedTeams.Count; i++)
            {
                for (int j = i + 1; j < includedTeams.Count; j++)
                {
                    List<int> possibleIndices = new List<int>();

                    for (int k = 0; k < allRounds.Count; k++)
                    {
                        if (allRounds[k].Where(t => t.Item1 == i || t.Item2 == i || t.Item1 == j || t.Item2 == j).Count() == 0)
                        {
                            possibleIndices.Add(k);
                        }
                    }

                    if (possibleIndices.Count == 0)
                    {
                        #region We need to panic.
                        //Panic
                        Tuple<int, int> currentOrphan = new Tuple<int, int>(i, j);
                        List<int> repeatPreventer = new List<int>();
                        
                        while (currentOrphan != null)
                        {   
                            //Fill up the repeatPreventer just in case.
                            if (repeatPreventer.Count == 0)
                            {
                                for (int m = 0; m < allRounds.Count; m++)
                                {
                                    repeatPreventer.Add(m);
                                }
                            }

                            //Try to place the pairing as normal.
                            List<int> possibleOrphanIndices = new List<int>();

                            for (int k = 0; k < allRounds.Count; k++)
                            {
                                if (allRounds[k].Where(t => t.Item1 == currentOrphan.Item1 || t.Item2 == currentOrphan.Item1 || t.Item1 == currentOrphan.Item2 || t.Item2 == currentOrphan.Item2).Count() == 0)
                                {
                                    possibleOrphanIndices.Add(k);
                                }
                            }

                            if (possibleOrphanIndices.Count == 1)
                            {
                                allRounds[possibleOrphanIndices[0]].Add(currentOrphan);
                                currentOrphan = null;
                            }
                            else if (possibleOrphanIndices.Count > 1) 
                            {
                                allRounds[possibleOrphanIndices[RNG.Roll(possibleOrphanIndices.Count)]].Add(currentOrphan);
                                currentOrphan = null;
                            }
                            else
                            {
                                //Go through each round in repeatPreventer for any that have Item1 of currentOrphan but NOT Item2.
                                List<int> item1indices = new List<int>();
                                foreach (int index in repeatPreventer)
                                {
                                    if (allRounds[index].Where(t => t.Item1 == currentOrphan.Item1 || t.Item2 == currentOrphan.Item1).Count() > 0 &&
                                        allRounds[index].Where(t => t.Item1 == currentOrphan.Item2 || t.Item2 == currentOrphan.Item2).Count() == 0)
                                    {
                                        item1indices.Add(index);
                                    }
                                }
                                                              
                                //If any are found
                                if (item1indices.Count > 0)
                                {
                                    //Choose one
                                    int randomRoundIndex = item1indices[RNG.Roll(item1indices.Count)];
                                    List<Tuple<int, int>> chosenRound = allRounds[randomRoundIndex];

                                    //Remove the found pair
                                    //We know this item exists, so we just take the first occurence.
                                    Tuple<int, int> removal = chosenRound.Where(t => t.Item1 == currentOrphan.Item1 || t.Item2 == currentOrphan.Item1).ToArray()[0];
                                    chosenRound.Remove(removal);

                                    //Place orphan into the round
                                    chosenRound.Add(currentOrphan);

                                    //Found pair becomes the orphan
                                    currentOrphan = removal;

                                    //Remove chosen index from repeatPreventer
                                    repeatPreventer.Remove(randomRoundIndex);
                                }
                                //Else
                                else
                                {
                                    //Go through each round in repeatPreventer for any that have Item2 of currentOrphan but NOT Item1.
                                    List<int> item2indices = new List<int>();
                                    foreach (int index in repeatPreventer)
                                    {
                                        if (allRounds[index].Where(t => t.Item1 == currentOrphan.Item2 || t.Item2 == currentOrphan.Item2).Count() > 0 &&
                                            allRounds[index].Where(t => t.Item1 == currentOrphan.Item1 || t.Item2 == currentOrphan.Item1).Count() == 0)
                                        {
                                            item2indices.Add(index);
                                        }
                                    }

                                    //If any are found
                                    if (item2indices.Count > 0)
                                    {
                                        //Choose one
                                        int randomRoundIndex = item2indices[RNG.Roll(item2indices.Count)];
                                        List<Tuple<int, int>> chosenRound = allRounds[randomRoundIndex];

                                        //Remove the found pair
                                        //We know this item exists, so we just take the first occurence.
                                        Tuple<int, int> removal = chosenRound.Where(t => t.Item1 == currentOrphan.Item2 || t.Item2 == currentOrphan.Item2).ToArray()[0];
                                        chosenRound.Remove(removal);

                                        //Place orphan into the round
                                        chosenRound.Add(currentOrphan);

                                        //Found pair becomes the orphan
                                        currentOrphan = removal;

                                        //Remove chosen index from repeatPreventer
                                        repeatPreventer.Remove(randomRoundIndex);
                                    }
                                    //Else
                                    else
                                    {
                                        //If repeatPreventer not at full length, we will let it refill.
                                        //Otherwise, ¯\_(ツ)_/¯
                                        if (repeatPreventer.Count == allRounds.Count)
                                        {
                                            throw new Exception("wow");
                                        }
                                        else
                                        {
                                            repeatPreventer = new List<int>();
                                        }
                                    }
                                }
                            }
                        }
                        //Done panicking
                        #endregion
                    }
                    else if (possibleIndices.Count == 1)
                    {
                        allRounds[possibleIndices[0]].Add(new Tuple<int, int>(i, j));
                    }
                    else
                    {
                        allRounds[possibleIndices[RNG.Roll(possibleIndices.Count)]].Add(new Tuple<int, int>(i, j));
                    }
                }
            }

            //Assume all is well, despite our worst fears
            bool swap = false;
            foreach (List<Tuple<int, int>> round in allRounds)
            {
                List<TourneyMatchup> tempHolder = new List<TourneyMatchup>();

                foreach (Tuple<int, int> pairing in round)
                {
                    if (swap)
                    {
                        tempHolder.Add(new TourneyMatchup(3, GetTeamInSlot, GetTeamInSlot, new int[] { allRounds.IndexOf(round), round.IndexOf(pairing) }, pairing.Item2, pairing.Item1));
                    }
                    else
                    {
                        tempHolder.Add(new TourneyMatchup(3, GetTeamInSlot, GetTeamInSlot, new int[] { allRounds.IndexOf(round), round.IndexOf(pairing) }, pairing.Item1, pairing.Item2));
                    }
                    swap = !swap;
                }

                int newPosY = 0;
                while (tempHolder.Count > 0)
                {
                    int randomIndex = RNG.Roll(tempHolder.Count);
                    tempHolder[randomIndex].cellPosition = new int[] { allRounds.IndexOf(round), newPosY };
                    newPosY++;
                    upcomingMatchups.Add(tempHolder[randomIndex]);
                    tempHolder.RemoveAt(randomIndex);
                }

                if (RNG.CoinFlip())
                {
                    swap = !swap;
                }
            }
        }

        /// <summary>
        /// Creates matches by attempting to create distinct rounds consisting of exclusive pairings over multiple days.
        /// </summary>
        /// <param name="overDays">The number of days this tournament takes place over.</param>
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
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Round Robin tournament.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="id">The ID of the tournament</param>
        /// <param name="teams">All of the teams participating in the tournament</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        public RoundRobinTournament(string name, int id, List<Team> teams, Dictionary<int, Hero> heroes)
            : base(name, id, teams, heroes)
        {
            //blank because why not
        }

        /// <summary>
        /// Creates a new Round Robin tournament that spans multiple days.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="id">The ID of the tournament</param>
        /// <param name="teams">All of the teams participating in the tournament</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        /// <param name="numberOfDays">The number of days this tournament goes over.</param>
        public RoundRobinTournament(string name, int id, List<Team> teams, Dictionary<int, Hero> heroes, int numberOfDays)
            : base(name, id, teams, heroes, numberOfDays)
        {
            //blank because why not
        }

        /// <summary>
        /// Creates a new Round Robin tournament that uses invites instead of fixed teams.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="ID">The ID of the tournament</param>
        /// <param name="numberOfTeams">The minimum number of teams participating in the tournament.</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        public RoundRobinTournament(string name, int ID, int numberOfTeams, Dictionary<int, Hero> heroes)
            : base(name, ID, numberOfTeams, heroes)
        {
            //blank
        }

        /// <summary>
        /// Creates a new Round Robin tournament that uses invites instead of fixed teams and spans multiple days.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="ID">The ID of the tournament</param>
        /// <param name="numberOfTeams">The minimum number of teams participating in the tournament.</param>
        /// <param name="heroes">All of the heroes allowed in the tournament</param>
        /// <param name="numberOfDays">The number of days this tournament spans</param>
        public RoundRobinTournament(string name, int ID, int numberOfTeams, Dictionary<int, Hero> heroes, int numberOfDays)
            : base(name, ID, numberOfTeams, heroes, numberOfDays)
        {
            //blank
        }
        #endregion
    }
}
