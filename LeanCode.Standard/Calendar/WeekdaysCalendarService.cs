using System;

namespace LeanCode.Standard.Calendar
{
    public class WeekdaysCalendarService : ICalendarService
    {
        public DateTime GetCalendarDate(int offset)
        {
            return GetCalendarDate(offset, DateTime.Today);
        }

        public DateTime GetCalendarDate(int offset, DateTime fromDate)
        {
            var direction = Math.Sign(offset);
            var validDayCount = 0;
            var totalDayCount = 0;

            while (validDayCount < Math.Abs(offset))
            {
                totalDayCount += direction;
                var currentDay = fromDate.AddDays(totalDayCount);
                if (currentDay.DayOfWeek != DayOfWeek.Saturday && currentDay.DayOfWeek != DayOfWeek.Sunday)
                {
                    validDayCount++;
                }
            }

            return fromDate.AddDays(totalDayCount);
        }

        public DateTime GetLastBusinessDate()
        {
            return GetLastBusinessDate(DateTime.Today);
        }

        public DateTime GetLastBusinessDate(DateTime baseDate)
        {
            return GetCalendarDate(-1, baseDate);
        }

        public DateTime GetCurrentBusinessDate()
        {
            return GetCurrentBusinessDate(DateTime.Today);
        }

        /// <summary>
        /// Returns the current business date and in case of weekend, it returns coming monday 
        /// </summary>
        /// <param name="baseDate"></param>
        /// <returns></returns>
        public DateTime GetCurrentBusinessDate(DateTime baseDate)
        {
            int offset = 0;
            switch (baseDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    offset = 1;
                    break;
                case DayOfWeek.Saturday:
                    offset = 2;
                    break;
            }
            return baseDate.AddDays(offset);
        }


    }
}