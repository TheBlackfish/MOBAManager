using MOBAManager.Management.Heroes;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Players
{
    partial class PlayerManager
    {
        /// <summary>
        /// Creates all of the players for the initial construction of the game.
        /// The players are immediately placed into the PlayerManager's player dictionary.
        /// </summary>
        private void CreatePlayers()
        {
            XDocument playerFile = XDocument.Load("Data/Players.xml");

            string[] playerNames = playerFile.Descendants("player").Select(xe => xe.Value).ToArray();

            int[] initialSkill = new int[] {    6, 6,
                                                5, 5, 5,
                                                4, 4, 4, 4,
                                                3, 3, 3, 3, 3,
                                                2, 2, 2, 2, 2, 2,
                                                1, 1, 1, 1, 1, 1, 1, 1,
                                                -1, -1, -1, -1,
                                                -2, -2,
                                                -3};

            while (playerNames.Length > 0 && initialSkill.Length > 0)
            {
                string curPlayer = playerNames[RNG.Roll(playerNames.Length)];

                allPlayers.Add(allPlayers.Count, new Player(allPlayers.Count, curPlayer, initialSkill[0]));

                playerNames = playerNames.Where(n => !n.Equals(curPlayer)).ToArray();
                initialSkill = initialSkill.Skip(1).ToArray();
            }
            while (playerNames.Length > 0)
            {
                allPlayers.Add(allPlayers.Count, new Player(allPlayers.Count, playerNames[0]));
                playerNames = playerNames.Skip(1).ToArray();
            }

            //Create each player's personal hero skills.
            List<Player> players = allPlayers.Select(kvp => kvp.Value).ToList();

            foreach (Player p in players)
            {
                List<int> allHeroIDs = new List<int>();
                for (int i = 0; i < 40; i++)
                {
                    allHeroIDs.Add(i);
                }

                List<int> allSkills = new List<int>();
                int maxSkill = RNG.Roll(3) + 2;
                int minSkill = 0 - RNG.Roll(5);

                for (int i = maxSkill; i >= 0; i--)
                {
                    for (int j = i; j <= maxSkill; j++)
                    {
                        allSkills.Add(i);
                    }
                }

                for (int i = minSkill; i <= 0; i++)
                {
                    for (int j = i; j >= minSkill; j--)
                    {
                        allSkills.Add(i);
                    }
                }

                foreach (int val in allSkills)
                {
                    int id = allHeroIDs[RNG.Roll(allHeroIDs.Count)];
                    p.SetHeroSkill(id, val);
                    allHeroIDs.Remove(id);
                }
            }
        }

        /// <summary>
        /// Creates a temporary player with an ID of -1 with random stats. The ID of -1 denotes that its stats will not be recorded.
        /// </summary>
        /// <returns></returns>
        public Player CreatePUBPlayer()
        {
            Player p = new Player(-1, "Pubber", 10 - RNG.Roll(20));

            List<int> allHeroIDs = new List<int>();
            for (int i = 0; i < 40; i++)
            {
                allHeroIDs.Add(i);
            }

            List<int> allSkills = new List<int>();
            int maxSkill = RNG.Roll(8) + 2;
            int minSkill = 0 - RNG.Roll(10);

            for (int i = maxSkill; i >= 0; i--)
            {
                for (int j = i; j <= maxSkill; j++)
                {
                    allSkills.Add(i);
                }
            }

            for (int i = minSkill; i <= 0; i++)
            {
                for (int j = i; j >= minSkill; j--)
                {
                    allSkills.Add(i);
                }
            }

            for (int i = 0; i < allSkills.Count * 2; i++)
            {
                int x = RNG.Roll(allSkills.Count);
                int y = RNG.Roll(allSkills.Count);

                int temp = allSkills[x];
                allSkills[x] = allSkills[y];
                allSkills[y] = temp;
            }

            foreach (int val in allSkills)
            {
                if (allHeroIDs.Count > 0)
                {
                    int id = allHeroIDs[RNG.Roll(allHeroIDs.Count)];
                    p.SetHeroSkill(id, val);
                    allHeroIDs.Remove(id);
                }
            }

            return p;
        }
    }
}