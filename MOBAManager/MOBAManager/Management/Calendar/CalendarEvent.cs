using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Calendar
{
    public class CalendarEvent
    {
        #region Enums
        /// <summary>
        /// The enum for controlling what type of CalendarEvent this is.
        /// </summary>
        public enum EventType { PUG };
        #endregion

        #region Public variables
        /// <summary>
        /// The type of event this CalendarEvent is.
        /// </summary>
        public EventType type;

        /// <summary>
        /// How many days this event has until it resolves.
        /// </summary>
        public int daysToResolution = 0;

        /// <summary>
        /// The first team ID to affect, if any.
        /// </summary>
        public int team1ID = -1;

        /// <summary>
        /// The second team ID to affect, if any.
        /// </summary>
        public int team2ID = -1;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new calendar event.
        /// </summary>
        /// <param name="eventType">The enum that determines which type of event this is.</param>
        /// <param name="daysLeft">How many days until this event resolves. (0 is the same day)</param>
        /// <param name="id1">The first ID to supplement. What this pertains to depends on the event type.</param>
        /// <param name="id2">The second ID to supplement. What this pertains to depends on the event type.</param>
        public CalendarEvent(EventType eventType, int daysLeft, int id1, int id2)
        {
            type = eventType;
            daysToResolution = daysLeft;

            switch (type)
            {
                case EventType.PUG:
                    team1ID = id1;
                    team2ID = id2;
                    break;
            }
        }
        #endregion
    }
}
