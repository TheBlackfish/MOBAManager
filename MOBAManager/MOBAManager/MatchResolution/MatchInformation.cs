using System;

namespace MOBAManager.MatchResolution
{
    public partial class Match
    {
        public Tuple<string, string, string, string> getFormattedTimers()
        {
            return ms.getFormattedTimers();
        }
    }
}