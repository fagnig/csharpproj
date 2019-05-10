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
        public DbConnection sqlCon;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

            //SqlCommand sqlCommand = new SqlCommand(sql, sqlCon);
            //sqlCommand.ExecuteNonQuery();
            // sqlCon.Close();
            sqlCon = this.Database.GetDbConnection();
        }

    }
}
