using System.Collections.Generic;
using System.Configuration;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using UsageLogger.Web.Models;

namespace UsageLogger.Web.Controllers
{
    [ApiController]
    [Route("/api/usage")]
    public class UsageController : ControllerBase
    {
        private const string PerUserPerDayDetailsQuery = @"
            SELECT
                SUM(duration) / 60 AS minutes_used,
                DATE(created) AS log_date,
                login_name
            FROM
                log_entry
            WHERE created BETWEEN DATE_SUB(NOW(), INTERVAL @days DAY) AND NOW()
            GROUP BY
                log_date, login_name
            ORDER BY
                log_date DESC
        ";

        private const string PerUserPerDaySummaryQuery = @"
            SELECT
                    SUM(duration) / 60 AS minutes_used,
                    login_name
            FROM
                log_entry
            WHERE created BETWEEN DATE_SUB(NOW(), INTERVAL @days DAY) AND NOW()
            GROUP BY
                login_name
        ";

        private string ConnectionString { get; }

        static UsageController()
        {
            // Tweak Dapper settings to match our column names better.
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public UsageController(IConfiguration configuration)
        {
            ConnectionString = configuration["USAGE_LOGGER_CONNECTION_STRING"];

            if (ConnectionString == null)
            {
                throw new ConfigurationErrorsException(
                    "USAGE_LOGGER_CONNECTION_STRING must be set. It should be a valid MariaDB connection string."
                );
            }
        }

        [HttpGet("per-user-per-day/details")]
        public IEnumerable<UsagePerUserPerDayDetails> GetDetails(int days = 7)
        {
            using var connection = new MySqlConnection(ConnectionString);

            return connection.Query<UsagePerUserPerDayDetails>(PerUserPerDayDetailsQuery,
                new
                {
                    days
                });
        }

        [HttpGet("per-user-per-day/summary")]
        public IEnumerable<UsagePerUserPerDaySummary> GetSummary(int days = 7)
        {
            using var connection = new MySqlConnection(ConnectionString);

            return connection.Query<UsagePerUserPerDaySummary>(PerUserPerDaySummaryQuery,
                new
                {
                    days
                });
        }
    }
}
