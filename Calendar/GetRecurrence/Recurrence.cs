namespace Calendar.GetRecurrence;

public class Recurrence : IRecurrence
    {
        public List<(DateTime Start, DateTime End)> GenerateOccurrences(
            DateTime start,
            DateTime end,
            string? recurrenceType,
            int? count,
            List<string>? recurrenceDays,
            DateTime? recurrenceEnd)
        {
            var occurrences = new List<(DateTime, DateTime)>();

            if (string.IsNullOrEmpty(recurrenceType))
            {
                occurrences.Add((start, end));
                return occurrences;
            }

            var currentStart = start;
            var currentEnd = end;

            int loopCount = count ?? 0;

            // For daily recurrence, if count is 0 or null, consider it as 1
            if (recurrenceType == "Daily" && loopCount <= 0) loopCount = 1;

            // Default to 1 occurrence if count is still 0
            if (loopCount <= 0) loopCount = 1;

            for (int i = 0; i < loopCount; i++)
            {
                if (recurrenceEnd.HasValue && currentStart > recurrenceEnd.Value) break;

                occurrences.Add((currentStart, currentEnd));

                currentStart = recurrenceType switch
                {
                    "Daily" => currentStart.AddDays(1),
                    "Weekly" => GetNextWeeklyDate(currentStart, recurrenceDays),
                    "Monthly" => currentStart.AddMonths(1),
                    "Custom" => GetNextCustomDate(currentStart, recurrenceDays),
                    _ => currentStart.AddDays(1)
                };

                currentEnd = currentStart.Add(end - start);
            }

            return occurrences;
        }

        private DateTime GetNextCustomDate(DateTime current, List<string>? recurrenceDays)
        {
            if (recurrenceDays == null || !recurrenceDays.Any())
                return current.AddDays(1);

            var daysLower = recurrenceDays.Select(d => d.ToLower().Substring(0, 3)).ToList();

            var next = current.AddDays(1);
            while (!daysLower.Contains(next.DayOfWeek.ToString().ToLower().Substring(0, 3)))
                next = next.AddDays(1);

            return next;
        }

        private DateTime GetNextWeeklyDate(DateTime current, List<string>? recurrenceDays)
        {
            if (recurrenceDays == null || !recurrenceDays.Any())
                return current.AddDays(7); // Default weekly step

            var daysLower = recurrenceDays.Select(d => d.ToLower().Substring(0, 3)).ToList();
            var next = current.AddDays(1);

            while (!daysLower.Contains(next.DayOfWeek.ToString().ToLower().Substring(0, 3)))
                next = next.AddDays(1);

            return next;
        }
    }