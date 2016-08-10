using MOBAManager.Management.Players;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Teams
{
    partial class TeamManager
    {
        /// <summary>
        /// Creates several default teams.
        /// Afterwards, they are inserted into the manager's dictionary.
        /// </summary>
        private void CreateTeams()
        {
            XDocument teamFile = XDocument.Load("Data/Teams.xml");

            string[] names = teamFile.Descendants("team").Select(xe => xe.Value).ToArray();

            foreach (string n in names)
            {
                Team newTeam = new Team(teams.Count, n);
                teams.Add(teams.Count, newTeam);
            }
        }

        /// <summary>
        /// Creates a new temporary team consisting of randomized players. This team has an ID of -1, denoting that its stats will not be recorded.
        /// </summary>
        /// <returns></returns>
        public Team CreatePUBTeam()
        {
            Team ret = new Teams.Team(-1, "PUB Team");
            for (int i = 0; i < 5; i++)
            {
                ret.AddMember(pm.CreatePUBPlayer());
            }
            return ret;
        }
    }
}