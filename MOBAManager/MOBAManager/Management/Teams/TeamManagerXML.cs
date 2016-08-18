using MOBAManager.Management.Players;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Teams
{
    sealed public partial class TeamManager
    {
        /// <summary>
        /// <para>Turns the TeamManager into an XElement with the type 'teams'.</para>
        /// <para>The XElement has no attributes.</para>
        /// <para>The XElement has 1+ nested elements.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>team (Numerous) - The Team object in XML form.</description>
        ///     </item>
        /// </list>
        /// </summary>
        public XElement ToXML()
        {
            XElement root = new XElement("teams");

            foreach (Team t in teams.Select(kvp => kvp.Value).ToList())
            {
                root.Add(t.ToXML());
            }

            return root;
        }

        /// <summary>
        /// Creates a new TeamManager from the XElement provided.
        /// </summary>
        /// <param name="pm">The PlayerManager that is related to this TeamManager.</param>
        /// <param name="src">The XElement to build from.</param>
        public TeamManager(PlayerManager pm, XElement src)
        {
            this.pm = pm;
            teams = new Dictionary<int, Team>();
            foreach (XElement t in src.Descendants("team"))
            {
                Team team = new Team(pm, t);
                teams.Add(team.ID, team);
            }
        }
    }
}