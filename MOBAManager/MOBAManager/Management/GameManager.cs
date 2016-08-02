using MOBAManager.Management.Calendar;
using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Statistics;
using MOBAManager.Management.Teams;
using MOBAManager.MatchResolution;
using MOBAManager.Resolution.BootcampResolution;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management
{
    sealed public class GameManager
    {
        #region Temporarily Public Variables
        /// <summary>
        /// The calendar manager of the current game.
        /// </summary>
        public CalendarManager calendarManager;

        /// <summary>
        /// The hero manager of the current game.
        /// </summary>
        public HeroManager heroManager;

        /// <summary>
        /// The player manager of the current game.
        /// </summary>
        public PlayerManager playerManager;

        /// <summary>
        /// The statistics manager of the current game.
        /// </summary>
        public StatisticsManager statsManager;

        /// <summary>
        /// The team manager of the current game.
        /// </summary>
        public TeamManager teamManager;
        #endregion

        #region Public Methods
        /// <summary>
        /// Turns a calendar event into a bootcamp session.
        /// </summary>
        /// <param name="ce">The CalendarEvent to translate.</param>
        /// <returns></returns>
        public BootcampSession TranslateEventToBootcamp(CalendarEvent ce)
        {
            if (ce.team1ID == 0)
            {
                return new BootcampSession(teamManager.GetTeamByID(ce.team1ID), heroManager.GetIDSortedNames(), true);
            }
            return new BootcampSession(teamManager.GetTeamByID(ce.team1ID), heroManager.GetIDSortedNames());
        }

        /// <summary>
        /// Turns a calendar event into a match.
        /// </summary>
        /// <param name="ce">The CalendarEvent to translate.</param>
        /// <returns></returns>
        public Match TranslateEventToMatch(CalendarEvent ce)
        {
            if (ce.team1ID == 0)
            {
                return new Match(true, teamManager.GetTeamByID(ce.team1ID), teamManager.GetTeamByID(ce.team2ID), 1, heroManager.GetHeroDictionary());
            }
            else if (ce.team2ID == 0)
            {
                return new Match(true, teamManager.GetTeamByID(ce.team1ID), teamManager.GetTeamByID(ce.team2ID), 2, heroManager.GetHeroDictionary());
            }
            else
            {
                return new Match(teamManager.GetTeamByID(ce.team1ID), teamManager.GetTeamByID(ce.team2ID), heroManager.GetHeroDictionary());
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the game manager and all of its subordinate managers.
        /// </summary>
        public GameManager()
        {
            heroManager = new HeroManager();

            playerManager = new PlayerManager();

            teamManager = new TeamManager();

            calendarManager = new CalendarManager(teamManager);

            statsManager = new StatisticsManager(heroManager.GetHeroDictionary(), playerManager.GetPlayerDictionary(), teamManager.GetTeamDictionary());

            teamManager.PopulateTeams(playerManager.GetAllPlayers());
        }
        #endregion
    }
}
