using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Database.Parameters;

public static class PharmacyWorkingHoursParameter
{
    public static SqlMapper.ICustomQueryParameter ToTableValuedParameter(this IEnumerable<PharmacyWorkingHours> workingHours)
    {
        var table = new DataTable();
        table.Columns.Add("PharmacyId", typeof(int));
        table.Columns.Add("Weekday", typeof(byte));
        table.Columns.Add("StartTime", typeof(TimeSpan));
        table.Columns.Add("StopTime", typeof(TimeSpan));

        foreach (var item in workingHours)
        {
            table.Rows.Add(item.PharmacyId, item.Weekday, item.StartTime, item.StopTime);
        }

        return table.AsTableValuedParameter("[pharmacy].[PharmacyWorkingHours]");
    }
}