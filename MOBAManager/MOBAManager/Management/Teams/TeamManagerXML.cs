using MOBAManager.Management.Players;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Teams
{
    sealed public partial class TeamManager
    {
        public XElement ToXML()
        {
            XElement root = new XElement("teams");

            foreach (Team t in teams.Select(kvp => kvp.Value).ToList())
            {
                root.Add(t.ToXML());
            }

            return root;
        }

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