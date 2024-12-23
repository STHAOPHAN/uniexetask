﻿namespace uniexetask.shared.Models.Request
{
    public class GroupListModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; } = null!;

        public string SubjectName { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string Status { get; set; } = null!;

        public bool HasMentor { get; set; }
    }
}
