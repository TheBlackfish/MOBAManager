﻿using MOBAManager.Management.Calendar;
using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Statistics;
using MOBAManager.Management.Teams;
using MOBAManager.Management.Tournaments;
using MOBAManager.MatchResolution;
using MOBAManager.Resolution.BootcampResolution;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MOBAManager.Management
{
    sealed public class GameManager
    {
        #region Private variables
        /// <summary>
        /// The minimum number of pub games to resolve each day.
        /// </summary>
        private int minPubsPerDay = 100;

        /// <summary>
        /// The maximum number of pub games to be added to minPubsPerDay for pub resolution each day.
        /// </summary>
        private int maxPubsAddedPerDay = 50;
        #endregion

        #region Temporarily Public Variables
        /// <summary>
        /// The calendar manager of the current game.
        /// </summary>
        public CalendarManager calendarManager;

        /// <summary>
        /// The hero manager of the current game.
        /// </summary>
        public HeroManager heroManager;

        /// <summary>
        /// The player manager of the current game.
        /// </summary>
        public PlayerManager playerManager;

        /// <summary>
        /// The statistics manager of the current game.
        /// </summary>
        public StatisticsManager statsManager;

        /// <summary>
        /// The team manager of the current game.
        /// </summary>
        public TeamManager teamManager;

        /// <summary>
        /// The tournament manager of the current game.
        /// </summary>
        public TournamentManager tournamentManager;
        #endregion

        #region Public Methods
        /// <summary>
        /// Turns a calendar event into a bootcamp session.
        /// </summary>
        /// <param name="ce">The CalendarEvent to translate.</param>
        /// <returns></returns>
        public BootcampSession TranslateEventToBootcamp(CalendarEvent ce)
        {
            if (ce.team1ID == 0)
            {
                return new BootcampSession(teamManager.GetTeamByID(ce.team1ID), heroManager.GetIDSortedNames(), true);
            }
            return new BootcampSession(teamManager.GetTeamByID(ce.team1ID), heroManager.GetIDSortedNames());
        }

        /// <summary>
        /// Turns a calendar event into a match.
        /// </summary>
        /// <param name="ce">The CalendarEvent to translate.</param>
        /// <returns></returns>
        public Match TranslateEventToMatch(CalendarEvent ce)
        {
            if (ce.team1ID == 0)
            {
                return new Match(true, teamManager.GetTeamByID(ce.team1ID), teamManager.GetTeamByID(ce.team2ID), 1, heroManager.GetHeroDictionary());
            }
            else if (ce.team2ID == 0)
            {
                return new Match(true, teamManager.GetTeamByID(ce.team1ID), teamManager.GetTeamByID(ce.team2ID), 2, heroManager.GetHeroDictionary());
            }
            else
            {
                return new Match(teamManager.GetTeamByID(ce.team1ID), teamManager.GetTeamByID(ce.team2ID), heroManager.GetHeroDictionary());
            }
        }

        /// <summary>
        /// Resolves a random number of public matches consisting of randomized teams of randomized players.
        /// Only the hero win/loss and pick/ban statistics are relevant for these matches.
        /// </summary>
        public void ResolvePubs()
        {
            int actualPubs = minPubsPerDay + RNG.Roll(maxPubsAddedPerDay);
            List<StatsBundle> pubData = new List<StatsBundle>();
            for (int i = 0; i < actualPubs; i++)
            {
                Match m = new Match(teamManager.CreatePUBTeam(), teamManager.CreatePUBTeam(), heroManager.GetHeroDictionary());
                m.InstantlyResolve();
                pubData.Add(m.GetStats());
            }
            statsManager.ProcessManyBundles(pubData, heroManager, playerManager, teamManager);
        }

        /// <summary>
        /// Calls ResolvePubs() a number of times equal to the multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier for how many ResolvePubs() calls should be made.</param>
        public void ResolvePubs(int multiplier)
        {
            for (int i = 0; i < multiplier; i++)
            {
                ResolvePubs();
            }
        }

        /// <summary>
        /// Saves the game to the default save location.
        /// </summary>
        public void Save()
        {
            XDocument saveFile = ToXML();
            saveFile.Save("save.xml");
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Translates this game manager into a XDocument meant to be saved.
        /// </summary>
        /// <returns></returns>
        private XDocument ToXML()
        {
            XElement root = new XElement("game");

            root.Add(heroManager.ToXML());
            root.Add(playerManager.ToXML());
            root.Add(teamManager.ToXML());
            root.Add(tournamentManager.ToXML());
            root.Add(calendarManager.ToXML());
            root.Add(statsManager.ToXML());

            XDocument saveFile = new XDocument(root);

            return saveFile;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the game manager and all of its subordinate managers.
        /// </summary>
        public GameManager()
        {
            heroManager = new HeroManager();

            playerManager = new PlayerManager();

            teamManager = new TeamManager(playerManager);

            tournamentManager = new TournamentManager();

            calendarManager = new CalendarManager(teamManager, tournamentManager);

            statsManager = new StatisticsManager(heroManager.GetHeroDictionary(), playerManager.GetPlayerDictionary(), teamManager.GetTeamDictionary());

            teamManager.PopulateTeams(playerManager.GetAllPlayers());

            tournamentManager.CreateTournaments(calendarManager, teamManager, heroManager);

            ResolvePubs(5);

            Save();
        }

        /// <summary>
        /// Creates the game manager using the provided XDocument.
        /// </summary>
        /// <param name="xml">The XDocument to build from.</param>
        public GameManager(XDocument xml)
        {
            XElement root = xml.Root;

            try
            {
                heroManager = new HeroManager(root.Element("heroes"));
                playerManager = new PlayerManager(root.Element("players"));
                teamManager = new TeamManager(playerManager, root.Element("teams"));
                tournamentManager = new TournamentManager(teamManager, heroManager, root.Element("tournaments"));
                calendarManager = new CalendarManager(teamManager, tournamentManager, root.Element("calendar"));
                statsManager = new StatisticsManager(heroManager, playerManager, teamManager, root.Element("stats"));
            }
            catch (NullReferenceException)
            {
                throw new XmlException("Save file is not properly formatted.");
            }
        }
        #endregion
    }
}
