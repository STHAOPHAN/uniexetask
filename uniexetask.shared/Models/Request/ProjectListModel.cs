﻿namespace uniexetask.shared.Models.Request
{
    public class ProjectListModel
    {
        public int ProjectId { get; set; }
        public int SubjectId { get; set; }
        public string? TopicCode { get; set; }
        public string? TopicName { get; set; }
        public string? Description { get; set; }
        public string? SubjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; }
        public bool? IsDeleted { get; set; }


        public decimal? ProgressPercentage { get; set; }

    }
}
