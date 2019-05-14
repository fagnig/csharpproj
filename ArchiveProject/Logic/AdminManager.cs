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

        public void CreateArchive(string name)
        {
            string hash = dbContext.GetHash();

            List<string> permSql = new List<string>
            {
                $"CREATE TABLE [tb_{hash}] ( id int NOT NULL IDENTITY(1,1) PRIMARY KEY);",//Create new table
                $"INSERT INTO ArchiveMapping VALUES ( ['{hash}'], ['{name}']);" //Map new table
            };

            dbContext.ExecTrans(permSql);
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
            dbContext.ExecNonQuery($"UPDATE ArchiveMapping SET name = '{tableNewName}' WHERE id = {tableHash}");
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

            if((Int32)dbContext.ExecScalar($"SELECT COUNT(*) FROM ArchivePermissions WHERE name = ['{permissionName}']") == 0)
            {
                dbContext.ExecNonQuery($"INSERT INTO ArchivePermissions VALUES ( ['{dbContext.GetHash()}'], ['{permissionName}']);");
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
    }
}
