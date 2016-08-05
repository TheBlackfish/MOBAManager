using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using MOBAManager.MatchResolution;
using MOBAManager.Management.Statistics;
using MOBAManager.Utility;
using MOBAManager.Resolution.BootcampResolution;
using MOBAManager.Management.Calendar;
using MOBAManager.UI.DailyResolution;
using MOBAManager.Management.Tournaments;

namespace MOBAManager.UI
{
    sealed public partial class EventResolutionControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// The position new labels should take when placed in the display.
        /// </summary>
        private Point newLabelPosition;

        /// <summary>
        /// The size labels should have when placed on the panel.
        /// </summary>
        private readonly Size labelSize;

        /// <summary>
        /// The control variable for if the player is currently playing a match.
        /// </summary>
        private readonly bool resolvingPlayerMatch = false;

        /// <summary>
        /// The list of all games that can be played in any order.
        /// </summary>
        private readonly List<Match> pugs;

        /// <summary>
        /// The list of all bootcamps that can be resolved in any order.
        /// </summary>
        private readonly List<BootcampSession> bootcamps;

        /// <summary>
        /// The list of tournaments to resolve today.
        /// </summary>
        private readonly List<Tournament> tournaments;

        /// <summary>
        /// The list of stat bundles being gathered up for all of the day's matches.
        /// </summary>
        private readonly List<StatsBundle> statistics;

        /// <summary>
        /// The timer that controls when events are resolved.
        /// </summary>
        private readonly System.Timers.Timer resolutionTimer;

        /// <summary>
        /// The control variable for if all of the events for this day are resolved yet or not.
        /// </summary>
        private bool allEventsResolved = false;

        /// <summary>
        /// The current player match to be resolved.
        /// </summary>
        private Match currentPlayerMatch = null;

        /// <summary>
        /// The current player bootcamp to be resolved.
        /// </summary>
        private BootcampSession currentPlayerBootcamp = null;

        /// <summary>
        /// The control variable for if the player has a match to resolve.
        /// </summary>
        private bool waitingToStartPlayerEvent = false;

        /// <summary>
        /// The function that is called when the user clicks to close this control.
        /// </summary>
        private readonly Action onCloseFunc = null;
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the day's statistics.
        /// </summary>
        /// <returns></returns>
        public List<StatsBundle> GetStatistics()
        {
            return statistics;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Resolves a random event from all of the lists of available events.
        /// </summary>
        private void ResolveRandomEvent(object sender, EventArgs e)
        {
            if (!resolvingPlayerMatch && (pugs.Count > 0 || bootcamps.Count > 0 || tournaments.Count > 0))
            {
                resolutionTimer.Enabled = false;

                List<EventType> possibleTypes = new List<EventType>();

                if (bootcamps.Count > 0)
                {
                    possibleTypes.Add(EventType.Bootcamp);
                }

                if (pugs.Count > 0)
                {
                    possibleTypes.Add(EventType.PUG);
                }

                if (tournaments.Count > 0)
                {
                    possibleTypes.Add(EventType.TournamentMatch);
                }

                EventType chosen = EventType.Null;
                if (possibleTypes.Count == 1)
                {
                    chosen = possibleTypes[0];
                }
                else
                {
                    chosen = possibleTypes[RNG.Roll(possibleTypes.Count)];
                }

                switch (chosen)
                {
                    case EventType.PUG:
                        resolvePickupGame();
                        break;
                    case EventType.Bootcamp:
                        resolveBootcamp();
                        break;
                    case EventType.TournamentMatch:
                        resolveTournamentMatch();
                        break;
                }
            }
            else
            {
                //Display "Click anywhere..." message.
                AddEventNotification("Click anywhere to continue...");
                resolutionTimer.Enabled = false;
                allEventsResolved = true;
            }
        }

        /// <summary>
        /// Adds a text-only label to the event panel.
        /// </summary>
        /// <param name="s"></param>
        private void AddEventNotification(string s)
        {
            Label l = new Label();
            l.Text = s;
            l.Location = new Point(newLabelPosition.X, newLabelPosition.Y + eventContainer.AutoScrollPosition.Y);
            l.Size = labelSize;
            newLabelPosition.Y += l.Height + 4;
            Action action = () => eventContainer.Controls.Add(l);
            BeginInvoke(action);
        }

        #region Bootcamp
        /// <summary>
        /// Resolves a random bootcamp.
        /// </summary>
        private void resolveBootcamp()
        {
            int curIndex = RNG.Roll(bootcamps.Count);
            BootcampSession cur = bootcamps[curIndex];
            bootcamps.RemoveAt(curIndex);

            if (cur.isPlayerControlled)
            {
                currentPlayerBootcamp = cur;
                waitingToStartPlayerEvent = true;
                resolutionTimer.Enabled = false;
                AddEventNotification("Click to begin bootcamp.");
            }
            else
            {
                cur.InstantlyResolve();
                AddEventNotification(cur.GetSummary());
                resolutionTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Creates a new bootcamp resolution control and displays it.
        /// </summary>
        private void StartCurrentPlayerBootcamp()
        {
            BootcampResolutionControl brc = new BootcampResolutionControl(currentPlayerBootcamp, onPlayerBootcampResolved);
            Action addition = () =>
            {
                Controls.Add(brc);
                brc.BringToFront();
            };
            BeginInvoke(addition);
        }

        /// <summary>
        /// Closes the bootcamp resolution control and continues with the normal resolution process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onPlayerBootcampResolved(object sender, EventArgs e)
        {
            foreach (Control o in Controls)
            {
                if (o is BootcampResolutionControl)
                {
                    Controls.Remove(o);
                }
            }

            AddEventNotification(currentPlayerBootcamp.GetSummary());
            resolutionTimer.Enabled = true;
            currentPlayerBootcamp = null;
            waitingToStartPlayerEvent = false;
        }
        #endregion

        #region PUGs
        /// <summary>
        /// Resolves a random pick-up game.
        /// </summary>
        private void resolvePickupGame()
        {
            //Choose a PUG to resolve.
            int curIndex = RNG.Roll(pugs.Count);
            Match cur = pugs[curIndex];
            pugs.RemoveAt(curIndex);

            //Either create a new Match resolution control for the player match, or instantly resolve it.
            if (cur.IsThreaded)
            {
                currentPlayerMatch = cur;
                waitingToStartPlayerEvent = true;
                resolutionTimer.Enabled = false;
                AddEventNotification("Click to begin the match against " + cur.GetAITeamName());
            }
            else
            {
                cur.InstantlyResolve();
                statistics.Add(cur.GetStats());
                cur.ResolveMatchEffects();
                AddEventNotification(cur.GetMatchSummary());
                resolutionTimer.Enabled = true;
            }
        }
        
        /// <summary>
        /// Adds the current player match to the event resolution.
        /// </summary>
        private void StartCurrentPlayerMatch()
        {
            MatchResolutionControl pm = new MatchResolutionControl(currentPlayerMatch, OnPlayerMatchResolved);
            Action addition = () =>
            {
                Controls.Add(pm);
                pm.BringToFront();
            };
            BeginInvoke(addition);
        }

        /// <summary>
        /// Removes all player interactions and records the results from any matches the player just finished.
        /// </summary>
        private void OnPlayerMatchResolved(object sender, EventArgs e)
        {
            Match completed = null;
            foreach (Control o in Controls)
            {
                if (o is MatchResolutionControl || o is MatchResultsControl)
                {
                    Controls.Remove(o);
                    if (o is MatchResultsControl)
                    {
                        completed = (o as MatchResultsControl).GetMatch();
                    }
                    else if (o is MatchResolutionControl)
                    {
                        completed = (o as MatchResolutionControl).GetMatch();
                    }
                }
            }
            if (completed != null)
            {
                AddEventNotification(completed.GetMatchSummary());
                completed.ResolveMatchEffects();
                statistics.Add(completed.GetStats());
            }
            resolutionTimer.Enabled = true;
            currentPlayerMatch = null;
            waitingToStartPlayerEvent = false;
        }

        /// <summary>
        /// Takes a list of pick-up games and creates 2-5 matches between the participants of those matches.
        /// </summary>
        /// <param name="pickupMatches">The list to extrapolate.</param>
        private static List<Match> RandomizePickupGames(List<Match> pickupMatches)
        {
            List<Match> ret = new List<Match>();

            foreach (Match m in pickupMatches)
            {
                int max = RNG.Roll(4) + 2;
                for (int i = 0; i < max; i++)
                {
                    ret.Add(m.Clone((i % 2 == 0)));
                }
            }

            return ret;
        }
        #endregion

        #region Tournament Matches
        /// <summary>
        /// Resolves a tournament match. If the match is player controlled it is queued to resolve upon player click, otherwise it is resolved immediately.
        /// </summary>
        private void resolveTournamentMatch()
        {
            Match m = tournaments[0].getMatch();
            if (m != null)
            {
                if (m.IsThreaded)
                {
                    currentPlayerMatch = m;
                    waitingToStartPlayerEvent = true;
                    resolutionTimer.Enabled = false;
                    AddEventNotification("Click to begin the tournament match against " + m.GetAITeamName());
                }
                else
                {
                    m.InstantlyResolve();
                    statistics.Add(m.GetStats());
                    m.ResolveMatchEffects();
                    AddEventNotification(m.GetMatchSummary());
                    resolutionTimer.Enabled = true;
                }
            }
            else
            {
                AddEventNotification(tournaments[0].GetSummary());
                tournaments.RemoveAt(0);
                if (tournaments.Count > 0)
                {
                    resolveTournamentMatch();
                }
                resolutionTimer.Enabled = true;
            }
        }
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new EventResolutionControl.
        /// </summary>
        /// <param name="pickupGames">The list of all games in which order of resolution does not matter.</param>
        public EventResolutionControl(string title, List<Match> pickupGames, List<BootcampSession> bootcamps, List<Tournament> tournaments, Action onClose)
        {
            InitializeComponent();

            titleText.Text = title;
            newLabelPosition = new Point(4, 4);
            labelSize = new Size(eventContainer.Width - 16, 16);
            onCloseFunc = onClose;
            pugs = RandomizePickupGames(pickupGames);
            this.bootcamps = bootcamps;
            this.tournaments = tournaments;
            resolutionTimer = new System.Timers.Timer(4000);
            resolutionTimer.Enabled = true;
            resolutionTimer.Elapsed += ResolveRandomEvent;
            statistics = new List<StatsBundle>();
        }
        #endregion

        #region Event responses
        /// <summary>
        /// Called when this control's parent is changed. Resizes the control to fit its parent.
        /// </summary>
        private void EventResolutionControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.ClientSize;
            }
        }

        /// <summary>
        /// Called when this control is clicked. Calls the close function if any exists and all events for the day are complete..
        /// </summary>
        private void EventResolutionControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (allEventsResolved && onCloseFunc != null)
            {
                onCloseFunc?.Invoke();
            }
            else if (waitingToStartPlayerEvent)
            {
                if (currentPlayerBootcamp != null)
                {
                    StartCurrentPlayerBootcamp();
                }
                else if (currentPlayerMatch != null)
                {
                    StartCurrentPlayerMatch();
                }
            }
        }
        #endregion
    }
}