using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MOBAManager.MatchResolution
{
    partial class MatchAI
    {
        #region Private Variables
        /// <summary>
        /// The maximum amount of time allowed during each pick/ban.
        /// </summary>
        private readonly double regularTimeMaximum = 30000;

        /// <summary>
        /// The amount of bonus time each team has overall.
        /// </summary>
        private readonly double bonusTimeMaximum = 90000;

        /// <summary>
        /// The counter for team 1's selections.
        /// </summary>
        private double team1RegularTimeCounter = 0;

        /// <summary>
        /// The counter for team 1's bonus time.
        /// </summary>
        private double team1BonusTimeCounter = 0;

        /// <summary>
        /// The counter for team 2's selections.
        /// </summary>
        private double team2RegularTimeCounter = 0;

        /// <summary>
        /// The counter for team 2's bonus time.
        /// </summary>
        private double team2BonusTimeCounter = 0;

        /// <summary>
        /// The current delay target in selection by a team.
        /// </summary>
        private double currentChoiceDelayMaximum = 0;

        /// <summary>
        /// The counter for the current delay.
        /// </summary>
        private double currentChoiceDelayCounter = 0;

        /// <summary>
        /// The Hero ID that the current team is eyeing.
        /// </summary>
        private int currentChoiceID = -1;

        /// <summary>
        /// The Hero ID that was randomly selected in case the team squanders all of their bonus time.
        /// </summary>
        private int currentChoiceRandomID = -1;

        /// <summary>
        /// The timer controlling when updates are handled.
        /// </summary>
        private System.Timers.Timer tickTimer = null;

        /// <summary>
        /// The timer controlling when the initial timer will start.
        /// </summary>
        private readonly System.Timers.Timer pregameTimer = null;
        #endregion

        #region Public Variables
        /// <summary>
        /// Returns if the AI is a part of a threaded match.
        /// </summary>
        public bool isThreaded
        {
            get
            {
                return (pregameTimer != null);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the timer and essentially starts the match.
        /// </summary>
        private void InitActualTimer(Object sender, EventArgs e)
        {
            pregameTimer.Enabled = false;

            tickTimer = new System.Timers.Timer(250);
            tickTimer.Enabled = true;
            tickTimer.Elapsed += OnTickTimerElapsed;
            tickTimer.AutoReset = true;
            SetCurrentSelectionDelay();
            InitSelectionThread();
        }

        /// <summary>
        /// Updates all of the appropriate counters and calls the appropriate threading methods if necessary.
        /// </summary>
        private void OnTickTimerElapsed(Object src, ElapsedEventArgs e)
        {
            bool useRandomPick = false;

            //Update timers
            if (match.GetCurrentActingTeam == 1)
            {
                if (team1RegularTimeCounter >= regularTimeMaximum)
                {
                    team1BonusTimeCounter += tickTimer.Interval;
                    if (team1BonusTimeCounter >= bonusTimeMaximum)
                    {
                        useRandomPick = true;
                    }
                }
                else
                {
                    team1RegularTimeCounter += tickTimer.Interval;
                }
            }
            else
            {
                if (team2RegularTimeCounter >= regularTimeMaximum)
                {
                    team2BonusTimeCounter += tickTimer.Interval;
                    if (team2BonusTimeCounter >= bonusTimeMaximum)
                    {
                        useRandomPick = true;
                    }
                }
                else
                {
                    team2RegularTimeCounter += tickTimer.Interval;
                }
            }

            if (useRandomPick)
            {
                //Resolve random selection.
                SetTeamSelection(match.GetCurrentActingTeam, currentChoiceRandomID, match.IsCurrentPhasePicking);
                match.AdvancePhase();
                if (match.ShouldContinue())
                {
                    if (match.GetCurrentActingTeam == 1)
                    {
                        team1RegularTimeCounter = 0;
                    }
                    else
                    {
                        team2RegularTimeCounter = 0;
                    }
                    InitSelectionThread();
                }
                else
                {
                    tickTimer.Enabled = false;
                }
            }
            else
            {
                //Update selection delays
                currentChoiceDelayCounter += tickTimer.Interval;
                if (currentChoiceDelayCounter >= currentChoiceDelayMaximum)
                {
                    //Resolve selections made previously.
                    if (currentChoiceID != -1)
                    {
                        SetTeamSelection(match.GetCurrentActingTeam, currentChoiceID, match.IsCurrentPhasePicking);
                        SetCurrentSelectionDelay();
                        match.AdvancePhase();
                        if (match.ShouldContinue())
                        {
                            if (match.GetCurrentActingTeam == 1)
                            {
                                team1RegularTimeCounter = 0;
                            }
                            else
                            {
                                team2RegularTimeCounter = 0;
                            }
                            InitSelectionThread();
                        }
                        else
                        {
                            tickTimer.Enabled = false;
                        }
                    }
                    else
                    {
                        SetCurrentSelectionDelay(false);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the current selection delay, clearing the previous value.
        /// </summary>
        private void SetCurrentSelectionDelay()
        {
            SetCurrentSelectionDelay(true);
        }

        /// <summary>
        /// Sets the current selection delay.
        /// </summary>
        /// <param name="reset">If true, this will clear any previous value the current selection delay had.</param>
        private void SetCurrentSelectionDelay(bool reset)
        {
            double time = Math.Truncate(RNG.RollDouble(5000));

            if (RNG.Roll(2) == 1)
            {
                time += 8000;
            }
            else if (RNG.Roll(3) == 1)
            {
                time += 25000;
            }
            else
            {
                time += 1000;
            }

            if (reset)
            {
                currentChoiceDelayMaximum = 0;
                currentChoiceDelayCounter = 0;

                int phase = match.GetCurrentPhase;
                int iterations = 0;
                if (phase == 3)
                {
                    iterations = 3;
                }
                else if (phase == 1 || phase == 2 || phase == 4)
                {
                    iterations = 2;
                }

                for (int i = 0; i < iterations; i++)
                {
                    if (RNG.Roll(3) != 1)
                    {
                        double addOn = Math.Truncate(Math.Sqrt(RNG.RollDouble(25000)));
                    }
                }
            }

            if (reset)
            {
                currentChoiceDelayMaximum += time;
            }
            else
            {
                currentChoiceDelayMaximum = currentChoiceDelayCounter + time;
            }
            Console.WriteLine("Current delay is " + currentChoiceDelayMaximum);
        }

        /// <summary>
        /// Delegates the hero selection process to a new thread.
        /// </summary>
        private void InitSelectionThread()
        {
            Task.Run(() => ThreadedSelectHeroes());
        }

        /// <summary>
        /// Sets the current selection IDs to whatever each team thinks is appropriate.
        /// </summary>
        private void ThreadedSelectHeroes()
        {
            Dictionary<int, int> baseline = BaselineHeroSelection(1, true, true);
            List<int> finalists = baseline.Select(kvp => kvp.Key).ToList();
            currentChoiceRandomID = finalists[RNG.Roll(finalists.Count)];

            if (match.GetCurrentActingTeam == 1)
            {
                if (match.IsCurrentPhasePicking)
                {
                    currentChoiceID = Team1Pick();
                }
                else
                {
                    currentChoiceID = Team2Pick();
                }
            }
            else
            {
                if (match.IsCurrentPhasePicking)
                {
                    currentChoiceID = Team2Pick();
                }
                else
                {
                    currentChoiceID = Team1Pick();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the pre-game timer.
        /// </summary>
        public void InitInitialTimer()
        {
            pregameTimer.Enabled = true;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Match AI with threading.
        /// </summary>
        /// <param name="threading">Set this to true to enable threading functionality.</param>
        /// <param name="match">The match this AI controls.</param>
        /// <param name="dict">The dictionary of all heroes.</param>
        /// <param name="team1">The list of players from Team 1.</param>
        /// <param name="team2">The list of players from Team 2.</param>
        public MatchAI(bool threading, Match match, Dictionary<int, Hero> dict, List<Player> team1, List<Player> team2)
            : this(match, dict, team1, team2)
        {
            if (threading)
            {
                pregameTimer = new System.Timers.Timer(5000);
                pregameTimer.Enabled = false;
                pregameTimer.Elapsed += InitActualTimer;
                pregameTimer.AutoReset = true;
            }
        }
        #endregion
    }
}
