using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.MatchResolution
{
    class Match
    {
        /// <summary>
        /// The match selection AI for this match.
        /// </summary>
        private MatchSelector ms;

        /// <summary>
        /// The first team in this match.
        /// </summary>
        private Team team1;

        /// <summary>
        /// The second team in this match.
        /// </summary>
        private Team team2;

        /// <summary>
        /// The control variable for who the winner is.
        /// </summary>
        private int winner = -1;

        /// <summary>
        /// Returns the ID of the winning team, or -1 if none has been decided yet.
        /// </summary>
        public int winnerID
        {
            get
            {
                if (winner == -1)
                {
                    return -1;
                }
                else if (winner == 1)
                {
                    return team1.ID;
                }
                else if (winner == 2)
                {
                    return team2.ID;
                }
                return -1;
            }
        }

        /// <summary>
        /// Outputs the match results to the console.
        /// </summary>
        private void reportMatchResults()
        {
            Console.WriteLine("Match Setup");
            Console.WriteLine("----------");
            Console.WriteLine(team1.teamName);
            foreach (Player p in team1.getTeammates())
            {
                Console.WriteLine("-" + p.playerName);
            }
            Console.WriteLine("----------");
            Console.WriteLine(team2.teamName);
            foreach (Player p in team2.getTeammates())
            {
                Console.WriteLine("-" + p.playerName);
            }
            Console.WriteLine("----------");
            Console.WriteLine(team1.teamName + " Bans");
            foreach (Hero h in ms.getTeamBans(1))
            {
                Console.WriteLine("-" + h.HeroName);
            }
            Console.WriteLine("----------");
            Console.WriteLine(team1.teamName + " Picks");
            foreach (Hero h in ms.getTeamSelections(1))
            {
                Console.WriteLine("-" + h.HeroName);
            }
            Console.WriteLine("----------");
            Console.WriteLine(team2.teamName + " Bans");
            foreach (Hero h in ms.getTeamBans(2))
            {
                Console.WriteLine("-" + h.HeroName);
            }
            Console.WriteLine("----------");
            Console.WriteLine(team2.teamName + " Picks");
            foreach (Hero h in ms.getTeamSelections(2))
            {
                Console.WriteLine("-" + h.HeroName);
            }
        }

        /// <summary>
        /// Has both teams go through the ban/pick phase.
        /// </summary>
        public void banPickPhase()
        {
            //First Ban Phase
            ms.setTeamSelection(1, ms.team2Pick(), false);
            ms.setTeamSelection(2, ms.team1Pick(), false);
            ms.setTeamSelection(1, ms.team2Pick(), false);
            ms.setTeamSelection(2, ms.team1Pick(), false);

            //First Pick Phase
            ms.setTeamSelection(1, ms.team1Pick());
            ms.setTeamSelection(2, ms.team2Pick());
            ms.setTeamSelection(2, ms.team2Pick());
            ms.setTeamSelection(1, ms.team1Pick());

            //Second Ban Phase
            ms.setTeamSelection(2, ms.team1Pick(), false);
            ms.setTeamSelection(1, ms.team2Pick(), false);
            ms.setTeamSelection(2, ms.team1Pick(), false);
            ms.setTeamSelection(1, ms.team2Pick(), false);

            //Second Pick Phase
            ms.setTeamSelection(2, ms.team2Pick());
            ms.setTeamSelection(1, ms.team1Pick());
            ms.setTeamSelection(2, ms.team2Pick());
            ms.setTeamSelection(1, ms.team1Pick());

            //Third Ban Phase
            ms.setTeamSelection(2, ms.team1Pick(), false);
            ms.setTeamSelection(1, ms.team2Pick(), false);

            //Third Pick Phase
            ms.setTeamSelection(1, ms.team1Pick());
            ms.setTeamSelection(2, ms.team2Pick());

            reportMatchResults();
        }

        /// <summary>
        /// Creates a new match using the teams provided
        /// </summary>
        /// <param name="one">The team with the first pick</param>
        /// <param name="two">The other team</param>
        /// <param name="allHeroes">The dictionary containing all heroes in the game.</param>
        public Match(Team one, Team two, Dictionary<int, Hero> allHeroes)
        {
            team1 = one;
            team2 = two;
            ms = new MatchSelector(allHeroes, one.getTeammates(), two.getTeammates());
        }
    }
}
