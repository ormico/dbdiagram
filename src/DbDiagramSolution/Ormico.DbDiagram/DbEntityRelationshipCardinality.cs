using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormico.DbDiagram
{
    internal enum DbEntityRelationshipCardinality
    {
        None,
        //Zero,
        ZeroOrOne,
        OneAndOnlyOne,
        OneOrMany,
        //One,
        //Many,
        ZeroOrMany
    }
}
