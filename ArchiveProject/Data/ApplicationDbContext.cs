using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArchiveProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbConnection sqlCon;

        public Dictionary<string, string> typeMap;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            typeMap = new Dictionary<string, string>
            {
                ["String"] = "nvarchar(256)",
                ["Boolean"] = "bit",
                ["Int32"] = "int",
                ["Date"] = "date"
            };

            sqlCon = this.Database.GetDbConnection();
        }



        public void DropTable(string tableToDrop)
        {
            sqlCon.Open();

            DbCommand dc = sqlCon.CreateCommand();
            dc.CommandText = $"DROP TABLE [{tableToDrop}]";
            dc.ExecuteNonQuery();

            sqlCon.Close();
        }

        public string GetHash()
        {
            Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(DateTime.Now.ToString("dd/MM/yyyy - hh:mm:ss"), 0x10, 0x3e8);
            byte[] dst = new byte[0x31];

            string tmp = Convert.ToBase64String(bytes.GetBytes(0x20));

            tmp.Replace('+','a');
            tmp.Replace('=', 'a');
            tmp.Replace('/', 'a');

            return tmp;
        }

        public void ExecNonQuery(string sqlString)
        {
            sqlCon.Open();

            DbCommand dc = sqlCon.CreateCommand();
            dc.CommandText = sqlString;
            dc.ExecuteNonQuery();

            sqlCon.Close();
        }

        public Object ExecScalar(string sqlString)
        {
            sqlCon.Open();

            DbCommand dc = sqlCon.CreateCommand();
            dc.CommandText = sqlString;
            Object ret = dc.ExecuteScalar();

            sqlCon.Close();

            return ret;
        }

        public DbDataReader ExecReader(string sqlString)
        {
            sqlCon.Open();

            DbCommand dc = sqlCon.CreateCommand();
            dc.CommandText = sqlString;
            DbDataReader ret = dc.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

            return ret;
        }

        public void ExecTrans(List<string> sqlStrings)
        {
            sqlCon.Open();

            DbCommand dc = sqlCon.CreateCommand();
            try
            {
                dc.CommandText = "BEGIN TRANSACTION;";
                dc.ExecuteNonQuery();

                foreach (string sqlString in sqlStrings)
                {
                    dc.CommandText = sqlString;
                    dc.ExecuteNonQuery();
                }

                dc.CommandText = "COMMIT;";
                dc.ExecuteNonQuery();

            }
            catch (SqlException)
            {
                dc.CommandText = "ROLLBACK;";
                dc.ExecuteNonQuery();
            }
            sqlCon.Close();
        }
    }
}
