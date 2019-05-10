using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArchiveProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

            //SqlCommand sqlCommand = new SqlCommand(sql, sqlCon);
            //sqlCommand.ExecuteNonQuery();
            // sqlCon.Close();
            this.Database.GetDbConnection().Open();
        }

        public void DBTEST()
        {
            string sql = $"UPDATE [AspNetUsers] SET [UserName] = 'asd'";

            DbCommand dc = this.Database.GetDbConnection().CreateCommand();
            dc.CommandText = sql;

            dc.ExecuteNonQuery();
        }
    }
}
