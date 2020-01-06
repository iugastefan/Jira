using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jira.Models
{
    public class Team
    {
        public Team()
        {
            Members = new List<Member>();
            Tasks = new List<Task>();
        }
        
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public Project Project { get; set; }
        public List<Member> Members { get; set; }
        public List<Task> Tasks { get; set; }
    }
}