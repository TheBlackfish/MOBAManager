using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Heroes
{
    sealed public partial class HeroManager
    {
        /// <summary>
        /// <para>Turns the HeroManager into an XElement with the type 'heroes'.</para>
        /// <para>The XElement has no attributes.</para>
        /// <para>The XElement has 1+ nested elements.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>hero - The Hero object turned into an XElement.</description>
        ///     </item>
        /// </list>
        /// </summary>
        public XElement ToXML()
        {
            XElement root = new XElement("heroes");

            foreach (Hero h in allHeroes.Select(kvp => kvp.Value).ToList())
            {
                root.Add(h.ToXML());
            }

            return root;
        }

        /// <summary>
        /// Creates a new HeroManager from the XElement provided.
        /// </summary>
        /// <param name="src">The XElement to build from.</param>
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