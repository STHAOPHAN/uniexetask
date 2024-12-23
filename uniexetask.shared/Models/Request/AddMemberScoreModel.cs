﻿namespace uniexetask.shared.Models.Request
{
    public class AddMemberScoreModel
    {
        public int ProjectId { get; set; }

        public int MilestoneId { get; set; }

        public List<StudentScore> StudentScores { get; set; } = new List<StudentScore>();
    }
    public class StudentScore
    {
        public int StudentId { get; set; }
        public double Score { get; set; }
        public string? Comment {  get; set; }
    } 
}
