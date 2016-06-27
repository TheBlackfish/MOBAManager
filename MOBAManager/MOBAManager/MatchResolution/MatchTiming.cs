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
        private double regularTimeMaximum = 30000;
        private double bonusTimeMaximum = 90000;

        private double team1RegularTimeCounter = 0;
        private double team1BonusTimeCounter = 0;
        private double team2RegularTimeCounter = 0;
        private double team2BonusTimeCounter = 0;

        private double currentChoiceDelayMaximum = 0;
        private double currentChoiceDelayCounter = 0;

        private int currentChoiceID = -1;
        private int currentChoiceRandomID = -1;

        private System.Timers.Timer tickTimer;

        private void initTimer()
        {
            tickTimer = new System.Timers.Timer(250);
            tickTimer.Enabled = true;
            tickTimer.Elapsed += OnTickTimerElapsed;
            tickTimer.AutoReset = true;
        }

        private void OnTickTimerElapsed(Object src, ElapsedEventArgs e)
        {
            Console.WriteLine("MatchAI Timer Tick!");
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
                initSelectionThread();
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
                        initSelectionThread();
                    }
                    else
                    {
                        setCurrentSelectionDelay(false);
                    }
                }
            }
        }

        private void setCurrentSelectionDelay()
        {
            setCurrentSelectionDelay(true);
        }

        private void setCurrentSelectionDelay(bool reset)
        {
            if (reset)
            {
                currentChoiceDelayMaximum = 0;
                currentChoiceDelayCounter = 0;
            }

            int iterations = rnd.Next(3);
            if (reset)
            {
                iterations += rnd.Next(6) - 1;
            }

            double time = 0.0;
            for (int i = 0; i < iterations; i++)
            {
                time += Math.Truncate(rnd.NextDouble() * 6);
            }

            currentChoiceDelayMaximum += (time * 1000);
        }

        private void initSelectionThread()
        {
            Thread t = new Thread(threadedSelectHeroes);
            t.Start();
        }

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

        public MatchAI(bool threading, Match match, Dictionary<int, Hero> dict, List<Player> team1, List<Player> team2)
            : this(match, dict, team1, team2)
        {
            if (threading)
            {
                initTimer();
            }
        }
    }
}