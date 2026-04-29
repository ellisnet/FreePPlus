================================================================================
AGENT-README: FreePPlus
A Comprehensive Guide for AI Coding Agents
================================================================================

OVERVIEW
--------
FreePPlus is a .NET library that reads and writes Excel files (.xlsx) using the
Office Open XML format. It does NOT require Microsoft Excel or any COM interop.

It is a fork of the popular EPPlus library version 4.5.3.3, licensed under
LGPL v3.

IMPORTANT: If you are familiar with EPPlus (v4.x), the API surface of FreePPlus
is essentially identical. The namespaces are the SAME as EPPlus: "OfficeOpenXml".
This means code written for EPPlus 4.x will work with FreePPlus with minimal
changes (just swap the NuGet package).

IMPORTANT: FreePPlus has one third-party dependency: CodeBrix.Imaging (for
image and font handling). It also depends on a few Microsoft.Extensions and
System.Security.Cryptography packages. All dependencies are automatically
pulled in via NuGet.

Source Repository: https://github.com/ellisnet/FreePPlus
License: LGPL v3 (GNU Lesser General Public License version 3)

================================================================================

INSTALLATION
------------
NuGet Package: FreePPlus.LgplLicenseForever
Dependencies:
  - CodeBrix.Imaging.ApacheLicenseForever
  - Microsoft.Extensions.Configuration
  - Microsoft.Extensions.Configuration.FileExtensions
  - Microsoft.Extensions.Configuration.Json
  - System.Security.Cryptography.Pkcs

Requirements: .NET 10.0 or higher

To add to a .NET 10+ project:

    dotnet add package FreePPlus.LgplLicenseForever

Or in a .csproj file (NuGet will resolve the latest version):

    <PackageReference Include="FreePPlus.LgplLicenseForever" />

IMPORTANT: The package name is "FreePPlus.LgplLicenseForever" (not just
"FreePPlus"). Always use this full package name when installing.

================================================================================

KEY NAMESPACES
--------------

    using OfficeOpenXml;                    // Core: ExcelPackage, ExcelWorkbook, ExcelWorksheet
    using OfficeOpenXml.Style;              // Styling: fonts, fills, borders, alignment
    using OfficeOpenXml.Table;              // Tables and table styles
    using OfficeOpenXml.Drawing;            // Pictures, shapes, drawings
    using OfficeOpenXml.Drawing.Chart;      // Charts (bar, line, pie, etc.)
    using OfficeOpenXml.DataValidation;     // Data validation rules
    using OfficeOpenXml.ConditionalFormatting; // Conditional formatting
    using OfficeOpenXml.FormulaParsing;     // Formula parsing engine
    using OfficeOpenXml.Table.PivotTable;   // Pivot tables
    using OfficeOpenXml.VBA;                // VBA macro support

NOTE: The namespaces use "OfficeOpenXml" (same as EPPlus 4.x), NOT "FreePPlus".

================================================================================

SUPPORTED FEATURES
-------------------
  - Cell ranges and cell addressing (A1 notation and row/column indices)
  - Cell styling (borders, colors, fills, fonts, number formats, alignment)
  - Data validation
  - Conditional formatting
  - Charts (bar, line, pie, area, scatter, and more)
  - Pictures and images
  - Shapes
  - Comments
  - Tables with styles
  - Pivot tables
  - Worksheet and workbook protection
  - File encryption (AES, password-protected xlsx)
  - VBA macro support
  - Formula calculation engine
  - Named ranges and named values
  - AutoFilter
  - Merged cells
  - Rich text formatting
  - Row/column insertion and deletion
  - Auto-fit column widths
  - Header/footer settings
  - Print settings (margins, page breaks, orientation)
  - Sparklines
  - Hyperlinks
  - Loading data from collections

================================================================================

CORE API REFERENCE
==================

1. CREATING A NEW EXCEL FILE
------------------------------

    using OfficeOpenXml;

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Sheet1");

    // Set cell values
    ws.Cells["A1"].Value = "Hello";
    ws.Cells["B1"].Value = 42;
    ws.Cells["C1"].Value = DateTime.Now;
    ws.Cells["D1"].Value = 3.14;

    // Save to file
    package.SaveAs(new FileInfo("output.xlsx"));

IMPORTANT: ExcelPackage implements IDisposable. Always use 'using' statements.

2. OPENING AN EXISTING FILE
------------------------------

From a file:

    using var package = new ExcelPackage(new FileInfo("existing.xlsx"));
    var ws = package.Workbook.Worksheets["Sheet1"];
    // or by index:
    var ws = package.Workbook.Worksheets[0];

From a stream:

    using var stream = File.OpenRead("existing.xlsx");
    using var package = new ExcelPackage(stream);

Opening an encrypted file:

    using var package = new ExcelPackage(new FileInfo("encrypted.xlsx"), "password");

3. READING CELL VALUES
------------------------

By address string:

    var text = ws.Cells["A1"].Text;              // Formatted text
    var value = ws.Cells["A1"].Value;             // Raw value (object)
    var typed = ws.Cells["A1"].GetValue<int>();   // Typed value

By row/column index (1-based):

    var value = ws.Cells[1, 1].Value;             // Row 1, Column 1 = A1
    var value = ws.Cells[2, 3].Value;             // Row 2, Column 3 = C2

4. WRITING CELL VALUES
------------------------

By address:

    ws.Cells["A1"].Value = "Text";
    ws.Cells["B1"].Value = 42;
    ws.Cells["C1"].Value = 3.14m;
    ws.Cells["D1"].Value = DateTime.Now;
    ws.Cells["E1"].Value = true;

By row/column:

    ws.SetValue("A1", "Text");
    ws.SetValue(1, 1, "Text");                    // Row/column (1-based)

Setting multiple values with short, long, float, double, decimal, byte:

    ws.SetValue("A1", (short)1);
    ws.SetValue("A2", (long)2);
    ws.SetValue("A3", (float)3);
    ws.SetValue("A4", (double)4);
    ws.SetValue("A5", (decimal)5);
    ws.SetValue("A6", (byte)6);

5. SAVING FILES
-----------------

Save to file:

    package.SaveAs(new FileInfo("output.xlsx"));

Save with encryption password:

    package.SaveAs(new FileInfo("output.xlsx"), "openPassword");

Save to byte array (for web/API scenarios):

    byte[] bytes = package.GetAsByteArray();

Save to stream:

    using var stream = new MemoryStream();
    package.SaveAs(stream);

In-memory round-trip:

    byte[] bytes;
    using (var pck = new ExcelPackage())
    {
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Hello";
        bytes = pck.GetAsByteArray();
    }

    using var ms = new MemoryStream(bytes);
    using var pck2 = new ExcelPackage(ms);
    var value = pck2.Workbook.Worksheets[0].Cells["A1"].Value;

================================================================================

CELL STYLING
=============

All styling is accessed via the .Style property on cells or ranges.

Font:

    ws.Cells["A1"].Style.Font.Bold = true;
    ws.Cells["A1"].Style.Font.Italic = true;
    ws.Cells["A1"].Style.Font.Size = 14;
    ws.Cells["A1"].Style.Font.Name = "Calibri";
    ws.Cells["A1"].Style.Font.Color.SetColor(255, 255, 0, 0);  // ARGB: Red
    ws.Cells["A1"].Style.Font.UnderLine = true;

Fill (background color):

    ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
    ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(0, 0, 51, 102);  // Dark blue

    // SetColor parameters: (alpha, red, green, blue)
    // Alpha 0 = opaque for fills, 255 = fully opaque for fonts

Borders:

    ws.Cells["A1:D4"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
    ws.Cells["A1:D4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    ws.Cells["A1:D4"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
    ws.Cells["A1:D4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

Number format:

    ws.Cells["B2:B10"].Style.Numberformat.Format = "#,##0.00";      // Currency-like
    ws.Cells["C2:C10"].Style.Numberformat.Format = "0.00%";          // Percentage
    ws.Cells["D2:D10"].Style.Numberformat.Format = "yyyy-mm-dd";     // Date
    ws.Cells["E2:E10"].Style.Numberformat.Format = "hh:mm:ss";       // Time

Alignment:

    ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    ws.Cells["A1"].Style.WrapText = true;

Styling a range (using block):

    using (var range = ws.Cells["A1:D1"])
    {
        range.Style.Font.Bold = true;
        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(0, 0, 51, 102);
        range.Style.Font.Color.SetColor(255, 255, 255, 255);
    }

Auto-fit column widths:

    ws.Cells["A1:D10"].AutoFitColumns();
    ws.Column(1).AutoFit();                       // Single column

================================================================================

FORMULAS
=========

Setting formulas:

    ws.Cells["D2"].Formula = "B2*C2";
    ws.Cells["A10"].Formula = "SUM(A1:A8)";
    ws.Cells["A11"].Formula = "AVERAGE(A1:A8)";
    ws.Cells["A12"].Formula = "SUBTOTAL(9,A1:A8)";

Setting formulas on a range (relative references auto-adjust):

    ws.Cells["D2:D10"].Formula = "B2*C2";         // Each row adjusts automatically

Setting formulas by row/column:

    ws.SetFormula(1, 2, "isblank(A1:A5)");

Calculating formulas in memory:

    ws.Calculate();                                // Calculate all formulas in worksheet

    // Read calculated values
    var sum = ws.Cells["A10"].Value;               // Gets the calculated result

Calculate a single formula expression:

    var result = ws.Calculate("2.5-A1+ABS(-3.0)-SIN(3)");

Calculate a single cell:

    ws.Cells["A4"].Calculate();

Supported formula functions include:
  SUM, AVERAGE, COUNT, COUNTA, MIN, MAX, IF, IFERROR, ISBLANK, ISTEXT,
  LEFT, RIGHT, MID, LEN, CONCATENATE, SUBTOTAL, INT, ROW, COLUMN,
  ABS, SIN, COS, and many more standard Excel functions.

================================================================================

NAMED RANGES AND NAMED VALUES
===============================

Named values (constants):

    ws.Names.AddValue("PRICE", 10);
    ws.Names.AddValue("QUANTITY", 11);

    // Use in formulas
    ws.Cells["A1"].Formula = "PRICE*QUANTITY";

Named formulas:

    ws.Names.AddFormula("AMOUNT", "PRICE*QUANTITY");

    // Update values and recalculate
    ws.Names["PRICE"].Value = 30;
    ws.Names["QUANTITY"].Value = 10;
    ws.Calculate();
    // ws.Names["AMOUNT"].Value is now 300

Named ranges:

    ws.Names.Add("SalesData", ws.Cells["A1:D10"]);

================================================================================

LOADING DATA FROM COLLECTIONS
===============================

This is one of the most powerful features for agents. Load any IEnumerable<T>
directly into a worksheet.

Basic usage:

    var inventory = new[]
    {
        new { Sku = "A100", Name = "Hammer", Stock = 37, Price = 12.10m },
        new { Sku = "A101", Name = "Nails",  Stock = 500, Price = 3.99m },
        new { Sku = "A102", Name = "Saw",    Stock = 12, Price = 15.37m },
    };

    ws.Cells["A1"].LoadFromCollection(inventory, true, TableStyles.Medium6);

Parameters:
  - collection: The IEnumerable<T> data source
  - printHeaders: true to include property names as header row
  - tableStyle: TableStyles enum value (or TableStyles.None for no table)

Starting at a specific cell:

    ws.Cells["D5"].LoadFromCollection(items, true);
    // Data starts at D5, headers in row 5, data from row 6

Header behavior:
  - Property names become column headers by default
  - [Description("...")] attribute overrides the header text
  - [DisplayName("...")] attribute overrides the header text
  - Underscores in property names are replaced with spaces automatically

Loading simple types (strings, decimals, DateTimes):

    var names = new List<string> { "Alice", "Bob", "Charlie" };
    ws.Cells["A1"].LoadFromCollection(names);

    var prices = new List<decimal> { 1.5m, 2.75m, 3.125m };
    ws.Cells["A1"].LoadFromCollection(prices);

Loading with specific members only:

    var members = typeof(MyClass)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.Name == "Id" || p.Name == "Name")
        .Cast<MemberInfo>()
        .ToArray();

    ws.Cells["A1"].LoadFromCollection(items, true, TableStyles.None,
        BindingFlags.Public | BindingFlags.Instance, members);

Return value:

    var range = ws.Cells["A1"].LoadFromCollection(items, true);
    // range contains the populated cell range (or null if empty + no headers)

Inheritance:
  - Derived class properties appear FIRST, then base class properties
  - This is important for column ordering

Table styles available:
    TableStyles.None, TableStyles.Dark1 through Dark11,
    TableStyles.Light1 through Light21,
    TableStyles.Medium1 through Medium28

================================================================================

TABLES
=======

Create a table from LoadFromCollection (as shown above), or manually:

    var table = ws.Tables.Add(ws.Cells["A1:D10"], "SalesTable");
    table.TableStyle = TableStyles.Medium6;
    table.ShowFilter = true;

Access existing tables:

    var table = ws.Tables[0];
    var table = ws.Tables["SalesTable"];
    int count = ws.Tables.Count;

================================================================================

CHARTS
=======

    using OfficeOpenXml.Drawing.Chart;

Create a chart:

    var chart = ws.Drawings.AddChart("SalesChart", eChartType.ColumnClustered);

    // Position the chart
    chart.SetPosition(5, 0, 5, 0);  // Row, RowOffset, Column, ColumnOffset
    chart.SetSize(600, 400);         // Width, Height in pixels

    // Add data series
    chart.Series.Add(ws.Cells["B2:B10"], ws.Cells["A2:A10"]);
    // First param: Y values, Second param: X labels

Available chart types (eChartType enum):
    ColumnClustered, ColumnStacked, ColumnStacked100,
    BarClustered, BarStacked, BarStacked100,
    Line, LineMarkers, LineMarkersStacked,
    Pie, PieExploded, Pie3D,
    Area, AreaStacked, AreaStacked100,
    Scatter, ScatterLines, ScatterSmooth,
    Doughnut, DoughnutExploded,
    Radar, RadarFilled,
    Surface, SurfaceTopView,
    Bubble, Bubble3DEffect,
    ... and more

Chart with multiple series:

    chart.Series.Add("'Sheet1'!V19:V24", "'Sheet1'!U19:U24");
    chart.Series.Add("'Sheet1'!W19:W24", "'Sheet1'!U19:U24");

================================================================================

PICTURES AND IMAGES
====================

    using OfficeOpenXml.Drawing;

Add a picture from a CodeBrix.Imaging Image object:

    using CodeBrix.Imaging;

    var image = Image.Load("photo.jpg");
    var picture = ws.Drawings.AddPicture("Logo", image);
    picture.SetPosition(0, 0, 5, 0);    // Row, RowOffset, Column, ColumnOffset
    picture.SetSize(200, 100);           // Width, Height

Create a test image programmatically:

    var image = ExcelPicture.CreateImage(100, 50);  // Width, Height
    var picture = ws.Drawings.AddPicture("TestImage", image);

================================================================================

COMMENTS
=========

    ws.Cells["A1"].AddComment("This is a comment", "Author Name");

    // Access existing comments
    var comment = ws.Cells["A1"].Comment;

================================================================================

WORKSHEET OPERATIONS
=====================

Add worksheet:

    var ws = package.Workbook.Worksheets.Add("New Sheet");

Delete worksheet:

    package.Workbook.Worksheets.Delete("Sheet1");
    package.Workbook.Worksheets.Delete(0);  // By index

Rename worksheet:

    ws.Name = "New Name";

Visibility:

    ws.Hidden = eWorkSheetHidden.Hidden;
    ws.Hidden = eWorkSheetHidden.VeryHidden;
    ws.Hidden = eWorkSheetHidden.Visible;

Row/Column operations:

    ws.InsertRow(3, 2);                   // Insert 2 rows at row 3
    ws.DeleteRow(3, 2);                   // Delete 2 rows starting at row 3
    ws.InsertColumn(3, 2);                // Insert 2 columns at column 3
    ws.DeleteColumn(3, 2);                // Delete 2 columns starting at column 3

    ws.Row(1).Hidden = true;              // Hide row 1
    ws.Column(1).Width = 25;              // Set column width

Merged cells:

    ws.Cells["A1:D1"].Merge = true;

AutoFilter:

    ws.Cells["A1:D10"].AutoFilter = true;

================================================================================

PROTECTION AND ENCRYPTION
===========================

Worksheet protection:

    ws.Protection.SetPassword("sheetPass");
    ws.Protection.AllowSelectLockedCells = true;
    ws.Protection.AllowSelectUnlockedCells = true;
    ws.Protection.AllowSort = true;
    ws.Protection.AllowAutoFilter = true;

Workbook protection:

    package.Workbook.Protection.SetPassword("workbookPass");
    package.Workbook.Protection.LockStructure = true;

File encryption (AES):

    package.SaveAs(new FileInfo("encrypted.xlsx"), "openPassword");

================================================================================

DATA VALIDATION
================

    using OfficeOpenXml.DataValidation;

    // Restrict to whole numbers between 1 and 100
    var validation = ws.DataValidations.AddIntegerValidation("B2:B100");
    validation.Formula.Value = 1;
    validation.Formula2.Value = 100;
    validation.ShowErrorMessage = true;
    validation.ErrorTitle = "Invalid Entry";
    validation.Error = "Please enter a number between 1 and 100";

    // Dropdown list validation
    var listValidation = ws.DataValidations.AddListValidation("C2:C100");
    listValidation.Formula.Values.Add("Option A");
    listValidation.Formula.Values.Add("Option B");
    listValidation.Formula.Values.Add("Option C");

================================================================================

CONDITIONAL FORMATTING
========================

    using OfficeOpenXml.ConditionalFormatting;

    // Add conditional formatting rules to highlight cells
    var cf = ws.ConditionalFormatting;
    // (See ConditionalFormattingTests.cs for comprehensive examples)

================================================================================

HYPERLINKS
===========

    // External URL
    ws.Cells["A1"].Hyperlink = new Uri("https://example.com");
    ws.Cells["A1"].Value = "Click here";
    ws.Cells["A1"].Style.Font.UnderLine = true;

    // Internal reference to another sheet
    ws.Cells["A2"].Hyperlink = new ExcelHyperLink("'Sheet2'!A1", "Go to Sheet2");

================================================================================

LOADING FROM CSV/TEXT
======================

    // Load CSV data into worksheet
    ws.Cells["A1"].LoadFromText("data.csv", new ExcelTextFormat
    {
        Delimiter = ',',
        TextQualifier = '"'
    });

================================================================================

VBA MACROS
===========

    using OfficeOpenXml.VBA;

    // Access VBA project
    var vbaProject = package.Workbook.VbaProject;

    // Note: The file must be saved as .xlsm for macros to work

================================================================================

SPARKLINES
===========

    // Sparklines are mini-charts within cells
    // See SparkLineTests.cs for usage examples

================================================================================

HEADER AND FOOTER
==================

    ws.HeaderFooter.OddHeader.LeftAlignedText = "Report Title";
    ws.HeaderFooter.OddFooter.CenteredText = "Page &P of &N";

================================================================================

PRINT SETTINGS
===============

    ws.PrinterSettings.Orientation = eOrientation.Landscape;
    ws.PrinterSettings.FitToPage = true;
    ws.PrinterSettings.TopMargin = 0.75m;
    ws.PrinterSettings.BottomMargin = 0.75m;
    ws.PrinterSettings.LeftMargin = 0.5m;
    ws.PrinterSettings.RightMargin = 0.5m;

================================================================================

COMPLETE EXAMPLES
=================

Example 1: Sales Report with Styling and Formulas
---------------------------------------------------
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using OfficeOpenXml.Table;

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Sales Report");

    // Headers
    ws.Cells["A1"].Value = "Product";
    ws.Cells["B1"].Value = "Quantity";
    ws.Cells["C1"].Value = "Unit Price";
    ws.Cells["D1"].Value = "Total";

    // Data
    ws.Cells["A2"].Value = "Widget";  ws.Cells["B2"].Value = 25;  ws.Cells["C2"].Value = 3.50;
    ws.Cells["A3"].Value = "Gadget";  ws.Cells["B3"].Value = 10;  ws.Cells["C3"].Value = 12.99;
    ws.Cells["A4"].Value = "Gizmo";   ws.Cells["B4"].Value = 50;  ws.Cells["C4"].Value = 1.75;

    // Formulas
    ws.Cells["D2:D4"].Formula = "B2*C2";

    // Style headers
    using (var range = ws.Cells["A1:D1"])
    {
        range.Style.Font.Bold = true;
        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(0, 0, 51, 102);
        range.Style.Font.Color.SetColor(255, 255, 255, 255);
    }

    // Number format
    ws.Cells["C2:D4"].Style.Numberformat.Format = "#,##0.00";

    // Auto-fit
    ws.Cells["A1:D4"].AutoFitColumns();

    package.SaveAs(new FileInfo("SalesReport.xlsx"));

Example 2: Load Collection with Table
---------------------------------------
    using OfficeOpenXml;
    using OfficeOpenXml.Table;

    var data = new[]
    {
        new { Name = "Alice", Department = "Engineering", Salary = 95000m },
        new { Name = "Bob", Department = "Marketing", Salary = 72000m },
        new { Name = "Charlie", Department = "Engineering", Salary = 105000m },
    };

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Employees");

    ws.Cells["A1"].LoadFromCollection(data, true, TableStyles.Medium6);
    ws.Cells["C2:C100"].Style.Numberformat.Format = "#,##0";
    ws.Cells.AutoFitColumns();

    package.SaveAs(new FileInfo("Employees.xlsx"));

Example 3: Read and Modify Existing File
------------------------------------------
    using OfficeOpenXml;

    using var package = new ExcelPackage(new FileInfo("input.xlsx"));
    var ws = package.Workbook.Worksheets["Sheet1"];

    // Read existing data
    for (int row = 2; row <= ws.Dimension.End.Row; row++)
    {
        var name = ws.Cells[row, 1].Text;
        var value = ws.Cells[row, 2].GetValue<int>();
        Console.WriteLine($"{name}: {value}");
    }

    // Add a new column
    ws.Cells["C1"].Value = "Status";
    for (int row = 2; row <= ws.Dimension.End.Row; row++)
    {
        ws.Cells[row, 3].Value = "Processed";
    }

    package.SaveAs(new FileInfo("output.xlsx"));

Example 4: In-Memory Excel for Web API
-----------------------------------------
    using OfficeOpenXml;

    public byte[] GenerateExcelReport(IEnumerable<OrderDto> orders)
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Orders");

        ws.Cells["A1"].LoadFromCollection(orders, true);
        ws.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }

Example 5: Multi-Sheet Workbook with Formulas
-----------------------------------------------
    using OfficeOpenXml;

    using var package = new ExcelPackage();

    var dataSheet = package.Workbook.Worksheets.Add("Data");
    dataSheet.Cells["A1"].Value = 100;
    dataSheet.Cells["A2"].Value = 200;
    dataSheet.Cells["A3"].Value = 300;

    var summarySheet = package.Workbook.Worksheets.Add("Summary");
    summarySheet.Cells["A1"].Value = "Total";
    summarySheet.Cells["B1"].Formula = "SUM(Data!A1:A3)";
    summarySheet.Cells["A2"].Value = "Average";
    summarySheet.Cells["B2"].Formula = "AVERAGE(Data!A1:A3)";

    package.Workbook.Calculate();

    package.SaveAs(new FileInfo("MultiSheet.xlsx"));

================================================================================

COMMON USING STATEMENT COMBINATIONS
=====================================

For most Excel tasks:

    using OfficeOpenXml;

For styling:

    using OfficeOpenXml;
    using OfficeOpenXml.Style;

For tables:

    using OfficeOpenXml;
    using OfficeOpenXml.Table;

For charts:

    using OfficeOpenXml;
    using OfficeOpenXml.Drawing.Chart;

For images:

    using OfficeOpenXml;
    using OfficeOpenXml.Drawing;
    using CodeBrix.Imaging;  // For loading image files

For comprehensive reports:

    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using OfficeOpenXml.Table;
    using OfficeOpenXml.Drawing.Chart;

================================================================================

WHAT THIS LIBRARY DOES NOT DO
===============================

Do NOT attempt to use FreePPlus for the following - it will not work:

  - Reading/writing .xls files (old Excel 97-2003 format) - ONLY .xlsx
  - Reading/writing CSV files directly (use LoadFromText for import only)
  - PDF export from Excel
  - Interacting with a running Excel application
  - Opening password-protected .xls files (only .xlsx encryption)
  - ODS (LibreOffice/OpenOffice) format
  - Google Sheets format
  - Real-time Excel add-in functionality
  - Excel Services or SharePoint integration
  - Power Query or Power Pivot
  - Complex conditional formatting rules beyond what EPPlus 4.x supported

This library IS for: creating, reading, modifying, and saving Excel .xlsx files
programmatically, without requiring Microsoft Excel to be installed.

================================================================================

MINIMUM VIABLE PROJECT TEMPLATE
=================================

To scaffold a new .NET 10 console project that uses FreePPlus:

    dotnet new console -n MyExcelApp --framework net10.0
    cd MyExcelApp
    dotnet add package FreePPlus.LgplLicenseForever

Then in Program.cs:

    using OfficeOpenXml;

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Sheet1");
    ws.Cells["A1"].Value = "Hello, Excel!";
    ws.Cells["B1"].Value = 42;
    package.SaveAs(new FileInfo("output.xlsx"));

    Console.WriteLine("Created output.xlsx!");

Build and run:

    dotnet build
    dotnet run

================================================================================

PERFORMANCE TIPS FOR CODING AGENTS
====================================

1. USE LoadFromCollection: When populating worksheets from data, always use
   LoadFromCollection instead of setting cells one by one. It's much faster
   and handles headers, types, and table creation automatically.

2. USE GetAsByteArray FOR WEB SCENARIOS: For ASP.NET or API scenarios,
   use GetAsByteArray() to return the Excel file as bytes. Avoid writing
   temporary files to disk.

3. DISPOSE PACKAGES: Always use 'using' statements. ExcelPackage holds
   resources that must be freed.

4. STYLE RANGES, NOT INDIVIDUAL CELLS: Apply styling to cell ranges like
   ws.Cells["A1:D10"] instead of individual cells. This is more efficient
   and produces less verbose code.

5. USE AutoFitColumns: Call AutoFitColumns() after populating data, not
   before. Column widths adjust based on cell content.

6. CALCULATE AFTER SETTING FORMULAS: Call ws.Calculate() or
   package.Workbook.Calculate() if you need to read formula results in code.
   Formulas are calculated by Excel when opened, but Calculate() lets you
   read results programmatically.

7. USE MemoryStream FOR TESTS: In unit tests, create ExcelPackage with
   new MemoryStream() to avoid disk I/O.

8. USE CELL ADDRESS FORMAT: "A1" is column A, row 1. Cell row/column
   indices are ALWAYS 1-based: ws.Cells[1, 1] = A1. But worksheet
   collection indexing (Worksheets[index]) is 0-based by default.
   See WORKSHEET INDEXING CONFIGURATION section.

================================================================================

COMMON PITFALLS TO AVOID
=========================

1. DO NOT confuse the NuGet package name with the namespace.
   - Package: FreePPlus.LgplLicenseForever
   - Namespace: OfficeOpenXml (NOT FreePPlus)

2. DO NOT try to open .xls files. Only .xlsx (Office Open XML) is supported.

3. DO NOT confuse cell addressing with worksheet collection indexing.
   - Cell row/column indices are ALWAYS 1-based: ws.Cells[1, 1] = A1.
     ws.Cells[0, 0] will throw an exception.
   - Worksheet collection indexing is 0-based BY DEFAULT:
     package.Workbook.Worksheets[0] returns the first worksheet.
   - See WORKSHEET INDEXING CONFIGURATION section below for details on
     changing worksheet collection indexing to 1-based.

4. DO NOT target .NET versions below 10.0. This library requires .NET 10+.

5. DO NOT forget to call Calculate() if you need to read formula results
   in code. Without Calculate(), formula cells will have null values until
   the file is opened in Excel.

6. DO NOT set cell values after setting formulas on the same cell. The
   formula will be overwritten.

7. DO NOT forget that LoadFromCollection with inherited classes puts derived
   class properties FIRST, then base class properties. This affects column
   ordering.

8. DO NOT use empty MemberInfo arrays with LoadFromCollection - it throws
   ArgumentException.

9. DO NOT forget the LGPL v3 license implications. If you modify the
    FreePPlus source code, you must make your modifications available under
    LGPL. Using it as a NuGet package (unmodified) in proprietary software
    is permitted under LGPL.

10. DO NOT set table.ShowTotal = true on tables created via
    ws.Tables.Add(). This causes Excel to report the file as corrupted
    when opened. Use ShowFilter, TableStyle, and other table properties
    instead.

11. DO NOT assume system fonts (especially italic and bold variants) are
    available on Linux. On Linux, you may need to install font packages
    (e.g. 'sudo apt install fonts-dejavu') to ensure font families with
    italic and bold variants are available for SetFromFont() operations.

================================================================================

WORKSHEET INDEXING CONFIGURATION
==================================

By default, the worksheet collection (package.Workbook.Worksheets[index]) uses
0-based indexing. This can be changed to 1-based indexing for compatibility or
preference.

IMPORTANT: This ONLY affects worksheet collection indexing. Cell row/column
addressing (ws.Cells[row, col]) is ALWAYS 1-based regardless of this setting.

Option 1: Set programmatically

    package.Compatibility.IsWorksheets1Based = true;
    // Now Worksheets[1] returns the first worksheet instead of Worksheets[0]

Option 2: Set via appsettings.json

    Add to your project's appsettings.json:

    {
      "FreePPlus": {
        "ExcelPackage": {
          "Compatibility": {
            "IsWorksheets1Based": true
          }
        }
      }
    }

    The legacy EPPlus key is also supported:

    {
      "EPPlus": {
        "ExcelPackage": {
          "Compatibility": {
            "IsWorksheets1Based": true
          }
        }
      }
    }

Default behavior summary:
  - IsWorksheets1Based = false (default):
    Worksheets[0] = first worksheet, Worksheets[1] = second worksheet
  - IsWorksheets1Based = true:
    Worksheets[1] = first worksheet, Worksheets[0] throws exception

================================================================================

DEEPER LEARNING: TEST FILE CROSS-REFERENCES
=============================================

The FreePPlus source repository contains extensive test files and a sample app.
If the documentation above is not sufficient, fetch and read the relevant file:

    https://github.com/ellisnet/FreePPlus
    Path: tests/FreePPlus.OfficeOpenXml.Tests/
    Path: samples/FreePPlus.OfficeOpenXml.SampleApp/

Feature-to-test-file mapping:

  Worksheet operations (creation, naming, visibility, cell values, rows,
  columns, insertion, deletion, formulas, styling, merging, AutoFilter,
  comments, data validation, conditional formatting, protection, hyperlinks,
  rich text, CSV loading, headers/footers, print settings):
    -> tests/FreePPlus.OfficeOpenXml.Tests/WorksheetTests.cs
       This is the MOST COMPREHENSIVE test file, covering virtually all
       worksheet functionality.

  Loading data from collections (headers, anonymous types, inheritance,
  member selection, Description/DisplayName attributes, table styles,
  round-trip, simple types):
    -> tests/FreePPlus.OfficeOpenXml.Tests/LoadFromCollectionTests.cs

  Formula calculation (SUM, AVERAGE, IF, LEFT, RIGHT, MID, INT, ISBLANK,
  ISTEXT, SUBTOTAL, named ranges, named values, date math):
    -> tests/FreePPlus.OfficeOpenXml.Tests/CalculationTests.cs

  Charts and pictures (bar, line, pie, scatter, area, doughnut charts,
  chart positioning, series data, images, drawing operations):
    -> tests/FreePPlus.OfficeOpenXml.Tests/DrawingTests.cs

  Cell styling (fonts, bold, italic, size, color):
    -> tests/FreePPlus.OfficeOpenXml.Tests/Style/ExcelFontTests.cs

  Cell styling (general):
    -> tests/FreePPlus.OfficeOpenXml.Tests/ExcelStyleTests.cs

  Conditional formatting:
    -> tests/FreePPlus.OfficeOpenXml.Tests/ConditionalFormatting/ConditionalFormattingTests.cs

  Encryption and file passwords:
    -> tests/FreePPlus.OfficeOpenXml.Tests/EncryptTests.cs

  Security:
    -> tests/FreePPlus.OfficeOpenXml.Tests/SecurityBaselineTests.cs

  Cell addresses and ranges:
    -> tests/FreePPlus.OfficeOpenXml.Tests/AddressTests.cs
    -> tests/FreePPlus.OfficeOpenXml.Tests/ExcelCellBaseTests.cs
    -> tests/FreePPlus.OfficeOpenXml.Tests/ExcelRangeBaseTests.cs

  Cell store internals:
    -> tests/FreePPlus.OfficeOpenXml.Tests/CellStoreTests.cs

  Comments:
    -> tests/FreePPlus.OfficeOpenXml.Tests/CommentsTests.cs

  VBA macros:
    -> tests/FreePPlus.OfficeOpenXml.Tests/VBATests.cs

  Sparklines:
    -> tests/FreePPlus.OfficeOpenXml.Tests/SparkLineTests.cs

  Reading template files:
    -> tests/FreePPlus.OfficeOpenXml.Tests/ReadTemplateTests.cs

  Issue-based regression tests:
    -> tests/FreePPlus.OfficeOpenXml.Tests/IssueBasedTests.cs

  Comprehensive sample application:
    -> samples/FreePPlus.OfficeOpenXml.SampleApp/

HOW TO USE: Fetch the raw file content from GitHub using a URL like:
    https://raw.githubusercontent.com/ellisnet/FreePPlus/main/{path}
For example:
    https://raw.githubusercontent.com/ellisnet/FreePPlus/main/tests/FreePPlus.OfficeOpenXml.Tests/LoadFromCollectionTests.cs

================================================================================

QUICK REFERENCE CARD
=====================

Install:         dotnet add package FreePPlus.LgplLicenseForever
Namespace:       using OfficeOpenXml;
Create package:  new ExcelPackage()
Open file:       new ExcelPackage(new FileInfo("file.xlsx"))
Open encrypted:  new ExcelPackage(new FileInfo("file.xlsx"), "password")
Add worksheet:   package.Workbook.Worksheets.Add("Name")
Get worksheet:   package.Workbook.Worksheets["Name"] or [0]
Set value:       ws.Cells["A1"].Value = "text"
Get value:       ws.Cells["A1"].Value or .GetValue<T>()
Set formula:     ws.Cells["A1"].Formula = "SUM(B1:B10)"
Calculate:       ws.Calculate()
Load data:       ws.Cells["A1"].LoadFromCollection(items, true, TableStyles.Medium6)
Style font:      ws.Cells["A1"].Style.Font.Bold = true
Style fill:      ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid
Number format:   ws.Cells["A1"].Style.Numberformat.Format = "#,##0.00"
Auto-fit:        ws.Cells.AutoFitColumns()
Merge:           ws.Cells["A1:D1"].Merge = true
Add chart:       ws.Drawings.AddChart("Name", eChartType.ColumnClustered)
Add picture:     ws.Drawings.AddPicture("Name", image)
Add comment:     ws.Cells["A1"].AddComment("text", "author")
Protect sheet:   ws.Protection.SetPassword("pass")
Save file:       package.SaveAs(new FileInfo("out.xlsx"))
Save encrypted:  package.SaveAs(new FileInfo("out.xlsx"), "password")
Save bytes:      package.GetAsByteArray()

File format:     .xlsx only (Office Open XML)
Cell indices:    1-based (ws.Cells[1,1] = A1)
Worksheet index: 0-based by default (configurable via IsWorksheets1Based)
Target:          .NET 10.0+

================================================================================
