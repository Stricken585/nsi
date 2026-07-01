namespace nsi.Models.Migrations;
using FluentMigrator;

[Migration(2026070112000)]  
public class CreateMedProdTable : Migration
{
    public override void Up()
    {
        Create.Table("medicalproducts")
            .WithColumn("id").AsInt32().Identity().PrimaryKey().NotNullable()
            .WithColumn("label").AsString(200).NotNullable()
            .WithColumn("description").AsString(2000).Nullable()
            .WithColumn("code").AsString(50).NotNullable()
            .WithColumn("status").AsString(10).Nullable();
    }
    public override void Down()
    {
        Delete.Table("medicalproducts");
    }
}