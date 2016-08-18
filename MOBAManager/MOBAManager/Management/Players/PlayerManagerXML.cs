using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Players
{
    sealed public partial class PlayerManager
    {
        /// <summary>
        /// <para>Turns the PlayerManager into an XElement with the type 'players'.</para>
        /// <para>The XElement has no attributes.</para>
        /// <para>The XElement has 1+ nested elements.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>player (Numerous) - The Player in XML form.</description>
        ///     </item>
        /// </list>
        /// </summary>
        public XElement ToXML()
        {
            XElement root = new XElement("players");

            foreach (Player p in allPlayers.Select(kvp => kvp.Value).ToList())
            {
                root.Add(p.ToXML());
            }

            return root;
        }

        /// <summary>
        /// Creates a new PlayerManager object from the provided XElement.
        /// </summary>
        /// <param name="src">The XElement to build from.</param>
        public PlayerManager(XElement src)
        {
            allPlayers = new Dictionary<int, Player>();

            foreach (XElement p in src.Descendants("player"))
            {
                Player player = new Player(p);
                allPlayers.Add(player.ID, player);
            }
        }
    }
}