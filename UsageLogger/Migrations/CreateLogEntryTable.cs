using SimpleMigrations;
using System;

namespace UsageLoggerService.Migrations
{
    [Migration(1, "Create log_entry table")]
    public class CreateLogEntryTable : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE log_entry (
                    id SERIAL NOT NULL PRIMARY KEY,
                    host_name TINYTEXT NOT NULL,
                    login_name TINYTEXT NOT NULL,
                    process_name TINYTEXT NOT NULL,
                    file_name TINYTEXT NOT NULL,
                    duration SMALLINT NOT NULL,
                    created TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                )
            ");
        }

        protected override void Down()
        {
            // Deliberately left out for now.
            throw new NotImplementedException();
        }
    }
}
