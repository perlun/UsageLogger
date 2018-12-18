using System;

namespace UsageLoggerWeb.Controllers
{
    public class TimeUsedPerDay
    {
        public string Id
        {
            get
            {
                return $"{LogDate.ToShortDateString()}_{LoginName}";
            }
        }

        public decimal HoursUsed { get; set; }
        public DateTime LogDate { get; set; }
        public string LogDateFormatted
        {
            get
            {
                return LogDate.ToString("yyyy-MM-dd");
            }
        }

        public string LoginName { get; set; }
    }
}