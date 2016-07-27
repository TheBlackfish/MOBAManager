using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Utility;
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
        public int DecideWinner()
        {
            //Get Team 1's optimal skill
            double team1 = TotalTeamSkill(1);

            //Get Team 2's optimal skill
            double team2 = TotalTeamSkill(2);

            if (team1 < 0 && team2 < 0)
            {
                while (team1 < 100 && team2 < 100)
                {
                    team1 += 100;
                    team2 += 100;
                }
            }
            else if (team1 < 0 && team2 > 0)
            {
                team2 -= team1 - 100;
                team1 = 100;
            }
            else if (team2 < 0 && team1 > 0)
            {
                team1 -= team2 - 100;
                team2 = 100;
            }

            double dieRoll = RNG.RollDouble(team1 + team2);

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
        public double GetTeamLineupSkill(int team)
        {
            if (team == 1)
            {
                return CalculateLineupSkill(1, team1Lineup);
            }
            else if (team == 2)
            {
                return CalculateLineupSkill(2, team2Lineup);
            }
            else
            {
                return double.MinValue;
            }
        }

        public void ResolveMatchEffects()
        {
            foreach (Tuple<Player, Hero> tph in team1Lineup)
            {
                tph.Item1.GainExperience(tph.Item2.ID);
            }

            foreach (Tuple<Player, Hero> tph in team2Lineup)
            {
                tph.Item1.GainExperience(tph.Item2.ID);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Calculates the best lineup for a team and saves it to the appropriate variable.
        /// </summary>
        /// <param name="team">The team to calculate this before.</param>
        /// <returns></returns>
        private List<Tuple<Player, Hero>> FinalizeLineup(int team)
        {
            List<Tuple<Player, Hero>> lineup = new List<Tuple<Player, Hero>>();
            List<Player> curTeam = null;
            List<int> curPicks = null;
            List<Tuple<int, int, double>> phVals = new List<Tuple<int, int, double>>();

            if (team == 1)
            {
                curTeam = team1Players;
                curPicks = team1Picks;
            }
            else
            {
                curTeam = team2Players;
                curPicks = team2Picks;
            }

            for (int i = 0; i < curTeam.Count; i++)
            {
                foreach (int heroID in curPicks)
                {
                    phVals.Add(Tuple.Create(i, heroID, curTeam[i].GetHeroSkill(heroID)));
                }
            }

            while (phVals.Count > 0 && lineup.Count < 5)
            {
                Tuple<int, int, double> topSelection = phVals.OrderByDescending(x => x.Item3).First();
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
        private double CalculateLineupSkill(int team, List<Tuple<Player, Hero>> lineup)
        {
            double finalValue = 0.0;
            List<int> oppPicks = team2Picks;
            if (team == 2)
            {
                oppPicks = team1Picks;
            }

            for (int i = 0; i < lineup.Count; i++)
            {
                Tuple<Player, Hero> tp = lineup[i];
                double curVal = tp.Item1.GetHeroSkill(tp.Item2.ID);
                curVal += tp.Item2.CalculatePerformance(
                    lineup.Skip(i + 1).Select(x => x.Item2.ID).ToList(),
                    oppPicks);
                finalValue += curVal;
            }
            if (finalValue >= 0)
            {
                return finalValue * finalValue;
            }
            else
            {
                return -(finalValue * finalValue);
            }
        }


        /// <summary>
        /// Totals up a team's total skill for deciding the winner of a match.
        /// </summary>
        /// <param name="team">The team to calculate skill for.</param>
        /// <returns></returns>
        private double TotalTeamSkill(int team)
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
                    lineup = FinalizeLineup(team);
                    if (team == 1)
                    {
                        team1Lineup = lineup;
                    }
                    else
                    {
                        team2Lineup = lineup;
                    }
                }

                return CalculateLineupSkill(team, lineup);
            }
            return int.MinValue;
        }
        #endregion
    }
}