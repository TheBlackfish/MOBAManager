using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.MatchResolution
{
    partial class MatchAI
    {
        private int totalTeamSkill(int team)
        {
            if ((team1Bans.Count == 5) && (team1Picks.Count == 5) && (team2Bans.Count == 5) && (team2Picks.Count == 5))
            {
                List<Player> curTeam = team1Players;
                if (team == 2)
                {
                    curTeam = team2Players;
                }

                List<int> curPicks = team1Picks;
                List<int> oppPicks = team2Picks;
                if (team == 2)
                {
                    curPicks = team2Picks;
                    oppPicks = team1Picks;
                }

                List<Tuple<Player, Hero>> finalPicks = new List<Tuple<Player, Hero>>();
                List<Tuple<int, int, int>> phVals = new List<Tuple<int, int, int>>();

                for (int i = 0; i < curTeam.Count; i++)
                {
                    foreach (int heroID in curPicks)
                    {
                        phVals.Add(Tuple.Create(i, heroID, curTeam[i].getHeroSkill(heroID)));
                    }
                }

                while (phVals.Count > 0 && finalPicks.Count < 5)
                {
                    Tuple<int, int, int> topSelection = phVals.OrderByDescending(x => x.Item3).First();
                    finalPicks.Add(Tuple.Create(curTeam[topSelection.Item1], allHeroes[topSelection.Item2]));
                    phVals = phVals.Where(x => x.Item1 != topSelection.Item1).Where(x => x.Item2 != topSelection.Item2).ToList();
                }

                if (finalPicks.Count == 5)
                {
                    int finalValue = 0;
                    for(int i = 0; i < finalPicks.Count; i++)
                    {
                        Tuple<Player, Hero> tp = finalPicks[i];
                        int curVal = tp.Item1.getHeroSkill(tp.Item2.ID);
                        curVal += tp.Item2.calculatePerformance(
                            finalPicks.Skip(i + 1).Select(x => x.Item2.ID).ToList(),
                            oppPicks);
                        finalValue += curVal;
                    }
                    return finalValue;
                }
                else
                {
                    throw new Exception("Something went wrong during score calculation!");
                }
            }
            return int.MinValue;
        }

        public int decideWinner()
        {
            //Get Team 1's optimal skill
            int team1 = totalTeamSkill(1);
            team1 *= team1;

            //Get Team 2's optimal skill
            int team2 = totalTeamSkill(2);
            team2 *= team2;

            int dieRoll = rnd.Next(team1 + team2);

            if (dieRoll < team1)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}