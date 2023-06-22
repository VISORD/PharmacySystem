using System.Globalization;

namespace PharmacySystem.WebAPI.Extensions;

public static class DateTimeExtensions
{
    private static readonly DateTime EarliestStartDate = DateTime.Parse("2000-01-01", CultureInfo.InvariantCulture);
    private static readonly DateTime LatestStopDate = DateTime.Parse("2199-12-31", CultureInfo.InvariantCulture);

    public static DateTime? AsStartDate(this DateTime date) => date.Date <= EarliestStartDate ? null : date;
    public static DateTime AsStartDate(this DateTime? date) => date is null || date <= EarliestStartDate ? EarliestStartDate : date.Value;

    public static DateTime? AsStopDate(this DateTime date) => date.Date >= LatestStopDate ? null : date;
    public static DateTime AsStopDate(this DateTime? date) => date is null || date >= LatestStopDate ? LatestStopDate : date.Value;

    public static string Format(this DateTime dateTime) => dateTime.ToString("HH:mm:ss dd.MM.yyyy");
}