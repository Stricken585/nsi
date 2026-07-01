namespace nsi.Models;

public class MedicalProducts : EntityBase
{
    [DictKey("label")]
    public virtual string Label { get; set; } = string.Empty;
    [DictKey("description")]
    public virtual string Description { get; set; } = string.Empty;
    [DictKey("code")]
    public virtual string Code { get; set; } = string.Empty;
    [DictKey("status")]
    public virtual string Status { get; set; } = string.Empty;   
}