using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateRooms
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class AddRoom : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();
            Transaction ts = new Transaction(doc, "Создание помещений");
            ts.Start();

            foreach (var level in listLevel)
            {
                
                
                PlanTopology topology = doc.get_PlanTopology(level);
                PlanCircuitSet circuitSet = topology.Circuits;

                Phase phase = new FilteredElementCollector(doc)
                    .OfClass(typeof(Phase))
                    .Where(x => x.Name.Equals("Новая конструкция"))
                    .Cast<Phase>()
                    .FirstOrDefault();
                int i = 0;

                //считывается номер этажа
                string N = level.Name.Substring(level.Name.IndexOf(" ") + 1); ;
                foreach (PlanCircuit pC in circuitSet)
                {
                    Room roomUnplaced = doc.Create.NewRoom(phase);
                    Room room = doc.Create.NewRoom(roomUnplaced, pC);
                    i++;
                    //номер помещения генерируется в формате "номер этажа.порядковый номер в рамках этажа"
                    room.get_Parameter(BuiltInParameter.ROOM_NUMBER).Set($"{N}.{i}");
                }
               
            }
            ts.Commit();
            return Result.Succeeded;




        }
    }
}
