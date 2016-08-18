using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Heroes
{
    sealed public partial class Hero
    {
        /// <summary>
        /// <para>Turns the CalendarEvent into an XElement with the type 'ce'.</para>
        /// <para>The XElement has 2 attributes.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>id - The ID of the hero.</description>
        ///     </item>
        ///     <item>
        ///         <description>name - The name of the hero.</description>
        ///     </item>
        /// </list>
        /// <para>The XElement has 3 nested elements.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>skill - The pure skill level of the hero.</description>
        ///     </item>
        ///     <item>
        ///         <description>synergies - The string representing this hero's hero synergies.</description>
        ///     </item>
        ///     <item>
        ///         <description>counters - The string representing this hero's hero counters.</description>
        ///     </item>
        /// </list>
        /// </summary>
        public XElement ToXML()
        {
            XElement root = new XElement("hero");

            root.SetAttributeValue("id", _id);
            root.SetAttributeValue("name", _name);

            root.Add(new XElement("skill", powerLevel));

            string synergyStr = "";
            foreach (KeyValuePair<int, double> kvp in synergies)
            {
                if (synergyStr.Length > 0)
                {
                    synergyStr += ",";
                }
                synergyStr += kvp.Key + "," + kvp.Value;
            }
            root.Add(new XElement("synergies", synergyStr));

            string counterStr = "";
            foreach (KeyValuePair<int, double> kvp in counters)
            {
                if (counterStr.Length > 0)
                {
                    counterStr += ",";
                }
                counterStr += kvp.Key + "," + kvp.Value;
            }
            root.Add(new XElement("counters", counterStr));

            return root;
        }

        /// <summary>
        /// Creates a new hero from the provided XElement.
        /// </summary>
        /// <param name="src">The XElement to build a hero from.</param>
        public Hero(XElement src)
        {
            _id = int.Parse(src.Attribute("id").Value);
            _name = src.Attribute("name").Value;
            powerLevel = int.Parse(src.Descendants("skill").First().Value);

            synergies = new Dictionary<int, double>();
            List<string> synergySplit = src.Descendants("synergies")
                .First()
                .Value
                .Split(new char[] { ',' })
                .ToList();
            if (synergySplit.Count % 2 != 0)
            {
                System.Console.WriteLine("Synergies do not have enough values");
            }
            while (synergySplit.Count >= 2)
            {
                int key = int.Parse(synergySplit[0]);
                double value = double.Parse(synergySplit[1]);
                synergies.Add(key, value);
                synergySplit = synergySplit.Skip(2).ToList();
            }

            counters = new Dictionary<int, double>();
            List<string> counterSplit = src.Descendants("counters")
                .First()
                .Value
                .Split(new char[] { ',' })
                .ToList();
            if (counterSplit.Count % 2 != 0)
            {
                System.Console.WriteLine("Counters do not have enough values");
            }
            while (counterSplit.Count >= 2)
            {
                int key = int.Parse(counterSplit[0]);
                double value = double.Parse(counterSplit[1]);
                counters.Add(key, value);
                counterSplit = counterSplit.Skip(2).ToList();
            }
        }
    }
}