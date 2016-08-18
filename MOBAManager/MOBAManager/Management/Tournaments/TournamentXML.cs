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
        /// <summary>
        /// Returns a string describing the tournament type.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetTournamentType();

        /// <summary>
        /// <para>Turns the Tournament into an XElement with the type 'team'.</para>
        /// <para>The XElement has 5 attributes.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>type - The type of tournament this tournament is.</description>
        ///     </item>
        ///     <item>
        ///         <description>id - The ID of the tournament.</description>
        ///     </item>
        ///     <item>
        ///         <description>name - The name of the tournament.</description>
        ///     </item>
        ///     <item>
        ///         <description>totalDays - The total number of days this tournament takes place over.</description>
        ///     </item>
        ///     <item>
        ///         <description>currentDay - The current day of the tournament.</description>
        ///     </item>
        /// </list>
        /// <para>The XElement has 4 nested elements.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>heroes - The string containing all of the hero IDs allowed in this tournament.</description>
        ///     </item>
        ///     <item>
        ///         <description>teams - The string containing all of the team IDs participating in this tournament.</description>
        ///     </item>
        ///     <item>
        ///         <description>resolved - The container for all resolved matchups in XML form.</description>
        ///     </item>
        ///     <item>
        ///         <description>upcoming - The container for all upcoming matchups in XML form.</description>
        ///     </item>
        /// </list>
        /// </summary>
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

            if (usesInvites)
            {
                XElement inviteRoot = new XElement("invites");
                for (int i = 0; i < inviteFunctions.Count; i++)
                {
                    XElement invite = new XElement("invite", ((Tournament)inviteFunctions[i].Target).ID);
                    invite.SetAttributeValue("type", "TopRanked");
                    if (inviteNumbers[i] != -1)
                    {
                        invite.SetAttributeValue("num", inviteNumbers[i]);
                    }
                    inviteRoot.Add(invite);
                }
                root.Add(inviteRoot);
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

        /// <summary>
        /// Adds a TourneyMatchup built from an XElement to either the upcoming matchups or resolved matchups depending on the parameters set.
        /// </summary>
        /// <param name="src">The XElement to build a matchup from.</param>
        /// <param name="resolved">If true, the matchup will be added to the resolved matchups. Else it gets added to the upcoming matchups instead.</param>
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

        /// <summary>
        /// Creates a new Tournament from the XElement provided.
        /// </summary>
        /// <param name="tm">The TeamManager that relates to this tournament.</param>
        /// <param name="hm">The HeroManager that relates to this tournament.</param>
        /// <param name="src">The XElement to build from.</param>
        public Tournament(TeamManager tm, HeroManager hm, TournamentManager tym, XElement src)
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

            inviteFunctions = new List<Func<int, List<Team>>>();
            inviteNumbers = new List<int>();

            if (src.Element("invites") != null)
            {
                usesInvites = true;
                foreach (XElement invite in src.Element("invites").Descendants("invite"))
                {
                    switch (invite.Attribute("type").Value)
                    {
                        case "TopRanked":
                            int tourneyID = int.Parse(invite.Value);
                            if (invite.Attribute("num") != null)
                            {
                                addInviteFunction(tym.GetTournamentByID(tourneyID).GetTopRankedTeams, int.Parse(invite.Attribute("num").Value));
                            }
                            else
                            {
                                addInviteFunction(tym.GetTournamentByID(tourneyID).GetTopRankedTeams);
                            }
                            break;
                    }
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