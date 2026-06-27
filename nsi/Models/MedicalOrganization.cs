namespace nsi.Models;

public class MedicalOrganization : INsiEntity
{
    public virtual int Id { get; set; }
    public virtual string Code { get; set; } = string.Empty;
    public virtual string NameFull { get; set; } = string.Empty;
    public virtual string NameShort { get; set; } = string.Empty;
    public virtual string Region { get; set; } = string.Empty;
    public virtual string EoOid { get; set; } = string.Empty;
    public virtual DateTime? BeginDate { get; set; }
    public virtual DateTime? EndDate { get; set; }

    public void Fill(Dictionary<string, string?> dict)
    {
        Code      = dict.GetValueOrDefault("code") ?? string.Empty;
        NameFull  = dict.GetValueOrDefault("nameFull") ?? string.Empty;
        NameShort = dict.GetValueOrDefault("nameShort") ?? string.Empty;
        Region    = dict.GetValueOrDefault("region") ?? string.Empty;
        EoOid     = dict.GetValueOrDefault("eoOid") ?? string.Empty;
        BeginDate = ParseDate(dict.GetValueOrDefault("beginDate"));
        EndDate   = ParseDate(dict.GetValueOrDefault("endDate"));
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
