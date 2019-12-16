using System.ComponentModel.DataAnnotations;

namespace Jira.Models
{
    public class Member
    {
        [Key] public string Mail { get; set; }
        [Key] public Project Project { get; set; }
    }
}