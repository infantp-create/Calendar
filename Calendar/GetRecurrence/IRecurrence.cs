namespace Calendar.GetRecurrence;

public interface IRecurrence
{
    List<(DateTime Start, DateTime End)> GenerateOccurrences(
        DateTime start,
        DateTime end,
        string? recurrenceType,
        int? count,
        List<string>? recurrenceDays,
        DateTime? recurrenceEnd);
}