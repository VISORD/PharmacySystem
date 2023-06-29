using System.Text.Json;

namespace PharmacySystem.WebAPI.Models.Common;

public abstract class ItemsPagingRequestBase
{
    public ICollection<Filter> Filtering { get; init; } = new List<Filter>();
    public ICollection<Ordering> Ordering { get; init; } = new List<Ordering>();
    public Paging Paging { get; init; } = new(Number: 0, Size: 10);

    public (ICollection<string> filters, ICollection<(string field, object value)> parameters) SqlFiltering<T>()
    {
        var filters = new List<string>();
        var parameters = new List<(string field, object value)>();
        foreach (var (field, value, matchMode) in Filtering)
        {
            if (value is null)
            {
                continue;
            }

            switch (matchMode)
            {
                case "startsWith":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] LIKE @{field} + '%'");
                    break;

                case "contains":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] LIKE '%' + @{field} + '%'");
                    break;

                case "notContains":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] NOT LIKE '%' + @{field} + '%'");
                    break;

                case "endsWith":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] LIKE '%' + @{field}");
                    break;

                case "equals":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] = @{field}");
                    break;

                case "notEquals":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] <> @{field}");
                    break;

                case "in":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] IN @{field}");
                    break;

                // Ignore
                case "between":
                    continue;

                case "lt":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] < @{field}");
                    break;

                case "lte":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] <= @{field}");
                    break;

                case "gt":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] > @{field}");
                    break;

                case "gte":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] >= @{field}");
                    break;

                case "dateIs":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] = @{field}");
                    break;

                case "dateIsNot":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] <> @{field}");
                    break;

                case "dateBefore":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] < @{field}");
                    break;

                case "dateAfter":
                    filters.Add($"[{field}] IS NOT NULL AND [{field}] > @{field}");
                    break;

                default:
                    continue;
            }

            if (((JsonElement) value).ValueKind == JsonValueKind.Array)
            {
                parameters.Add((field, ((JsonElement) value).EnumerateArray().Select(x => x.ToString()).ToArray()));
            }
            else
            {
                parameters.Add((field, value.ToString())!);
            }
        }

        return (filters, parameters);
    }

    public ICollection<string> SqlOrdering(params string[] defaults)
    {
        if (Ordering.Count > 0)
        {
            return Ordering.Where(x => x.IsAscending is not null)
                .Select(order => $"[{order.Field}] {(order.IsAscending == true ? "ASC" : "DESC")}")
                .ToArray();
        }

        return defaults.Select(x => $"[{x}] ASC").ToArray();
    }
}