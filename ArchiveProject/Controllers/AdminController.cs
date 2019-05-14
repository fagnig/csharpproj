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

            // Get Model
            AdminViewModel avm = new AdminViewModel();
            AdminPopulator ap = new AdminPopulator(dbContext);
            avm.permissions = uv.getAllPermissions();
            avm.users = ap.getAllUsers();
            avm.table = uv.getUserTableList(key);
            return View(avm);
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

        public string GetHash()
        {
            return new UserValidator(dbContext).GetHash();
        }

        public void InsertArchive(string id, string name)
        {
            new TableDeployer(dbContext).CreateTable(name, id);
        }

        public void AddColToArchive(string id, string colName, string colType)
        {
            TableDeployer tb = new TableDeployer(dbContext);

            if (!tb.typeMap.ContainsKey(colType)) { return; }

            ModelUpdater md = new ModelUpdater(dbContext);
            md.addColToTable(colName, tb.typeMap[colType], id);
        }

        public void DeleteArchive(string id)
        {
            new ModelUpdater(dbContext).dropTable(id);
        }

        public void RenameArchive(string id, string name)
        {
            new ModelUpdater(dbContext).updateFields(id, "name", "ArchiveMapping", name);
        }


        public void RenamePermission(string id, string name)
        {
            new ModelUpdater(dbContext).updateFields(id, "name", "ArchivePermissions", name);
        }

        public void DeletePermission(string id)
        {
            new PermissionUpdater(dbContext).deletePermission(id);
        }
    }
}