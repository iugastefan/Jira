using System.Collections.Generic;
using System.Linq;
using Jira.Data;
using Jira.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Controllers
{
    [Authorize(Roles = "Manager,Member,Admin")]
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TeamController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Show(int projectId, int teamId)
        {
            ViewBag.Team = GetTeam(teamId);
            ViewBag.Members = GetTeamMembers(projectId, teamId);
            ViewBag.ProjectId = projectId;
            ViewBag.Project = _db.Projects.First(p => p.Id == projectId);
            ViewBag.Tasks = _db.Tasks.Where(t => t.Team.Id == teamId).Select(t=>t);
            return View();
        }

        public IActionResult New(int projectId)
        {
            ViewBag.projectId = projectId;
            return View();
        }

        [HttpPost]
        public IActionResult New(Team ta, int projectId)
        {
            var team = new Team
            {
                Name = ta.Name
            };
            _db.Projects.Find(projectId).Teams.Add(team);
            _db.SaveChangesAsync();
            return RedirectToAction("Show", "Team", new {projectId = projectId, teamId = team.Id});
        }

        public IActionResult Delete(int projectId, string teamName)
        {
            var teams = _db.Projects.Where(p => p.Id == projectId).SelectMany(p => p.Teams);
            var team = teams.First(t => t.Name.Equals(teamName));
            _db.Projects.Find(projectId).Teams.Remove(team);
            _db.SaveChangesAsync();
            return RedirectToAction("Read", "Projects", new {id = projectId});
        }

        private Team GetTeam(int teamId)
        {
            return _db.Teams.First(t => t.Id == teamId);
        }

        private Member GetMember(int memberId)
        {
            return _db.Members.First(m => m.Id == memberId);
        }

        private List<Member> GetTeamMembers(int projectId, int teamId)
        {
            // return _db.Projects.Where(p => p.Id == projectId).SelectMany(p => p.Teams).Where(t => t.Id == teamId)
            //     .SelectMany(t => t.Members).ToList();
            return _db.Members.Where(m => m.Team.Id == teamId).ToList();
        }

        public IActionResult AddMember(int projectId, int teamId)
        {
            var projectMembers = _db.Projects.Where(p => p.Id == projectId).SelectMany(p => p.Members);
            // var teamMembers = _db.Projects.Where(p => p.Id == projectId).SelectMany(p => p.Teams)
            //     .Where(t => t.Id == teamId).SelectMany(t => t.Members);
            var teamMembers = _db.Members.Where(m=>m.Team !=null);
            var availableMembers = projectMembers.Except(teamMembers);
            ViewBag.Members = availableMembers;
            ViewBag.Team = GetTeam(teamId);
            ViewBag.Project = _db.Projects.Single(p => p.Id == projectId);
            return View();
        }

        [HttpPost]
        public IActionResult AddMember(int projectId, int teamId, int memberId)
        {
            var team = GetTeam(teamId);
            var member = GetMember(memberId);
            team.Members.Add(member);
            _db.SaveChangesAsync();
            return RedirectToAction("Show", "Team", new {projectId, teamId});
        }

        public IActionResult DeleteMember(int projectId, int teamId, int memberId)
        {
            _db.Teams.First(t => t.Id == teamId).Members.Remove(_db.Members.First(m => m.Team.Id == teamId));
            _db.SaveChangesAsync();
            return RedirectToAction("Show", "Team", new {projectId, teamId});
        }
    }
}