// See https://aka.ms/new-console-template for more information
using Ormico.DbDiagram.Databases.Dataverse;
using Ormico.DbDiagram.Diagramming.PlantUml;
using System.Xml.Linq;

//todo: read config
XDocument xConfig = XDocument.Load("config.json");
Console.WriteLine(xConfig.ToString());

DataverseReader reader = new DataverseReader();


//C:\Users\ZackMoore\Projects\dbexamples\ScavengerHunt\Dataverse\ormico_ScavengerHunt\src

reader.AddSolution(xConfig.Element("DbDiagram").Element("Dataverse").Element("SolutionFolder").Value);
var db = reader.Import();



PlantUmlDiagrammer plantuml = new PlantUmlDiagrammer(xConfig, db);
plantuml.CreateDiagrams();