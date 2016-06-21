using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.Management.Players
{
    partial class PlayerManager
    {
        /// <summary>
        /// Creates all of the players for the initial construction of the game. The players are immediately placed into the PlayerManager's player dictionary.
        /// </summary>
        private void createPlayers()
        {
            Random rnd = new Random();

            string[] playerNames = new string[]
            {
                "TheBlackfish",
                "gal_axy",
                "Dol Amroth",
                "SergeantPuppers",
                "XtinctionX",
                "Fresh",
                "rainbow&river",
                "john1337",
                "Dondo",
                "Kittey",
                "FreeZing",
                "skysky",
                "EdgeMaster",
                "nkl",
                "Smile",
                "Mango",
                "TheRealest",
                "skerg",
                "Old Man Clint",
                "waow!",
                "NO LEGS",
                "Britishname Complicated",
                "US[A]",
                "Darkness",
                "Chicago Ted",
                "crybaby [10 YEARS!]",
                "ebola",
                "Frostgecko",
                "420 Booty Wizard",
                "xXxKILLERxXx",
                "Bob Ross",
                "KZ",
                "SirExplosions",
                "that one guy",
                "PotatoFarmer",
                "Fucking Linda",
                "4343",
                "stupid-bulder",
                "Amazing Llama!",
                "Tenty McTentacleface",
                "Siberian",
                "genuine",
                "EGein",
                "kalem",
                "Brometheus",
                "V00D00",
                "2clid",
                "go pod",
                "FULL GANDHI",
                "JDN"
            };

            int[] initialSkill = new int[] {    5, 5,
                                                4, 4, 4, 4,
                                                3, 3, 3, 3, 3,
                                                2, 2, 2, 2, 2, 2,
                                                1, 1, 1, 1, 1, 1, 1, 1,
                                                -1, -1, -1, -1,
                                                -2, -2,
                                                -3};

            while (playerNames.Length > 0 && initialSkill.Length > 0)
            {
                var curPlayer = playerNames[rnd.Next(playerNames.Length)];

                var newPlayer = new Player(allPlayers.Count, curPlayer, initialSkill[0]);
                allPlayers.Add(allPlayers.Count, newPlayer);

                playerNames = playerNames.Where(n => !n.Equals(curPlayer)).ToArray();
                initialSkill = initialSkill.Skip(1).ToArray();
            }
            while (playerNames.Length > 0)
            {
                var newPlayer = new Player(allPlayers.Count, playerNames[0]);
                allPlayers.Add(allPlayers.Count, newPlayer);
                playerNames = playerNames.Skip(1).ToArray();
            }
        }
    }
}