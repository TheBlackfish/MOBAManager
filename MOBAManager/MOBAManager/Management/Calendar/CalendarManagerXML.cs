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