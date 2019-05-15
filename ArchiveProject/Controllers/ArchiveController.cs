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

namespace ArchiveProject.Controllers
{
    public class ArchiveController : Controller
    {

        private readonly ApplicationDbContext dbContext;

        private PermissionManager pm;
        private ArchiveManager am;

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
            //prm.DeployRequiredTables();

            // Get user identifier
            var key = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Check user
            ViewData["isAdmin"] = pm.IsUserAdmin(key);

            // Populate available archives
            ViewData["Archives"] = am.GetUserTableList(key);

            // Populate model
            ArchiveViewModel avm = new ArchiveViewModel();
            if(!String.IsNullOrEmpty(id)) { avm = am.GetTable(id); }

            return View(avm);
        }

        public void UpdateDbValue(string id, string column, string table, string value)
        {
            am.UpdateField(id, column, table, value);
        }

        public void DeleteDbRow(string id, string table)
        {
            am.DropRow(table, id);
        }

        public object CreateDbRow(string id)
        {
            return am.InsertRow(id);
        }
    }
}