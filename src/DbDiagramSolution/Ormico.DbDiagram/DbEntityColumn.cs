using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ormico.DbDiagram
{
    internal class DbEntityColumn
    {
        public DbEntityColumn(string name, string type, int precision = 0, int scale = 0, bool isNullable = false)
        {
            Name = name;
            Type = type;
            Precision = precision;
            Scale = scale;
            IsNullable = isNullable;
        }

        public string Name { get; set; }
        public string Type { get; set; }

        /// <summary>
        /// For numeric types this will represent the precision of the number or the total number of digits 
        /// before and after the decimal, in most database systems.
        /// For string types, this will represent the length of the string. 
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Scale { get; set; }

        public bool IsNullable { get; set; }

        public List<DbEntityAttribute> Attributes { get; set; } = new();
    }
}
