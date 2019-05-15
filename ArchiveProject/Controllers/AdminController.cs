using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArchiveProject.Data;
using ArchiveProject.Logic;
using ArchiveProject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ArchiveProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private AdminManager adm;
        private PermissionManager pm;


        public AdminController(ApplicationDbContext context)
        {
            dbContext = context;
            adm = new AdminManager(dbContext);
            pm = new PermissionManager(dbContext);
        }

        public IActionResult Index()
        {
            // Get user identifier
            var key = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Check if admin
            if(!pm.IsUserAdmin(key)) { return RedirectToAction("Index", "Archive"); }

            // Get Model
            AdminViewModel avm = new AdminViewModel();
            avm.permissions = adm.GetPermissions();
            avm.users = adm.GetAllUsers();
            avm.table = adm.GetArchives();
            return View(avm);
        }

        public string GetHash()
        {
            return dbContext.GetHash();
        }

        public void InsertArchive(string id, string name)
        {
            adm.CreateArchive(id, name);
        }

        public void AddColToArchive(string id, string colName, string colType)
        {
            adm.AddCol(id, colName, colType);
        }

        public void RemoveColFromArchive(string id, string colName)
        {
            adm.RemoveCol(colName, id);
        }

        public void DeleteArchive(string id)
        {
            adm.DropArchive(id);
        }

        public void RenameArchive(string id, string name)
        {
            adm.RenameArchive(id, name);
        }


        public void RenamePermission(string id, string name)
        {
            adm.RenamePermission(id, name);
        }

        public void DeletePermission(string id)
        {
            adm.DropPermission(id);
        }

        public string GetColumns(string id)
        {
            return JsonConvert.SerializeObject(adm.GetColumns(id));
        }
    }
}