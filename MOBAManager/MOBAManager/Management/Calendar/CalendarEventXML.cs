using System;
using System.Xml.Linq;

namespace MOBAManager.Management.Calendar
{
    sealed public partial class CalendarEvent
    {
        /// <summary>
        /// <para>Turns the CalendarEvent into an XElement with the type 'ce'.</para>
        /// <para>The XElement has 1 attribute.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>type - The type of event that this CalendarEvent is.</description>
        ///     </item>
        /// </list>
        /// <para>The XElement has 1-4 nested elements.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>daysToResolution - The number of days until this CalendarEvent transpires.</description>
        ///     </item>
        ///     <item>
        ///         <description>team1ID (Optional) - The ID of the first team involved in this event.</description>
        ///     </item>
        ///     <item>
        ///         <description>team2ID (Optional) - The ID of the second team involved in this event.</description>
        ///     </item>
        ///     <item>
        ///         <description>tournamentID (Optional) - The ID of the tournament involved in this event.</description>
        ///     </item>
        /// </list>
        /// </summary>
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

        /// <summary>
        /// Creates a new CalendarEvent from the XElement provided.
        /// </summary>
        /// <param name="src">The XElement that this CalendarEvent will be built from.</param>
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