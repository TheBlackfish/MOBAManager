using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Calendar
{
    public partial class CalendarManager
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

        private TeamManager tm;
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the current in-game date formatted to text.
        /// </summary>
        /// <returns></returns>
        public string getFormattedDate()
        {
            return currentDate.ToString("MMMM d, yyyy");
        }

        /// <summary>
        /// Returns the current month and year as a DateTime on the first of the month.
        /// </summary>
        /// <returns></returns>
        public DateTime getFormattedDateTime()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        public int getDaysToDate(DateTime futureTime)
        {
            return (int)(futureTime - currentDate).Days;
        }

        public bool addCalendarEvent(CalendarEvent ce)
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
        public bool addPickupGame(int team1ID, int team2ID)
        {
            return addPickupGame(team1ID, team2ID, currentDate);
        }

        /// <summary>
        /// Creates a new pickup game event for a future date.
        /// </summary>
        /// <param name="team1ID">The left team's ID.</param>
        /// <param name="team2ID">The right team's ID</param>
        /// <param name="date">The date on which the game will occur.</param>
        /// <returns></returns>
        public bool addPickupGame(int team1ID, int team2ID, DateTime date)
        {
            TimeSpan timeRemaining = date - currentDate;
            allEvents.Add(new CalendarEvent(CalendarEvent.EventType.PUG, (int)Math.Round(timeRemaining.TotalDays), team1ID, team2ID));
            return true;
        }

        public bool addPickupGame(int team1ID, int team2ID, int offset)
        {
            allEvents.Add(new CalendarEvent(CalendarEvent.EventType.PUG, offset, team1ID, team2ID));
            return true;
        }

        /// <summary>
        /// Returns a list containing all events that should happen on the current in-game date.
        /// </summary>
        /// <returns></returns>
        public List<CalendarEvent> getTodaysEvents()
        {
            return allEvents.Where(ce => ce.daysToResolution == 0).ToList();
        }

        /// <summary>
        /// Advances the calender and removes all events that have already occurred.
        /// </summary>
        public void incrementCalender()
        {
            foreach (CalendarEvent ce in allEvents)
            {
                ce.daysToResolution--;
            }
            allEvents = allEvents.Where(ce => ce.daysToResolution >= 0).ToList();
            currentDate = currentDate.AddDays(1);
        }

        public bool teamHasEventsOnDate(int teamID, int dayOffset)
        {
            return (allEvents.Where(ce => ce.daysToResolution == dayOffset).Where(ce => ce.team1ID == teamID || ce.team2ID == teamID).Count() > 0);
        }

        public List<CalendarEvent> getEventsForTeamOnDate(int teamID, int dayOffset)
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
        public List<Tuple<int, string>> getEventStatusForTeamInMonth(int teamID, int month, int year)
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
                    List<CalendarEvent> allEventsOnDate = getEventsForTeamOnDate(teamID, i);
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

                            if (ce.type == CalendarEvent.EventType.PUG)
                            {
                                if (ce.team1ID == teamID)
                                {
                                    evtdesc += "Pick-up game against " + tm.getTeamName(ce.team2ID);
                                }
                                else
                                {
                                    evtdesc += "Pick-up game against " + tm.getTeamName(ce.team1ID);
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
            scheduleRandomEventsForEachAITeam(4);
        }
        #endregion
    }
}