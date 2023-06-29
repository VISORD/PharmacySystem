using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Database.Parameters;

public static class PharmacyMedicamentRateParameter
{
    public static SqlMapper.ICustomQueryParameter ToTableValuedParameter(this IEnumerable<PharmacyMedicamentRate> rates)
    {
        var table = new DataTable();
        table.Columns.Add("PharmacyId", typeof(int));
        table.Columns.Add("MedicamentId", typeof(int));
        table.Columns.Add("StartDate", typeof(DateTime));
        table.Columns.Add("StopDate", typeof(DateTime));
        table.Columns.Add("RetailPrice", typeof(decimal));

        foreach (var item in rates)
        {
            table.Rows.Add(item.PharmacyId, item.MedicamentId, item.StartDate, item.StopDate, item.RetailPrice);
        }

        return table.AsTableValuedParameter("[pharmacy].[PharmacyMedicamentRate]");
    }
}