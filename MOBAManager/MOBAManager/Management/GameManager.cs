using MOBAManager.Management.Calendar;
using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Statistics;
using MOBAManager.Management.Teams;
using MOBAManager.MatchResolution;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management
{
    public class GameManager
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
        /// Turns a calendar event into a match.
        /// </summary>
        /// <param name="ce">The CalendarEvent to translate.</param>
        /// <returns></returns>
        public Match translateEventToMatch(CalendarEvent ce)
        {
            if (ce.team1ID == 0)
            {
                return new Match(true, teamManager.getTeamByID(ce.team1ID), teamManager.getTeamByID(ce.team2ID), 1, heroManager.getHeroDictionary());
            }
            else if (ce.team2ID == 0)
            {
                return new Match(true, teamManager.getTeamByID(ce.team1ID), teamManager.getTeamByID(ce.team2ID), 2, heroManager.getHeroDictionary());
            }
            else
            {
                return new Match(teamManager.getTeamByID(ce.team1ID), teamManager.getTeamByID(ce.team2ID), heroManager.getHeroDictionary());
            }
        }

        /// <summary>
        /// Adds a bunch of temporary matches to the calendar.
        /// </summary>
        public void fillCalendar()
        {
            for (int i = 1; i < teamManager.getAllTeams().Count; i++)
            {
                for (int j = i + 1; j < teamManager.getAllTeams().Count; j++)
                {
                    calendarManager.addPickupGame(i, j);
                }
            }

            for (int i = 3; i < teamManager.getAllTeams().Count; i++)
            {
                for (int j = i + 1; j < teamManager.getAllTeams().Count; j++)
                {
                    calendarManager.addPickupGame(i, j, DateTime.Now.AddDays(1));
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the game manager and all of its subordinate managers.
        /// </summary>
        public GameManager()
        {
            calendarManager = new CalendarManager();

            heroManager = new HeroManager();

            playerManager = new PlayerManager();

            teamManager = new TeamManager();

            statsManager = new StatisticsManager(heroManager.getHeroDictionary(), playerManager.getPlayerDictionary(), teamManager.getTeamDictionary());

            teamManager.populateTeams(playerManager.getAllPlayers());
        }
        #endregion
    }
}
