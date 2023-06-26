using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Models.Common;

namespace PharmacySystem.WebAPI.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> FilterByRequest<T>(this IQueryable<T> query, IEnumerable<Filter> filtering) where T : notnull
    {
        foreach (var (column, value, matchMode) in filtering)
        {
            if (value is null)
            {
                continue;
            }

            // TODO: resolve variants
            switch (matchMode)
            {
                // case "startsWith":
                //     query = query.Where(x => EF.Property<string>(x, column).StartsWith(value));
                //     break;
                //
                // case "contains":
                //     query = query.Where(x => EF.Property<string>(x, column).Contains(value));
                //     break;
                //
                // case "notContains":
                //     query = query.Where(x => !EF.Property<string>(x, column).Contains(value));
                //     break;
                //
                // case "endsWith":
                //     query = query.Where(x => EF.Property<string>(x, column).EndsWith(value));
                //     break;
                //
                // case "equals":
                //     query = query.Where(x => EF.Property<string>(x, column).Equals(value));
                //     break;
                //
                // case "notEquals":
                //     query = query.Where(x => !EF.Property<string>(x, column).Equals(value));
                //     break;
                //
                // case "in":
                //     query = query;
                //     break;
                //
                // case "between":
                //     query = query;
                //     break;
                //
                // case "lt":
                //     query = query.Where(x => Comparer<object>.Default.Compare(value, EF.Property<object>(x, column)) < 0);
                //     break;
                //
                // case "lte":
                //     query = query.Where(x => Comparer<object>.Default.Compare(value, EF.Property<object>(x, column)) <= 0);
                //     break;
                //
                // case "gt":
                //     query = query.Where(x => Comparer<object>.Default.Compare(value, EF.Property<object>(x, column)) > 0);
                //     break;
                //
                // case "gte":
                //     query = query.Where(x => Comparer<object>.Default.Compare(value, EF.Property<object>(x, column)) >= 0);
                //     break;
                //
                // case "dateIs":
                // {
                //     if (DateTime.TryParse(value, CultureInfo.InvariantCulture, out var dateTime))
                //     {
                //         query = query.Where(x => dateTime.Date.Equals(EF.Property<DateTime>(x, column)));
                //     }
                //
                //     break;
                // }
                //
                // case "dateIsNot":
                // {
                //     if (DateTime.TryParse(value, CultureInfo.InvariantCulture, out var dateTime))
                //     {
                //         query = query.Where(x => !dateTime.Date.Equals(EF.Property<DateTime>(x, column)));
                //     }
                //
                //     break;
                // }
                //
                // case "dateBefore":
                // {
                //     if (DateTime.TryParse(value, CultureInfo.InvariantCulture, out var dateTime))
                //     {
                //         query = query.Where(x => dateTime.Date < EF.Property<DateTime>(x, column));
                //     }
                //
                //     break;
                // }
                //
                // case "dateAfter":
                // {
                //     if (DateTime.TryParse(value, CultureInfo.InvariantCulture, out var dateTime))
                //     {
                //         query = query.Where(x => dateTime.Date > EF.Property<DateTime>(x, column));
                //     }
                //
                //     break;
                // }
            }
        }

        return query;
    }

    public static IQueryable<T> OrderByRequest<T>(this IQueryable<T> query, IEnumerable<Ordering> ordering) where T : notnull
    {
        // TODO: Refactor
        return ordering.Aggregate(query, (current, order) => order.IsAscending switch
        {
            true => current.OrderBy(x => EF.Property<object>(x, order.Field)),
            false => current.OrderByDescending(x => EF.Property<object>(x, order.Field)),
            _ => current
        });
    }
}