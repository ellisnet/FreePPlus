# FreePPlus
Create advanced Excel spreadsheets using .NET, without the need of interop.

FreePPlus is a .NET library that reads and writes Excel files using the Office Open XML format (xlsx). 
FreePPlus has one dependency other than .NET - the CodeBrix.Imaging package for image and font handling.

FreePPlus is provided as a .NET 10 library and associated `FreePPlus.LgplLicenseForever` NuGet package.
FreePPlus supports applications and assemblies that target Microsoft .NET version 10.0 and later.

Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 
2025; and will be actively supported by Microsoft until Nov 14, 2028. Please update your C#/.NET code 
and projects to the latest LTS version of Microsoft .NET.

FreePPlus is a fork of the code of the popular EPPlus library version 4.5.3.3 - see below for licensing
details.
 
## FreePPlus supports:
* Cell Ranges 
* Cell styling (Border, Color, Fill, Font, Number, Alignments) 
* Data validation 
* Conditional formatting 
* Charts 
* Pictures 
* Shapes 
* Comments 
* Tables 
* Pivot tables 
* Protection 
* Encryption 
* VBA 
* Formula calculation 
* Many more... 

## Sample Code

### Create a New Excel File

```csharp
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

// Create a new workbook and add a worksheet
using var package = new ExcelPackage();
var ws = package.Workbook.Worksheets.Add("Sales Report");

// Add headers
ws.Cells["A1"].Value = "Product";
ws.Cells["B1"].Value = "Quantity";
ws.Cells["C1"].Value = "Unit Price";
ws.Cells["D1"].Value = "Total";

// Add data
ws.Cells["A2"].Value = "Widget";
ws.Cells["B2"].Value = 25;
ws.Cells["C2"].Value = 3.50;

ws.Cells["A3"].Value = "Gadget";
ws.Cells["B3"].Value = 10;
ws.Cells["C3"].Value = 12.99;

ws.Cells["A4"].Value = "Gizmo";
ws.Cells["B4"].Value = 50;
ws.Cells["C4"].Value = 1.75;

// Add formulas
ws.Cells["D2:D4"].Formula = "B2*C2";

// Style the header row
using (var range = ws.Cells["A1:D1"])
{
    range.Style.Font.Bold = true;
    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
    range.Style.Fill.BackgroundColor.SetColor(0, 0, 51, 102);    // dark blue
    range.Style.Font.Color.SetColor(255, 255, 255, 255);         // white
}

// Format currency columns
ws.Cells["C2:D4"].Style.Numberformat.Format = "#,##0.00";

// Auto-fit column widths
ws.Cells["A1:D4"].AutoFitColumns();

// Save to a file
package.SaveAs(new FileInfo("SalesReport.xlsx"));
```

### Open and Read an Existing File

```csharp
using OfficeOpenXml;

using var package = new ExcelPackage(new FileInfo("SalesReport.xlsx"));
var ws = package.Workbook.Worksheets["Sales Report"];

// Read cell values
for (int row = 2; row <= 4; row++)
{
    var product = ws.Cells[row, 1].Text;
    var quantity = ws.Cells[row, 2].GetValue<int>();
    Console.WriteLine($"{product}: {quantity} units");
}
```

### Load Data from a Collection

```csharp
using OfficeOpenXml;
using OfficeOpenXml.Table;

var inventory = new[]
{
    new { Sku = "A100", Name = "Hammer", Stock = 37, Price = 12.10m },
    new { Sku = "A101", Name = "Nails",  Stock = 500, Price = 3.99m },
    new { Sku = "A102", Name = "Saw",    Stock = 12, Price = 15.37m },
};

using var package = new ExcelPackage();
var ws = package.Workbook.Worksheets.Add("Inventory");

// Load collection with headers and a table style
ws.Cells["A1"].LoadFromCollection(inventory, true, TableStyles.Medium6);
ws.Cells["A1:D1"].AutoFitColumns();

package.SaveAs(new FileInfo("Inventory.xlsx"));
```

### Add Formulas and Calculate

```csharp
using OfficeOpenXml;

using var package = new ExcelPackage();
var ws = package.Workbook.Worksheets.Add("Calculations");

ws.Cells["A1"].Value = 100;
ws.Cells["A2"].Value = 200;
ws.Cells["A3"].Value = 300;
ws.Cells["A4"].Formula = "SUM(A1:A3)";
ws.Cells["A5"].Formula = "AVERAGE(A1:A3)";

// Calculate formulas in memory
ws.Calculate();

Console.WriteLine($"Sum:     {ws.Cells["A4"].Value}");   // 600
Console.WriteLine($"Average: {ws.Cells["A5"].Value}");   // 200

package.SaveAs(new FileInfo("Calculations.xlsx"));
```

### Protect a Worksheet and Workbook

```csharp
using OfficeOpenXml;

using var package = new ExcelPackage();
var ws = package.Workbook.Worksheets.Add("Protected");

ws.Cells["A1"].Value = "This sheet is protected";

// Protect the worksheet (uses SHA-512 hashing)
ws.Protection.SetPassword("sheetPass");
ws.Protection.AllowSelectLockedCells = true;
ws.Protection.AllowSelectUnlockedCells = true;

// Protect the workbook structure
package.Workbook.Protection.SetPassword("workbookPass");
package.Workbook.Protection.LockStructure = true;

package.SaveAs(new FileInfo("Protected.xlsx"));
```

### Save with Encryption

```csharp
using OfficeOpenXml;

using var package = new ExcelPackage();
var ws = package.Workbook.Worksheets.Add("Encrypted");
ws.Cells["A1"].Value = "Sensitive data";

// Save with a file-open password (AES encryption)
package.SaveAs(new FileInfo("Encrypted.xlsx"), "openPassword");
```

Note that significant additional sample code is available in the `FreePPlus.OfficeOpenXml.Tests`
project; and in `FreePPlus.OfficeOpenXml.SampleApp`.

## License
The project is licensed under the GNU Lesser General Public License (LGPL) version 3.
see: https://en.wikipedia.org/wiki/GNU_Lesser_General_Public_License

All code from EPPlus version 4.5.3.3 was licensed under the GNU Lesser General Public License (LGPL) 
version 3 - as of Jan 30, 2020.  This project (FreePPlus) complies with all provisions of the open 
source license of EPPlus version 4.5.3.3 (code) - including making all modified, adapted and derived 
code freely available as open source, under the same license as the EPPlus code license.
