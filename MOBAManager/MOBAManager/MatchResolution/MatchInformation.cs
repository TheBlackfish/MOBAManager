using MOBAManager.Management.Heroes;
using System;
using System.Collections.Generic;

namespace MOBAManager.MatchResolution
{
    public partial class Match
    {
        public Tuple<string, string, string, string> getFormattedTimers()
        {
            return ms.getFormattedTimers();
        }

        public string getFormattedTeam1Bans()
        {
            List<Hero> selections = ms.getTeamBans(1);
            return formatHeroNames(selections);
        }

        public string getFormattedTeam2Bans()
        {
            List<Hero> selections = ms.getTeamBans(2);
            return formatHeroNames(selections);
        }

        public string getFormattedTeam1Picks()
        {
            List<Hero> selections = ms.getTeamSelections(1);
            return formatHeroNames(selections);
        }

        public string getFormattedTeam2Picks()
        {
            List<Hero> selections = ms.getTeamSelections(2);
            return formatHeroNames(selections);
        }

        private string formatHeroNames(List<Hero> lh)
        {
            if (lh.Count > 0)
            {
                string ret = "";
                foreach (Hero h in lh)
                {
                    ret += h.HeroName;
                    if (h != lh[lh.Count - 1])
                    {
                        ret += Environment.NewLine;
                    }
                }
                return ret;
            }
            return "";
        }
    }
}