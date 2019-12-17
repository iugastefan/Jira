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
            ViewBag.projectId = id;
            return View();
        }

        [HttpPost]
        public IActionResult New(Task ta, int id)
        {
            ta.Project = _db.Projects.First(p => p.Id == id);
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
    }
}