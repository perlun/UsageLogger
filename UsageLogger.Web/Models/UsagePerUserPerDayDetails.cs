using System;

namespace UsageLogger.Web.Models
{
    public class UsagePerUserPerDayDetails
    {
        public int MinutesUsed { get; set; }
        public DateTime LogDate { get; set; }
        public string LoginName { get; set; }
    }
}
