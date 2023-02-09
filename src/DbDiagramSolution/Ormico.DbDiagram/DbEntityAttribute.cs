using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormico.DbDiagram
{
    internal class DbEntityAttribute
    {
        public DbEntityAttribute(string name, string? value = null)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string? Value { get; set; }
    }
}
