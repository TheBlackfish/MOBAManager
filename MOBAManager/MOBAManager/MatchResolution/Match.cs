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
    public partial class Match : IDisposable
    {
        #region Private variables
        /// <summary>
        /// The match selection AI for this match.
        /// </summary>
        private MatchAI ms;

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
        private int _winner = -1;
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the number of the winning team, or -1 if none has been decided yet.
        /// </summary>
        public int winner
        {
            get
            {
                if (_winner == -1)
                {
                    return -1;
                }
                else if (_winner == 1)
                {
                    return 1;
                }
                else if (_winner == 2)
                {
                    return 2;
                }
                return -1;
            }
        }

        /// <summary>
        /// Has both teams go through the ban/pick phase instantly and then decides a winner.
        /// </summary>
        public void instantlyResolve()
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

            _winner = ms.decideWinner();
        }
        #endregion

        #region Constructors
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
            ms = new MatchAI(this, allHeroes, one.getTeammates(), two.getTeammates());
        }
        #endregion

        #region Disposal methods
        /// <summary>
        /// Disposes the Match entirely.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deallocates resources depending on what type of disposing to perform.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ms.Dispose();
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~Match()
        {
            Dispose(false);
        }
        #endregion
    }
}
