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
    public class ModelUpdater
    {
        private readonly ApplicationDbContext dbContext;

        public ModelUpdater(ApplicationDbContext context)
        {
            dbContext = context;

        }

        public object insertRow(string tableHash)
        {
            dbContext.sqlCon.Open();
            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"SELECT * FROM [tb_{tableHash}] WHERE 1=2;";
            DbDataReader dr = dc.ExecuteReader();
            dr.Read();
            string columns = $"[{dr.GetName(1)}]";
            string values = "NULL";
            for(int i = 2; i < dr.FieldCount; i++)
            {
                    columns += $",[{dr.GetName(i)}]";
                    values += ",NULL";
            }
            dr.Close();

            dc.CommandText = $"INSERT INTO tb_{tableHash}({columns}) OUTPUT INSERTED.id VALUES({values});";
            Object id = dc.ExecuteScalar();
            dbContext.sqlCon.Close();
            return id;
        }

        public void updateFields(string id, string column, string table, string value)
        {

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"UPDATE [tb_{table}] SET [{column}] = '{value}' WHERE id = '{id}'";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();

        }

        public void dropRow(string tableHash, string id)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"DELETE FROM [tb_{tableHash}] WHERE id = '{id}'";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }

        public void dropTable(string tableHash)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"DROP TABLE [tb_{tableHash}]";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }

        public void addColToTable(string colName, string colType, string tableHash)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"ALTER TABLE [tb_{tableHash}] ADD COLUMN [{colName}] [{colType}]";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }

        public void removeColFromTable(string colName, string tableHash)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"ALTER TABLE [tb_{tableHash}] DROP COLUMN [{colName}]";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();
        }

    }
}
