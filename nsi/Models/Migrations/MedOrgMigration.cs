namespace nsi.Models.Migrations;
using FluentMigrator;

[Migration(2026062721000)]  
public class CreateMedOrgTable : Migration
{
    public override void Up()
    {
        Create.Table("medicalorganizations")
            .WithColumn("id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("code").AsString(50).NotNullable()
            .WithColumn("namefull").AsString(1000).NotNullable()
            .WithColumn("nameshort").AsString(500).NotNullable()
            .WithColumn("region").AsString(500).NotNullable()
            .WithColumn("eooid").AsString(100).NotNullable()
            .WithColumn("begindate").AsDateTime().Nullable()
            .WithColumn("enddate").AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.Table("medicalorganizations");
    }
}
