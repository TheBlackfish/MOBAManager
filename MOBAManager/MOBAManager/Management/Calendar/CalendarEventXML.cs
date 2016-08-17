using System;
using System.Xml.Linq;

namespace MOBAManager.Management.Calendar
{
    sealed public partial class CalendarEvent
    {
        public XElement ToXML()
        {
            XElement root = new XElement("ce");

            root.SetAttributeValue("type", type);
            root.Add(new XElement("daysToResolution", daysToResolution));

            if (team1ID != -1)
            {
                root.Add(new XElement("team1ID", team1ID));
            }

            if (team2ID != -1)
            {
                root.Add(new XElement("team2ID", team2ID));
            }

            if (tournamentID != -1)
            {
                root.Add(new XElement("tournamentID", tournamentID));
            }

            return root;
        }

        public CalendarEvent(XElement src)
        {
            type = (EventType) Enum.Parse(typeof(EventType), src.Attribute("type").Value);
            daysToResolution = int.Parse(src.Element("daysToResolution").Value);

            if (src.Element("team1ID") != null)
            {
                team1ID = int.Parse(src.Element("team1ID").Value);
            }
            else
            {
                team1ID = -1;
            }
            
            if (src.Element("team2ID") != null)
            {
                team2ID = int.Parse(src.Element("team2ID").Value);
            }
            else
            {
                team2ID = -1;
            }
            
            if (src.Element("tournamentID") != null)
            {
                tournamentID = int.Parse(src.Element("tournamentID").Value);
            }
            else
            {
                tournamentID = -1;
            }
        }
    }
}