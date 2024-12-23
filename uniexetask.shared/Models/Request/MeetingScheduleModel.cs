﻿namespace uniexetask.shared.Models.Request
{
    public class MeetingScheduleModel
    {
        public int ScheduleId { get; set; }
        public string MeetingScheduleName { get; set; } = null!;

        public int? GroupId { get; set; }

        public int? MentorId { get; set; }

        public string Location { get; set; } = null!;

        public DateTime  MeetingDate { get; set; }

        public int Duration { get; set; }

        public string Type { get; set; } = null!;

        public string Content { get; set; } = null!;
        public string? Url { get; set; }

        public bool IsDeleted { get; set; }
    }
}
