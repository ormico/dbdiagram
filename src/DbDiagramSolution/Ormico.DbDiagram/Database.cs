using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormico.DbDiagram
{
    internal class Database
    {
        public Database()
        {
        }

        public SortedDictionary<string, DbEntity> EntitiesByName { get; private set; } = new();
        public SortedDictionary<string, DbRelationship> RelationshipsByName { get; private set; } = new();
    }
}
