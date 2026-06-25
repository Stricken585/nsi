namespace nsi.Models;

public class MedicalOrganization
{
    public virtual int Id { get; set; }
    public virtual string Code { get; set; } = string.Empty;
    public virtual string NameFull { get; set; } = string.Empty;
    public virtual string NameShort { get; set; } = string.Empty;
    public virtual string Region { get; set; } = string.Empty;
    public virtual string EoOid { get; set; } = string.Empty;
    public virtual DateTime? BeginDate { get; set; }
    public virtual DateTime? EndDate { get; set; }
}
