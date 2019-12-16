using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Jira.Models
{
    public class Comment
    {
        [Key] public int Id { get; set; }
        [Required] public Task Task { get; set; }
        [Required] public string Creator { get; set; }
        [Required] public string Content { get; set; }
    }
}