using ArchiveProject.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Logic
{
    public class ModelPopulator
    {
        public ArchiveViewModel GetTable(string tableToGet)
        {
            ArchiveViewModel table = new ArchiveViewModel();

            string conString, sql;
            SqlConnection sqlCon;
            conString = @"Server=.\SQLEXPRESS;Initial Catalog=ArchiveProject;Trusted_Connection=True";
            sql = $"SELECT * from {tableToGet}";

            sqlCon = new SqlConnection(conString);
            sqlCon.Open();

            SqlCommand sqlCommand;
            SqlDataReader sqlDataReader;

            sqlCommand = new SqlCommand(sql, sqlCon);

            sqlDataReader = sqlCommand.ExecuteReader();

            bool first = true;

            while (sqlDataReader.Read())
            {
                List<Object> tmpList = new List<object>();

                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                {
                    if (first)
                    {
                        table.typelist.Add(new KeyValuePair<string, Type>(sqlDataReader.GetName(i), sqlDataReader.GetFieldType(i)));
                       
                    }

                    

                    tmpList.Add(sqlDataReader.GetValue(i));
                }

                first = false;

                table.values.Add(tmpList);
            }


            sqlCon.Close();

            return table;
        }
    }
}
