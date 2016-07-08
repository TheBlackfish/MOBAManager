using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.MatchResolution
{
    partial class MatchAI
    {
        #region Public methods
        /// <summary>
        /// Decides the winner of the match and returns either 1 or 2 depending on if it was Team 1 or Team 2.
        /// </summary>
        /// <returns></returns>
        public int decideWinner()
        {
            //Get Team 1's optimal skill
            int team1 = totalTeamSkill(1);

            //Get Team 2's optimal skill
            int team2 = totalTeamSkill(2);

            int dieRoll = rnd.Next(team1 + team2);

            Console.WriteLine("T1:" + team1 + " vs T2:" + team2 + " -- Die roll:" + dieRoll);

            if (dieRoll < team1)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// Returns the skill level of the team selected.
        /// </summary>
        /// <param name="team">The slot of the team to get the total skill for. 1 or 2.</param>
        /// <returns></returns>
        public int getTeamLineupSkill(int team)
        {
            if (team == 1)
            {
                return calculateLineupSkill(1, team1Lineup);
            }
            else if (team == 2)
            {
                return calculateLineupSkill(2, team2Lineup);
            }
            else
            {
                return int.MinValue;
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Calculates the best lineup for a team and saves it to the appropriate variable.
        /// </summary>
        /// <param name="team">The team to calculate this before.</param>
        /// <returns></returns>
        private List<Tuple<Player, Hero>> finalizeLineup(int team)
        {
            List<Tuple<Player, Hero>> lineup = new List<Tuple<Player, Hero>>();
            List<Player> curTeam = null;
            List<int> curPicks = null;
            List<Tuple<int, int, int>> phVals = new List<Tuple<int, int, int>>();

            if (team == 1)
            {
                curTeam = team1Players;
                curPicks = team1Picks;
            }
            else if (team == 2)
            {
                curTeam = team2Players;
                curPicks = team2Picks;
            }
            else
            {
                throw new Exception("Team number is not valid!");
            }

            for (int i = 0; i < curTeam.Count; i++)
            {
                foreach (int heroID in curPicks)
                {
                    phVals.Add(Tuple.Create(i, heroID, curTeam[i].getHeroSkill(heroID)));
                }
            }

            while (phVals.Count > 0 && lineup.Count < 5)
            {
                Tuple<int, int, int> topSelection = phVals.OrderByDescending(x => x.Item3).First();
                lineup.Add(Tuple.Create(curTeam[topSelection.Item1], allHeroes[topSelection.Item2]));
                phVals = phVals.Where(x => x.Item1 != topSelection.Item1).Where(x => x.Item2 != topSelection.Item2).ToList();
            }

            return lineup;
        }

        /// <summary>
        /// Calculates the skill level of a team's lineup.
        /// </summary>
        /// <param name="team">The team to calculate the skill level for.</param>
        /// <param name="lineup">That team's lineup.</param>
        /// <returns></returns>
        private int calculateLineupSkill(int team, List<Tuple<Player, Hero>> lineup)
        {
            int finalValue = 0;
            List<int> oppPicks = team2Picks;
            if (team == 2)
            {
                oppPicks = team1Picks;
            }

            for (int i = 0; i < lineup.Count; i++)
            {
                Tuple<Player, Hero> tp = lineup[i];
                int curVal = tp.Item1.getHeroSkill(tp.Item2.ID);
                curVal += tp.Item2.calculatePerformance(
                    lineup.Skip(i + 1).Select(x => x.Item2.ID).ToList(),
                    oppPicks);
                finalValue += curVal;
            }
            return finalValue * finalValue;
        }


        /// <summary>
        /// Totals up a team's total skill for deciding the winner of a match.
        /// </summary>
        /// <param name="team">The team to calculate skill for.</param>
        /// <returns></returns>
        private int totalTeamSkill(int team)
        {
            if ((team1Bans.Count == 5) && (team1Picks.Count == 5) && (team2Bans.Count == 5) && (team2Picks.Count == 5))
            {
                List<Tuple<Player, Hero>> lineup = team1Lineup;
                if (team == 2)
                {
                    lineup = team2Lineup;
                }
                if (lineup.Count < 5)
                {
                    lineup = finalizeLineup(team);
                    if (team == 1)
                    {
                        team1Lineup = lineup;
                    }
                    else
                    {
                        team2Lineup = lineup;
                    }
                }

                return calculateLineupSkill(team, lineup);
            }
            return int.MinValue;
        }
        #endregion
    }
}