﻿namespace uniexetask.api.Models.Request
{
    public class ProjectPendingModel
    {
        public required string GroupName { get; set; }
        public required string Topic { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
    }
}