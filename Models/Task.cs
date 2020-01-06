using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jira.Models
{
    public class Task
    {
        public Task()
        {
            Comments = new List<Comment>();
        }

        [Key] public int Id { get; set; }
        public Team Team { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Content { get; set; }
        public Status Status { get; set; }
        public List<Comment> Comments { get; set; }
        [Required]
        public Member AssignedMember { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public enum Status
    {
        Completed,
        NotStarted,
        InProgress
    }
}