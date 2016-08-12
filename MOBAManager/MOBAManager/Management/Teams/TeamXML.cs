using MOBAManager.Management.Players;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MOBAManager.Management.Teams
{
    sealed public partial class Team
    {
        public XElement ToXML()
        {
            XElement root = new XElement("team");

            root.SetAttributeValue("id", _id);
            root.SetAttributeValue("name", _name);

            string playersStr = "";
            foreach (Player p in teammates)
            {
                if (playersStr.Length > 0)
                {
                    playersStr += ",";
                }
                playersStr += p.ID;
            }
            root.Add("players", playersStr);

            return root;
        }

        public Team(PlayerManager pm, XElement src)
        {
            _id = int.Parse(src.Attribute("id").Value);
            _name = src.Attribute("name").Value;
            teammates = new List<Player>();
            foreach (XElement p in src.Descendants("players"))
            {
                teammates.Add(new Player(p));
            }
        }
    }
}