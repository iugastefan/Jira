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


        public IActionResult New(int projectId, int teamId)
        {
            var members = _db.Members.Where(t => t.Team.Id == teamId);
            ViewBag.Members = members;
            ViewBag.projectId = projectId;
            ViewBag.teamId = teamId;
            return View();
        }

        [HttpPost]
        public IActionResult New(Task ta, int projectId, int teamId, int memberId)
        {
            ta.Status = Status.NotStarted;
            ta.Team = _db.Teams.First(t => t.Id == teamId);
            ta.AssignedMember = _db.Members.First(m => m.Id == memberId);
            _db.Tasks.Add(ta);
            _db.SaveChangesAsync();
            return RedirectToAction("Show", "Team", new {projectId, teamId});
        }

        public IActionResult Delete(int projectId, int teamId, int taskId)
        {
            var task = _db.Tasks.First(t => t.Id == taskId);
            _db.Tasks.Remove(task);
            _db.SaveChangesAsync();
            return RedirectToAction("Show", "Team", new {projectId=projectId, teamId=teamId});
        }

        public IActionResult Edit(int projectId, int teamId, int taskId)
        {
            var team = _db.Teams.First(t => t.Id == teamId);
            var members = _db.Members.Where(m => m.Team.Id == teamId);
            ViewBag.Members = members;
            ViewBag.projectId = projectId;
            ViewBag.teamId = teamId;
            ViewBag.oldTask = _db.Tasks.First(t => t.Id == taskId);
            ViewBag.statusList = new List<Status> {Status.Completed, Status.InProgress, Status.NotStarted};
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Task ta, int projectId, int teamId, int taskId, int memberId)
        {
            // var team = _db.Projects.Find(projectId).Teams.Find(t => t.Id == teamId);
            var oldTask = _db.Tasks.First(t => t.Id == taskId);
            oldTask.Title = ta.Title;
            oldTask.Content = ta.Content;
            oldTask.Status = ta.Status;
            oldTask.AssignedMember = _db.Members.First(m => m.Id == memberId);
            _db.SaveChangesAsync();

            return RedirectToAction("Show", "Team", new {projectId,teamId});
        }
    }
}