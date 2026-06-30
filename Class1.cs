using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Link_Tool_R25
{
    // This class creates the ribbon and button
    public class App : IExternalApplication
    {
        // This method is called when Revit starts up
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "Link Tool";  // Name of the ribbon tab
            try
            {
                application.CreateRibbonTab(tabName); //    Create a new ribbon tab

            }
            catch (Exception) // Catch the exception if the tab already exists
            {
                // Tab already exists, do nothing
            }

            #region Create Ribbon Panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName,"Link Tool"); // Create a new ribbon panel

            #endregion

            #region First button

            PushButtonData buttonData = new PushButtonData(  // Create a new button
                "LinkToolCommand", // Name of the command
                "Link Tool", // Text on the button
                typeof(App).Assembly.Location, // Path to the assembly containing the command
                typeof(LinkToolCommand).FullName // Full name of the command class
            );
            #endregion

            #region Second button

            PushButtonData buttonData2 = new PushButtonData(  // Create a new button
                "TagAutoColor", // Name of the command
                "Tag Color", // Text on the button
                typeof(App).Assembly.Location, // Path to the assembly containing the command
                typeof(TagAutoColor).FullName // Full name of the command class
            );

            PushButtonData buttonData3 = new PushButtonData(  // Create a new button
                "SheetGenerate", // Name of the command
                "Sheet Generate", // Text on the button
                typeof(App).Assembly.Location, // Path to the assembly containing the command
                typeof(SheetGenerate).FullName // Full name of the command class
            );

            PushButtonData buttonData4 = new PushButtonData(  // Create a new button
                "ExcelExport", // Name of the command
                "Excel Export", // Text on the button
                typeof(App).Assembly.Location, // Path to the assembly containing the command
                typeof(ExcelExport).FullName // Full name of the command class
            );

            #endregion

            #region add the button to the ribbon panel
            PushButton pushButton = (PushButton)ribbonPanel.AddItem(buttonData); // Add the button to the ribbon panel
            pushButton.ToolTip = "Link Tool for Revit 2025"; // Tooltip for the button

            PushButton pushButton2 = (PushButton)ribbonPanel.AddItem(buttonData2); // Add the button to the ribbon panel
            pushButton2.ToolTip = "Tag Color for duct in revit 2025"; // Tooltip for the button

            PushButton pushButton3 = (PushButton)ribbonPanel.AddItem(buttonData3); // Add the button to the ribbon panel
            pushButton3.ToolTip = "Sheet Generate for revit 2025"; // Tooltip for the button

            PushButton pushButton4 = (PushButton)ribbonPanel.AddItem(buttonData4); // Add the button to the ribbon panel
            pushButton4.ToolTip = "Excel Export for revit 2025"; // Tooltip for the button

            return Result.Succeeded; // Return success
            #endregion
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

}
