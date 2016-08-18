using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Tournaments
{
    public partial class TournamentManager
    {
        /// <summary>
        /// <para>Turns the TournamentManager into an XElement with the type 'tournaments'.</para>
        /// <para>The XElement has no attributes.</para>
        /// <para>The XElement has 1+ nested element.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>tournament - The tournament in XML form.</description>
        ///     </item>
        /// </list>
        /// </summary>
        public XElement ToXML()
        {
            XElement root = new XElement("tournaments");

            foreach (Tournament t in tournaments)
            {
                root.Add(t.ToXML());
            }

            return root;
        }

        /// <summary>
        /// Creates a TournamentManager from the provided XElement.
        /// </summary>
        /// <param name="tm">The TeamManager that relates to this TournamentManager.</param>
        /// <param name="hm">The HeroManager that relates to this TournamentManager.</param>
        /// <param name="src">The XElement to build from.</param>
        public TournamentManager(TeamManager tm, HeroManager hm, XElement src)
            :this()
        {
            if (src.Descendants("tournament").Count() > 0)
            {
                foreach (XElement tourney in src.Descendants("tournament"))
                {
                    switch (tourney.Attribute("type").Value)
                    {
                        case "Double Elimination":
                            tournaments.Add(new DoubleEliminationTournament(tm, hm, tourney));
                            break;
                        case "Round Robin":
                            tournaments.Add(new RoundRobinTournament(tm, hm, tourney));
                            break;
                        case "Single Elimination":
                            tournaments.Add(new SingleEliminationTournament(tm, hm, tourney));
                            break;
                    }
                }
            }
        }
    }
}