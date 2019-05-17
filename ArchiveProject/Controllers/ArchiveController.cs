using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArchiveProject.Data;
using ArchiveProject.Logic;
using ArchiveProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ArchiveProject.Controllers
{
    public class ArchiveController : Controller
    {

        private readonly ApplicationDbContext dbContext;
        private PermissionManager pm;
        private ArchiveManager am;
        private string key;

        public ArchiveController(ApplicationDbContext context)
        {
            dbContext = context;
            pm = new PermissionManager(dbContext);
            am = new ArchiveManager(dbContext);
        }

        public IActionResult Index(string id)
        {
            // Check required tables
            PreRequisiteManager prm = new PreRequisiteManager(dbContext);
            prm.DeployRequiredTables();

            // Get user identifier
            key = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Check user
            ViewData["isAdmin"] = pm.IsUserAdmin(key);

            // Populate available archives
            ViewData["Archives"] = am.GetUserTableList(key);

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        public void UpdateDbValue(string id, string column, string table, string value)
        {
            if (pm.CanUserAccess(key, table))
            {
                return;
            }
            am.UpdateField(id, column, table, value);
        }

        public void DeleteDbRow(string id, string table)
        {
            if (pm.CanUserAccess(key, table))
            {
                return;
            }
            am.DropRow(table, id);
        }

        public object CreateDbRow(string id)
        {
            if (pm.CanUserAccess(key, id))
            {
                return null;
            }
            return am.InsertRow(id);
        }

        public string GetArchive(string id)
        {
            if (pm.CanUserAccess(key, id))
            {
                return "";
            }
            return JsonConvert.SerializeObject(am.GetTable(id));
        }

        public string GetArchiveHeader(string id)
        {
            if (pm.CanUserAccess(key, id))
            {
                return "";
            }
            return JsonConvert.SerializeObject(am.GetTableHeader(id));
        }
    }
}