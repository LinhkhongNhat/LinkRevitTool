using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;



namespace Link_Tool_R25
{
    [Transaction(TransactionMode.Manual)]
    public class SheetGenerate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the current document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            try
            {

                ICollection<ElementId> selectedIds = sel.GetElementIds();
                if (selectedIds.Count == 0)
                {
                    TaskDialog.Show("Error", "Please select at least one element.");
                    return Result.Failed;
                }

                // Get the selected element
                Element selectedElement = doc.GetElement(selectedIds.FirstOrDefault());

                if (selectedElement == null)
                {
                    TaskDialog.Show("Error", "Please select an element.");
                    return Result.Failed;
                }

                // Check if the selected element is a view
                if (selectedElement is View view)
                {
                    // Proceed with creating sheet and adding view
                    using (Transaction tx = new Transaction(doc, "Sheet Generate"))
                    {
                        tx.Start();

                        // Get all title blocks
                        FilteredElementCollector collector = new FilteredElementCollector(doc)
                            .OfCategory(BuiltInCategory.OST_TitleBlocks)
                            .OfClass(typeof(FamilySymbol));

                        FamilySymbol titleBlock = collector.FirstOrDefault() as FamilySymbol;

                        if (titleBlock == null)
                        {
                            TaskDialog.Show("Error", "No title blocks found in the document.");
                            return Result.Failed;
                        }

                        if (!titleBlock.IsActive)
                        {
                            titleBlock.Activate();
                        }

                        // Create new sheet
                        ViewSheet newSheet = ViewSheet.Create(doc, titleBlock.Id);
                        newSheet.Name = "New Sheet";
                        newSheet.SheetNumber = "A-001";



                        // Add the view to the sheet
                        Viewport vp = Viewport.Create(doc, newSheet.Id, view.Id, XYZ.Zero);

                        TaskDialog.Show("Success", "View added to new sheet successfully.");


                        // get titleblock center
                        BoundingBoxXYZ titleBlockBox = titleBlock.get_BoundingBox(null); // Get the bounding box of the title block
                        XYZ titleBlockCenter = (titleBlockBox.Min + titleBlockBox.Max) / 2; // Calculate the center point of the title block


                        //get sheet center
                        //BoundingBoxXYZ sheetBox = newSheet.get_BoundingBox(null);// Get the bounding box of the sheet
                        //XYZ sheetCenter = (sheetBox.Min + sheetBox.Max) / 2; // Calculate the center point of the sheet
                        //TaskDialog.Show("Sheet Center", $"Sheet Center: {sheetCenter}");
                        //get view center
                        XYZ vpCenter = vp.GetBoxCenter(); // Get the bounding box of the view
                        TaskDialog.Show("View Center", $"View Center: {vpCenter}");

                        //move view to sheet center
                        XYZ moveVector = titleBlockCenter - vpCenter; // Calculate the vector to move the view to the center of the sheet

                        TaskDialog.Show("Viewport will be moved ", $"Move Vector: {moveVector}"); // Show the move vector in a dialog box

                        ElementTransformUtils.MoveElement(doc, vp.Id, moveVector); // Move the view to the center of the sheet
                        //check the new position of the view
                        XYZ newViewCenter = vp.GetBoxCenter(); // Get the bounding box of the view after moving
                        TaskDialog.Show("New View Center", $"New View Center: {newViewCenter}");

                        tx.Commit();
                    }
                    TaskDialog.Show("Success", "View added to new sheet successfully.");
                }
                else
                {
                    TaskDialog.Show("Error", "Selected element is not a view.");
                }

            }
            catch (Exception ex) { return Result.Failed; }
            return Result.Succeeded;
        }

    }
}
