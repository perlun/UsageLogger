using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace UsageLoggerWeb.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private IDbConnection connection;

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        public SampleDataController()
        {
            connection = new MySqlConnection(Program.ConnectionString);
            connection.Open();
        }

        [HttpGet("[action]")]
        public IEnumerable<TimeUsedPerDay> WeatherForecasts(int startDateIndex)
        {
            using (var cmd = connection.CreateCommand())
            {
                var commandDefinition = new CommandDefinition(@"
                    SELECT
                        ROUND(SUM(duration) / 3600, 2) AS HoursUsed,
                        DATE(created) AS LogDate,
                        login_name AS LoginName
                    FROM log_entry
                    GROUP BY LogDate, LoginName
                    ORDER BY LogDate DESC
                    LIMIT 20
                ");

                var result = connection.Query<TimeUsedPerDay>(commandDefinition);
                return result;
            }
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}
