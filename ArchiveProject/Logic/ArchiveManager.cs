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
    public class ArchiveManager
    {
        private readonly ApplicationDbContext dbContext;
        PermissionManager pm;

        public ArchiveManager(ApplicationDbContext context)
        {
            dbContext = context;
            pm = new PermissionManager(dbContext);
        }

        public object InsertRow(string tableHash)
        {
            
            DbDataReader dr = dbContext.ExecReader($"SELECT * FROM [tb_{tableHash}] WHERE 1=2;");
            dr.Read();
            string columns = $"[{dr.GetName(1)}]";
            string values = "NULL";
            for (int i = 2; i < dr.FieldCount; i++)
            {
                columns += $",[{dr.GetName(i)}]";
                values += ",NULL";
            }
            dr.Close();

            return dbContext.ExecScalar($"INSERT INTO tb_{tableHash}({columns}) OUTPUT INSERTED.id VALUES({values});");
        }

        public void DropRow(string tableHash, string id)
        {
            dbContext.ExecNonQuery($"DELETE FROM [tb_{tableHash}] WHERE id = '{id}'");
        }

        public void UpdateField(string id, string column, string table, string value)
        {
            dbContext.ExecNonQuery($"UPDATE [tb_{table}] SET [{column}] = '{value}' WHERE id = '{id}'");
        }

        public ArchiveViewModel GetTable(string archiveId)
        {
            ArchiveViewModel archive = new ArchiveViewModel();

            DbDataReader dr;
            try { dr = dbContext.ExecReader($"SELECT * FROM [tb_{archiveId}] ORDER BY id"); }
            catch (SqlException) { return new ArchiveViewModel(); }
            bool first = true;

            while (dr.Read())
            {
                List<Object> tmpList = new List<object>();

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (first)
                    {
                        archive.typelist.Add(new KeyValuePair<string, Type>(dr.GetName(i), dr.GetFieldType(i)));

                    }

                    tmpList.Add(dr.GetValue(i));
                }

                first = false;

                archive.values.Add(tmpList);
            }

            dr.Close();

            archive.tableHash = archiveId;
            archive.tableTitle = (string) dbContext.ExecScalar($"SELECT [name] FROM [ArchiveMapping] WHERE [id] = '{archiveId}';");

            return archive;
        }


        public List<List<Object>> GetUserTableList(string userHash)
        {
            string sqlBuild = $"SELECT * FROM ArchivePermMapping WHERE id_perm IN (";

            List<int> userRoles = pm.GetUserRoles(userHash);

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



            DbDataReader dr = dbContext.ExecReader(sqlBuild);

            List<string> tmpHashes = new List<string>();

            while (dr.Read())
            {
                tmpHashes.Add(dr.GetString(1));
            }
            dr.Close();

            ////////////////////////////////
            ///

            if (tmpHashes.Count() == 0)
            {
                return new List<List<Object>>();
            }

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

            dr = dbContext.ExecReader(sqlBuild);

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
