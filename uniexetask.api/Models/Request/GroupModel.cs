﻿namespace uniexetask.api.Models.Request
{
    public class GroupModel
    {
        public string GroupName { get; set; } = null!;

        public int SubjectId { get; set; }

        public bool HasMentor { get; set; }
        public bool IsCurrentPeriod { get; set; }

        public string Status { get; set; } = null!;
    }
}
