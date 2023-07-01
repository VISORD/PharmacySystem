using System.Globalization;

namespace PharmacySystem.WebAPI.Extensions;

public static class DecimalExtensions
{
    public static string Format(this decimal value) => $"{value.ToString("0.####", CultureInfo.CurrentCulture)} ₽";
}