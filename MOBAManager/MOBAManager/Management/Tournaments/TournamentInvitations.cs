using MOBAManager.Management.Heroes;
using MOBAManager.Management.Teams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.Management.Tournaments
{
    public abstract partial class Tournament
    {
        #region Protected variables
        /// <summary>
        /// The control variable for if this tournament uses invite functions or not.
        /// </summary>
        protected bool usesInvites = false;

        /// <summary>
        /// The List of function delegates that serve as invitation functions.
        /// </summary>
        protected List<Func<int, List<Team>>> inviteFunctions;

        /// <summary>
        /// The list of integers corresponding to how many teams from each invite should be added to this tournament.
        /// </summary>
        protected List<int> inviteNumbers;

        /// <summary>
        /// The minimum number of teams for this tournament.
        /// </summary>
        protected int minNumberOfTeams = 0;
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the invitation function provided to this tournament as an auto-fill invite.
        /// </summary>
        /// <param name="inviteFunc">The invitation function</param>
        public void addInviteFunction(Func<int, List<Team>> inviteFunc)
        {
            addInviteFunction(inviteFunc, -1);
        }

        /// <summary>
        /// Adds the invitation function provided to this tournament with the specified number of invites.
        /// </summary>
        /// <param name="inviteFunc">The invitation function</param>
        /// <param name="num">The number of teams to invite, or -1 to have this function auto-fill instead.</param>
        public void addInviteFunction(Func<int, List<Team>> inviteFunc, int num)
        {
            inviteFunctions.Add(inviteFunc);
            inviteNumbers.Add(num);
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Resolves all invitation functions and adds all teams to this tournament.
        /// </summary>
        protected void ResolveInvitations()
        {
            resolveInvitations(0);
        }

        /// <summary>
        /// Resolves all invitation functions, adding the amount provided to the number of teams invited from each invitation.
        /// </summary>
        /// <param name="increasedAmount">The number of teams to add to each invitation's results.</param>
        protected void resolveInvitations(int increasedAmount)
        {
            List<Team> tempHolder = new List<Team>();

            if (minNumberOfTeams != 0 && inviteNumbers.Where(i => i == -1).Count() > 0)
            {
                int remainingTeamsNeeded = minNumberOfTeams - inviteNumbers.Where(i => i > 0).Sum();
                int countOfFillerInvites = inviteNumbers.Where(i => i == -1).Count();
                while (remainingTeamsNeeded % countOfFillerInvites != 0)
                {
                    remainingTeamsNeeded++;
                }
                for (int i = 0; i < inviteNumbers.Count; i++)
                {
                    if (inviteNumbers[i] == -1)
                    {
                        inviteNumbers[i] = remainingTeamsNeeded / countOfFillerInvites;
                    }
                }
            }
            
            for (int i = 0; i < inviteFunctions.Count; i++)
            {
                tempHolder = tempHolder.Concat(inviteFunctions[i](inviteNumbers[i] + increasedAmount)).Distinct().ToList();
            }

            if (tempHolder.Count >= minNumberOfTeams)
            {
                includedTeams = tempHolder;
            }
            else
            {
                resolveInvitations(increasedAmount + 1);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new tournament that will use invitations and spans multiple days.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="ID">The ID of the tournament</param>
        /// <param name="numberOfTeams">The minimum number of teams for this tournament.</param>
        /// <param name="heroes">The allowed heroes in this tournament.</param>
        /// <param name="numberOfDays">The number of days this tournament takes place over.</param>
        public Tournament(string name, int ID, int numberOfTeams, Dictionary<int, Hero> heroes, int numberOfDays)
        {
            _name = name;
            _ID = ID;
            usesInvites = true;
            minNumberOfTeams = numberOfTeams;
            upcomingMatchups = new List<TourneyMatchup>();
            resolvedMatchups = new List<TourneyMatchup>();
            inviteFunctions = new List<Func<int, List<Team>>>();
            inviteNumbers = new List<int>();
            includedTeams = new List<Team>();
            allowedHeroes = heroes;
            totalDays = numberOfDays;
        }

        /// <summary>
        /// Creates a new tournament that will use invitations.
        /// </summary>
        /// <param name="name">The name of the tournament</param>
        /// <param name="ID">The ID of the tournament</param>
        /// <param name="numberOfTeams">The minimum number of teams for this tournament.</param>
        /// <param name="heroes">The allowed heroes in this tournament.</param>
        public Tournament(string name, int ID, int numberOfTeams, Dictionary<int, Hero> heroes)
            : this(name, ID, numberOfTeams, heroes, 1)
        {
            //Nothing needed here.
        }
        #endregion
    }
}