using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ormico.DbDiagram.Databases.Dataverse
{
    internal class DataverseReader
    {
        List<string> solutionFolders = new();

        public void AddSolution(string SolutionFolder)
        {
            this.solutionFolders.Add(SolutionFolder);
        }

        public Database Import()
        {
            Database rc = new();

            foreach(var folder in this.solutionFolders)
            {
                // load OptionSets
                LoadOptionSets(folder, rc);

                // load all entities
                LoadEntities(folder, rc);

                // load relationships
                LoadRelationships(folder, rc);
            }

            return rc;
        }

        void LoadEntities(string folderPath, Database db)
        {
            //Dictionary<string, DbEntity>
            var entities = db.EntitiesByName;
            var entitiesPath = Path.Combine(folderPath, "Entities");

            foreach (var subfolder in Directory.GetDirectories(entitiesPath))
            {
                string entityFilePathName = Path.Combine(subfolder, "Entity.xml");

                if (File.Exists(entityFilePathName))
                {
                    DbEntity entity = null;

                    XDocument xdoc = XDocument.Load(entityFilePathName);
                    var xEntity = xdoc.Element("Entity").Element("EntityInfo").Element("entity");
                    var tmpEntName = xEntity.Attribute("Name").Value;

                    if (entities.ContainsKey(tmpEntName))
                    {
                        entity = entities[tmpEntName];
                    }
                    else
                    {
                        entity = new(tmpEntName);
                    }

                    var xAttributes = xEntity.Element("attributes").Elements("attribute");
                    foreach (var xa in xAttributes)
                    {
                        var isNullable = true;
                        List<DbEntityConstraint> constraints = new();
                        List<DbEntityAttribute> attributes = new();
                        int pres = 0;
                        int scal = 0;

                        var colType = xa.Element("Type").Value;
                        DbEntityColumn newCol = new(
                            xa?.Element("Name")?.Value,
                            colType,
                            pres,
                            scal,
                            isNullable);
                        
                        var rl = xa.Element("RequiredLevel").Value;
                        
                        newCol.Attributes.Add(new DbEntityAttribute("RequiredLevel", rl));
                        attributes.Add(new DbEntityAttribute("RequiredLevel", rl));

                        if(string.Equals(rl, "required", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isNullable = false;
                        }
                        else if (string.Equals(rl, "systemrequired", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isNullable = false;
                        }
                        else if (string.Equals(rl, "recommended", StringComparison.InvariantCultureIgnoreCase))
                        {
                        }
                        else if (string.Equals(rl, "none", StringComparison.InvariantCultureIgnoreCase))
                        {
                        }

                        if (string.Equals(colType, "primarykey", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isNullable = false;
                            constraints.Add(new DbEntityConstraint());
                            entity.PrimaryKey.Add(newCol);
                        }
                        else if (string.Equals(colType, "state", StringComparison.InvariantCultureIgnoreCase))
                        {

                        }
                        else if (string.Equals(colType, "statuscode", StringComparison.InvariantCultureIgnoreCase))
                        {

                        }
                        else if (string.Equals(colType, "lookup", StringComparison.InvariantCultureIgnoreCase))
                        {
                            /* what are the other loopkup styles and how should they be mapped?
                             <LookupStyle>single</LookupStyle>
                             <LookupTypes />
                             */
                            // foreign key
                            attributes.Add(new DbEntityAttribute("FK", rl));
                            // instead of addign relationship here, wait until all tables are loaded and then read the relationship folder
                        }
                        else if (string.Equals(colType, "picklist", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // add optionset as a special entity to diagram and add relationship to this entity
                            // read optionset 
                            var xoptionset = xa.Element("optionset");
                            var xoptionsetName = xa.Element("OptionSetName");

                            if (xoptionset != null)
                            {
                                var optionset = CreateOptionSet(xoptionset);
                                db.EntitiesByName.Add(optionset.Name, optionset);
                                var optRelationship = new DbRelationship($"localchoice-{entity.Name}.{newCol.Name}.{optionset.Name}", entity, optionset, DbEntityRelationshipCardinality.None, DbEntityRelationshipCardinality.None);
                                optRelationship.PrimaryDbEntityColumns.Add(newCol);
                                db.RelationshipsByName.Add(optRelationship.Name, optRelationship);
                            }
                            else if(xoptionsetName != null)
                            {
                                string optionsetName = xoptionsetName.Value;
                                
                                if(db.EntitiesByName.TryGetValue(optionsetName, out var optionset))
                                {
                                    var optRelationship = new DbRelationship($"optionset-{entity.Name}.{newCol.Name}.{optionset.Name}", entity, optionset, DbEntityRelationshipCardinality.None, DbEntityRelationshipCardinality.None);
                                    optRelationship.PrimaryDbEntityColumns.Add(newCol);
                                    db.RelationshipsByName.Add(optRelationship.Name, optRelationship);
                                }
                                //todo: else what?
                            }
                        }
                        //else if (string.Equals(colType, "bit", StringComparison.InvariantCultureIgnoreCase))
                        //{
                        //    //bit is also a boolean/optionset combo but we normally just show it as a boolean unless you want to set what it is mapped to Yes/No, True/False, etc
                        //}
                        else
                        {
                            var maxLen = xa.Element("MaxLength");

                            if (maxLen != null)
                            {
                                //todo: TryParse
                                pres = int.Parse(maxLen.Value);
                            }

                            //todo: how do i get precision?

                            var accuracy = xa.Element("Accuracy");

                            if(accuracy != null)
                            {
                                //todo: TryParse
                                scal = int.Parse(accuracy.Value);
                            }
                        }

                        newCol.Precision = pres;
                        newCol.Scale = scal;
                        newCol.Attributes.AddRange(attributes);
                        newCol.IsNullable = isNullable;

                        entity.Columns.Add(newCol);
                    }

                    entities.Add(entity.Name, entity);
                }
            }
        }

        bool SkipAttribute(string attributeName)
        {
            bool rc = false;
            switch (attributeName?.ToLowerInvariant())
            {
                case "createdby":
                case "createdon":
                case "createdonbehalfby":
                case "importsequencenumber":
                case "modifiedby":
                case "modifiedon":
                case "modifiedonbehalfby":
                case "overriddencreatedon":
                case "statecode":
                case "statuscode":
                case "timezoneruleversionnumber":
                case "utcconversiontimezonecode":
                case "owningbusinessunit":
                case "owningteam":
                case "owninguser":
                case "ownerid":
                    rc = true;
                    break;
            }
            return rc;
        }

        void LoadOptionSets(string folderPath, Database db)
        {
            var optionsetFolder = Path.Combine(folderPath, "OptionSets");

            foreach (var opsFilePath in Directory.GetFiles(optionsetFolder, "*.xml"))
            {
                if (File.Exists(opsFilePath))
                {
                    XDocument xdoc = XDocument.Load(opsFilePath);
                    if(xdoc != null)
                    {
                        var newOptSet = CreateOptionSet(xdoc.Element("optionset"));
                        db.EntitiesByName.Add(newOptSet.Name, newOptSet);
                    }
                    else
                    {
                        throw new Exception($"xml file '{opsFilePath}' failed to open");
                    }
                }
            }
        }

        DbEntity CreateOptionSet(XElement xoptionset)
        {
            if(xoptionset == null)
            {
                throw new ArgumentNullException(nameof(xoptionset));
            }

            string name = xoptionset.Attribute("Name").Value;
            //todo: how many types of optionsets are there?
            string type = xoptionset.Element("OptionSetType").Value;
            string isCustomizable = xoptionset.Element("IsCustomizable").Value;

            DbEntity rc = new(name);
            rc.Attributes.Add(new DbEntityAttribute("OptionSetType", type));
            rc.Attributes.Add(new DbEntityAttribute("IsCustomizable", isCustomizable));

            foreach (var option in xoptionset.Element("options").Elements())
            {
                var v = option.Attribute("value").Value;
                //todo: labels looks like a collection instead of a single value. what does it alternate by? lang code? which lang code should I search for? should we specify?
                var l = option.Element("labels").Elements("label").FirstOrDefault().Attribute("description").Value;
                //todo: could make a subtype or or make DbEntityColumn an interface and create an Enum Column type
                rc.Columns.Add(new DbEntityColumn(l,v));
            }

            return rc;
        }

        private void LoadRelationships(string folder, Database db)
        {
            //Dictionary<string, DbEntity>
            var relationships = db.RelationshipsByName;
            var entities = db.EntitiesByName;
            var entitiesPath = Path.Combine(folder, "Other", "Relationships");

            foreach (var relationshipPath in Directory.GetFiles(entitiesPath, "*.xml"))
            {
                if (File.Exists(relationshipPath))
                {
                    DbEntity? entity = null;

                    XDocument xdoc = XDocument.Load(relationshipPath);
                    var xRelationships = xdoc.Element("EntityRelationships").Elements();

                    foreach (var xr in xRelationships)
                    {
                        var relName = xr.Attribute("Name").Value;
                        var referencingEntityName = xr.Element("ReferencingEntityName").Value;
                        var referencedEntityName = xr.Element("ReferencedEntityName").Value;
                        var referencingAttributeName = xr.Element("ReferencingAttributeName").Value;

                        //todo: need to better get cardinality
                        if (relationships.TryGetValue(relName, out var rel) == false)
                        {
                            if (entities.TryGetValue(referencingEntityName, out var pEntity) == false)
                            {
                                continue;
                                pEntity = new(referencingEntityName);
                            }

                            if (entities.TryGetValue(referencedEntityName, out var sEntity) == false)
                            {
                                continue;
                                sEntity = new(referencedEntityName);
                            }

                            //todo: why isn't Columns a Dictionary?
                            var sRefCol = pEntity.Columns.Where(x => x.Name == referencingEntityName).FirstOrDefault();
                            if (sRefCol == null)
                            {
                                // best guess at properties
                                sRefCol = new(referencedEntityName, "lookup", isNullable: true);
                                pEntity.Columns.Add(sRefCol);
                            }

                            rel = new(relName, pEntity, sEntity, DbEntityRelationshipCardinality.ZeroOrMany, DbEntityRelationshipCardinality.ZeroOrOne, new() { sRefCol }, null);
                            relationships.Add(relName, rel);
                        }
                        else
                        {
                            throw new Exception("what now?");
                        }
                    }
                }
            }
        }
    }
}
