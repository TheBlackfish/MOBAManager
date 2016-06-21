using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.Management.Heroes
{
    partial class HeroManager
    {
        private void createHeroes()
        {
            Random rnd = new Random();

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

            while (heroNames.Length > 0 && initialSkill.Length > 0)
            {
                var curHero = heroNames[rnd.Next(heroNames.Length)];

                var newHero = new Hero(allHeroes.Count, curHero, initialSkill[0]);
                allHeroes.Add(allHeroes.Count, newHero);

                heroNames = heroNames.Where(n => !n.Equals(curHero)).ToArray();
                initialSkill = initialSkill.Skip(1).ToArray();
            }
            while (heroNames.Length > 0)
            {
                var newHero = new Hero(allHeroes.Count, heroNames[0]);
                allHeroes.Add(allHeroes.Count, newHero);
                heroNames = heroNames.Skip(1).ToArray();
            }
        }
    }
}