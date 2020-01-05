using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Jira.Models
{
    public class Member
    {
        [Key] public int Id { get; set; }
        public Team Team { get; set; }
        public Project Project { get; set; }
        public string Mail { get; set; }
    }
}