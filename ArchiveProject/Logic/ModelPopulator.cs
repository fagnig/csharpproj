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


    }
}
