using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Players
{
    sealed public partial class Player
    {
        public XElement ToXML()
        {
            XElement root = new XElement("player");

            root.SetAttributeValue("id", _id);
            root.SetAttributeValue("name", _name);

            root.Add(new XElement("skill", pureSkill));

            string heroStr = "";
            foreach (KeyValuePair<int, double> kvp in heroSkills)
            {
                if (heroStr.Length > 0)
                {
                    heroStr += ",";
                }
                heroStr += kvp.Key + "," + kvp.Value;
            }
            root.Add(new XElement("heroes", heroStr));

            string teamworkStr = "";
            foreach (KeyValuePair<int, double> kvp in teamworkSkills)
            {
                if (teamworkStr.Length > 0)
                {
                    teamworkStr += ",";
                }
                teamworkStr += kvp.Key + "," + kvp.Value;
            }
            root.Add(new XElement("teamwork", teamworkStr));

            return root;
        }

        public Player(XElement src)
        {
            _id = int.Parse(src.Attribute("id").Value);
            _name = src.Attribute("name").Value;
            pureSkill = int.Parse(src.Descendants("skill").First().Value);

            heroSkills = new Dictionary<int, double>();
            List<string> heroSplit = src.Descendants("heroes")
                .First()
                .Value
                .Split(new char[] { ',' })
                .ToList();
            if (heroSplit.Count % 2 != 0)
            {
                System.Console.WriteLine("Heroes do not have enough values");
            }
            while (heroSplit.Count >= 2)
            {
                int key = int.Parse(heroSplit[0]);
                double value = double.Parse(heroSplit[1]);
                heroSkills.Add(key, value);
                heroSplit = heroSplit.Skip(2).ToList();
            }

            teamworkSkills = new Dictionary<int, double>();
            List<string> teamworkSplit = src.Descendants("teamwork")
                .First()
                .Value
                .Split(new char[] { ',' })
                .ToList();
            if (teamworkSplit.Count % 2 != 0)
            {
                System.Console.WriteLine("Teamwork does not have enough values");
            }
            while (teamworkSplit.Count >= 2)
            {
                int key = int.Parse(teamworkSplit[0]);
                double value = double.Parse(teamworkSplit[1]);
                teamworkSkills.Add(key, value);
                teamworkSplit = teamworkSplit.Skip(2).ToList();
            }
        }
    }
}