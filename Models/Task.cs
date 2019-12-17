using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jira.Models
{
    public class Task
    {
        [Key] public int TaskId { get; set; }
        public Project Project { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Content { get; set; }
        public Status Status { get; set; }
        public List<Comment> Comments { get; set; }
        public string MemberName { get; set; }
    }

    public enum Status
    {
        Completed,
        NotStarted,
        InProgress
    }
}