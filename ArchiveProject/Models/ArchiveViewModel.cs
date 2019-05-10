using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveProject.Models
{
    public class ArchiveViewModel
    {
        public string tableTitle;
        public string tableHash;


        public List<KeyValuePair<string, Type>> typelist;

        public List<List<Object>> values;

        public ArchiveViewModel()
        {
            this.tableTitle = "";
            this.tableHash = "";
            typelist = new List<KeyValuePair<string, Type>>();
            values = new List<List<object>>();
        }
    }
}
