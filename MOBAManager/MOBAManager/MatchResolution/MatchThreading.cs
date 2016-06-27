using System;

namespace MOBAManager.MatchResolution
{
    public partial class Match
    {

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
        private int deferredPhase = -1;

        private int playerTeam = -1;

        private bool _changed = true;

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

        public void advancePhase()
        {
            deferredPhase++;
            if (deferredPhase == 20)
            {
                ms.decideWinner();
            }
        }

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
    }
}