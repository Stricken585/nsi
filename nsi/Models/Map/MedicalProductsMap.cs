using FluentNHibernate.Mapping;

namespace nsi.Models.Map;

public class MedicalProductsMap : ClassMap<MedicalProducts>
{
    public  MedicalProductsMap()
    {
        Table("medicalproducts");
        Not.LazyLoad();
        Id(x => x.Id).GeneratedBy.Identity();
        Map(x => x.Label).Not.Nullable().Length(200);
        Map(x => x.Description).Nullable().Length(2000);
        Map(x=> x.Code).Not.Nullable().Length(50);
        Map(x => x.Status).Nullable().Length(10);
    }
}