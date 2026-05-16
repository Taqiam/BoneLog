using System.Globalization;

namespace BoneLog.Tools;

public static class DateTimeHelper
{
    public static DateTime? ToDateTiem(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt) ? dt : null;
    }
}
