using MOBAManager.Management.Teams;
using MOBAManager.Management.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Calendar
{
    sealed public partial class CalendarManager
    {
        #region Private variables
        /// <summary>
        /// The DateTime pertaining to the current in-game date.
        /// </summary>
        private DateTime currentDate;

        /// <summary>
        /// The list of all calendar events.
        /// </summary>
        private List<CalendarEvent> allEvents;

        /// <summary>
        /// The team manager that corresponds to the same game as this calendar manager.
        /// </summary>
        private TeamManager teamManager;

        /// <summary>
        /// The tournament manager that corresponds to the same game as this calendar manager.
        /// </summary>
        private TournamentManager tournamentManager;
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the current in-game date formatted to text.
        /// </summary>
        /// <returns></returns>
        public string GetFormattedDate()
        {
            return currentDate.ToString("MMMM d, yyyy");
        }

        /// <summary>
        /// Returns the current month and year as a DateTime on the first of the month.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFormattedDateTime()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        /// <summary>
        /// Returns how many days are between the current and the future date.
        /// </summary>
        /// <param name="futureTime"></param>
        /// <returns></returns>
        public int GetDaysToDate(DateTime futureTime)
        {
            return (int)(futureTime - currentDate).Days;
        }

        /// <summary>
        /// Directly adds a calendar event to the list of events.
        /// </summary>
        /// <param name="ce"></param>
        /// <returns></returns>
        public bool AddCalendarEvent(CalendarEvent ce)
        {
            if (ce.team1ID != -1)
            {
                RemoveAllEventsForTeamOnOffset(ce.team1ID, ce.daysToResolution);
            }
            if (ce.team2ID != -1)
            {
                RemoveAllEventsForTeamOnOffset(ce.team2ID, ce.daysToResolution);
            }
            if (ce.tournamentID != -1)
            {
                if (tournamentManager.GetTournamentByID(ce.tournamentID).GetAllTeams().Count > 0)
                {
                    List<int> ids = tournamentManager.GetTournamentByID(ce.tournamentID).GetAllTeams().Select(t => t.ID).ToList();
                    foreach (int i in ids)
                    {
                        RemoveAllEventsForTeamOnOffset(i, ce.daysToResolution);
                    }
                }
            }
            allEvents.Add(ce);
            return true;
        }

        /// <summary>
        /// Removes all events for the specified team on the specified offset.
        /// </summary>
        /// <param name="teamID">The team to remove events for.</param>
        /// <param name="offset">The offset for which events will be removed.</param>
        /// <returns></returns>
        public bool RemoveAllEventsForTeamOnOffset(int teamID, int offset)
        {
            allEvents = allEvents.Where(ev => !((ev.team1ID == teamID || ev.team2ID == teamID) && ev.daysToResolution == offset)).ToList();
            return true;
        }

        /// <summary>
        /// Removes all events for the specified teams on the specified offset.
        /// </summary>
        /// <param name="team1ID">The first team to remove events for.</param>
        /// <param name="team2ID">The second team to remove events for.</param>
        /// <param name="offset">The offset for which events will be removed.</param>
        /// <returns></returns>
        public bool RemoveAllEventsForTeamsOnOffset(int team1ID, int team2ID, int offset)
        {
            RemoveAllEventsForTeamOnOffset(team1ID, offset);
            RemoveAllEventsForTeamOnOffset(team2ID, offset);
            return true;
        }

        /// <summary>
        /// Creates a new bootcamp event on the current date for the specified team.
        /// </summary>
        /// <param name="teamID">The team to schedule a bootcamp for.</param>
        /// <returns></returns>
        public bool AddBootcamp(int teamID)
        {
            return AddBootcamp(teamID, 0);
        }

        /// <summary>
        /// Creates a new bootcamp event on the current date for the specified team on the specified date time.
        /// </summary>
        /// <param name="teamID">The team to schedule a bootcamp for.</param>
        /// <param name="date">The DateTime to schedule the event on.</param>
        /// <returns></returns>
        public bool AddBootcamp(int teamID, DateTime date)
        {
            TimeSpan timeRemaining = date - currentDate;
            AddCalendarEvent(new CalendarEvent(EventType.Bootcamp, (int)Math.Round(timeRemaining.TotalDays), teamID, -1));
            return true;
        }

        /// <summary>
        /// Creates a new bootcamp event on the current date for the specified team on the specified offset.
        /// </summary>
        /// <param name="teamID">The team to schedule a bootcamp for.</param>
        /// <param name="offset">The offset to schedule the event on.</param>
        /// <returns></returns>
        public bool AddBootcamp(int teamID, int offset)
        {
            AddCalendarEvent(new CalendarEvent(EventType.Bootcamp, offset, teamID, -1));
            return true;
        }

        /// <summary>
        /// Creates a new pickup game event for the same day.
        /// </summary>
        /// <param name="team1ID">The left team's ID.</param>
        /// <param name="team2ID">The right team's ID.</param>
        /// <returns></returns>
        public bool AddPickupGame(int team1ID, int team2ID)
        {
            return AddPickupGame(team1ID, team2ID, 0);
        }

        /// <summary>
        /// Creates a new pickup game event for a future date.
        /// </summary>
        /// <param name="team1ID">The left team's ID.</param>
        /// <param name="team2ID">The right team's ID</param>
        /// <param name="date">The date on which the game will occur.</param>
        /// <returns></returns>
        public bool AddPickupGame(int team1ID, int team2ID, DateTime date)
        {
            TimeSpan timeRemaining = date - currentDate;
            AddCalendarEvent(new CalendarEvent(EventType.PUG, (int)Math.Round(timeRemaining.TotalDays), team1ID, team2ID));
            return true;
        }

        /// <summary>
        /// Creates a new pickup game event on the specified offset..
        /// </summary>
        /// <param name="team1ID">The left team's ID.</param>
        /// <param name="team2ID">The right team's ID</param>
        /// <param name="offset">The offset to put the event at.</param>
        /// <returns></returns>
        public bool AddPickupGame(int team1ID, int team2ID, int offset)
        {
            AddCalendarEvent(new CalendarEvent(EventType.PUG, offset, team1ID, team2ID));
            return true;
        }

        /// <summary>
        /// Adds a tournament placeholder event to the calendar.
        /// </summary>
        /// <param name="tournamentID">ID of the tournament.</param>
        /// <param name="date">Date of the tournament</param>
        /// <returns></returns>
        public bool AddTournamentDate(int tournamentID, DateTime date)
        {
            TimeSpan timeRemaining = date - currentDate;
            AddCalendarEvent(new CalendarEvent(EventType.TournamentPlaceholder, (int)Math.Round(timeRemaining.TotalDays), tournamentID, -1));
            return true;
        }

        /// <summary>
        /// Adds a tournament placeholder event to the calendar.
        /// </summary>
        /// <param name="tournamentID">ID of the tournament.</param>
        /// <param name="offset">Offset when the offset would occur</param>
        /// <returns></returns>
        public bool AddTournamentDate(int tournamentID, int offset)
        {
            AddCalendarEvent(new CalendarEvent(EventType.TournamentPlaceholder, offset, tournamentID, -1));
            return true;
        }

        /// <summary>
        /// Adds multiple tournament placeholder events to the calender, starting with the starting date and continuing for the number of days specified.
        /// </summary>
        /// <param name="tournamentID">ID of the tournament.</param>
        /// <param name="date">Date of when the tournament starts</param>
        /// <param name="daysOfTournament">How long the tournament lasts for.</param>
        /// <returns></returns>
        public bool AddTournamentDates(int tournamentID, DateTime date, int daysOfTournament)
        {
            for (int i = 0; i < daysOfTournament; i++)
            {
                AddTournamentDate(tournamentID, date.AddDays(i));
            }
            return true;
        }

        /// <summary>
        /// Adds multiple tournament placeholder events to the calender, starting with the starting date and continuing for the number of days specified.
        /// </summary>
        /// <param name="tournamentID">ID of the tournament.</param>
        /// <param name="date">Offset for the first day of the tournament</param>
        /// <param name="daysOfTournament">How long the tournament lasts for.</param>
        /// <returns></returns>
        public bool AddTournamentDates(int tournamentID, int offset, int daysOfTournament)
        {
            for (int i = 0; i < daysOfTournament; i++)
            {
                AddTournamentDate(tournamentID, offset + i);
            }
            return true;
        }

        /// <summary>
        /// Returns a list containing all events that should happen on the current in-game date.
        /// <para>Additionally, each team that does not have an event for the day schedules a bootcamp to occupy their time.</para>
        /// </summary>
        /// <returns></returns>
        public List<CalendarEvent> GetTodaysEvents()
        {
            List<CalendarEvent> todaysEvents = allEvents.Where(ce => ce.daysToResolution == 0).ToList();

            foreach (Team t in teamManager.GetAllTeams())
            {
                if (!todaysEvents.Any(ce => ce.team1ID == t.ID || ce.team2ID == t.ID) && t.ID != 0)
                {
                    bool shouldAddBootcamp = true;
                    if (todaysEvents.Any(ce => ce.type == EventType.TournamentPlaceholder))
                    {
                        foreach(int tid in todaysEvents.Where(ce => ce.type == EventType.TournamentPlaceholder).Select(ce => ce.tournamentID).ToList())
                        {
                            if (tournamentManager.GetTournamentByID(tid).GetAllTeams().Contains(t)) {
                                shouldAddBootcamp = false;
                            }
                        }
                    }
                    if (shouldAddBootcamp)
                    {
                        CalendarEvent placeholder = new CalendarEvent(EventType.Bootcamp, 0, t.ID, -1);
                        todaysEvents.Add(placeholder);
                        allEvents.Add(placeholder);
                    }
                }
            }

            return todaysEvents;
        }

        /// <summary>
        /// Advances the calender and removes all events that have already occurred.
        /// </summary>
        public void IncrementCalender()
        {
            foreach (CalendarEvent ce in allEvents)
            {
                ce.DecrementDaysLeft();
            }
            allEvents = allEvents.Where(ce => ce.daysToResolution >= 0).ToList();
            currentDate = currentDate.AddDays(1);
        }

        /// <summary>
        /// Returns true if the team specified has any events on the day specified.
        /// </summary>
        /// <param name="teamID">The ID of the team</param>
        /// <param name="dayOffset">The day to search events for.</param>
        /// <returns></returns>
        public bool TeamHasEventsOnDate(int teamID, int dayOffset)
        {
            if (allEvents.Any(ce => ce.type == EventType.TournamentPlaceholder && ce.daysToResolution == dayOffset))
            {
                return true;
            }
            
            return (allEvents.Where(ce => ce.daysToResolution == dayOffset).Where(ce => ce.team1ID == teamID || ce.team2ID == teamID).Count() > 0);
        }

        /// <summary>
        /// Returns a list of all events involving a team on the specified date.
        /// </summary>
        /// <param name="teamID">The team to get all events on the date for.</param>
        /// <param name="dayOffset">The day to search events for.</param>
        /// <returns></returns>
        public List<CalendarEvent> GetEventsForTeamOnDate(int teamID, int dayOffset)
        {
            return allEvents.Where(ce => ce.daysToResolution == dayOffset).Where(ce => ce.team1ID == teamID || ce.team2ID == teamID).ToList();
        }

        /// <summary>
        /// Returns a list of all events on the specified date.
        /// </summary>
        /// <param name="dayOffset">The day to search events for.</param>
        /// <returns></returns>
        public List<CalendarEvent> GetEventsOnDate(int dayOffset)
        {
            return allEvents.Where(ce => ce.daysToResolution == dayOffset).ToList();
        }

        /// <summary>
        /// Returns a list of tuples that describe a team's events over the course of a month. The tuples are stored in the list in order of the days they occur.
        /// The statuses themselves consist of an integer and a string description. If the integer is -1, the day has already occurred and no events are possible on that day.
        /// If the integer is 0, the day has no events and a generic placeholder will be presented as the day's summary. If 1, the day has one or more events summarised in the
        /// event description string.
        /// </summary>
        /// <param name="teamID">The ID of the team to get event statuses for.</param>
        /// <param name="month">The month, 1-12</param>
        /// <param name="year">The year</param>
        /// <param name="tm">The TeamManager object as reference</param>
        /// <returns></returns>
        public List<Tuple<int, string>> GetEventStatusForTeamInMonth(int teamID, int month, int year)
        {
            DateTime targetDate = new DateTime(year, month, 1);
            int minOffset = (targetDate - currentDate).Days;
            int maxOffset = minOffset + DateTime.DaysInMonth(year, month);

            List<Tuple<int, string>> ret = new List<Tuple<int, string>>();

            for (int i = minOffset; i < maxOffset; i++)
            {
                Tuple<int, string> val = new Tuple<int, string>(0, "");
                if (i < 0)
                {
                    val = new Tuple<int, string>(-1, "");
                }
                else
                {
                    List<CalendarEvent> allEventsOnDate = GetEventsForTeamOnDate(teamID, i);
                    if (allEventsOnDate.Count > 0)
                    {
                        //Find out what type of events and concat descriptions of them.
                        string evtdesc = "";
                        foreach (CalendarEvent ce in allEventsOnDate)
                        {
                            if (evtdesc.Length > 0)
                            {
                                evtdesc += Environment.NewLine;
                            }

                            if (ce.type == EventType.PUG)
                            {
                                if (ce.team1ID == teamID)
                                {
                                    evtdesc += "Pick-up game against " + teamManager.GetTeamName(ce.team2ID);
                                }
                                else
                                {
                                    evtdesc += "Pick-up game against " + teamManager.GetTeamName(ce.team1ID);
                                }
                            }
                            else if (ce.type == EventType.Bootcamp)
                            {
                                evtdesc += "Bootcamp";
                            }
                        }
                        if (evtdesc.Length > 0)
                        {
                            val = new Tuple<int, string>(1, evtdesc);
                        }
                    }
                }
                ret.Add(val);
            }

            return ret;
        }

        /// <summary>
        /// Returns a list of tuples that describe all tournaments over the course of a month. The tuples are stored in the list in order of the days they occur.
        /// The statuses themselves consist of an integer and a string description. If the integer is -1, the day has a tournament occurring and will block calendar input.
        /// If the integer is 0, the day has no events and a generic placeholder will be presented as the day's summary. If 1, the day has one or more events summarised in the
        /// event description string.
        /// </summary>
        /// <param name="month">The month, 1-12</param>
        /// <param name="year">The year</param>
        /// <returns></returns>
        public List<Tuple<int, string>> GetTournamentsInMonth(int month, int year)
        {
            DateTime targetDate = new DateTime(year, month, 1);
            int minOffset = (targetDate - currentDate).Days;
            int maxOffset = minOffset + DateTime.DaysInMonth(year, month);

            List<Tuple<int, string>> ret = new List<Tuple<int, string>>();

            for (int i = minOffset; i < maxOffset; i++)
            {
                List<CalendarEvent> allEventsOnDate = GetEventsOnDate(i);
                if (allEventsOnDate.Count > 0)
                {
                    //Find out what tournament is running and add its name to the return value.
                    string evtdesc = "";
                    foreach (CalendarEvent ce in allEventsOnDate)
                    {
                        if (ce.type == EventType.TournamentPlaceholder)
                        {
                            if (evtdesc.Length > 0)
                            {
                                evtdesc += Environment.NewLine;
                            }
                            evtdesc += tournamentManager.GetTournamentByID(ce.tournamentID).name;
                        }
                    }
                    if (evtdesc.Length > 0)
                    {
                        ret.Add(new Tuple<int, string>(-1, evtdesc));
                    }
                    else
                    {
                        ret.Add(new Tuple<int, string>(0, ""));
                    }
                }
                else
                {
                    ret.Add(new Tuple<int, string>(0, ""));
                }
            }

            return ret;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new calendar manager.
        /// </summary>
        public CalendarManager(TeamManager teamManager, TournamentManager tournamentManager)
        {
            this.teamManager = teamManager;
            this.tournamentManager = tournamentManager;
            currentDate = DateTime.Now;
            allEvents = new List<CalendarEvent>();
            ScheduleRandomEventsForEachAITeam(4);
        }
        #endregion
    }
}