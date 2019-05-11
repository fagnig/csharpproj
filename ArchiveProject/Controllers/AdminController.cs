using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArchiveProject.Data;
using ArchiveProject.Logic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ArchiveProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AdminController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            // Get user identifier
            var key = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Check if admin
            UserValidator uv = new UserValidator(dbContext);
            if(!uv.isUserAdmin(key)) { return RedirectToAction("Index", "Archive"); }
            return View();
        }

        public string LoadPermissions()
        {
            List<List<Object>> perms = new List<List<Object>>();
            List<Object> perm = new List<Object>();
            perm.Add(0);
            perm.Add("admin");
            perms.Add(perm);
            perm = new List<Object>();
            perm.Add(1);
            perm.Add("default");
            perms.Add(perm);
            var json = JsonConvert.SerializeObject(perms);
            System.Diagnostics.Debug.WriteLine("KIG DOG: " + json);
            return json;
        }
    }
}