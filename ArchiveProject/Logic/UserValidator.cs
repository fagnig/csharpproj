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
    public class UserValidator
    {
        private readonly ApplicationDbContext dbContext;

        public UserValidator(ApplicationDbContext context)
        {
            dbContext = context;

        }

        public List<int> getTableRoles(string tableHash)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"SELECT * FROM ArchivePermMapping WHERE id_table = '{tableHash}'";

            DbDataReader dr = dc.ExecuteReader();

            List<int> tmpList = new List<int>();

            while (dr.Read())
            {
                tmpList.Add((int)dr.GetValue(0));
            }

            dbContext.sqlCon.Close();

            return tmpList;
        }
        public List<int> getUserRoles(string userHash)
        {

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"SELECT * FROM ArchiveUserPermMapping WHERE id_user = '{userHash}'";

            DbDataReader dr = dc.ExecuteReader();

            List<int> tmpList = new List<int>();

            while (dr.Read())
            {
                tmpList.Add((int)dr.GetValue(1));
            }

            dbContext.sqlCon.Close();

            return tmpList;
        }

        public bool isUserAdmin(string userHash)
        {
            List<int> userRoles = getUserRoles(userHash);

            return userRoles.Contains(0);
        }
        public bool canUserAccess(string userHash, string tableHash)
        {
            if (isUserAdmin(userHash)) {
                return true;
            }

            List<int> userRoles = getUserRoles(userHash);
            List<int> tableRoles = getTableRoles(tableHash);

            foreach (int role in userRoles)
            {
                if (tableRoles.Contains(role)) { return true; }
            }

            return false;
        }

        public List<List<Object>> getUserTableList(string userHash)
        {
            string sqlBuild = $"SELECT * FROM ArchivePermMapping WHERE id_role IN (";

            List<int> userRoles = getUserRoles(userHash);

            if (userRoles.Count() == 0)
            {
                return new List<List<Object>>();
            }

            for (int i = 0; i < userRoles.Count(); i++)
            {

                sqlBuild += $"'{userRoles[i]}'";
                if (i != userRoles.Count() - 1)
                {
                    sqlBuild += ", ";
                }
            }

            sqlBuild += ");";

            if (isUserAdmin(userHash))
            {
                sqlBuild = "SELECT * FROM ArchivePermMapping";
            }

            dbContext.sqlCon.Open();
            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = sqlBuild;

            DbDataReader dr = dc.ExecuteReader();

            List<string> tmpHashes = new List<string>();

            while (dr.Read())
            {
                tmpHashes.Add(dr.GetString(1));
            }
            dr.Close();

            ////////////////////////////////
            ///
            sqlBuild = $"SELECT * FROM ArchiveMapping WHERE id IN (";
            for (int i = 0; i < tmpHashes.Count(); i++)
            {

                sqlBuild += $"'{tmpHashes[i]}'";
                if (i != tmpHashes.Count() - 1)
                {
                    sqlBuild += ", ";
                }
            }

            sqlBuild += ");";

            dc.CommandText = sqlBuild;

            dr = dc.ExecuteReader();

            List<List<Object>> tmpList = new List<List<Object>>();

            while (dr.Read())
            {
                List<Object> tmpSubList = new List<Object>();
                tmpSubList.Add(dr.GetString(0));
                tmpSubList.Add(dr.GetString(1));
                tmpList.Add(tmpSubList);
            }

            dr.Close();
            dbContext.sqlCon.Close();
            return tmpList;
        }


        public List<List<Object>> getAllPermissions()
        {

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();

            dc.CommandText = "SELECT * FROM ArchivePermissions";

            DbDataReader dr = dc.ExecuteReader();

            dr = dc.ExecuteReader();

            List<List<Object>> tmpList = new List<List<Object>>();

            while (dr.Read())
            {
                List<Object> tmpSubList = new List<Object>();
                tmpSubList.Add(dr.GetInt32(0));
                tmpSubList.Add(dr.GetString(1));
                tmpList.Add(tmpSubList);
            }

            return tmpList;
        }

        public string GetHash()
        {
            return ((uint)DateTime.Now.ToString("dd/MM/yyyy - hh:mm:ss").GetHashCode()).ToString();
        }
    }
}
