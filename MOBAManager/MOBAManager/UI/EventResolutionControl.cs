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

namespace MOBAManager.UI
{
    public partial class EventResolutionControl : UserControl
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        private Random rng;

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

        private List<StatsBundle> statistics;

        /// <summary>
        /// The timer that controls when events are resolved.
        /// </summary>
        private System.Timers.Timer resolutionTimer;

        /// <summary>
        /// Resolves a random event from all of the lists of available events.
        /// </summary>
        public void resolveRandomEvent(object sender, EventArgs e)
        {
            if (!resolvingPlayerMatch && pugs.Count > 0)
            {
                resolutionTimer.Enabled = false;

                //Choose a PUG to resolve.
                int curIndex = rng.Next(pugs.Count);
                Match cur = pugs[curIndex];
                pugs.RemoveAt(curIndex);

                //Either create a new Match resolution control for the player match, or instantly resolve it.
                if (cur.isThreaded)
                {
                    MatchResolutionControl pm = new MatchResolutionControl(cur, onPlayerMatchResolved);
                    Action addition = () =>
                    {
                        Controls.Add(pm);
                        pm.BringToFront();
                    };
                    BeginInvoke(addition);
                }
                else
                {
                    cur.instantlyResolve();
                    addEventNotification(cur);
                    resolutionTimer.Enabled = true;
                }
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
            }
            resolutionTimer.Enabled = true;
        }

        public List<StatsBundle> getStatistics()
        {
            return statistics;
        }

        /// <summary>
        /// Adds a label to the event container with details about the match provided.
        /// </summary>
        /// <param name="m">The match to report on.</param>
        private void addEventNotification(Match m)
        {
            Label l = new Label();
            l.Text = m.getSummary();
            l.Location = new Point(newLabelPosition.X, newLabelPosition.Y);
            l.Size = labelSize;
            newLabelPosition.Y += l.Height + 4;
            Action action = () => eventContainer.Controls.Add(l);
            BeginInvoke(action);
        }

        /// <summary>
        /// Creates a new EventResolutionControl.
        /// </summary>
        /// <param name="pickupGames">The list of all games in which order of resolution does not matter.</param>
        public EventResolutionControl(List<Match> pickupGames)
        {
            InitializeComponent();

            newLabelPosition = new Point(4, 4);
            labelSize = new Size(eventContainer.Width - 16, 16);
            pugs = pickupGames;
            rng = new Random();
            resolutionTimer = new System.Timers.Timer(4000);
            resolutionTimer.Enabled = true;
            resolutionTimer.Elapsed += resolveRandomEvent;
            statistics = new List<StatsBundle>();
        }
    }
}