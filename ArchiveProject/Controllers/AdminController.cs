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
            avm.permissions = ap.getAllPermissions();
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

        public void InsertArchive(string hash, string name)
        {
            new TableDeployer(dbContext).CreateTable(name, hash);
        }

        public void AddColToArchive(string hash, string colName, string colType)
        {
            TableDeployer tb = new TableDeployer(dbContext);

            if (!tb.typeMap.ContainsKey(colType)) { return; }

            ModelUpdater md = new ModelUpdater(dbContext);
            md.addColToTable(colName, tb.typeMap[colType], hash);
        }

        public void DeleteArchive(string hash)
        {
            new ModelUpdater(dbContext).dropTable(hash);
        }

        public void RenameArchive(string hash, string name)
        {
            new ModelUpdater(dbContext).updateFields(hash, "name", "ArchiveMapping", name);
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