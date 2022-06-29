using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(1)]
public class Init : Migration
{
    public override void Up()
    {
        Create.Table("devices")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("full_name").AsFixedLengthString(50).NotNullable()
            .WithColumn("platform").AsInt64().NotNullable()
            .WithColumn("version").AsFixedLengthString(20).NotNullable()
            .WithColumn("last_update").AsDate().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Table("events")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("device_id").AsInt64().NotNullable().ForeignKey("devices", "id")
            .WithColumn("name").AsFixedLengthString(50).NotNullable()
            .WithColumn("description").AsFixedLengthString(200).NotNullable()
            .WithColumn("date").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
    }

    public override void Down() => throw new NotImplementedException();
}
