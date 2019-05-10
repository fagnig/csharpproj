using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArchiveProject.Models;
using System.Data.SqlClient;
using ArchiveProject.Data;
using ArchiveProject.Logic;

namespace ArchiveProject.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            TableDeployer tb = new TableDeployer(dbContext);
            tb.deployRequiredTables();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Database(string id)
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
