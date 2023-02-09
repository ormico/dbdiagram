using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormico.DbDiagram
{
    internal class DbEntity
    {
        public DbEntity(string dbNamespace, string name)
        {
            Namespace = dbNamespace;
            Name = name;
        }
        public DbEntity(string name)
        {
            Namespace = string.Empty;
            Name = name;
        }

        public string Namespace { get; set; }
        public string Name { get; set; }
        public List<DbEntityAttribute> Attributes { get; set; } = new();
        public List<DbEntityColumn> Columns { get; set; } = new();

        public List<DbEntityConstraint> Constraints { get; set; } = new();

        public List<DbEntityColumn> PrimaryKey { get; set; } = new();
 
        public DbEntityColumn? GetColumnByName(string Name)
        {
            return Columns.FirstOrDefault(i => i.Name == Name);
        }

        public DbEntityConstraint? GetConstraintByName(string Name)
        {
            return Constraints.FirstOrDefault(i => i.Name == Name);
        }

        public DbEntityAttribute? GetAttributeByName(string Name)
        {
            return Attributes.FirstOrDefault(i => i.Name == Name);
        }
    }
}
