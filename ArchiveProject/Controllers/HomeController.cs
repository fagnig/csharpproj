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

        public IActionResult Index()
        {
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

            ModelPopulator mdl = new ModelPopulator();

            ArchiveViewModel tmp2 = mdl.GetTable(string.IsNullOrEmpty(id) ? "dbo.MSreplication_options" : id);
            tmp2.tableHash = string.IsNullOrEmpty(id) ? "dbo.MSreplication_options" : id;
            tmp2.tableTitle = string.IsNullOrEmpty(id) ? "dbo.MSreplication_options" : id;
            return View(tmp2);
        }

        public void UpdateDbValue(string id, string column, string table, string value)
        {
            string conString = @"Server=.\SQLEXPRESS;Initial Catalog=ArchiveProject;Trusted_Connection=True";
            string sql = $"UPDATE [{table}] SET [{column}] = '{value}' WHERE id = '{id}'";
            SqlConnection sqlCon = new SqlConnection(conString);
            sqlCon.Open();
            SqlCommand sqlCommand = new SqlCommand(sql, sqlCon);
            sqlCommand.ExecuteNonQuery();
            sqlCon.Close();
        }
    }
}
