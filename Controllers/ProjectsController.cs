using System;
using System.Linq;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Controllers
{
    [Authorize(Roles = "Manager,Member,Admin")]
    public class ProjectsController : Controller
    {
        // GET
        private readonly ApplicationDbContext _db;

        public ProjectsController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var allProjects = _db.Projects.Select(project => project);
                ViewBag.AllProjects = allProjects;
            }

            var projectsYouAreMember = from proj in _db.Projects
                join member in _db.Members on proj.Id equals member.Project.Id
                where member.Mail.Equals(User.Identity.Name)
                select proj;

            var projectsYouAreManager = from proj in _db.Projects
                where proj.Manager.Equals(User.Identity.Name)
                select proj;

            ViewBag.ProjectsYouAreMember = projectsYouAreMember;
            ViewBag.ProjectsYouAreManager = projectsYouAreManager;
            return View();
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(string title)
        {
            try
            {
                var project = new Project()
                {
                    Manager = User.Identity.Name, Title = title
                };
                _db.Projects.Add(project);

                _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var proj = _db.Projects.First(p => p.Id == id);
                if (!proj.Manager.Equals(User.Identity.Name) && !User.IsInRole("Admin"))
                    return RedirectToAction("Index");
                var members = _db.Members.Where(m => m.Project.Id == id);
                foreach (var member in members)
                {
                    _db.Members.Remove(member);
                }

                _db.Projects.Remove(proj);
                _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Read(int id)
        {
            var project = _db.Projects.First(p => p.Id == id);
            var member = _db.Members.Any(m => m.Project.Id == id && m.Mail.Equals(User.Identity.Name));

            if (!User.IsInRole("Admin") && !member && !project.Manager.Equals(User.Identity.Name))
                return RedirectToAction("Index");
            ViewBag.Project = project;
            ViewBag.Members = _db.Members.Where(m => m.Project.Id == id);
            var tasks = _db.Tasks.Where(task => task.Project.Id == id);
            ViewBag.Tasks = tasks;
            return View();
        }

        public IActionResult AddMember(int id)
        {
            var members = from users in _db.Users select users.Email;
            ViewBag.Members = members;
            ViewBag.Id = id;
            ViewBag.ProjectName = _db.Projects.First(p => p.Id == id).Title;
            return View();
        }


        public IActionResult RemoveMember(int id, string mail)
        {
            if (!User.IsInRole("Admin") && !_db.Projects.First(p => p.Id == id).Manager.Equals(User.Identity.Name))
                return RedirectToAction("Read", new {id});
            var member = _db.Members.First(m => m.Mail.Equals(mail) && m.Project.Id == id);
            _db.Members.Remove(member);
            _db.SaveChangesAsync();
            return RedirectToAction("Read", new {id});
        }

        [HttpPost]
        public IActionResult AddMember(int id, string mail)
        {
            if (_db.Members.Any(m => m.Mail.Equals(mail)))
                return RedirectToAction("Read", new {id});

            try
            {
                var proj = _db.Projects.First(p => p.Id == id);
                if (!proj.Manager.Equals(User.Identity.Name)) return RedirectToAction("Read", new {id});

                var member = new Member() {Mail = mail, Project = proj};
                _db.Members.Add(member);
                _db.SaveChangesAsync();

                return RedirectToAction("Read", new {id});
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
    }
}