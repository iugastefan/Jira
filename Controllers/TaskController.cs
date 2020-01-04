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


        public IActionResult New(int id)
        {
            var members = _db.Members.Where(m => m.Project.Id == id).Select(m => m.Mail);
            ViewBag.Members = members;
            ViewBag.projectId = id;
            return View();
        }

        [HttpPost]
        public IActionResult New(Task ta, int id)
        {
            ta.Project = _db.Projects.First(p => p.Id == id);
            ta.Status = Status.NotStarted;
            _db.Tasks.Add(ta);
            _db.SaveChangesAsync();
            return RedirectToAction("Read", "Projects", new {id});
        }

        public IActionResult Delete(int projectId, int taskId)
        {
            var task = _db.Tasks.First(p => p.TaskId == taskId);
            _db.Tasks.Remove(task);
            _db.SaveChangesAsync();
            return RedirectToAction("Read", "Projects", new {id = projectId});
        }

        public IActionResult Edit(int taskId, int projectId)
        {
            var members = _db.Members.Where(m => m.Project.Id == projectId).Select(m => m.Mail);
            ViewBag.Members = members;
            ViewBag.projectId = projectId;
            ViewBag.oldTask = _db.Tasks.Find(taskId);
            ViewBag.statusList = new List<Status> {Status.Completed, Status.InProgress, Status.NotStarted};
            return View();
        }

        [HttpPost]
        public IActionResult Edit(int taskId, Task ta, int projectId)
        {
            var oldTask = _db.Tasks.First(t => t.TaskId == taskId);
            oldTask.Title = ta.Title;
            oldTask.Content = ta.Content;
            oldTask.Status = ta.Status;
            oldTask.MemberName = ta.MemberName;
            _db.SaveChangesAsync();

            return RedirectToAction("Read", "Projects", new {id = projectId});
        }
    }
}