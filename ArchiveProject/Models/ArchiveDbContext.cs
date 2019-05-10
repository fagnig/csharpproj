using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Models
{
    public class ArchiveDbContext : DbContext
    {
        public ArchiveDbContext()
        {  
            
        }
        public ArchiveDbContext(DbContextOptions<ArchiveDbContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
