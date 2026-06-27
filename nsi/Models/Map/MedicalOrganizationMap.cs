using FluentNHibernate.Mapping;

namespace nsi.Models.Map;

public class MedicalOrganizationMap : ClassMap<MedicalOrganization>
{
    public MedicalOrganizationMap()
    {
        Table("MedicalOrganizations");
        Not.LazyLoad();
        Id(x => x.Id).GeneratedBy.Sequence("medicalorganizations_id_seq");
        Map(x => x.Code).Not.Nullable().Length(50);
        Map(x => x.NameFull).Not.Nullable().Length(500);
        Map(x => x.NameShort).Not.Nullable().Length(200);
        Map(x => x.Region).Not.Nullable().Length(200);
        Map(x => x.EoOid).Not.Nullable().Length(100);
        Map(x => x.BeginDate).Nullable();
        Map(x => x.EndDate).Nullable();
    }
}