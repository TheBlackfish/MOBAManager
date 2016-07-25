using MOBAManager.Management.Teams;
using MOBAManager.Utility;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.Management.Calendar
{
    public partial class CalendarManager
    {
        public void scheduleRandomEventsForEachAITeam()
        {
            scheduleRandomEventsForEachAITeam(1);
        }

        public void scheduleRandomEventsForEachAITeam(int iterations)
        {
            if (iterations < 1)
            {
                return;
            }

            List<Team> allTeams = tm.getAllTeams();

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                foreach (Team t in allTeams)
                {
                    if (t.ID != 0)
                    {
                        //Choose random type of event
                        //For right now, we only have PUGs.

                        //Choose random offset that is open for the current team
                        //We want to roll an integer between 0 - 20 and keep it in x1.
                        //Then we roll an integer between 0 - x2, where x2 = 20 - x1.
                        //This creates a sort of bell curve where higher values are rolled less often.
                        int randOffset = RNG.roll(15 - RNG.roll(15));
                        while (teamHasEventsOnDate(t.ID, randOffset))
                        {
                            randOffset++;
                        }

                        //Set up event, changing targets as needed
                        //Again, only PUGs right now, which need another team that is open on that date.
                        List<Team> possibleOpponents = new List<Team>();
                        while (possibleOpponents.Count == 0)
                        {
                            possibleOpponents = allTeams.Where(team => team.ID != 0 && team.ID != t.ID).Where(team => !teamHasEventsOnDate(team.ID, randOffset)).ToList();
                            if (possibleOpponents.Count == 0)
                            {
                                randOffset++;
                            }
                        }

                        int oppID = possibleOpponents[RNG.roll(possibleOpponents.Count)].ID;

                        if (RNG.roll(2) == 0)
                        {
                            addPickupGame(t.ID, oppID, randOffset);
                        }
                        else
                        {
                            addPickupGame(oppID, t.ID, randOffset);
                        }
                    }           
                }
            }
        }
    }
}