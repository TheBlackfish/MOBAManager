using MOBAManager.Management.Teams;
using MOBAManager.Utility;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.Management.Calendar
{
    public partial class CalendarManager
    {
        /// <summary>
        /// Schedules a random event for each AI team.
        /// </summary>
        public void ScheduleRandomEventsForEachAITeam()
        {
            ScheduleRandomEventsForEachAITeam(1);
        }

        /// <summary>
        /// Schedules any number of random events for each AI team.
        /// </summary>
        /// <param name="iterations">The number of events each AI team should schedule.</param>
        public void ScheduleRandomEventsForEachAITeam(int iterations)
        {
            if (iterations < 1)
            {
                return;
            }

            List<Team> allTeams = teamManager.GetAllTeams();

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                foreach (Team t in allTeams)
                {
                    if (t.ID != 0)
                    {
                        //Choose random type of event
                        EventType type = (EventType)EventType.GetValues(typeof(EventType)).GetValue(1+RNG.Roll(2));

                        switch (type)
                        {
                            case EventType.PUG:
                                scheduleRandomPickupGameForTeam(t);
                                break;
                            case EventType.Bootcamp:
                                scheduleRandomBootcampForTeam(t);
                                break;
                        }
                    }           
                }
            }
        }

        /// <summary>
        /// Schedules a bootcamp for the specified team on a random date within the future.
        /// </summary>
        /// <param name="t">The team to schedule a bootcamp for.</param>
        private void scheduleRandomBootcampForTeam(Team t)
        {
            //Choose random offset that is open for the current team
            //We want to roll an integer between 0 - 15 and keep it in x1.
            //Then we roll an integer between 0 - x2, where x2 = 15 - x1.
            //This creates a sort of bell curve where higher values are rolled less often.
            int randOffset = RNG.Roll(15 - RNG.Roll(15));
            while (TeamHasEventsOnDate(t.ID, randOffset))
            {
                randOffset++;
            }

            AddBootcamp(t.ID, randOffset);
        }

        /// <summary>
        /// Schedules a pick-up for the specified team on a random date within the future.
        /// </summary>
        /// <param name="t">The team to schedule a bootcamp for.</param>
        private void scheduleRandomPickupGameForTeam(Team t)
        {
            //Choose random offset that is open for the current team
            //We want to roll an integer between 0 - 15 and keep it in x1.
            //Then we roll an integer between 0 - x2, where x2 = 15 - x1.
            //This creates a sort of bell curve where higher values are rolled less often.
            int randOffset = RNG.Roll(15 - RNG.Roll(15));
            while (TeamHasEventsOnDate(t.ID, randOffset))
            {
                randOffset++;
            }

            //Set up event, changing targets as needed
            //Again, only PUGs right now, which need another team that is open on that date.
            List<Team> possibleOpponents = new List<Team>();
            while (possibleOpponents.Count == 0)
            {
                possibleOpponents = teamManager.GetAllTeams().Where(team => team.ID != 0 && team.ID != t.ID).Where(team => !TeamHasEventsOnDate(team.ID, randOffset)).ToList();
                if (possibleOpponents.Count == 0)
                {
                    randOffset++;
                }
            }

            int oppID = possibleOpponents[RNG.Roll(possibleOpponents.Count)].ID;

            if (RNG.Roll(2) == 0)
            {
                AddPickupGame(t.ID, oppID, randOffset);
            }
            else
            {
                AddPickupGame(oppID, t.ID, randOffset);
            }
        }
    }
}