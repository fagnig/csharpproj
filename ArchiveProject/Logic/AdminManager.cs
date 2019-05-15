using ArchiveProject.Data;
using ArchiveProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Logic
{
    public class AdminManager
    {
        private readonly ApplicationDbContext dbContext;

        public AdminManager(ApplicationDbContext context)
        {
            dbContext = context;

        }

        ////////////////////
        //USERS
        ////////////////////
        public List<List<Object>> GetAllUsers()
        {
            DbDataReader dr = dbContext.ExecReader("SELECT * FROM AspNetUsers");

            List<List<Object>> tmpList = new List<List<Object>>();

            while (dr.Read())
            {
                List<Object> tmpSubList = new List<Object>();

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    tmpSubList.Add(dr.GetValue(i));
                }

                tmpList.Add(tmpSubList);
            }

            dr.Close();

            return tmpList;
        }


        ////////////////////
        //ARCHIVE
        ////////////////////

        public void CreateArchive(string hash, string name)
        {
            List<string> permSql = new List<string>
            {
                $"CREATE TABLE [tb_{hash}] ( id int NOT NULL IDENTITY(1,1) PRIMARY KEY);",//Create new table
                $"INSERT INTO ArchiveMapping (id, name) VALUES ( '{hash}', '{name}');" //Map new table
            };

            dbContext.ExecTrans(permSql);
        }

        public void CreateArchive(string name)
        {
            string hash = dbContext.GetHash();

            CreateArchive(hash, name);
        }

        public void DropArchive(string tableHash)
        {
            //Drop the given table
            dbContext.DropTable("tb_" + tableHash);
            //Remove the dropped tables mapping
            dbContext.ExecNonQuery($"DELETE FROM [ArchiveMapping] WHERE id = '{tableHash}'");
        }

        public void RenameArchive(string tableHash, string tableNewName)
        {
            dbContext.ExecNonQuery($"UPDATE ArchiveMapping SET name = '{tableNewName}' WHERE id = '{tableHash}'");
        }

        public List<List<Object>> GetArchives()
        {
            DbDataReader dr = dbContext.ExecReader("SELECT * FROM ArchiveMapping");

            List<List<Object>> tmpList = new List<List<Object>>();

            while (dr.Read())
            {
                List<Object> tmpSubList = new List<Object>
                {
                    dr.GetString(0),
                    dr.GetString(1)
                };
                tmpList.Add(tmpSubList);
            }

            dr.Close();
            return tmpList;
        }

       ////////////////////
       //PERMISSIONS
       ////////////////////
       

        public void CreatePermission(string permissionName)
        {
            CreatePermission(dbContext.GetHash(), permissionName);

        }
        public void CreatePermission(string hash, string permissionName)
        {

            if ((Int32)dbContext.ExecScalar($"SELECT COUNT(*) FROM ArchivePermissions WHERE name = ['{permissionName}']") == 0)
            {
                dbContext.ExecNonQuery($"INSERT INTO ArchivePermissions VALUES ( ['{hash}'], ['{permissionName}']);");
            }

        }

        public void DropPermission(string permissionHash)
        {
            //Delete all references to the given permission

            List<string> permSql = new List<string>
            {
                $"DELETE FROM ArchivePermMapping WHERE id_perm = ['{permissionHash}'];",
                $"DELETE FROM ArchiveUserPermMapping WHERE id_perm = ['{permissionHash}'];",
                $"DELETE FROM ArchivePermissions WHERE id = ['{permissionHash}'];"
            };

            dbContext.ExecTrans(permSql);
        }
        public void RenamePermission(string permissionHash, string newName)
        {
            dbContext.ExecNonQuery($"UPDATE ArchivePermissions SET name = ['{newName}'] WHERE id = ['{permissionHash}'];");
        }

        public List<List<Object>> GetPermissions()
        {

            DbDataReader dr = dbContext.ExecReader("SELECT * FROM ArchivePermissions");

            List<List<Object>> tmpList = new List<List<Object>>();

            while (dr.Read())
            {
                List<Object> tmpSubList = new List<Object>();
                tmpSubList.Add(dr.GetString(0));
                tmpSubList.Add(dr.GetString(1));
                tmpList.Add(tmpSubList);
            }

            dr.Close();

            return tmpList;
        }

        public void AddCol(string tableHash, string colName, string colType)
        {
            if (!dbContext.typeMap.ContainsKey(colType)) { return; }

            dbContext.ExecNonQuery($"ALTER TABLE [tb_{tableHash}] ADD COLUMN [{colName}] [{dbContext.typeMap[colType]}]");
        }

        public void RemoveCol(string colName, string tableHash)
        {
            dbContext.ExecNonQuery($"ALTER TABLE [tb_{tableHash}] DROP COLUMN [{colName}];");
        }

        public void AssignPerm(string userHash, string perm)
        {
            dbContext.ExecNonQuery($"INSERT IGNORE INTO [ArchiveUserPermMapping] VALUES ('{userHash}', '{perm}');");
        }
        public void RemovePerm(string userHash, int perm)
        {      
            dbContext.ExecNonQuery($"DELETE FROM [ArchiveUserPermMapping] WHERE id_perm=['{perm}'], id_user = ['{userHash}']);");
        }

        public void AssignTable(string tableHash, string perm)
        {
            dbContext.ExecNonQuery($"INSERT IGNORE INTO [ArchivePermMapping] VALUES (['{perm}'], ['{tableHash}']);");
        }

        public void RemoveTable(string tableHash, string perm)
        {
            dbContext.ExecNonQuery($"DELETE FROM [ArchivePermMapping] WHERE id_role=['{perm}'], id_table = ['{tableHash}']);");
        }

        public List<List<Object>> GetColumns(string tableHash)
        {
            DbDataReader dr = dbContext.ExecReader($"SELECT * FROM tb_{tableHash} WHERE 1=2");

            dr.Read();

            int tmp = dr.FieldCount;

            List<List<Object>> tmpList = new List<List<Object>>();

            for(int i = 0; i<tmp; i++)
            {
                List<Object> tmpSubList = new List<Object>();
                tmpSubList.Add(dr.GetName(i));
                tmpSubList.Add(dr.GetFieldType(i).Name);
                tmpList.Add(tmpSubList);
            }

            dr.Close();

            return tmpList;
        }

        public List<List<Object>> GetPermissionMapping(string permissionHash)
        {
            List<List<Object>> tmpList = new List<List<object>>();

            DbDataReader dr = dbContext.ExecReader("SELECT * FROM ArchiveMapping");

            List<List<Object>> listArchive = new List<List<object>>();

            while (dr.Read())
            {
                List<Object> tmpSubList = new List<object>
                {
                    dr.GetValue(0),
                    dr.GetValue(1)
                };

                listArchive.Add(tmpSubList);
            }

            dr.Close();

            

            foreach (List<Object> archive in listArchive)
            {
                int ret = (int)dbContext.ExecScalar($"SELECT COUNT(*) FROM ArchivePermMapping WHERE id_perm = '{permissionHash}' AND id_table = '{archive[0]}'");

                List<Object> permMapping = new List<Object>
                {
                    archive[0],
                    archive[1],
                    ret > 0
                };

                tmpList.Add(permMapping);
            }

            return tmpList;
        }
    }
}
