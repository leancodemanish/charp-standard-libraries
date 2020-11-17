using System;

namespace LeanCode.Standard.Calendar
{
    public interface ICalendarService
    {
        DateTime GetCalendarDate(int offset);

        DateTime GetCalendarDate(int offset, DateTime fromDate);

        DateTime GetLastBusinessDate();

        DateTime GetCurrentBusinessDate();
    }
}