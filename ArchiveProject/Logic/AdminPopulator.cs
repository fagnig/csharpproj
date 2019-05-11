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
    public class AdminPopulator
    {
        private readonly ApplicationDbContext dbContext;

        public AdminPopulator(ApplicationDbContext context)
        {
            dbContext = context;

        }

        public List<List<Object>> getAllPermissions()
        {

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = "SELECT * FROM ArchivePermissions";

            DbDataReader dr = dc.ExecuteReader();

            List<List<Object>> tmpList = new List<List<Object>>();

            while (dr.Read())
            {
                List<Object> tmpSubList = new List<Object>();
                tmpSubList.Add(dr.GetInt32(0));
                tmpSubList.Add(dr.GetString(1));
                tmpList.Add(tmpSubList);
            }

            dbContext.sqlCon.Close();

            return tmpList;
        }

        public List<List<Object>> getAllUsers()
        {

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = "SELECT * FROM AspNetUsers";

            DbDataReader dr = dc.ExecuteReader();

            List<List<Object>> tmpList = new List<List<Object>>();

            while (dr.Read())
            {
                List<Object> tmpSubList = new List<Object>();

                for(int i=0; i < dr.FieldCount; i++)
                {
                    tmpSubList.Add(dr.GetValue(i));
                }
                
                tmpList.Add(tmpSubList);
            }

            dbContext.sqlCon.Close();

            return tmpList;
        }



        public void UpdatePermission(int id, string newName)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"UPDATE ArchivePermissions SET name = '{newName}' WHERE id = {id}";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }
        public void UpdateTable(string tableHash, string tableNewName)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = $"UPDATE ArchiveMapping SET name = '{tableNewName}' WHERE id = {tableHash}";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }
    }
}
