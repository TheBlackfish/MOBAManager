using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Tournaments
{
    public partial class TournamentManager
    {
        public XElement ToXML()
        {
            XElement root = new XElement("tournaments");

            foreach (Tournament t in tournaments)
            {
                root.Add(t.ToXML());
            }

            return root;
        }

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