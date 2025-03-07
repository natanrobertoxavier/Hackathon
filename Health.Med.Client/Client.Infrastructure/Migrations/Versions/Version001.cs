using FluentMigrator;

namespace Client.Infrastructure.Migrations.Versions;

[Migration((long)NumberVersions.CreateClientsTable, "Create client table")]
public class Version001 : Migration
{
    public override void Down()
    {
    }

    public override void Up()
    {
        var table = VersionBase.InsertStandardColumns(Create.Table("Clients"));

        table
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Email").AsString().NotNullable()
            .WithColumn("CPF").AsString().NotNullable()
            .WithColumn("Password").AsString(2000).NotNullable();
    }
}
