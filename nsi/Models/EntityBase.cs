using System.Reflection;

namespace nsi.Models;

public abstract class EntityBase : IEntity
{
    public virtual int Id { get; set; }

    public virtual void Fill(Dictionary<string, string?> dict)
    {
        foreach (var property in GetType().GetProperties())
        {
            var attr = property.GetCustomAttribute<DictKeyAttribute>();
            if (attr == null) continue;

            var value = dict.GetValueOrDefault(attr.Name);

            if (property.PropertyType == typeof(string))
                property.SetValue(this, value ?? string.Empty);
            else if (property.PropertyType == typeof(DateTime?))
                property.SetValue(this, ParseDate(value));
        }
    }

    private static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        if (DateTime.TryParseExact(value, "dd.MM.yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var date))
            return date;
        return null;
    }
}
