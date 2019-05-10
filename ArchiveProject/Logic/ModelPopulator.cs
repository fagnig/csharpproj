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
    public class ModelPopulator
    {
        private readonly ApplicationDbContext dbContext;

        public ModelPopulator(ApplicationDbContext context)
        {
            dbContext = context;

        }

        public ArchiveViewModel GetTable(string tableToGet)
        {
            ArchiveViewModel table = new ArchiveViewModel();

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"SELECT * from {tableToGet}";
            DbDataReader dr = dc.ExecuteReader();
            bool first = true;

            while (dr.Read())
            {
                List<Object> tmpList = new List<object>();

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (first)
                    {
                        table.typelist.Add(new KeyValuePair<string, Type>(dr.GetName(i), dr.GetFieldType(i)));
                       
                    }

                    tmpList.Add(dr.GetValue(i));
                }

                first = false;

                table.values.Add(tmpList);
            }

            dbContext.sqlCon.Close();

            return table;
        }
    }
}
