namespace nsi.Models;

public class MedicalOrganization : EntityBase
{
    [DictKey("code")]
    public virtual string Code { get; set; } = string.Empty;

    [DictKey("nameFull")]
    public virtual string NameFull { get; set; } = string.Empty;

    [DictKey("nameShort")]
    public virtual string NameShort { get; set; } = string.Empty;

    [DictKey("region")]
    public virtual string Region { get; set; } = string.Empty;

    [DictKey("eoOid")]
    public virtual string EoOid { get; set; } = string.Empty;

    [DictKey("beginDate")]
    public virtual DateTime? BeginDate { get; set; }

    [DictKey("endDate")]
    public virtual DateTime? EndDate { get; set; }
}
