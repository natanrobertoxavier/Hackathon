using FluentMigrator;

namespace Doctor.Infrastructure.Migrations.Versions;

[Migration((long)NumberVersions.CreateSpecialtiesTable, "Create specialty table")]
public class Version002 : Migration
{
    public override void Down()
    {
    }

    public override void Up()
    {
        var table = VersionBase.InsertStandardColumns(Create.Table("Specialties"));

        table
            .WithColumn("UserId").AsGuid().NotNullable()
            .WithColumn("Description").AsString(100).NotNullable()
            .WithColumn("StandardDescription").AsString(35).NotNullable();
    }
}