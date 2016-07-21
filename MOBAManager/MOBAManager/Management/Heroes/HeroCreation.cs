using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.Management.Heroes
{
    partial class HeroManager
    {
        /// <summary>
        /// Creates all of the heros for the initial construction of the game.
        /// The heroes are immediately placed into the HeroManager's hero dictionary.
        /// </summary>
        private void createHeroes()
        {
            string[] heroNames = new string[]
            {
                "Abdul Alhazred",
                "Ambrose Dewart",
                "Arianna",
                "Bast",
                "Bran Mak Morn",
                "Charles Dexter Ward",
                "Draken",
                "Eldin The Wanderer",
                "Gustaf",
                "Hastur",
                "Hydra",
                "Ithaqua",
                "Juk-Shabb",
                "Karakal",
                "Kuranes",
                "Nodens",
                "Obed Marsh",
                "Orryx",
                "Pickman", 
                "Randolph Carter",
                "Sathubbo",
                "Shathak",
                "Starbane",
                "Tstll",
                "Uvhash",
                "Vulthoom",
                "Yharnam",
                "Yig",
                "Zathog",
                "Zo-Kalar"
            };

            int[] initialSkill = new int[] { 7, 6, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1, 1, -1, -1, -1, -1 };

            //Create initial heroes
            while (heroNames.Length > 0 && initialSkill.Length > 0)
            {
                string curHero = heroNames[RNG.roll(heroNames.Length)];

                Hero newHero = new Hero(allHeroes.Count, curHero, initialSkill[0]);
                allHeroes.Add(allHeroes.Count, newHero);

                heroNames = heroNames.Where(n => !n.Equals(curHero)).ToArray();
                initialSkill = initialSkill.Skip(1).ToArray();
            }
            while (heroNames.Length > 0)
            {
                Hero newHero = new Hero(allHeroes.Count, heroNames[0]);
                allHeroes.Add(allHeroes.Count, newHero);
                heroNames = heroNames.Skip(1).ToArray();
            }

            List<int[]> synergyPairs = new List<int[]>();
            List<int[]> counterPairs = new List<int[]>();

            for (int i = 0; i < allHeroes.Count; i++)
            {
                for (int j = i + 1; j < allHeroes.Count; j++)
                {
                    synergyPairs.Add(new int[] { i, j });
                    counterPairs.Add(new int[] { i, j });
                }
            }

            //Create synergies
            List<int> synergyValues = new List<int>();
            while (synergyValues.Count < synergyPairs.Count)
            {
                synergyValues.Add(4);
                synergyValues.Add(3);
                synergyValues.Add(3);
                synergyValues.Add(2);
                synergyValues.Add(2);
                synergyValues.Add(2);
                synergyValues.Add(1);
                synergyValues.Add(1);
                synergyValues.Add(1);
                synergyValues.Add(1);
                synergyValues.Add(0);
                synergyValues.Add(0);
                synergyValues.Add(0);
                synergyValues.Add(0);
                synergyValues.Add(-1);
                synergyValues.Add(-1);
                synergyValues.Add(-2);
            }

            foreach (int[] p in synergyPairs)
            {
                int index = RNG.roll(synergyValues.Count);
                int val = synergyValues[index];

                allHeroes[p[0]].setSynergy(p[1], val);
                allHeroes[p[1]].setSynergy(p[0], val);

                synergyValues.RemoveAt(index);
            }

            //Create counters
            List<int[]> counterValues = new List<int[]>();

            while (counterValues.Count < counterPairs.Count)
            {
                for (int i = 2; i >= 0; i--)
                {
                    for (int j = 0; j >= -2; j--)
                    {
                        counterValues.Add(new int[] { i, j });
                    }
                }
            }

            foreach (int[] p in counterPairs)
            {
                int index = RNG.roll(counterValues.Count);
                int[] val = counterValues[index];

                if (RNG.roll(2) == 0)
                {
                    allHeroes[p[0]].setCounter(p[1], val[0]);
                    allHeroes[p[1]].setCounter(p[0], val[1]);
                }
                else
                {
                    allHeroes[p[0]].setCounter(p[1], val[1]);
                    allHeroes[p[1]].setCounter(p[0], val[0]);
                }

                counterValues.RemoveAt(index);
            }
        }
    }
}