using MOBAManager.Management.Players;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Teams
{
    sealed public partial class Team
    {
        /// <summary>
        /// <para>Turns the Team into an XElement with the type 'team'.</para>
        /// <para>The XElement has 2 attributes.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>id - The ID of the team.</description>
        ///     </item>
        ///     <item>
        ///         <description>name - The name of the team.</description>
        ///     </item>
        /// </list>
        /// <para>The XElement has 1 nested element.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>players - The string containing all player IDs belonging to this team.</description>
        ///     </item>
        /// </list>
        /// </summary>
        public XElement ToXML()
        {
            XElement root = new XElement("team");

            root.SetAttributeValue("id", _id);
            root.SetAttributeValue("name", _name);

            string playersStr = "";
            foreach (Player p in teammates)
            {
                if (playersStr.Length > 0)
                {
                    playersStr += ",";
                }
                playersStr += p.ID;
            }
            root.Add(new XElement("players", playersStr));

            return root;
        }

        /// <summary>
        /// Creates a new Team from the XElement provided.
        /// </summary>
        /// <param name="pm">The PlayerManager that is related to this team.</param>
        /// <param name="src">The XElement to build from.</param>
        public Team(PlayerManager pm, XElement src)
        {
            _id = int.Parse(src.Attribute("id").Value);
            _name = src.Attribute("name").Value;
            teammates = new List<Player>();
            List<int> playerIDs = src.Element("players")
                .Value
                .Split(new char[] { ',' })
                .Select(str => int.Parse(str))
                .ToList();
            foreach (int id in playerIDs)
            {
                teammates.Add(pm.GetPlayerByID(id));
            }
        }
    }
}