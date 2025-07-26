using ClandestineRouter.Data;
using System.Globalization;
using TimeZoneConverter;

namespace ClandestineRouter.Services;
public interface ITimeZoneService
{
    /// <summary>
    /// Converts a UTC DateTime to the specified IANA time zone
    /// </summary>
    /// <param name="utcDateTime">The UTC DateTime to convert</param>
    /// <param name="ianaTimeZoneId">IANA time zone identifier (e.g., "America/New_York")</param>
    /// <returns>DateTime in the target time zone</returns>
    DateTime ConvertFromUtc(DateTime utcDateTime, string ianaTimeZoneId);

    /// <summary>
    /// Converts a UTC date string to the specified IANA time zone
    /// </summary>
    /// <param name="utcDateTimeString">The UTC DateTime string to convert (ISO 8601 format recommended)</param>
    /// <param name="ianaTimeZoneId">IANA time zone identifier (e.g., "America/New_York")</param>
    /// <returns>DateTime in the target time zone, or null if string parsing fails</returns>
    DateTime? ConvertFromUtc(string utcDateTimeString, string ianaTimeZoneId);

    /// <summary>
    /// Converts a UTC DateTime to the user's local time zone
    /// </summary>
    /// <param name="utcDateTime">The UTC DateTime to convert</param>
    /// <param name="user">User with LocalTimeZone property</param>
    /// <returns>DateTime in the user's time zone, or UTC if user has no timezone set</returns>
    DateTime ConvertToUserTime(DateTime utcDateTime, ApplicationUser user);

    /// <summary>
    /// Converts a UTC date string to the user's local time zone
    /// </summary>
    /// <param name="utcDateTimeString">The UTC DateTime string to convert</param>
    /// <param name="user">User with LocalTimeZone property</param>
    /// <returns>DateTime in the user's time zone, or null if string parsing fails</returns>
    DateTime? ConvertToUserTime(string utcDateTimeString, ApplicationUser user);

    /// <summary>
    /// Converts a DateTime from one time zone to another
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <param name="fromIanaTimeZoneId">Source IANA time zone identifier</param>
    /// <param name="toIanaTimeZoneId">Target IANA time zone identifier</param>
    /// <returns>DateTime in the target time zone</returns>
    DateTime ConvertBetweenTimeZones(DateTime dateTime, string fromIanaTimeZoneId, string toIanaTimeZoneId);

    /// <summary>
    /// Converts a date string from one time zone to another
    /// </summary>
    /// <param name="dateTimeString">The DateTime string to convert</param>
    /// <param name="fromIanaTimeZoneId">Source IANA time zone identifier</param>
    /// <param name="toIanaTimeZoneId">Target IANA time zone identifier</param>
    /// <returns>DateTime in the target time zone, or null if string parsing fails</returns>
    DateTime? ConvertBetweenTimeZones(string dateTimeString, string fromIanaTimeZoneId, string toIanaTimeZoneId);

    /// <summary>
    /// Safely parses a UTC date string to DateTime
    /// </summary>
    /// <param name="utcDateTimeString">The UTC DateTime string to parse</param>
    /// <returns>Parsed DateTime in UTC, or null if parsing fails</returns>
    DateTime? ParseUtcString(string utcDateTimeString);

    /// <summary>
    /// Gets the time zone offset for a specific IANA time zone at a given date
    /// </summary>
    /// <param name="ianaTimeZoneId">IANA time zone identifier</param>
    /// <param name="dateTime">The date to check offset for (accounts for DST)</param>
    /// <returns>TimeSpan offset from UTC</returns>
    TimeSpan GetTimeZoneOffset(string ianaTimeZoneId, DateTime dateTime);

    /// <summary>
    /// Validates if an IANA time zone identifier is valid
    /// </summary>
    /// <param name="ianaTimeZoneId">IANA time zone identifier to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidTimeZone(string ianaTimeZoneId);

    /// <summary>
    /// Gets the display name for an IANA time zone
    /// </summary>
    /// <param name="ianaTimeZoneId">IANA time zone identifier</param>
    /// <returns>User-friendly display name</returns>
    string GetTimeZoneDisplayName(string ianaTimeZoneId);
}

public class TimeZoneService : ITimeZoneService
{
    public DateTime ConvertFromUtc(DateTime utcDateTime, string ianaTimeZoneId)
    {
        if (string.IsNullOrWhiteSpace(ianaTimeZoneId))
        {
            return utcDateTime; // Return UTC if no timezone specified
        }

        try
        {
            // Convert IANA to Windows time zone ID if necessary
            var windowsTimeZoneId = TZConvert.IanaToWindows(ianaTimeZoneId);
            var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);

            // Ensure the input is treated as UTC
            var utcTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, targetTimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            // Fallback: try direct lookup in case it's already a Windows ID
            try
            {
                var fallbackTimeZone = TimeZoneInfo.FindSystemTimeZoneById(ianaTimeZoneId);
                var utcTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, fallbackTimeZone);
            }
            catch
            {
                // If all else fails, return UTC
                return utcDateTime;
            }
        }
        catch
        {
            // Return UTC if conversion fails
            return utcDateTime;
        }
    }

    public DateTime? ParseUtcString(string utcDateTimeString)
    {
        if (string.IsNullOrWhiteSpace(utcDateTimeString))
        {
            return null;
        }

        try
        {
            // Try multiple parsing strategies for flexibility
            DateTime result;

            // First try: ISO 8601 format (most common for APIs)
            if (DateTime.TryParse(utcDateTimeString, null, DateTimeStyles.RoundtripKind | DateTimeStyles.AdjustToUniversal, out result))
            {
                return DateTime.SpecifyKind(result, DateTimeKind.Utc);
            }

            // Second try: Standard parsing with assumption it's UTC
            if (DateTime.TryParse(utcDateTimeString, out result))
            {
                return DateTime.SpecifyKind(result, DateTimeKind.Utc);
            }

            // Third try: Exact format matching for common patterns
            string[] formats = {
                "yyyy-MM-ddTHH:mm:ssZ",           // ISO 8601 with Z
                "yyyy-MM-ddTHH:mm:ss.fffZ",       // ISO 8601 with milliseconds and Z
                "yyyy-MM-ddTHH:mm:ss",            // ISO 8601 without Z
                "yyyy-MM-ddTHH:mm:ss.fff",        // ISO 8601 with milliseconds, no Z
                "yyyy-MM-dd HH:mm:ss",            // Standard format with space
                "MM/dd/yyyy HH:mm:ss",            // US format
                "dd/MM/yyyy HH:mm:ss",            // European format
                "yyyy-MM-dd",                     // Date only
                "MM/dd/yyyy",                     // US date only
                "dd/MM/yyyy"                      // European date only
            };

            if (DateTime.TryParseExact(utcDateTimeString, formats, null, DateTimeStyles.None, out result))
            {
                return DateTime.SpecifyKind(result, DateTimeKind.Utc);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public DateTime? ConvertFromUtc(string utcDateTimeString, string ianaTimeZoneId)
    {
        var parsedDateTime = ParseUtcString(utcDateTimeString);
        if (!parsedDateTime.HasValue)
        {
            return null;
        }

        return ConvertFromUtc(parsedDateTime.Value, ianaTimeZoneId);
    }

    public DateTime ConvertToUserTime(DateTime utcDateTime, ApplicationUser user)
    {
        if (user?.LocalTimeZone == null)
        {
            return utcDateTime; // Return UTC if user has no timezone
        }

        return ConvertFromUtc(utcDateTime, user.LocalTimeZone);
    }

    public DateTime? ConvertToUserTime(string utcDateTimeString, ApplicationUser user)
    {
        if (user?.LocalTimeZone == null)
        {
            return ParseUtcString(utcDateTimeString); // Return parsed UTC if user has no timezone
        }

        return ConvertFromUtc(utcDateTimeString, user.LocalTimeZone);
    }

    public DateTime ConvertBetweenTimeZones(DateTime dateTime, string fromIanaTimeZoneId, string toIanaTimeZoneId)
    {
        try
        {
            // Convert source to Windows timezone
            var fromWindowsId = TZConvert.IanaToWindows(fromIanaTimeZoneId);
            var fromTimeZone = TimeZoneInfo.FindSystemTimeZoneById(fromWindowsId);

            // Convert target to Windows timezone
            var toWindowsId = TZConvert.IanaToWindows(toIanaTimeZoneId);
            var toTimeZone = TimeZoneInfo.FindSystemTimeZoneById(toWindowsId);

            // First convert to UTC, then to target timezone
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, fromTimeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, toTimeZone);
        }
        catch
        {
            // If conversion fails, return original datetime
            return dateTime;
        }
    }

    public DateTime? ConvertBetweenTimeZones(string dateTimeString, string fromIanaTimeZoneId, string toIanaTimeZoneId)
    {
        var parsedDateTime = ParseUtcString(dateTimeString);
        if (!parsedDateTime.HasValue)
        {
            return null;
        }

        return ConvertBetweenTimeZones(parsedDateTime.Value, fromIanaTimeZoneId, toIanaTimeZoneId);
    }

    public TimeSpan GetTimeZoneOffset(string ianaTimeZoneId, DateTime dateTime)
    {
        try
        {
            var windowsTimeZoneId = TZConvert.IanaToWindows(ianaTimeZoneId);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);

            // Get offset for the specific date (accounts for DST)
            return timeZone.GetUtcOffset(dateTime);
        }
        catch
        {
            return TimeSpan.Zero; // Return UTC offset if lookup fails
        }
    }

    public bool IsValidTimeZone(string ianaTimeZoneId)
    {
        if (string.IsNullOrWhiteSpace(ianaTimeZoneId))
        {
            return false;
        }

        try
        {
            var windowsTimeZoneId = TZConvert.IanaToWindows(ianaTimeZoneId);
            TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
            return true;
        }
        catch
        {
            // Try direct lookup as fallback
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(ianaTimeZoneId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public string GetTimeZoneDisplayName(string ianaTimeZoneId)
    {
        try
        {
            var windowsTimeZoneId = TZConvert.IanaToWindows(ianaTimeZoneId);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
            return timeZone.DisplayName;
        }
        catch
        {
            return ianaTimeZoneId; // Return the ID itself if display name can't be found
        }
    }
}

// Extension methods for convenience
public static class TimeZoneServiceExtensions
{
    /// <summary>
    /// Extension method to convert UTC DateTime to user's timezone
    /// </summary>
    public static DateTime ToUserTime(this DateTime utcDateTime, ITimeZoneService timeZoneService, ApplicationUser user)
    {
        return timeZoneService.ConvertToUserTime(utcDateTime, user);
    }

    /// <summary>
    /// Extension method to convert UTC DateTime string to user's timezone
    /// </summary>
    public static DateTime? ToUserTime(this string utcDateTimeString, ITimeZoneService timeZoneService, ApplicationUser user)
    {
        return timeZoneService.ConvertToUserTime(utcDateTimeString, user);
    }

    /// <summary>
    /// Extension method to convert UTC DateTime to specific timezone
    /// </summary>
    public static DateTime ToTimeZone(this DateTime utcDateTime, ITimeZoneService timeZoneService, string ianaTimeZoneId)
    {
        return timeZoneService.ConvertFromUtc(utcDateTime, ianaTimeZoneId);
    }

    /// <summary>
    /// Extension method to convert UTC DateTime string to specific timezone
    /// </summary>
    public static DateTime? ToTimeZone(this string utcDateTimeString, ITimeZoneService timeZoneService, string ianaTimeZoneId)
    {
        return timeZoneService.ConvertFromUtc(utcDateTimeString, ianaTimeZoneId);
    }

    /// <summary>
    /// Extension method to safely parse UTC string
    /// </summary>
    public static DateTime? ToUtcDateTime(this string utcDateTimeString, ITimeZoneService timeZoneService)
    {
        return timeZoneService.ParseUtcString(utcDateTimeString);
    }
}

// Usage examples and registration
public static class ServiceRegistration
{
    public static IServiceCollection AddTimeZoneServices(this IServiceCollection services)
    {
        services.AddScoped<ITimeZoneService, TimeZoneService>();
        return services;
    }
}