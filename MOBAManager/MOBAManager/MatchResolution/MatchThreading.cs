using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly int playerTeam = -1;

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
        public bool HasChanged()
        {
            if (_changed)
            {
                _changed = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns if the game has finished.
        /// </summary>
        public bool HasFinished
        {
            get
            {
                return _finished;
            }
        }

        /// <summary>
        /// Returns if the player team has just acted.
        /// </summary>
        public bool PlayerTeamActed()
        {
            if (_playerTeamActed)
            {
                _playerTeamActed = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Advances the current phase of the match.
        /// </summary>
        public void AdvancePhase()
        {
            if (GetCurrentActingTeam == playerTeam)
            {
                _playerTeamActed = true;
            }
            deferredPhase++;
            if (deferredPhase == 20)
            {
                Task.Run(() => ThreadedResolveWinner());
            }
            _changed = true;
        }

        /// <summary>
        /// If a match is in the resolution phase, this returns false.
        /// </summary>
        /// <returns></returns>
        public bool ShouldContinue()
        {
            return (deferredPhase < 20);
        }

        /// <summary>
        /// If a match is currently in a picking or banning phase, this returns true.
        /// </summary>
        public bool IsCurrentlyActing
        {
            get
            {
                return (deferredPhase != -1);
            }
        }

        /// <summary>
        /// Returns either 1 or 2, depending on which team should currently be acting.
        /// </summary>
        public int GetCurrentActingTeam
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
        public int GetCurrentPhase
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
        public bool IsCurrentPhasePicking
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
        public void ThreadedResolveWinner()
        {
            _winner = ms.DecideWinner();
            Thread.Sleep(5000);
            _finished = true;
        }

        /// <summary>
        /// Starts the AI's internal timers.
        /// </summary>
        public void StartMatch()
        {
            if (IsThreaded)
            {
                ms.InitInitialTimer();
            }
        }

        /// <summary>
        /// Returns if the game is threaded and therefore a player match.
        /// </summary>
        public bool IsThreaded
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
                ms = new MatchAI(true, this, allHeroes, teamA.GetTeammates(), teamB.GetTeammates());
            }
        }
        #endregion
    }
}