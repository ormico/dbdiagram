using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormico.DbDiagram
{
    internal class DbEntityConstraint
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<DbEntityColumn> Columns { get; set; }
    }
}
