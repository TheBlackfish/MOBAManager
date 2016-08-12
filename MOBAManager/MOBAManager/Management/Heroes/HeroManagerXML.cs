using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Heroes
{
    sealed public partial class HeroManager
    {
        public XElement ToXML()
        {
            XElement root = new XElement("heroes");

            foreach (Hero h in allHeroes.Select(kvp => kvp.Value).ToList())
            {
                root.Add(h.ToXML());
            }

            return root;
        }

        public HeroManager(XElement src)
        {
            allHeroes = new Dictionary<int, Hero>();
            
            foreach (XElement h in src.Descendants("hero"))
            {
                Hero hero = new Hero(h);
                allHeroes.Add(hero.ID, hero);
            }
        }
    }
}