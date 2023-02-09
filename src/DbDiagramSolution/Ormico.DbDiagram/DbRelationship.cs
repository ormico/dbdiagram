using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormico.DbDiagram
{
    internal class DbRelationship
    {
        public DbRelationship(string name, DbEntity primary, DbEntity secondary, DbEntityRelationshipCardinality primaryCardinality, DbEntityRelationshipCardinality secondaryCardinality, List<DbEntityColumn> primaryDbEntityColumns, List<DbEntityColumn> secondaryDbEntityColumns)
        {
            Name = name;
            Primary = primary;
            Secondary = secondary;
            PrimaryCardinality = primaryCardinality;
            SecondaryCardinality = secondaryCardinality;
            PrimaryDbEntityColumns = primaryDbEntityColumns;
            SecondaryDbEntityColumns = secondaryDbEntityColumns;
        }

        public DbRelationship(string name, DbEntity primary, DbEntity secondary, DbEntityRelationshipCardinality primaryCardinality, DbEntityRelationshipCardinality secondaryCardinality)
        {
            Name = name;
            Primary = primary;
            Secondary = secondary;
            PrimaryCardinality = primaryCardinality;
            SecondaryCardinality = secondaryCardinality;
        }

        public string Name { get; set; }
        public DbEntity Primary { get; set; }
        public DbEntity Secondary { get; set; }

        public DbEntityRelationshipCardinality PrimaryCardinality { get; set; }
        public DbEntityRelationshipCardinality SecondaryCardinality { get; set; }

        public List<DbEntityColumn> PrimaryDbEntityColumns { get; set; } = new();
        public List<DbEntityColumn> SecondaryDbEntityColumns { get; set; } = new();
    }
}
