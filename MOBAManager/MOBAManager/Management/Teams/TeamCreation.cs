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
    }
}