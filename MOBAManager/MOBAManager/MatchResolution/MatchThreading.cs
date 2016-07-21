using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace MOBAManager.MatchResolution
{
    public partial class Match
    {
        #region Private Variables
        /// <summary>
        /// The current phase of the deferred game.
        /// The numbers correspond to the following:
        /// <para>-1 - Pregame</para>
        /// <para>0 - Team 1 Ban</para>
        /// <para>1 - Team 2 Ban</para>
        /// <para>2 - Team 1 Ban</para>
        /// <para>3 - Team 2 Ban</para>
        /// <para>4 - Team 1 Pick</para>
        /// <para>5 - Team 2 Pick</para>
        /// <para>6 - Team 2 Pick</para>
        /// <para>7 - Team 1 Pick</para>
        /// <para>8 - Team 2 Ban</para>
        /// <para>9 - Team 1 Ban</para>
        /// <para>10 - Team 2 Ban</para>
        /// <para>11 - Team 1 Ban</para>
        /// <para>12 - Team 2 Pick</para>
        /// <para>13 - Team 1 Pick</para>
        /// <para>14 - Team 2 Pick</para>
        /// <para>15 - Team 1 Pick</para>
        /// <para>16 - Team 2 Ban</para>
        /// <para>17 - Team 1 Ban</para>
        /// <para>18 - Team 1 Pick</para>
        /// <para>19 - Team 2 Pick</para>
        /// <para>20 - Resolution</para>
        /// </summary>
        private int deferredPhase = 0; //TODO Revert to -1.

        /// <summary>
        /// The team that the player is controlling. -1 is the default.
        /// </summary>
        private int playerTeam = -1;

        /// <summary>
        /// The control variable for if the match has changed in anyway and requires updating the user control.
        /// </summary>
        private bool _changed = true;

        /// <summary>
        /// The control variable for if the match has changed due to the player's team acting.
        /// </summary>
        private bool _playerTeamActed = false;

        /// <summary>
        /// The control variable for if the match is finished.
        /// </summary>
        private bool _finished = false;
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns if the game has changed, but only once. If true, this will change to false immediately afterwards.
        /// </summary>
        public bool hasChanged
        {
            get
            {
                if (_changed)
                {
                    _changed = false;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns if the game has finished.
        /// </summary>
        public bool hasFinished
        {
            get
            {
                return _finished;
            }
        }

        /// <summary>
        /// Returns if the player team has just acted.
        /// </summary>
        public bool playerTeamActed
        {
            get
            {
                if (_playerTeamActed)
                {
                    _playerTeamActed = false;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Advances the current phase of the match.
        /// </summary>
        public void advancePhase()
        {
            if (getCurrentActingTeam == playerTeam)
            {
                _playerTeamActed = true;
            }
            deferredPhase++;
            if (deferredPhase == 20)
            {
                Thread t = new Thread(threadedResolveWinner);
                t.Start();
            }
            _changed = true;
        }

        /// <summary>
        /// If a match is in the resolution phase, this returns false.
        /// </summary>
        /// <returns></returns>
        public bool shouldContinue()
        {
            return (deferredPhase < 20);
        }

        /// <summary>
        /// If a match is currently in a picking or banning phase, this returns true.
        /// </summary>
        public bool isCurrentlyActing
        {
            get
            {
                return (deferredPhase != -1);
            }
        }

        /// <summary>
        /// Returns either 1 or 2, depending on which team should currently be acting.
        /// </summary>
        public int getCurrentActingTeam
        {
            get
            {
                int[] team1Phases = new int[] { 0, 2, 4, 7, 9, 11, 13, 15, 17, 18 };
                foreach (int n in team1Phases)
                {
                    if (deferredPhase == n)
                    {
                        return 1;
                    }
                }
                return 2;
            }
        }

        /// <summary>
        /// <para>Returns the current phase of the ban/pick phase.</para>
        /// <para>0 - First Ban Phase</para>
        /// <para>1 - First Pick Phase</para>
        /// <para>2 - Second Ban Phase</para>
        /// <para>3 - Second Pick Phase</para>
        /// <para>4 - Third Ban Phase</para>
        /// <para>5 - Third Pick Phase</para>
        /// </summary>
        public int getCurrentPhase
        {
            get
            {
                if (deferredPhase >= 18)
                {
                    return 5;
                }
                else if (deferredPhase >= 16)
                {
                    return 4;
                }
                else if (deferredPhase >= 12)
                {
                    return 3;
                }
                else if (deferredPhase >= 8)
                {
                    return 2;
                }
                else if (deferredPhase >= 4)
                {
                    return 1;
                }
                else if (deferredPhase >= 0)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Returns true if the current phase is a pick phase. Returns false if it is a ban phase.
        /// </summary>
        public bool isCurrentPhasePicking
        {
            get
            {
                int[] pickPhases = new int[] { 4, 5, 6, 7, 12, 13, 14, 15, 18, 19 };
                foreach (int n in pickPhases)
                {
                    if (deferredPhase == n)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Decides and reports the winner.
        /// </summary>
        public void threadedResolveWinner()
        {
            _winner = ms.decideWinner();
            Thread.Sleep(5000);
            _finished = true;
        }

        /// <summary>
        /// Starts the AI's internal timers.
        /// </summary>
        public void startMatch()
        {
            if (isThreaded)
            {
                ms.initInitialTimer();
            }
        }

        /// <summary>
        /// Returns if the game is threaded and therefore a player match.
        /// </summary>
        public bool isThreaded
        {
            get { return ms.isThreaded; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new match that utilizes threading. This should be the go-to overload for any match involving a player team.
        /// </summary>
        /// <param name="threading">Set to true to actually use all threading functionality.</param>
        /// <param name="teamA">The team to take slot 1.</param>
        /// <param name="teamB">The team to take slot 2.</param>
        /// <param name="whichTeamIsPlayer">The team the player is controlling. 1 or 2 only.</param>
        /// <param name="allHeroes">The Dictionary containing all heroes.</param>
        public Match(bool threading, Team teamA, Team teamB, int whichTeamIsPlayer, Dictionary<int, Hero> allHeroes)
            : this(teamA, teamB, allHeroes)
        {
            if (threading)
            {
                playerTeam = whichTeamIsPlayer;
                ms = new MatchAI(true, this, allHeroes, teamA.getTeammates(), teamB.getTeammates());
            }
        }
        #endregion
    }

    partial class MatchAI
    {
        #region Private Variables
        /// <summary>
        /// The maximum amount of time allowed during each pick/ban.
        /// </summary>
        private double regularTimeMaximum = 30000;

        /// <summary>
        /// The amount of bonus time each team has overall.
        /// </summary>
        private double bonusTimeMaximum = 90000;

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
        private System.Timers.Timer pregameTimer = null;
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
        private void initActualTimer(Object sender, EventArgs e)
        {
            pregameTimer.Enabled = false;

            tickTimer = new System.Timers.Timer(250);
            tickTimer.Enabled = true;
            tickTimer.Elapsed += OnTickTimerElapsed;
            tickTimer.AutoReset = true;
            setCurrentSelectionDelay();
            initSelectionThread();
        }

        /// <summary>
        /// Updates all of the appropriate counters and calls the appropriate threading methods if necessary.
        /// </summary>
        private void OnTickTimerElapsed(Object src, ElapsedEventArgs e)
        {
            bool useRandomPick = false;

            //Update timers
            if (match.getCurrentActingTeam == 1)
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
                setTeamSelection(match.getCurrentActingTeam, currentChoiceRandomID, match.isCurrentPhasePicking);
                match.advancePhase();
                if (match.shouldContinue())
                {
                    if (match.getCurrentActingTeam == 1)
                    {
                        team1RegularTimeCounter = 0;
                    }
                    else
                    {
                        team2RegularTimeCounter = 0;
                    }
                    initSelectionThread();
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
                        setTeamSelection(match.getCurrentActingTeam, currentChoiceID, match.isCurrentPhasePicking);
                        setCurrentSelectionDelay();
                        match.advancePhase();
                        if (match.shouldContinue())
                        {
                            if (match.getCurrentActingTeam == 1)
                            {
                                team1RegularTimeCounter = 0;
                            }
                            else
                            {
                                team2RegularTimeCounter = 0;
                            }
                            initSelectionThread();
                        }
                        else
                        {
                            tickTimer.Enabled = false;
                        }
                    }
                    else
                    {
                        setCurrentSelectionDelay(false);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the current selection delay, clearing the previous value.
        /// </summary>
        private void setCurrentSelectionDelay()
        {
            setCurrentSelectionDelay(true);
        }

        /// <summary>
        /// Sets the current selection delay.
        /// </summary>
        /// <param name="reset">If true, this will clear any previous value the current selection delay had.</param>
        private void setCurrentSelectionDelay(bool reset)
        {
            double time = Math.Truncate(RNG.rollDouble(5000));

            if (RNG.roll(2) == 1)
            {
                time += 8000;
            }
            else if (RNG.roll(3) == 1)
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

                int phase = match.getCurrentPhase;
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
                    if (RNG.roll(3) != 1)
                    {
                        double addOn = Math.Truncate(Math.Sqrt(RNG.rollDouble(25000)));
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
        private void initSelectionThread()
        {
            Thread t = new Thread(threadedSelectHeroes);
            t.Start();
        }

        /// <summary>
        /// Sets the current selection IDs to whatever each team thinks is appropriate.
        /// </summary>
        private void threadedSelectHeroes()
        {
            Dictionary<int, int> baseline = baselineHeroSelection(1, true, true);
            List<int> finalists = baseline.Select(kvp => kvp.Key).ToList();
            currentChoiceRandomID = finalists[RNG.roll(finalists.Count)];

            if (match.getCurrentActingTeam == 1)
            {
                if (match.isCurrentPhasePicking)
                {
                    currentChoiceID = team1Pick();
                }
                else
                {
                    currentChoiceID = team2Pick();
                }
            }
            else
            {
                if (match.isCurrentPhasePicking)
                {
                    currentChoiceID = team2Pick();
                }
                else
                {
                    currentChoiceID = team1Pick();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the pre-game timer.
        /// </summary>
        public void initInitialTimer()
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
                pregameTimer.Elapsed += initActualTimer;
                pregameTimer.AutoReset = true;
            }
        }
        #endregion
    }
}