using System;

namespace MOBAManager.Management.Calendar
{
    sealed public partial class CalendarManager
    {
        /// <summary>
        /// Returns the offset until the next occurance of June 21st.
        /// </summary>
        /// <returns></returns>
        public int GetSummerSolsticeOffset()
        {
            DateTime ss = new DateTime(currentDate.Year, 6, 21);
            while (ss < currentDate)
            {
                ss = ss.AddYears(1);
            }
            return GetDaysToDate(ss);
        }

        /// <summary>
        /// Returns the offset until the next occurance of December 21st.
        /// </summary>
        /// <returns></returns>
        public int GetWinterSolsticeOffset()
        {
            DateTime ss = new DateTime(currentDate.Year, 12, 21);
            while (ss < currentDate)
            {
                ss = ss.AddYears(1);
            }
            return GetDaysToDate(ss);
        }
    }
}