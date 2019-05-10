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


        public void updateFields(string id, string column, string table, string value)
        {

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"UPDATE [{table}] SET [{column}] = '{value}' WHERE id = '{id}'";
            dc.ExecuteNonQuery();

            dbContext.sqlCon.Close();

        }
    }
}
