using FluentMigrator;

namespace Doctor.Infrastructure.Migrations.Versions;

[Migration((long)NumberVersions.CreateDoctorsTable, "Create doctor table")]
public class Version001 : Migration
{
    public override void Down()
    {
    }

    public override void Up()
    {
        var table = VersionBase.InsertStandardColumns(Create.Table("Doctors"));

        table
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Email").AsString().NotNullable()
            .WithColumn("CR").AsString(15).NotNullable()
            .WithColumn("Password").AsString(2000).NotNullable()
            .WithColumn("UserId").AsGuid().NotNullable();
    }
}
