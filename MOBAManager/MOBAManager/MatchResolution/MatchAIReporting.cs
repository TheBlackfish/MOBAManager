using System;

namespace MOBAManager.MatchResolution
{
    partial class MatchAI
    {
        public Tuple<string, string, string, string> getFormattedTimers()
        {
            string[] placeholder = new string[4];
            double min = 0;

            double seconds = Math.Truncate(bonusTimeMaximum / 1000) - Math.Truncate(team1BonusTimeCounter / 1000);
            while (seconds > 60)
            {
                min++;
                seconds -= 60;
            }
            placeholder[0] = min.ToString() + ":" + ((seconds < 10)?("0"):("")) + seconds.ToString();

            seconds = Math.Truncate(regularTimeMaximum / 1000) - Math.Truncate(team1RegularTimeCounter / 1000);
            min = 0;
            while (seconds > 60)
            {
                min++;
                seconds -= 60;
            }
            placeholder[1] = min.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();

            seconds = Math.Truncate(bonusTimeMaximum / 1000) - Math.Truncate(team2BonusTimeCounter / 1000);
            min = 0;
            while (seconds > 60)
            {
                min++;
                seconds -= 60;
            }
            placeholder[2] = min.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();

            seconds = Math.Truncate(regularTimeMaximum / 1000) - Math.Truncate(team2RegularTimeCounter / 1000);
            min = 0;
            while (seconds > 60)
            {
                min++;
                seconds -= 60;
            }
            placeholder[3] = min.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();

            return new Tuple<string, string, string, string>(placeholder[0], placeholder[1], placeholder[2], placeholder[3]);
        }
    }
}