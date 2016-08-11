using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using MOBAManager.MatchResolution;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MOBAManager.Management.Tournaments
{
    public abstract partial class Tournament
    {
        #region Internal classes
        /// <summary>
        /// The internal class that represents a Bo# match-up between two teams. Utilizes function delegates to properly get participating teams.
        /// </summary>
        protected class TourneyMatchup
        {
            #region Private variables
            /// <summary>
            /// The referal list of all matches performed thus far for this match-up.
            /// </summary>
            List<Match> matchesInMatchup;

            /// <summary>
            /// Helper variable for getting a team from a tournament slot, if that is what is used for team 1.
            /// </summary>
            int team1Slot = -1;

            /// <summary>
            /// Helper variable for getting a team from a tournament slot, if that is what is used for team 2.
            /// </summary>
            int team2Slot = -1;

            /// <summary>
            /// The function delegate for retrieving team 1.
            /// </summary>
            Func<int, Team> getTeam1Func;

            /// <summary>
            /// The function delegate for retrieving team 2.
            /// </summary>
            Func<int, Team> getTeam2Func;

            /// <summary>
            /// The number of matches that this match-up will take, max.
            /// </summary>
            int numberOfMatches = 0;

            /// <summary>
            /// The number of wins for team 1 thus far.
            /// </summary>
            int team1Wins
            {
                get
                {
                    return matchesInMatchup.Where(m => m.WinnerID == GetTeam1().ID).Count();
                }
            }

            /// <summary>
            /// The number of wins for team 2 thus far.
            /// </summary>
            int team2Wins
            {
                get
                {
                    return matchesInMatchup.Where(m => m.WinnerID == GetTeam2().ID).Count();
                }
            }
            #endregion

            #region Public variables
            /// <summary>
            /// The tournament day this match-up occurs on.
            /// </summary>
            public int DayOfMatch = 0;

            /// <summary>
            /// The position of the tournament in the display table.
            /// </summary>
            public int[] cellPosition = new int[] { -1, -1 };
            #endregion

            #region Private methods
            /// <summary>
            /// Calls the function delegate specified for team 1 and returns the result.
            /// </summary>
            /// <returns></returns>
            Team GetTeam1()
            {
                return getTeam1Func?.Invoke(team1Slot);
            }
            
            /// <summary>
            /// Calls the function delegate specified for team 2 and returns the result.
            /// </summary>
            /// <returns></returns>
            Team GetTeam2()
            {
                return getTeam2Func?.Invoke(team2Slot);
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Returns both teams in this match in an array.
            /// The teams are delivered in reverse order if there have been an odd number of matches already completed.
            /// </summary>
            /// <returns></returns>
            public Team[] GetTeams()
            {
                if (matchesInMatchup.Count % 2 == 1)
                {
                    return new Team[] { GetTeam2(), GetTeam1() }; 
                }
                return new Team[] { GetTeam1(), GetTeam2() };
            }

            /// <summary>
            /// Returns true if the match-up is finished due to completing all matches or one team has won the majority of the matches.
            /// </summary>
            /// <returns></returns>
            public bool isComplete()
            {
                if (matchesInMatchup.Where(m => m.Winner == -1).Count() > 0)
                {
                    return false;
                }

                if (numberOfMatches == matchesInMatchup.Count)
                {
                    return true;
                }

                if (team1Wins == numberOfMatches || team2Wins == numberOfMatches)
                {
                    return true;
                }

                if (team1Wins >= Math.Round((double)numberOfMatches / 2, MidpointRounding.AwayFromZero) ||
                    team2Wins >= Math.Round((double)numberOfMatches / 2, MidpointRounding.AwayFromZero))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns the winner of this match-up. Most commonly used as a function delegate for other matches.
            /// </summary>
            /// <param name="ignored">Put anything you want here, it will be ignored.</param>
            /// <returns></returns>
            public Team GetWinner(int ignored)
            {
                if (isComplete())
                {
                    if (team1Wins > team2Wins)
                    {
                        return GetTeam1();
                    }
                    return GetTeam2();
                }
                return null;
            }

            /// <summary>
            /// Returns the loser of this match-up. Most commonly used as a function delegate for other matches.
            /// </summary>
            /// <param name="ignored">Put anything you want here, it will be ignored.</param>
            /// <returns></returns>
            public Team GetLoser(int ignored)
            {
                if (isComplete())
                {
                    if (team1Wins > team2Wins)
                    {
                        return GetTeam2();
                    }
                    return GetTeam1();
                }
                return null;
            }

            /// <summary>
            /// Returns a display label with the match-up information.
            /// </summary>
            /// <returns></returns>
            public Label GetLabel()
            {
                Label l = new Label();

                Team one = GetTeam1();
                Team two = GetTeam2();

                l.Text = (one != null ? one.TeamName : "TBD") + Environment.NewLine + "VS" + Environment.NewLine + (two != null ? two.TeamName : "TBD");
                l.TextAlign = ContentAlignment.MiddleCenter;
                return l;          
            }

            /// <summary>
            /// Adds a match to the list of matches in this match-up.
            /// </summary>
            /// <param name="m"></param>
            public void registerMatch(Match m)
            {
                matchesInMatchup.Add(m);
            }
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new match-up.
            /// </summary>
            /// <param name="numberOfMatches">Number of matches this match-up will take, max.</param>
            /// <param name="team1Function">The function delegate for team 1 in this match.</param>
            /// <param name="team2Function">The function delegate for team 2 in this match.</param>
            public TourneyMatchup(int numberOfMatches, Func<int, Team> team1Function, Func<int, Team> team2Function, int[] cellPosition)
            {
                matchesInMatchup = new List<Match>();
                this.numberOfMatches = numberOfMatches;
                this.cellPosition = cellPosition;
                getTeam1Func = team1Function;
                getTeam2Func = team2Function;
            }

            /// <summary>
            /// Creates a new match-up.
            /// </summary>
            /// <param name="numberOfMatches">Number of matches this match-up will take, max.</param>
            /// <param name="team1Function">The function delegate for team 1 in this match.</param>
            /// <param name="team2Function">The function delegate for team 2 in this match.</param>
            /// <param name="team1Slot">The integer to use as a parameter for team1Function.</param>
            /// <param name="team2Slot">The integer to use as a parameter for team2Function.</param>
            public TourneyMatchup(int numberOfMatches, Func<int, Team> team1Function, Func<int, Team> team2Function, int[] cellPosition, int team1Slot, int team2Slot)
                : this(numberOfMatches, team1Function, team2Function, cellPosition)
            {
                this.team1Slot = team1Slot;
                this.team2Slot = team2Slot;
            }

            /// <summary>
            /// Creates a new match-up.
            /// </summary>
            /// <param name="numberOfMatches">Number of matches this match-up will take, max.</param>
            /// <param name="team1Function">The function delegate for team 1 in this match.</param>
            /// <param name="team2Function">The function delegate for team 2 in this match.</param>
            /// <param name="team1Slot">The integer to use as a parameter for team1Function.</param>
            /// <param name="team2Slot">The integer to use as a parameter for team2Function.</param>
            /// <param name="dayOfMatch">The offset from the start of the tournament that this match-up occurs on.</param>
            public TourneyMatchup(int numberOfMatches, Func<int, Team> team1Function, Func<int, Team> team2Function, int[] cellPosition, int team1Slot, int team2Slot, int dayOfMatch)
                : this(numberOfMatches, team1Function, team2Function, cellPosition, team1Slot, team2Slot)
            {
                this.DayOfMatch = dayOfMatch;
            }
            #endregion
        }
        #endregion

        #region Protected Variables
        /// <summary>
        /// The teams in this tournament.
        /// </summary>
        protected List<Team> includedTeams;

        /// <summary>
        /// The list of all tournament match-ups that have still to resolve. They are stored in order of resolution, so index 0 is the next match-up to resolve.
        /// </summary>
        protected List<TourneyMatchup> upcomingMatchups;

        /// <summary>
        /// The list of all tournament match-ups that have resolved. This exists to both prevent garbage collection on objects that are still needed, and to provide a nice interface for statistics gathering.
        /// </summary>
        protected List<TourneyMatchup> resolvedMatchups;

        /// <summary>
        /// The Dictionary of all heroes allowed in this tournament.
        /// </summary>
        protected Dictionary<int, Hero> allowedHeroes;

        /// <summary>
        /// The day this tournament is currently on. Only match-ups matching this day will resolve.
        /// </summary>
        protected int currentDay = 0;

        /// <summary>
        /// The total number of days this tournament covers.
        /// </summary>
        protected int totalDays = 1;

        /// <summary>
        /// The ID of the tournament.
        /// </summary>
        protected int _ID;

        /// <summary>
        /// The name of the tournament.
        /// </summary>
        protected string _name;

        /// <summary>
        /// Whether the tournament is currently enabled.
        /// </summary>
        protected bool _enabled = false;
        #endregion

        #region Abstract methods
        /// <summary>
        /// Override this to create the specific way the subclass sets up matches.
        /// </summary>
        public abstract void SetupMatches();

        /// <summary>
        /// Override this to create the specific way the subclass sets up matches over multiple days.
        /// </summary>
        public abstract void SetupMatches(int overDays);

        /// <summary>
        /// Override this to create the way to determine team rankings in this tournament.
        /// </summary>
        /// <returns></returns>
        public abstract List<Team> GetRankedResults();
        #endregion

        #region Public variables
        /// <summary>
        /// Returns the ID of this tournament.
        /// </summary>
        public int ID
        {
            get
            {
                return _ID;
            }
        }

        /// <summary>
        /// Returns the name of the tournament.
        /// </summary>
        public string name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Variable for whether or not the tournament is active.
        /// </summary>
        public bool enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Returns the team in the slot. Commonly used as 
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        protected Team GetTeamInSlot(int slot)
        {
            return includedTeams[slot];
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the next match of the tournament, or null if no matches are left to be completed today.
        /// </summary>
        /// <returns></returns>
        public Match GetMatch()
        {
            if (usesInvites && upcomingMatchups.Count == 0 && resolvedMatchups.Count == 0 && includedTeams.Count == 0)
            {
                ResolveInvitations();
                SetupMatches(totalDays);
            }

            TourneyMatchup cur = upcomingMatchups[0];

            if (cur.isComplete())
            {
                resolvedMatchups.Add(cur);
                upcomingMatchups = upcomingMatchups.Skip(1).ToList();
                if (upcomingMatchups.Count == 0)
                {
                    return null;
                }
                else
                {
                    cur = upcomingMatchups[0];
                }
            }

            if (cur.DayOfMatch == currentDay)
            {
                Team[] teams = cur.GetTeams();
                if (teams[0].ID != 0 && teams[1].ID != 0)
                {
                    Match m = new Match(teams[0], teams[1], allowedHeroes);
                    cur.registerMatch(m);
                    return m;
                }
                else
                {
                    Match m = new Match(true, teams[0], teams[1], teams[0].ID == 0 ? 1 : 2, allowedHeroes);
                    cur.registerMatch(m);
                    return m;
                }
            }
            return null;
        }

        /// <summary>
        /// Advances the day and disables the tournament.
        /// </summary>
        public void AdvanceDay()
        {
            currentDay++;
            _enabled = false;
        }

        /// <summary>
        /// Returns true if there are no more days left to resolve in this tournament.
        /// </summary>
        /// <returns></returns>
        public bool IsComplete()
        {
            if (includedTeams.Count == 0)
            {
                return false;
            }
            else
            {
                return ((currentDay + 1) == totalDays && upcomingMatchups.Count == 0);
            }
        }

        /// <summary>
        /// Returns a list of all of the teams in the tournament.
        /// </summary>
        /// <returns></returns>
        public List<Team> GetAllTeams()
        {
            return includedTeams;
        }

        /// <summary>
        /// Returns a list of the current top X teams in this tournament, where X = the number provided.
        /// </summary>
        /// <param name="ranks">The number of teams to retrieve.</param>
        /// <returns></returns>
        public List<Team> GetTopRankedTeams(int ranks)
        {
            if (ranks >= includedTeams.Count)
            {
                return GetRankedResults();
            }
            return GetRankedResults().Take(ranks).ToList();
        }

        /// <summary>
        /// Returns a summary of what the tournament has done on the current day.
        /// </summary>
        /// <returns></returns>
        public string GetSummary()
        {
            if (IsComplete())
            {
                return name + " has completed! " + GetTopRankedTeams(1)[0].TeamName + " has come out on top!";
            }
            else
            {
                return name + " has finished Day " + (currentDay + 1) + ".";
            }
        }

        /// <summary>
        /// Creates a grid for displaying the tournament bracket.
        /// </summary>
        /// <returns></returns>
        public TableLayoutPanel GetDisplayPanel()
        {
            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.SuspendLayout();

            int maxWidth = 0;
            int maxHeight = 0;

            foreach (TourneyMatchup tm in upcomingMatchups.Concat(resolvedMatchups))
            {
                Label l = tm.GetLabel();

                if (l.ClientSize.Height > maxHeight)
                {
                    maxHeight = l.Size.Height;
                }

                if (l.ClientSize.Width > maxWidth)
                {
                    maxWidth = l.Size.Width;
                }

                if (tlp.ColumnCount <= tm.cellPosition[0])
                {
                    tlp.ColumnCount = tm.cellPosition[0] + 1;
                }

                if (tlp.RowCount <= tm.cellPosition[1])
                {
                    tlp.RowCount = tm.cellPosition[1] + 1;
                }

                tlp.Controls.Add(l, tm.cellPosition[0], tm.cellPosition[1]);
            }

            maxWidth += 20;
            maxHeight += 20;

            foreach (Control c in tlp.Controls)
            {
                if (c is Label)
                {
                    c.Size = new Size(maxWidth, maxHeight);
                }
            }

            for (int i = 0; i <= tlp.ColumnCount; i++)
            {
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, maxWidth));
            }

            for (int i = 0; i <= tlp.RowCount; i++)
            {
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, maxHeight));
            }

            tlp.Size = new Size(tlp.ColumnCount * maxWidth, tlp.RowCount * maxHeight);

            tlp.ResumeLayout();
            return tlp;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new tournament that spans multiple days and creates all matches in that tournament.
        /// </summary>
        /// <param name="name">The name of the tournament.</param>
        /// <param name="ID">The ID of the tournament.</param>
        /// <param name="teams">The teams in the tournament.</param>
        /// <param name="heroes">The allowed heroes in the tournament.</param>
        /// <param name="numberOfDays">The number of days this tournament takes place over.</param>
        public Tournament(string name, int ID, List<Team> teams, Dictionary<int, Hero> heroes, int numberOfDays)
        {
            _name = name;
            _ID = ID;
            includedTeams = teams;
            upcomingMatchups = new List<TourneyMatchup>();
            resolvedMatchups = new List<TourneyMatchup>();
            allowedHeroes = heroes;
            totalDays = numberOfDays;
            SetupMatches(numberOfDays);
        }

        /// <summary>
        /// Creates a new tournament and creates all matches in that tournament.
        /// </summary>
        /// <param name="name">The name of the tournament.</param>
        /// <param name="ID">The ID of the tournament.</param>
        /// <param name="teams">The teams in the tournament.</param>
        /// <param name="heroes">The allowed heroes in the tournament.</param>
        public Tournament(string name, int ID, List<Team> teams, Dictionary<int, Hero> heroes)
        {
            _name = name;
            _ID = ID;
            includedTeams = teams;
            upcomingMatchups = new List<TourneyMatchup>();
            resolvedMatchups = new List<TourneyMatchup>();
            allowedHeroes = heroes;
            SetupMatches();
        }
        #endregion
    }
}
