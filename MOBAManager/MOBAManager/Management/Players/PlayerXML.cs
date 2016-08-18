using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Players
{
    sealed public partial class Player
    {
        /// <summary>
        /// <para>Turns the Player into an XElement with the type 'player'.</para>
        /// <para>The XElement has 2 attributes.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>id - The ID of the player.</description>
        ///     </item>
        ///     <item>
        ///         <description>name - The name of the player.</description>
        ///     </item>
        /// </list>
        /// <para>The XElement has 3 nested elements.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>skill - The pure skill of the player.</description>
        ///     </item>
        ///     <item>
        ///         <description>heroes - The string representing the player's various hero skills.</description>
        ///     </item>
        ///     <item>
        ///         <description>teamwork - The string representing the player's teamwork skills with other players.</description>
        ///     </item>
        /// </list>
        /// </summary>
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

        /// <summary>
        /// Creates a new Player from the provided XElement.
        /// </summary>
        /// <param name="src">The XElement to build from.</param>
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