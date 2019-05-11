using ArchiveProject.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Logic
{
    public class PermissionUpdater
    {
        private readonly ApplicationDbContext dbContext;

        public PermissionUpdater(ApplicationDbContext context)
        {
            dbContext = context;

        }

        public void assignTableToPerm(string tableHash,int perm)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"INSERT IGNORE INTO [ArchivePermMapping] VALUES ({perm}, 'tb_{tableHash}');";
            dc.ExecuteNonQuery();
            
            dbContext.sqlCon.Close();
        }

        public void removeTablefromPerm(string tableHash, int perm)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"DELETE FROM [ArchivePermMapping] WHERE id_role={perm}, id_table = 'tb_{tableHash}');";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }


        public void createPermission(string name)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"SELECT COUNT(*) FROM ArchivePermissions WHERE name = '{name}'";

            if ((Int32)dc.ExecuteScalar() == 0)
            {
                dc.CommandText = $"INSERT INTO ArchivePermissions VALUES ('{name}');";
                dc.ExecuteNonQuery();
            }

            dbContext.sqlCon.Close();
        }
    }
}