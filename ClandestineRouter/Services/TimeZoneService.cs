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
    private readonly ILogger<TimeZoneService>? _logger;

    public TimeZoneService(ILogger<TimeZoneService>? logger = null)
    {
        _logger = logger;
    }

    public DateTime ConvertFromUtc(DateTime utcDateTime, string ianaTimeZoneId)
    {
        if (string.IsNullOrWhiteSpace(ianaTimeZoneId))
        {
            _logger?.LogWarning("No timezone specified, returning UTC time");
            return utcDateTime; // Return UTC if no timezone specified
        }

        try
        {
            // Convert IANA to Windows time zone ID if necessary
            var windowsTimeZoneId = TZConvert.IanaToWindows(ianaTimeZoneId);
            var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);

            // Ensure the input is treated as UTC
            var utcTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, targetTimeZone);

            // For form display purposes, return as Unspecified to avoid browser timezone conflicts
            return DateTime.SpecifyKind(convertedTime, DateTimeKind.Unspecified);
        }
        catch (TimeZoneNotFoundException ex)
        {
            _logger?.LogWarning(ex, "TimeZone not found: {TimeZoneId}, trying fallback", ianaTimeZoneId);

            // Fallback: try direct lookup in case it's already a Windows ID
            try
            {
                var fallbackTimeZone = TimeZoneInfo.FindSystemTimeZoneById(ianaTimeZoneId);
                var utcTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
                var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, fallbackTimeZone);
                return DateTime.SpecifyKind(convertedTime, DateTimeKind.Unspecified);
            }
            catch (Exception fallbackEx)
            {
                _logger?.LogError(fallbackEx, "Fallback timezone conversion failed for: {TimeZoneId}", ianaTimeZoneId);
                return utcDateTime;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Timezone conversion failed for: {TimeZoneId}", ianaTimeZoneId);
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
            DateTime result;

            // First try: ISO 8601 format with proper timezone handling
            if (DateTime.TryParse(utcDateTimeString, null,
                DateTimeStyles.RoundtripKind | DateTimeStyles.AdjustToUniversal, out result))
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
                "yyyy-MM-ddTHH:mm:ss.fffffffZ",   // ISO 8601 with ticks and Z
                "yyyy-MM-ddTHH:mm:ss",            // ISO 8601 without Z
                "yyyy-MM-ddTHH:mm:ss.fff",        // ISO 8601 with milliseconds, no Z
                "yyyy-MM-ddTHH:mm:ss.fffffff",    // ISO 8601 with ticks, no Z
                "yyyy-MM-dd HH:mm:ss",            // Standard format with space
                "MM/dd/yyyy HH:mm:ss",            // US format
                "dd/MM/yyyy HH:mm:ss",            // European format
                "yyyy-MM-dd",                     // Date only
                "MM/dd/yyyy",                     // US date only
                "dd/MM/yyyy"                      // European date only
            };

            if (DateTime.TryParseExact(utcDateTimeString, formats, null,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
            {
                return DateTime.SpecifyKind(result, DateTimeKind.Utc);
            }

            _logger?.LogWarning("Failed to parse datetime string: {DateTimeString}", utcDateTimeString);
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Exception parsing datetime string: {DateTimeString}", utcDateTimeString);
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
            _logger?.LogDebug("User has no timezone set, returning UTC time");
            return utcDateTime; // Return UTC if user has no timezone
        }

        return ConvertFromUtc(utcDateTime, user.LocalTimeZone);
    }

    public DateTime? ConvertToUserTime(string utcDateTimeString, ApplicationUser user)
    {
        if (user?.LocalTimeZone == null)
        {
            _logger?.LogDebug("User has no timezone set, parsing as UTC");
            return ParseUtcString(utcDateTimeString); // Return parsed UTC if user has no timezone
        }

        return ConvertFromUtc(utcDateTimeString, user.LocalTimeZone);
    }

    public DateTime ConvertBetweenTimeZones(DateTime dateTime, string fromIanaTimeZoneId, string toIanaTimeZoneId)
    {
        if (string.IsNullOrWhiteSpace(fromIanaTimeZoneId) || string.IsNullOrWhiteSpace(toIanaTimeZoneId))
        {
            _logger?.LogWarning("Missing timezone parameters for conversion");
            return dateTime;
        }

        try
        {
            // Convert source to Windows timezone
            var fromWindowsId = TZConvert.IanaToWindows(fromIanaTimeZoneId);
            var fromTimeZone = TimeZoneInfo.FindSystemTimeZoneById(fromWindowsId);

            // Convert target to Windows timezone
            var toWindowsId = TZConvert.IanaToWindows(toIanaTimeZoneId);
            var toTimeZone = TimeZoneInfo.FindSystemTimeZoneById(toWindowsId);

            // Specify the kind of the input datetime
            var sourceDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

            // First convert to UTC, then to target timezone
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(sourceDateTime, fromTimeZone);
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, toTimeZone);

            return DateTime.SpecifyKind(convertedTime, DateTimeKind.Unspecified);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Timezone conversion failed from {FromZone} to {ToZone}",
                fromIanaTimeZoneId, toIanaTimeZoneId);
            return dateTime;
        }
    }

    public DateTime? ConvertBetweenTimeZones(string dateTimeString, string fromIanaTimeZoneId, string toIanaTimeZoneId)
    {
        // For string conversion, assume it's in the source timezone (not UTC)
        if (string.IsNullOrWhiteSpace(dateTimeString))
        {
            return null;
        }

        DateTime parsedDateTime;
        if (!DateTime.TryParse(dateTimeString, out parsedDateTime))
        {
            _logger?.LogWarning("Failed to parse datetime string for timezone conversion: {DateTimeString}", dateTimeString);
            return null;
        }

        return ConvertBetweenTimeZones(parsedDateTime, fromIanaTimeZoneId, toIanaTimeZoneId);
    }

    public TimeSpan GetTimeZoneOffset(string ianaTimeZoneId, DateTime dateTime)
    {
        if (string.IsNullOrWhiteSpace(ianaTimeZoneId))
        {
            return TimeSpan.Zero;
        }

        try
        {
            var windowsTimeZoneId = TZConvert.IanaToWindows(ianaTimeZoneId);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);

            // Get offset for the specific date (accounts for DST)
            return timeZone.GetUtcOffset(dateTime);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get timezone offset for: {TimeZoneId}", ianaTimeZoneId);
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
        if (string.IsNullOrWhiteSpace(ianaTimeZoneId))
        {
            return "UTC";
        }

        try
        {
            var windowsTimeZoneId = TZConvert.IanaToWindows(ianaTimeZoneId);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
            return timeZone.DisplayName;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to get display name for timezone: {TimeZoneId}", ianaTimeZoneId);
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