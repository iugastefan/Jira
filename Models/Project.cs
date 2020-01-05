using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Jira.Models
{
    public class Project
    {
        public Project()
        {
            Members=new List<Member>();
            Teams = new List<Team>();
        }
        [Key] public int Id { get; set; }
        [Required] public string Manager { get; set; }
        [Required] public string Title { get; set; }
        public List<Member> Members { get; set; }
        public List<Team> Teams { get; set; }
    }
}