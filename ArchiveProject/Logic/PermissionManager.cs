using ArchiveProject.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Logic
{
    public class PermissionManager
    {
        private readonly ApplicationDbContext dbContext;

        public PermissionManager(ApplicationDbContext context)
        {
            dbContext = context;

        }

        public List<string> GetTableRoles(string tableHash)
        {
            
            DbDataReader dr = dbContext.ExecReader($"SELECT * FROM ArchivePermMapping WHERE id_table = '{tableHash}'");

            List<string> tmpList = new List<string>();

            while (dr.Read())
            {
                tmpList.Add((string)dr.GetValue(0));
            }

            dr.Close();

            return tmpList;
        }
        public List<string> GetUserRoles(string userHash)
        {

            DbDataReader dr = dbContext.ExecReader($"SELECT * FROM ArchiveUserPermMapping WHERE id_user = '{userHash}'");

            List<string> tmpList = new List<string>();

            while (dr.Read())
            {
                tmpList.Add((string)dr.GetValue(1));
            }

            dr.Close();

            return tmpList;
        }

        public bool IsUserAdmin(string userHash)
        {
            List<string> userRoles = GetUserRoles(userHash);

            return userRoles.Contains("0");
        }
        public bool CanUserAccess(string userHash, string tableHash)
        {
            if (IsUserAdmin(userHash))
            {
                return true;
            }

            List<string> userRoles = GetUserRoles(userHash);
            List<string> tableRoles = GetTableRoles(tableHash);

            foreach (string role in userRoles)
            {
                if (tableRoles.Contains(role)) { return true; }
            }

            return false;
        }
    }
}