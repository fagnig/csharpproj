using ArchiveProject.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
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

        public void assignPermToUser(string userHash, int perm)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"INSERT IGNORE INTO [ArchiveUserPermMapping] VALUES ('{userHash}', {perm});";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }
        public void removePermFromUser(string userHash, int perm)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"DELETE FROM [ArchiveUserPermMapping] WHERE id_perm={perm}, id_user = '{userHash}');";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }

        public void assignTableToPerm(string tableHash,int perm)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"INSERT IGNORE INTO [ArchivePermMapping] VALUES ({perm}, '{tableHash}');";
            dc.ExecuteNonQuery();
            
            dbContext.sqlCon.Close();
        }

        public void removeTablefromPerm(string tableHash, int perm)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"DELETE FROM [ArchivePermMapping] WHERE id_role={perm}, id_table = '{tableHash}');";
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

        public void deletePermission(int perm)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            try
            {
                dc.CommandText = "BEGIN TRANSACTION;";
                dc.ExecuteNonQuery();

                dc.CommandText = $"DELETE FROM ArchivePermMapping WHERE id_perm = {perm}";
                dc.ExecuteNonQuery();

                dc.CommandText = $"DELETE FROM ArchiveUserPermMapping WHERE id_perm = {perm}";
                dc.ExecuteNonQuery();

                dc.CommandText = $"DELETE FROM ArchivePermissions WHERE id = {perm}";
                dc.ExecuteNonQuery();

                dc.CommandText = "COMMIT;";
                dc.ExecuteNonQuery();

            } catch (SqlException) {
                dc.CommandText = "ROLLBACK;";
            }

            dbContext.sqlCon.Close();
        }

        
    }
}