using System.Data;
using Dapper;

namespace PharmacySystem.WebAPI.Database.Parameters;

public static class IntegerItemParameter
{
    public static SqlMapper.ICustomQueryParameter ToTableValuedParameter(this IEnumerable<int> values)
    {
        var table = new DataTable();
        table.Columns.Add("Value", typeof(int));

        foreach (var integer in values)
        {
            table.Rows.Add(integer);
        }

        return table.AsTableValuedParameter("[core].[IntegerItem]");
    }
}