using FluentMigrator.Builders.Create.Table;

namespace User.Infrastructure.Migrations.Versions;

public class VersionBase
{
    public static ICreateTableColumnOptionOrWithColumnSyntax InsertStandardColumns(
        ICreateTableWithColumnOrSchemaOrDescriptionSyntax table)
    {
        return table
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("RegistrationDate").AsDateTime().NotNullable();
    }
}