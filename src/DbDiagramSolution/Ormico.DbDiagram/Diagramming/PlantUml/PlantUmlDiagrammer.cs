using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ormico.DbDiagram.Diagramming.PlantUml
{
    internal class PlantUmlDiagrammer
    {
        public PlantUmlDiagrammer(XDocument configuration, Database db)
        {
            Configuration = configuration;
            Db = db;
        }

        public XDocument Configuration { get; set; }
        public Database Db { get; }
        public Uri PlantUmlServer { get; set; }

        public void CreateDiagrams()
        {
            string outFolder = Configuration.Element("DbDiagram").Element("Output").Element("Folder").Value;
            if(Directory.Exists(outFolder))
            {
                //todo: this will delete directory and all contents. i want to delete contents w/o deleting dir
                Directory.Delete(outFolder, true);
            }

            Directory.CreateDirectory(outFolder);

            // create new file 
            //todo: user project name instead of 'erd'
            using var file = File.Create(Path.Combine(outFolder, "erd.plantuml"));
            using var stream = new StreamWriter(file);

            // write begin
            /*
            @startuml
            Entity01 }|..|| Entity02
            Entity03 }o..o| Entity04
            Entity05 ||--o{ Entity06
            Entity07 |o--|| Entity08
            @enduml
            */
            stream.WriteLine("@startuml");

            // options
            stream.WriteLine("' options");
            stream.WriteLine("hide circle");
            stream.WriteLine("skinparam linetype ortho");
            stream.WriteLine();

            // write entities
            stream.WriteLine("' entities");
            foreach (var entity in Db.EntitiesByName.Values)
            {
                stream.WriteLine($"entity \"{entity.Name}\" {{");
                // primary key
                foreach(var pk in entity.PrimaryKey)
                {
                    // all PK fields should be required, right?
                    WriteEntityColumn(stream, pk);
                }
                stream.WriteLine("--");

                // columns
                foreach (var col in entity.Columns.Where(c => entity.PrimaryKey.Contains(c) == false))
                {
                    // all PK fields should be required, right?
                    WriteEntityColumn(stream, col);
                }

                stream.WriteLine("}");
                stream.WriteLine();
            }

            // write relationships
            //stream.WriteLine();
            stream.WriteLine("' relationships");
            foreach (var relationship in Db.RelationshipsByName.Values)
            {
                string pCard = GetCardinalityString(relationship.PrimaryCardinality, RelationshipRole.Primary);
                string sCard = GetCardinalityString(relationship.SecondaryCardinality, RelationshipRole.Secondary);
                stream.WriteLine($"{relationship.Primary.Name} {pCard}--{sCard} {relationship.Secondary.Name}");
            }

            //stream.WriteLine();

            // write end
            stream.WriteLine("@enduml");
        }

        private static void WriteEntityColumn(StreamWriter stream, DbEntityColumn pk)
        {
            var required = pk.IsNullable ? "" : "*";
            stream.Write($"    {required}{pk.Name} {pk.Type}");
            if (pk.Precision != 0 && pk.Scale == 0)
            {
                stream.Write($"({pk.Precision})");
            }
            else if (pk.Precision == 0 && pk.Scale != 0)
            {
                stream.Write($"(X,{pk.Scale})");
            }
            else if (pk.Precision != 0 && pk.Scale != 0)
            {
                stream.Write($"({pk.Precision},{pk.Scale})");
            }

            stream.WriteLine();
        }

        enum RelationshipRole
        {
            Primary,
            Secondary
        }

        string GetCardinalityString(DbEntityRelationshipCardinality cardinality, RelationshipRole relationshipRole)
        {
            string rc = string.Empty;
            if(relationshipRole == RelationshipRole.Primary)
            {
                switch(cardinality)
                {
                    case DbEntityRelationshipCardinality.OneAndOnlyOne:
                        rc = "||";
                        break;
                    case DbEntityRelationshipCardinality.ZeroOrOne:
                        rc = "|o";
                        break;
                    case DbEntityRelationshipCardinality.ZeroOrMany:
                        rc = "}o";
                        break;
                    case DbEntityRelationshipCardinality.OneOrMany:
                        rc = "}|";
                        break;
                }
            }
            else
            {
                switch (cardinality)
                {
                    case DbEntityRelationshipCardinality.OneAndOnlyOne:
                        rc = "||";
                        break;
                    case DbEntityRelationshipCardinality.ZeroOrOne:
                        rc = "o|";
                        break;
                    case DbEntityRelationshipCardinality.ZeroOrMany:
                        rc = "o{";
                        break;
                    case DbEntityRelationshipCardinality.OneOrMany:
                        rc = "|{";
                        break;
                }
            }

            return rc;
        }
    }
}
