using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Players
{
    sealed public partial class PlayerManager
    {
        public XElement ToXML()
        {
            XElement root = new XElement("players");

            foreach (Player p in allPlayers.Select(kvp => kvp.Value).ToList())
            {
                root.Add(p.ToXML());
            }

            return root;
        }

        public PlayerManager(XElement src)
        {
            allPlayers = new Dictionary<int, Player>();

            foreach (XElement p in src.Descendants("players"))
            {
                Player player = new Player(p);
                allPlayers.Add(player.ID, player);
            }
        }
    }
}