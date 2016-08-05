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
            string[] names = new string[]
            {
                "Alpha Team",
                "ChimpOut",
                "China One",
                "Living Legends",
                "MaSci",
                "Screaming Eagles",
                "Unicorns",
                "XG Pro"
            };

            foreach (string n in names)
            {
                Team newTeam = new Team(teams.Count, n);
                teams.Add(teams.Count, newTeam);
            }
        }
    }
}