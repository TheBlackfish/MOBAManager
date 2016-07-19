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

namespace MOBAManager.UI
{
    public partial class EventResolutionControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// The position new labels should take when placed in the display.
        /// </summary>
        private Point newLabelPosition;

        /// <summary>
        /// The size labels should have when placed on the panel.
        /// </summary>
        private Size labelSize;

        /// <summary>
        /// The control variable for if the player is currently playing a match.
        /// </summary>
        private bool resolvingPlayerMatch = false;

        /// <summary>
        /// The list of all games that can be played in any order.
        /// </summary>
        private List<Match> pugs;

        /// <summary>
        /// The list of stat bundles being gathered up for all of the day's matches.
        /// </summary>
        private List<StatsBundle> statistics;

        /// <summary>
        /// The timer that controls when events are resolved.
        /// </summary>
        private System.Timers.Timer resolutionTimer;

        /// <summary>
        /// The control variable for if all of the events for this day are resolved yet or not.
        /// </summary>
        private bool allEventsResolved = false;

        /// <summary>
        /// The current player match to be resolved
        /// </summary>
        private Match currentPlayerMatch = null;

        /// <summary>
        /// The control variable for if the player has a match to resolve.
        /// </summary>
        private bool waitingToStartPlayerMatch = false;

        /// <summary>
        /// The function that is called when the user clicks to close this control.
        /// </summary>
        private Action onCloseFunc = null;
        #endregion

        #region Public methods
        /// <summary>
        /// Resolves a random event from all of the lists of available events.
        /// </summary>
        public void resolveRandomEvent(object sender, EventArgs e)
        {
            if (!resolvingPlayerMatch && pugs.Count > 0)
            {
                resolutionTimer.Enabled = false;

                //Choose a PUG to resolve.
                int curIndex = RNG.roll(pugs.Count);
                Match cur = pugs[curIndex];
                pugs.RemoveAt(curIndex);

                //Either create a new Match resolution control for the player match, or instantly resolve it.
                if (cur.isThreaded)
                {
                    currentPlayerMatch = cur;
                    waitingToStartPlayerMatch = true;
                    resolutionTimer.Enabled = false;
                    addEventNotification("Click to begin the match against " + cur.getAITeamName());
                }
                else
                {
                    cur.instantlyResolve();
                    statistics.Add(cur.getStats());
                    addEventNotification(cur);
                    resolutionTimer.Enabled = true;
                }
            }
            else
            {
                //Display "Click anywhere..." message.
                addEventNotification("Click anywhere to continue...");
                resolutionTimer.Enabled = false;
                allEventsResolved = true;
            }
        }
        
        /// <summary>
        /// Removes all player interactions and records the results from any matches the player just finished.
        /// </summary>
        public void onPlayerMatchResolved(object sender, EventArgs e)
        {
            Match completed = null;
            foreach(Control o in Controls)
            {
                if (o is MatchResolutionControl || o is MatchResultsControl)
                {
                    Controls.Remove(o);
                    if (o is MatchResultsControl)
                    {
                        completed = (o as MatchResultsControl).getMatch();
                    }
                    else if (o is MatchResolutionControl)
                    {
                        completed = (o as MatchResolutionControl).getMatch();
                    }
                }
            }
            if (completed != null)
            {
                addEventNotification(completed);
                statistics.Add(completed.getStats());
            }
            resolutionTimer.Enabled = true;
            currentPlayerMatch = null;
            waitingToStartPlayerMatch = false;
        }

        /// <summary>
        /// Returns the day's statistics.
        /// </summary>
        /// <returns></returns>
        public List<StatsBundle> getStatistics()
        {
            return statistics;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Adds a text-only label to the event panel.
        /// </summary>
        /// <param name="s"></param>
        private void addEventNotification(string s)
        {
            Label l = new Label();
            l.Text = s;
            l.Location = new Point(newLabelPosition.X, newLabelPosition.Y + eventContainer.AutoScrollPosition.Y);
            l.Size = labelSize;
            newLabelPosition.Y += l.Height + 4;
            Action action = () => eventContainer.Controls.Add(l);
            BeginInvoke(action);
        }

        /// <summary>
        /// Adds a label to the event container with details about the match provided.
        /// </summary>
        /// <param name="m">The match to report on.</param>
        private void addEventNotification(Match m)
        {
            Label l = new Label();
            l.Text = m.getSummary();
            l.Location = new Point(newLabelPosition.X, newLabelPosition.Y + eventContainer.AutoScrollPosition.Y);
            l.Size = labelSize;
            newLabelPosition.Y += l.Height + 4;
            Action action = () => eventContainer.Controls.Add(l);
            BeginInvoke(action);
        }
        
        /// <summary>
        /// Adds the current player match to the event resolution.
        /// </summary>
        private void startCurrentPlayerMatch()
        {
            MatchResolutionControl pm = new MatchResolutionControl(currentPlayerMatch, onPlayerMatchResolved);
            Action addition = () =>
            {
                Controls.Add(pm);
                pm.BringToFront();
            };
            BeginInvoke(addition);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new EventResolutionControl.
        /// </summary>
        /// <param name="pickupGames">The list of all games in which order of resolution does not matter.</param>
        public EventResolutionControl(string title, List<Match> pickupGames, Action onClose)
        {
            InitializeComponent();

            titleText.Text = title;
            newLabelPosition = new Point(4, 4);
            labelSize = new Size(eventContainer.Width - 16, 16);
            onCloseFunc = onClose;
            pugs = pickupGames;
            resolutionTimer = new System.Timers.Timer(4000);
            resolutionTimer.Enabled = true;
            resolutionTimer.Elapsed += resolveRandomEvent;
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
            else if (waitingToStartPlayerMatch && currentPlayerMatch != null)
            {
                startCurrentPlayerMatch();
            }
        }
        #endregion
    }
}