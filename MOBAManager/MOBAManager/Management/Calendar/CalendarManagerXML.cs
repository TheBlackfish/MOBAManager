using MOBAManager.Management.Teams;
using MOBAManager.Management.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Calendar
{
    sealed public partial class CalendarManager
    {
        /// <summary>
        /// <para>Turns the CalenderManager into an XElement with the type 'calendar'.</para>
        /// <para>The XElement has no attributes.</para>
        /// <para>The XElement has 2+ nested elements.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>timestamp - The current date of the calendar manager in string form.</description>
        ///     </item>
        ///     <item>
        ///         <description>ce (Numerous) - The XElement representing a calendar event.</description>
        ///     </item>
        /// </list>
        /// </summary>
        public XElement ToXML()
        {
            XElement root = new XElement("calendar");

            root.Add(new XElement("timestamp", currentDate.ToString("MM-dd-yyyy")));

            foreach (CalendarEvent ce in allEvents)
            {
                root.Add(ce.ToXML());
            }

            return root;
        }

        /// <summary>
        /// Creates a new CalendarManager using the XElement provided.
        /// </summary>
        /// <param name="tm">The TeamManager related to this calendar manager.</param>
        /// <param name="tym">The TournamentManager related to this calendar manager.</param>
        /// <param name="src">The XElement to build this CalendarManager from.</param>
        public CalendarManager(TeamManager tm, TournamentManager tym, XElement src)
        {
            allEvents = new List<CalendarEvent>();
            currentDate = DateTime.Parse(src.Element("timestamp").Value).AddMinutes(1);
             
            teamManager = tm;
            tournamentManager = tym;
            
            foreach (XElement elem in src.Elements("ce"))
            {
                allEvents.Add(new CalendarEvent(elem));
            }
        }
    }
}