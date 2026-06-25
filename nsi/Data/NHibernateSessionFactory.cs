using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace nsi.Data;

public static class NHibernateSessionFactory
{
    public static ISessionFactory Build(string connectionString)
    {
        return Fluently.Configure()
            .Database(PostgreSQLConfiguration.PostgreSQL82.ConnectionString(connectionString))
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<MedicalOrganizationMap>())
            .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
            .BuildSessionFactory();
    }
}
