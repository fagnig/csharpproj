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
        private string key;


        public AdminController(ApplicationDbContext context)
        {
            dbContext = context;
            adm = new AdminManager(dbContext);
            pm = new PermissionManager(dbContext);
        }

        public IActionResult Index()
        {
            // Get user identifier
            key = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

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
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            adm.CreateArchive(id, name);
        }

        public void InsertPerm(string id, string name)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            adm.CreatePermission(id, name);
        }

        public void AddColToArchive(string id, string colName, string colType)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            adm.AddCol(id, colName, colType);
        }

        public void RemoveColFromArchive(string id, string colName)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            adm.RemoveCol(colName, id);
        }

        public void DeleteArchive(string id)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            adm.DropArchive(id);
        }

        public void RenameArchive(string id, string name)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            adm.RenameArchive(id, name);
        }


        public void RenamePermission(string id, string name)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            adm.RenamePermission(id, name);
        }

        public void DeletePermission(string id)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            adm.DropPermission(id);
        }

        public string GetColumns(string id)
        {
            if (pm.IsUserAdmin(key))
            {
                return "";
            }
            return JsonConvert.SerializeObject(adm.GetColumns(id));
        }

        public string GetPermissionMapping(string id)
        {
            if (pm.IsUserAdmin(key))
            {
                return "";
            }
            return JsonConvert.SerializeObject(adm.GetPermissionMapping(id));
        }

        public void SetPermissionMapping(string id, string idArchive, bool assign)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            if (assign) { adm.AssignTable(idArchive, id); }
            else { adm.RemoveTable(idArchive, id); }
        }

        public string GetUserMapping(string id)
        {
            if (pm.IsUserAdmin(key))
            {
                return "";
            }
            return JsonConvert.SerializeObject(adm.GetUserMapping(id));
        }

        public void SetUserMapping(string id, string idPerm, bool assign)
        {
            if (pm.IsUserAdmin(key))
            {
                return;
            }
            if (assign) { adm.AssignPerm(id, idPerm); }
            else { adm.RemovePerm(id, idPerm); }
        }
    }
}