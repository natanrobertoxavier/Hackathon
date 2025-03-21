using FluentMigrator;

namespace Consultation.Infrastructure.Migrations.Versions;

[Migration((long)NumberVersions.CreateConsultationsTable, "Create consultation table")]
public class Version001 : Migration
{
    public override void Down()
    {
    }

    public override void Up()
    {
        var table = VersionBase.InsertStandardColumns(Create.Table("Consultations"));

        table
            .WithColumn("ClientId").AsGuid().NotNullable()
            .WithColumn("DoctorId").AsGuid().NotNullable()
            .WithColumn("ConsultationDate").AsDateTime().NotNullable();
    }
}
