using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Management.Heroes
{
    partial class HeroManager
    {
        private Dictionary<int, Hero> allHeroes;

        public List<Hero> getAllHeroes()
        {
            return allHeroes.Select(kvp => kvp.Value).ToList();
        }

        public HeroManager()
        {
            createHeroes();
        }
    }
}
