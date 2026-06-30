using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Link_Tool_R25
{
    [Transaction(TransactionMode.Manual)]
    public class TagAutoColor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument; // Get the active document
            Document doc = uidoc.Document; // Get the document
            View activeView = doc.ActiveView; // Get the active view

            using (Transaction tx = new Transaction(doc, "Tag Auto Color")) // Start a new transaction
            {
                tx.Start();

                var tags = new FilteredElementCollector (doc, activeView.Id)
                    .OfClass(typeof(IndependentTag)) // Filter for IndependentTag elements
                    .Cast<IndependentTag>() // Cast the elements to IndependentTag
                    .ToList(); // Convert to a list

                List<string> idList = new List<string>();
                List<string> elementList = new List<string>(); // collect host/system name strings
                List<string> elementColors = new List<string>(); // collect host/system name strings

                int updatedCount = 0; 

                foreach (var tag in tags) // loop through each tag
                {
                    ICollection<LinkElementId> taggedIds = tag.GetTaggedElementIds(); // Get the IDs of the elements that the tag is tagging
                    idList.Add(taggedIds.ToString());
                    updatedCount++; // Increment the count of updated tags

                    foreach (LinkElementId taggedId in taggedIds)
                    {
                        //only handle local elements, not linked elements
                        if (taggedId.HostElementId == ElementId.InvalidElementId) continue;

                        Element host = doc.GetElement(taggedId.HostElementId); // Get the element that the tag is tagging
                        MEPCurve mepCurve = host as MEPCurve;
                        if (mepCurve == null) continue;

                        MEPSystem system = mepCurve.MEPSystem; // Get the MEP system of the element
                        if (system == null) continue;

                        MEPSystemType systemType = doc.GetElement(system.GetTypeId()) as MEPSystemType; // Get the MEP system type of the element
                        if (systemType == null) continue;

                        elementList.Add(systemType.Name); // Add the host element's name to the list
                        elementColors.Add($"{systemType.Name} - RGB({systemType.LineColor.Red}, {systemType.LineColor.Green}, {systemType.LineColor.Blue})");

                        Color tagColor = systemType.LineColor; // Get the line color of the MEP system type

                        OverrideGraphicSettings ogs = new OverrideGraphicSettings(); // Create a new OverrideGraphicSettings object
                        ogs.SetProjectionLineColor(tagColor); // Set the projection line color to the tag color
                        ogs.SetCutLineColor(tagColor); // Set the cut line color to the tag color

                        activeView.SetElementOverrides(tag.Id, ogs); // Apply the override graphic settings to the tag

                    }


                }

                tx.Commit();
                TaskDialog.Show("Tag Auto Color", $"Updated {updatedCount} tags with system colors."); // Show a dialog with the count of updated tags
                TaskDialog.Show("Tagged Element IDs", $"Tagged Element IDs:\n{string.Join("\n", idList)}"); // Show a dialog with the tagged element IDs
                TaskDialog.Show("Tagged Element Host Names", $"Tagged Element Host Names:\n{string.Join("\n", elementList)}"); // Show a dialog with the tagged element host names
                TaskDialog.Show("Tagged Element Colors", $"Tagged Element Colors:\n{string.Join("\n", elementColors)}"); // Show a dialog with the tagged element colors


            }
            return Result.Succeeded;
        }
    }
}
