using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Heroes
{
    partial class HeroManager
    {
        /// <summary>
        /// Creates all of the heros for the initial construction of the game.
        /// The heroes are immediately placed into the HeroManager's hero dictionary.
        /// </summary>
        private void CreateHeroes()
        {
            XDocument heroFile = XDocument.Load("Data/Heroes.xml");

            string[] heroNames = heroFile.Descendants("hero")
                .Select(xe => xe.Value)
                .OrderBy(s => s)
                .ToArray();

            //Create initial skills
            List<int> initialSkill = new List<int>();
            initialSkill.AddRange(new int[] {   7, 7, 7, 7,
                                                6, 6, 6,
                                                5, 5,
                                                4, 4,
                                                3, 3,
                                                2, 2, 2,
                                                1, 1, 1, 1,
                                                -1, -1, -1, -1,
                                                -2, -2, -2,
                                                -3, -3,
                                                -4});
            while (initialSkill.Count != heroNames.Length)
            {
                initialSkill.Add(0);
            }

            //Create initial heroes
            foreach (string curHero in heroNames)
            {
                int randSkillIndex = RNG.Roll(initialSkill.Count);
                allHeroes.Add(allHeroes.Count, new Hero(allHeroes.Count, curHero, initialSkill[randSkillIndex]));
                initialSkill.RemoveAt(randSkillIndex);
            }

            List <int[]> synergyPairs = new List<int[]>();
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
                int index = RNG.Roll(synergyValues.Count);

                allHeroes[p[0]].SetSynergy(p[1], synergyValues[index]);
                allHeroes[p[1]].SetSynergy(p[0], synergyValues[index]);

                synergyValues.RemoveAt(index);
            }

            //Create counters
            List<int[]> counterValues = new List<int[]>();

            while (counterValues.Count < counterPairs.Count)
            {
                for (int i = 5; i >= 0; i--)
                {
                    for (int j = 0; j >= -5; j--)
                    {
                        counterValues.Add(new int[] { i, j });
                    }
                }
            }

            foreach (int[] p in counterPairs)
            {
                int index = RNG.Roll(counterValues.Count);

                if (RNG.Roll(2) == 0)
                {
                    allHeroes[p[0]].SetCounter(p[1], counterValues[index][0]);
                    allHeroes[p[1]].SetCounter(p[0], counterValues[index][1]);
                }
                else
                {
                    allHeroes[p[0]].SetCounter(p[1], counterValues[index][1]);
                    allHeroes[p[1]].SetCounter(p[0], counterValues[index][0]);
                }

                counterValues.RemoveAt(index);
            }
        }
    }
}