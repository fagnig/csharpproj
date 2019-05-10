using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Logic
{
    public class TableDeployer
    {
        Dictionary<string, string> typeMap;

        public TableDeployer()
        {
            typeMap = new Dictionary<string, string>();

            typeMap["String"] = "nvarchar(256)";
            typeMap["Boolean"] = "bit";
            typeMap["Integer"] = "int";
            typeMap["Date"] = "date";
            
        }

        public int CreateTable(List<KeyValuePair<string,string>> schema, string name)
        {
            string conString, sql="";
            SqlConnection sqlCon;

            uint tmp = (uint) name.GetHashCode();
            string hashedname = tmp.ToString();


            conString = @"Server =.\; Database = ArchiveProject; Trusted_Connection = True; MultipleActiveResultSets = True";

            sql = "CREATE TABLE tb_" + hashedname + "(";

            for(int i = 0; i< schema.Count(); i++)
            {
                sql += schema[i].Key + " " + typeMap[schema[i].Value];
                if(i != schema.Count() - 1)
                {
                    sql += ",";
                }
            }

            sql += ");";

            sqlCon = new SqlConnection(conString);
            sqlCon.Open();

            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand(sql, sqlCon);
            sqlCommand.ExecuteNonQuery();

            sqlCon.Close();

            MapTable(hashedname,name);

            return 0;
        }

        public int MapTable(string hash, string name)
        {
            string conString, sql;
            SqlConnection sqlCon;

            conString = @"Server =.\; Database = ArchiveProject; Trusted_Connection = True; MultipleActiveResultSets = True";
            sql = $"INSERT INTO ArchiveMapping VALUES ( '{hash}', '{name}');";

            sqlCon = new SqlConnection(conString);
            sqlCon.Open();

            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand(sql, sqlCon);
            sqlCommand.ExecuteNonQuery();

            sqlCon.Close();


            return 0;
        }

        public int deployRequiredTables()
        {

            string conString, sql;
            SqlConnection sqlCon;
         
            conString = @"Server =.\; Database = ArchiveProject; Trusted_Connection = True; MultipleActiveResultSets = True";
            sql = "CREATE TABLE ArchiveMapping( hash nvarchar(256), name nvarchar(256));";

            sqlCon = new SqlConnection(conString);
            sqlCon.Open();
 
            try
            {
                SqlCommand sqlCommand;
                sqlCommand = new SqlCommand(sql, sqlCon);
                sqlCommand.ExecuteNonQuery();
            } catch (SqlException)
            {
                //Tables exist
            }

            sqlCon.Close();


            return 0;
        }
    }
}
