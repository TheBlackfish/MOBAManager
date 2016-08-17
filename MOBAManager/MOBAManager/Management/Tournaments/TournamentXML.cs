using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System;
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
            //Create tourneymatchup by going through each variable needed
            int tmID = int.Parse(src.Attribute("id").Value);
            int numberOfMatches = int.Parse(src.Attribute("matches").Value);
            int dayOfMatch = int.Parse(src.Attribute("dayOfMatch").Value);
            int[] cellPosition = src.Element("cellPos")
                .Value
                .Split(new char[] { ',' })
                .Select(str => int.Parse(str))
                .ToArray();

            XElement teamOne = src.Element("teamOne");
            int team1Slot = -1;
            int team1Wins = int.Parse(teamOne.Value);
            if (teamOne.Attribute("slot") != null)
            {
                team1Slot = int.Parse(teamOne.Attribute("slot").Value);
            }
            Func<int, Team> team1Function = null;
            switch (teamOne.Attribute("func").Value)
            {
                case "GetWinner":
                    int targetID = int.Parse(teamOne.Attribute("match").Value);
                    foreach (TourneyMatchup prev in upcomingMatchups.Concat(resolvedMatchups))
                    {
                        if (prev.ID == targetID)
                        {
                            team1Function = prev.GetWinner;
                        }
                    }
                    break;
                case "GetLoser":
                    targetID = int.Parse(teamOne.Attribute("match").Value);
                    foreach (TourneyMatchup prev in upcomingMatchups.Concat(resolvedMatchups))
                    {
                        if (prev.ID == targetID)
                        {
                            team1Function = prev.GetLoser;
                        }
                    }
                    break;
                case "GetSlot":
                    team1Function = GetTeamInSlot;
                    break;
            }


            XElement teamTwo = src.Element("teamTwo");
            int team2Slot = -1;
            int team2Wins = int.Parse(teamOne.Value);
            if (teamTwo.Attribute("slot") != null)
            {
                team2Slot = int.Parse(teamTwo.Attribute("slot").Value);
            }
            Func<int, Team> team2Function = null;
            switch (teamTwo.Attribute("func").Value)
            {
                case "GetWinner":
                    int targetID = int.Parse(teamTwo.Attribute("match").Value);
                    foreach (TourneyMatchup prev in upcomingMatchups.Concat(resolvedMatchups))
                    {
                        if (prev.ID == targetID)
                        {
                            team2Function = prev.GetWinner;
                        }
                    }
                    break;
                case "GetLoser":
                    targetID = int.Parse(teamTwo.Attribute("match").Value);
                    foreach (TourneyMatchup prev in upcomingMatchups.Concat(resolvedMatchups))
                    {
                        if (prev.ID == targetID)
                        {
                            team2Function = prev.GetLoser;
                        }
                    }
                    break;
                case "GetSlot":
                    team2Function = GetTeamInSlot;
                    break;
            }

            TourneyMatchup tm = new TourneyMatchup(tmID, numberOfMatches, team1Function, team2Function, cellPosition, team1Slot, team2Slot, dayOfMatch, team1Wins, team2Wins);

            if (resolved)
            {
                resolvedMatchups.Add(tm);
            }
            else
            {
                upcomingMatchups.Add(tm);
            }
        }

        public Tournament(TeamManager tm, HeroManager hm, XElement src)
        {
            _ID = int.Parse(src.Attribute("id").Value);
            _name = src.Attribute("name").Value;
            totalDays = int.Parse(src.Attribute("totalDays").Value);
            currentDay = int.Parse(src.Attribute("currentDay").Value);

            allowedHeroes = new Dictionary<int, Hero>();
            if (src.Descendants("heroes").Count() > 0)
            {
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

            includedTeams = new List<Team>();
            if (src.Descendants("teams").Count() > 0)
            {
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

            resolvedMatchups = new List<TourneyMatchup>();
            if (src.Descendants("resolved").Count() > 0)
            {
                List<XElement> resolvedMatchXML = src.Descendants("resolved")
                    .Descendants("tm")
                    .ToList();
                foreach (XElement matchup in resolvedMatchXML)
                {
                    AddMatchupFromXML(matchup, true);
                }
            }

            upcomingMatchups = new List<TourneyMatchup>();
            if (src.Descendants("upcoming").Count() > 0)
            {
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