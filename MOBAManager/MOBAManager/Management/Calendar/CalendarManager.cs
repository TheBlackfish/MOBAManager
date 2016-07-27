using MOBAManager.Management.Teams;
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
        private TeamManager tm;
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
            allEvents.Add(ce);
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
            return AddPickupGame(team1ID, team2ID, currentDate);
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
            allEvents.Add(new CalendarEvent(EventType.PUG, (int)Math.Round(timeRemaining.TotalDays), team1ID, team2ID));
            return true;
        }

        public bool AddPickupGame(int team1ID, int team2ID, int offset)
        {
            allEvents.Add(new CalendarEvent(EventType.PUG, offset, team1ID, team2ID));
            return true;
        }

        /// <summary>
        /// Returns a list containing all events that should happen on the current in-game date.
        /// </summary>
        /// <returns></returns>
        public List<CalendarEvent> GetTodaysEvents()
        {
            return allEvents.Where(ce => ce.daysToResolution == 0).ToList();
        }

        /// <summary>
        /// Advances the calender and removes all events that have already occurred.
        /// </summary>
        public void IncrementCalender()
        {
            foreach (CalendarEvent ce in allEvents)
            {
                ce.decrementDaysLeft();
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
                Tuple<int, string> val = new Tuple<int, string>(0, "---");
                if (i < 0)
                {
                    val = new Tuple<int, string>(-1, "---");
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
                                    evtdesc += "Pick-up game against " + tm.GetTeamName(ce.team2ID);
                                }
                                else
                                {
                                    evtdesc += "Pick-up game against " + tm.GetTeamName(ce.team1ID);
                                }
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
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new calendar manager.
        /// </summary>
        public CalendarManager(TeamManager tm)
        {
            this.tm = tm;
            currentDate = DateTime.Now;
            allEvents = new List<CalendarEvent>();
            ScheduleRandomEventsForEachAITeam(4);
        }
        #endregion
    }
}