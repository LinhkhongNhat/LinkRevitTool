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
    // This class is the actual command
    [Transaction(TransactionMode.Manual)] // This attribute specifies that the command will be executed in a manual transaction mode
    public class LinkToolCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Link Tool", "Link Tool command executed successfully.");
            return Result.Succeeded;
        }
    }
}
