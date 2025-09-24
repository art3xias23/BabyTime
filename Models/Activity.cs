using SQLite;

namespace BabyTime.Models
{
    public class Activity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string ActivityType { get; set; }

        public DateTime CreatedAt { get; set; }
    }


    public class ActivityDisplay
    {
        public int Id { get; set; }
        public string TimeString { get; set; }
        public string EndTimeString { get; set; }
        public string ActivityType { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsFinished => EndDateTime.HasValue;
    }

}