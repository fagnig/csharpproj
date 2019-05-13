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

        public ArchiveController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index(string id)
        {
            // Check required tables
            TableDeployer td = new TableDeployer(dbContext);
            td.deployRequiredTables();

            // Get user identifier
            var key = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Check user
            UserValidator uv = new UserValidator(dbContext);
            ViewData["isAdmin"] = uv.isUserAdmin(key);

            // Populate available archives
            ViewData["Archives"] = uv.getUserTableList(key);

            // Populate model
            ModelPopulator mdl = new ModelPopulator(dbContext);
            ArchiveViewModel avm = new ArchiveViewModel();
            if(!String.IsNullOrEmpty(id))
            {
                avm = mdl.GetTable(id);
                avm.tableHash = id;
                avm.tableTitle = id;
            }

            return View(avm);
        }

        public IActionResult Data(string id)
        {

            ModelPopulator mdl = new ModelPopulator(dbContext);


            ArchiveViewModel tmp = mdl.GetTable(string.IsNullOrEmpty(id) ? "ArchiveMapping" : id);
            tmp.tableHash = string.IsNullOrEmpty(id) ? "ArchiveMapping" : id;
            tmp.tableTitle = string.IsNullOrEmpty(id) ? "ArchiveMapping" : id;
            return View(tmp);
        }

        public void UpdateDbValue(string id, string column, string table, string value)
        {
            ModelUpdater mu = new ModelUpdater(dbContext);

            mu.updateFields(id, column, table, value);
        }

        public void DeleteDbRow(string id, string table)
        {
            ModelUpdater mu = new ModelUpdater(dbContext);
            mu.dropRow(table, id);
        }

        public object CreateDbRow(string id)
        {
            ModelUpdater mu = new ModelUpdater(dbContext);
            return mu.insertRow(id);
        }
    }
}