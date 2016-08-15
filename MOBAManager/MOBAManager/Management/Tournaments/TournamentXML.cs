using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MOBAManager.Management.Tournaments
{
    public abstract partial class Tournament
    {
        protected abstract string GetTournamentType();

        public XElement ToXML()
        {
            XElement root = new XElement("tournament");

            root.SetAttributeValue("type", GetTournamentType());
            root.SetAttributeValue("id", _ID);
            root.SetAttributeValue("name", _name);
            root.SetAttributeValue("totalDays", totalDays);
            root.SetAttributeValue("currentDay", currentDay);

            string heroesStr = "";
            foreach (Hero h in allowedHeroes.Select(kvp => kvp.Value).ToList())
            {
                if (heroesStr.Length > 0)
                {
                    heroesStr += ",";
                }
                heroesStr += h.ID;
            }
            root.Add(new XElement("heroes", heroesStr));

            if (includedTeams.Count > 0)
            {
                string teamStr = "";
                foreach (Team t in includedTeams)
                {
                    if (teamStr.Length > 0)
                    {
                        teamStr += ",";
                    }
                    teamStr += t.ID;
                }
                root.Add(new XElement("teams", teamStr));
            }
            else
            {
                //Invitations *shudder*
            }

            if (upcomingMatchups.Count > 0)
            {
                XElement upcoming = new XElement("upcoming");
                foreach (TourneyMatchup tm in upcomingMatchups)
                {
                    upcoming.Add(tm.ToXML());
                }
                root.Add(upcoming);
            }
            
            if (resolvedMatchups.Count > 0)
            {
                XElement resolved = new XElement("resolved");
                foreach (TourneyMatchup tm in resolvedMatchups)
                {
                    resolved.Add(tm.ToXML());
                }
                root.Add(resolved);
            }

            return root;
        }

        private void AddMatchupFromXML(XElement src, bool resolved)
        {

        }

        public Tournament(TeamManager tm, HeroManager hm, XElement src)
        {
            _ID = int.Parse(src.Attribute("id").Value);
            _name = src.Attribute("name").Value;
            totalDays = int.Parse(src.Attribute("totalDays").Value);
            currentDay = int.Parse(src.Attribute("currentDay").Value);

            if (src.Descendants("heroes").Count() > 0)
            {
                allowedHeroes = new Dictionary<int, Hero>();
                List<int> heroList = src.Descendants("heroes")
                    .First()
                    .Value
                    .Split(new char[] { ',' })
                    .Select(str => int.Parse(str))
                    .OrderBy(val => val)
                    .ToList();
                foreach (int i in heroList)
                {
                    if (allowedHeroes.Select(kvp => kvp.Value)
                        .Select(h => h.ID == i)
                        .Count() == 0)
                    {
                        allowedHeroes.Add(i, hm.GetHeroByID(i));
                    }
                }
            }
            else
            {
                allowedHeroes = hm.GetHeroDictionary();
            }
            
            if (src.Descendants("teams").Count() > 0)
            {
                includedTeams = new List<Team>();
                List<int> teamList = src.Descendants("teams")
                    .First()
                    .Value
                    .Split(new char[] { ',' })
                    .Select(str => int.Parse(str))
                    .ToList();
                foreach (int i in teamList)
                {
                    includedTeams.Add(tm.GetTeamByID(i));
                }
            }

            if (src.Descendants("resolved").Count() > 0)
            {
                resolvedMatchups = new List<TourneyMatchup>();
                List<XElement> resolvedMatchXML = src.Descendants("resolved")
                    .Descendants("tm")
                    .ToList();
                foreach (XElement matchup in resolvedMatchXML)
                {
                    AddMatchupFromXML(matchup, true);
                }
            }

            if (src.Descendants("upcoming").Count() > 0)
            {
                upcomingMatchups = new List<TourneyMatchup>();
                List<XElement> upcomingMatchXML = src.Descendants("upcoming")
                    .Descendants("tm")
                    .ToList();
                foreach (XElement matchup in upcomingMatchXML)
                {
                    AddMatchupFromXML(matchup, false);
                }
            }
        }
    }
}