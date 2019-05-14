using ArchiveProject.Data;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Logic
{
    public class TableDeployer
    {
        public Dictionary<string, string> typeMap;

        private readonly ApplicationDbContext dbContext;

        public TableDeployer(ApplicationDbContext context)
        {
            dbContext = context;

            typeMap = new Dictionary<string, string>
            {
                ["String"] = "nvarchar(256)",
                ["Boolean"] = "bit",
                ["Integer"] = "int",
                ["Date"] = "date"
            };

        }

        public int CreateTable(string name, string hash)
        {

            string sql = "CREATE TABLE tb_" + hash + "( id int NOT NULL IDENTITY(1,1) PRIMARY KEY);";

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = sql;
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();

            MapTable(hash, name);

            return 0;
        }

        public int CreateTable(List<KeyValuePair<string,string>> schema, string name, string hash)
        {  
            if(schema.Count() == 0)
            {
                return -1;
            }


            string sql = "CREATE TABLE tb_" + hash + "( id int NOT NULL IDENTITY(1,1) PRIMARY KEY, ";

            for(int i = 0; i< schema.Count(); i++)
            {
                sql += schema[i].Key + " " + typeMap[schema[i].Value];
                if(i != schema.Count() - 1)
                {
                    sql += ",";
                }
            }

            sql += ");";

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = sql;
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();

            MapTable(hash,name);

            return 0;
        }

        public int MapTable(string hash, string name)
        {
            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"INSERT INTO ArchiveMapping VALUES ( '{hash}', '{name}');";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();

            return 0;
        }

        public int deployRequiredTables()
        {

            dbContext.sqlCon.Open();
            DbCommand dc = dbContext.sqlCon.CreateCommand();
            

            try {
                dc.CommandText = "CREATE TABLE ArchiveMapping( id nvarchar(256), name nvarchar(256));";
                dc.ExecuteNonQuery();
            }
            catch (SqlException) {/*Table exists*/}

            try {
                dc.CommandText = "CREATE TABLE ArchivePermissions( id nvarchar(256) NOT NULL, name nvarchar(256));";
                dc.ExecuteNonQuery();
            }
            catch (SqlException) {/*Table exists*/}

            try
            {
                dc.CommandText = "SELECT COUNT(*) FROM ArchivePermissions WHERE id = 0";

                if ((Int32)dc.ExecuteScalar() == 0)
                {
                    dc.CommandText = "INSERT INTO ArchivePermissions VALUES ('Admin');";
                    dc.ExecuteNonQuery();
                }
            }
            catch (SqlException) {/*Admin role exists*/}

            try { 
                dc.CommandText = "CREATE TABLE ArchivePermMapping( id_perm nvarchar(256), id_table nvarchar(256));";
                dc.ExecuteNonQuery();
            }
            catch (SqlException) {/*Table exists*/}

            try {
                dc.CommandText = "CREATE TABLE ArchiveUserPermMapping( id_user nvarchar(450), id_perm int);";
                dc.ExecuteNonQuery();
            }
            catch (SqlException) {/*Table exists*/}


            dbContext.sqlCon.Close();


            return 0;
        }

        public void deployDefaultData(string currentUserHash)
        {
            List<KeyValuePair<string, string>> tmpTable = new List<KeyValuePair<string, string>>();
            

        }
    }
}
