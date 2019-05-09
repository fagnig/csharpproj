using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArchiveProject.Models;
using System.Data.SqlClient;

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

        public IActionResult Database()
        {
            string conString, sql;
            SqlConnection sqlCon;

            conString = @"Data Source=localhost;Initial Catalog=master; User ID=sa; Password=Meme4321";
            sql = "SELECT * from dbo.MSreplication_options";

            sqlCon = new SqlConnection(conString);
            sqlCon.Open();

            SqlCommand sqlCommand;
            SqlDataReader sqlDataReader;
            String Output = "";


            sqlCommand = new SqlCommand(sql, sqlCon);

            sqlDataReader = sqlCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                {
                    Output += sqlDataReader.GetValue(i) + " - ";
                }
                Output += Environment.NewLine;
            }

            ViewData["Message"] = Output;

            return View();
        }
    }
}
