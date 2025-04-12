using FluentMigrator;

namespace Doctor.Infrastructure.Migrations.Versions;

[Migration((long)NumberVersions.CreateTableServiceDay, "Create service day table")]
public class Version003 : Migration
{
    public override void Down()
    {
    }

    public override void Up()
    {
        var table = VersionBase.InsertStandardColumns(Create.Table("ServiceDays"));

        table
            .WithColumn("DoctorId").AsGuid().NotNullable()
            .WithColumn("Day").AsString(15).NotNullable()
            .WithColumn("StartTime").AsCustom("TIME").NotNullable()
            .WithColumn("EndTime").AsCustom("TIME").NotNullable();
    }
}