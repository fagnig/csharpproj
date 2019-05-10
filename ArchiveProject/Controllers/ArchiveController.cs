using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiveProject.Data;
using ArchiveProject.Logic;
using ArchiveProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArchiveProject.Controllers
{
    public class ArchiveController : Controller
    {

        private readonly ApplicationDbContext dbContext;

        public ArchiveController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
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
    }
}