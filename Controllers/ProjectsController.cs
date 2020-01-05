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

            var projectsYouAreMember =
                _db.Projects.AsEnumerable().Where(p => p.Members.Exists(m => m.Mail.Equals(User.Identity.Name)));

            var projectsYouAreManager =
                _db.Projects.AsEnumerable().Where(proj => proj.Manager.Equals(User.Identity.Name));

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
                var project = new Project
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
                var proj = _db.Projects.Single(p => p.Id == id);
                if (!proj.Manager.Equals(User.Identity.Name) && !User.IsInRole("Admin"))
                    return RedirectToAction("Index");
                _db.Projects.Where(p => p.Id == id).SelectMany(p => p.Members).ToList().Clear();
                _db.Projects.Where(p => p.Id == id).SelectMany(p => p.Teams).ToList().Clear();
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

            var member = project.Members.Any(m => m.Mail.Equals(User.Identity.Name));

            if (!User.IsInRole("Admin") && !member && !project.Manager.Equals(User.Identity.Name))
                return RedirectToAction("Index");
            ViewBag.Project = project;
            var teams = _db.Projects.Where(p => p.Id == id).SelectMany(p => p.Teams);
            var members = _db.Projects.Where(p => p.Id == id).SelectMany(p => p.Members);
            ViewBag.Members = members;
            ViewBag.Teams = teams;
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
            var member = _db.Projects.Where(p => p.Id == id).SelectMany(p => p.Members).First(m => m.Mail.Equals(mail));
            _db.Projects.Find(id).Members.Remove(member);
            _db.SaveChangesAsync();
            return RedirectToAction("Read", new {id});
        }

        [HttpPost]
        public IActionResult AddMember(int id, string mail)
        {
            var project = _db.Projects.Find(id);
            if (project.Members.Any(m => m.Mail.Equals(mail)))
                return RedirectToAction("Read", new {id});

            try
            {
                if (!project.Manager.Equals(User.Identity.Name)) return RedirectToAction("Read", new {id});

                var member = new Member {Mail = mail};
                project.Members.Add(member);
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