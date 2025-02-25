using FluentMigrator;

namespace User.Infrastructure.Migrations.Versions;

[Migration((long)NumberVersions.CreateUsersTable, "Create user table")]
public class Version001 : Migration
{
    public override void Down()
    {
    }

    public override void Up()
    {
        var table = VersionBase.InsertStandardColumns(Create.Table("Users"));

        table
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Email").AsString().NotNullable()
            .WithColumn("Password").AsString(2000).NotNullable();
    }
}
