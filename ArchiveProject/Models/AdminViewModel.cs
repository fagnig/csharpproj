using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Models
{
    public class AdminViewModel
    {

        public List<List<Object>> permissions;
        public List<List<Object>> users;
        public List<List<Object>> table;

        public AdminViewModel()
        {
            permissions = new List<List<Object>>();
            users = new List<List<Object>>();
            table = new List<List<Object>>();
        }
    }
}
