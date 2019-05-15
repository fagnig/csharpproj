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
    public class PreRequisiteManager
    {

        private readonly ApplicationDbContext dbContext;

        public ArchiveManager am;

        public PreRequisiteManager(ApplicationDbContext context)
        {
            dbContext = context;
            am = new ArchiveManager(dbContext);
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
                sql += schema[i].Key + " " + dbContext.typeMap[schema[i].Value];
                if(i != schema.Count() - 1)
                {
                    sql += ",";
                }
            }

            sql += ");";

            dbContext.ExecNonQuery(sql);

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

        public int DeployRequiredTables()
        {

            try {
                dbContext.ExecNonQuery("CREATE TABLE ArchiveMapping( id nvarchar(256), name nvarchar(256));");
            }
            catch (SqlException) { dbContext.TryCloseConnection(); }

            try {
                dbContext.ExecNonQuery("CREATE TABLE ArchivePermissions( id nvarchar(256) NOT NULL, name nvarchar(256));");
            }
            catch (SqlException) { dbContext.TryCloseConnection(); }

            try
            {

                if ((Int32)dbContext.ExecScalar("SELECT COUNT(*) FROM ArchivePermissions WHERE id = '0'") == 0)
                {
                    dbContext.ExecNonQuery("INSERT INTO ArchivePermissions VALUES ('0', 'Admin');");
                }
            }
            catch (SqlException) { dbContext.TryCloseConnection(); }

            try { 
                dbContext.ExecNonQuery("CREATE TABLE ArchivePermMapping( id_perm nvarchar(256), id_table nvarchar(256));");
            }
            catch (SqlException) { dbContext.TryCloseConnection(); }

            try {
                dbContext.ExecNonQuery("CREATE TABLE ArchiveUserPermMapping( id_user nvarchar(450), id_perm int);");
            }
            catch (SqlException) { dbContext.TryCloseConnection(); }

            return 0;
        }

        public void DeployDefaultData()
        {
            List<KeyValuePair<string, string>> tmpTable = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", "String"),
                new KeyValuePair<string, string>("Phone", "Integer"),
                new KeyValuePair<string, string>("DateofBirth", "Date"),
                new KeyValuePair<string, string>("Address", "String")
            };

            string hash = dbContext.GetHash();

            CreateTable(tmpTable, "People",hash);
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(Name,Phone,DateofBirth,Address) VALUES('Jens Jensen',12345678,DATE '2000-03-12','Jenisgade');");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(Name,Phone,DateofBirth,Address) VALUES('Bent Bentsen',23456789,DATE '2001-05-20','Fenisgade');");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(Name,Phone,DateofBirth,Address) VALUES('Ib Ibsen',34567890,DATE '2002-08-23','Tenisgade');");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(Name,Phone,DateofBirth,Address) VALUES('Karl Karlsen',45678901,DATE '2003-02-17','Klenisgade');");

            tmpTable = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("KeyName", "String"),
                new KeyValuePair<string, string>("KeyId", "Integer")
            };

            hash = dbContext.GetHash();

            CreateTable(tmpTable, "Keys", hash);
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Police', 123414);");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Master', 12983879);");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Bedroom', 123855);");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Bathroom', 41231231);");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Tool Closet', 53298);");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Shed', 942891);");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Safe', 223499);");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Hidden Stash', 234812);");
            dbContext.ExecNonQuery($"INSERT INTO tb_{hash}(KeyName,KeyId) VALUES('Secret Vault', 842184);");
        }
    }
}
