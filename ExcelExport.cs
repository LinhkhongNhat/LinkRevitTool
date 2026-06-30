using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ClosedXML.Excel;
using System.Linq;
using Autodesk.Revit.Attributes;

namespace Link_Tool_R25
{
    [Transaction(TransactionMode.Manual)]
    public class ExcelExport : IExternalCommand
    {
        public Result Execute(ExternalCommandData externalCommandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = externalCommandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Promt user to select a schedule
            ViewSchedule schedule = doc.ActiveView as ViewSchedule;
            if (schedule == null)
            {
                TaskDialog.Show("Export Schedule", "Please select a schedule to export.");
                return Result.Failed;
            }

            string filePath = "C:\\Temp\\ScheduleExport.xlsx"; // Change this to your desired file path
            ExportScheduleToExcel(schedule, filePath);
            return Result.Succeeded;
        }


        // Method to export schedule data to Excel
        private void ExportScheduleToExcel(ViewSchedule schedule, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(schedule.Name);
                // Get the schedule data
                TableData tableData = schedule.GetTableData();
                TableSectionData sectionData = tableData.GetSectionData(SectionType.Body);
                // Write headers
                for (int col = 0; col < sectionData.NumberOfColumns; col++)
                {
                    worksheet.Cell(1, col + 1).Value = sectionData.GetCellText(0, col);
                }
                // Write data
                for (int row = 0; row < sectionData.NumberOfRows; row++)
                {
                    for (int col = 0; col < sectionData.NumberOfColumns; col++)
                    {
                        worksheet.Cell(row + 2, col + 1).Value = sectionData.GetCellText(row, col);
                    }
                }
                workbook.SaveAs(filePath);
                TaskDialog.Show("Export Schedule", $"Schedule exported to {filePath}");
            }
        }
    }
}
