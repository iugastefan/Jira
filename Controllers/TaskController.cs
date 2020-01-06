using System;
using System.Collections.Generic;
using System.Linq;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Controllers
{
    [Authorize(Roles = "Manager,Member,Admin")]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TaskController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Read(int projectId, int teamId, int taskId, bool update, int commentToUpdate)
        {
            ViewBag.Task = _db.Tasks.First(t => t.Id == taskId);
            ViewBag.Team = _db.Teams.First(t => t.Id == teamId);
            ViewBag.ProjectId = projectId;
            ViewBag.Project = _db.Projects.First(p => p.Id == projectId);
            ViewBag.Comments = _db.Comments.Where(c => c.Task.Id == taskId).ToList();
            ViewBag.User = _db.Members.First(m => m.Team.Id == teamId && m.Mail.Equals(User.Identity.Name));
            ViewBag.Update = update;
            ViewBag.CommentToUpdate = commentToUpdate;

            return View();
        }

        public IActionResult Create(int projectId, int teamId)
        {
            var members = _db.Members.Where(t => t.Team.Id == teamId);
            ViewBag.Members = members;
            ViewBag.projectId = projectId;
            ViewBag.teamId = teamId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Task ta, int projectId, int teamId, int memberId)
        {
            try
            {
                var team = _db.Teams.First(t => t.Id == teamId);
                var assignedMember = _db.Members.First(m => m.Id == memberId);
                var task = new Task
                {
                    Content = ta.Content, Title = ta.Title, Status = Status.NotStarted, Team = team,
                    AssignedMember = assignedMember
                };
                _db.Tasks.Add(task);
                _db.SaveChangesAsync();
            }
            catch (Exception)
            {
            }

            return RedirectToAction("Show", "Team", new {projectId, teamId});
        }

        public IActionResult Delete(int projectId, int teamId, int taskId)
        {
            var task = _db.Tasks.First(t => t.Id == taskId);
            _db.Tasks.Remove(task);
            _db.SaveChangesAsync();
            return RedirectToAction("Show", "Team", new {projectId = projectId, teamId = teamId});
        }

        public IActionResult Update(int projectId, int teamId, int taskId)
        {
            var members = _db.Members.Where(m => m.Team.Id == teamId);
            ViewBag.Members = members;
            ViewBag.projectId = projectId;
            ViewBag.teamId = teamId;
            ViewBag.oldTask = _db.Tasks.First(t => t.Id == taskId);
            ViewBag.statusList = new List<Status> {Status.Completed, Status.InProgress, Status.NotStarted};
            return View();
        }

        [HttpPost]
        public IActionResult Update(Task ta, int projectId, int teamId, int taskId, int memberId)
        {
            var oldTask = _db.Tasks.First(t => t.Id == taskId);
            oldTask.Title = ta.Title;
            oldTask.Content = ta.Content;
            if (ta.Status != oldTask.Status)
            {
                switch (ta.Status)
                {
                    case Status.Completed:
                        oldTask.EndDate = DateTime.Now;
                        oldTask.StartDate = DateTime.Now;
                        break;
                    case Status.InProgress:
                        oldTask.StartDate = DateTime.Now;
                        oldTask.EndDate = null;
                        break;
                    case Status.NotStarted:
                        oldTask.StartDate = null;
                        oldTask.EndDate = null;
                        break;
                }

                oldTask.Status = ta.Status;
            }

            var member = _db.Members.First(m => m.Id == memberId);
            oldTask.AssignedMember = member ?? oldTask.AssignedMember;
            _db.SaveChangesAsync();

            return RedirectToAction("Show", "Team", new {projectId, teamId});
        }

        [HttpPost]
        public IActionResult AddComment(int projectId, int teamId, int taskId, string content)
        {
            var task = _db.Tasks.First(t => t.Id == taskId);
            var author = _db.Members.First(m => m.Team.Id == teamId && m.Mail.Equals(User.Identity.Name));
            var comment = new Comment
            {
                Task = task,
                Author = author,
                Content = content
            };
            _db.Tasks.First(t => t.Id == taskId).Comments.Add(comment);
            _db.SaveChangesAsync();
            return RedirectToAction("Read", "Task", new {projectId, teamId, taskId});
        }

        public IActionResult DeleteComment(int projectId, int teamId, int taskId, int commentId)
        {
            _db.Tasks.First(t => t.Id == taskId).Comments.Remove(_db.Comments.First(c => c.Id == commentId));
            _db.SaveChangesAsync();
            return RedirectToAction("Read", "Task", new {projectId, teamId, taskId});
        }

        public IActionResult UpdateComment(int projectId, int teamId, int taskId, int commentToUpdate)
        {
            return RedirectToAction("Read", "Task", new {projectId, teamId, taskId, update = true, commentToUpdate});
        }

        [HttpPost]
        public IActionResult UpdateComment(int projectId, int teamId, int taskId, int commentId, string content)
        {
            var comment = _db.Comments.First(c => c.Id == commentId);
            comment.Content = content;
            _db.SaveChangesAsync();
            return RedirectToAction("Read", "Task", new {projectId, teamId, taskId});
        }
    }
}