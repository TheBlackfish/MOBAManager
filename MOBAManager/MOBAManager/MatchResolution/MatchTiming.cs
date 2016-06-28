using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace MOBAManager.MatchResolution
{
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
        private System.Timers.Timer tickTimer;
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the timer and essentially starts the match.
        /// </summary>
        private void initTimer()
        {
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
            double time = Math.Truncate(rnd.NextDouble() * 5000);

            if (rnd.Next(2) == 1)
            {
                time += 8000;
            }
            else if (rnd.Next(3) == 1)
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
                    if (rnd.Next(3) != 1)
                    {
                        double addOn = Math.Truncate(Math.Sqrt(rnd.NextDouble() * 25000));
                    }
                }
            }

            currentChoiceDelayMaximum += time;
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
            currentChoiceRandomID = selectRandomly();
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
                initTimer();
            }
        }
        #endregion
    }
}