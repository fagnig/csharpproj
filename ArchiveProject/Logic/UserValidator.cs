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
    public class UserValidator
    {
        private readonly ApplicationDbContext dbContext;

        public UserValidator(ApplicationDbContext context)
        {
            dbContext = context;

        }


        public List<int> getUserRoles(string userHash)
        {

            dbContext.sqlCon.Open();

            DbCommand dc = dbContext.sqlCon.CreateCommand();
            dc.CommandText = $"SELECT * FROM ArchiveUserRoleMapping WHERE id_user = '{userHash}'";
      
            DbDataReader dr = dc.ExecuteReader();

            List<int> tmpList = new List<int>();

            while (dr.Read())
            {
                tmpList.Add((int)dr.GetValue(1));
            }

            dbContext.sqlCon.Close();

            return tmpList;
        }
    }
}
