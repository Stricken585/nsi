using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using nsi.Models.Map;

namespace nsi.Data;

public static class NHibernateSessionFactory
{
    public static ISessionFactory Build(string connectionString)
    {
        return Fluently.Configure()
            .Database(PostgreSQLConfiguration.PostgreSQL82.ConnectionString(connectionString))
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<MedicalOrganizationMap>())
            .BuildSessionFactory();
    }
}
