namespace DuckI.Models;

public class Calendar
{
    public int CalendarId { get; set; }
    public string CsvPath { get; set; }
    public UserCalendar UserCalendar { get; set; }
}