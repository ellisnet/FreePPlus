================================================================================
AGENT-README: FreePPlus
A Guide for AI Coding Agents — CONSUMING the FreePPlus.LgplLicenseForever
NuGet package
================================================================================

OVERVIEW
========
FreePPlus reads and writes Excel files (.xlsx / .xlsm) using the Office Open
XML format. It does NOT require Microsoft Excel, COM interop, or any Office
component to be installed; it manipulates the OOXML package directly, so it
runs on Windows, Linux and macOS and inside containers.

Target framework: .NET 10 or later.

PROVENANCE: FreePPlus is a fork of EPPlus; the exact upstream release is
recorded in THIRD-PARTY-NOTICES.txt. The root namespace is
"OfficeOpenXml" — the SAME namespace the upstream project used — so code
written against that generation of the upstream API compiles against FreePPlus
after swapping the NuGet package reference. There is NO "FreePPlus" namespace;
never write "using FreePPlus;". Do not assume APIs from later upstream major
versions (range-level Insert/Delete, ToText/ToDataTable, ExcelTextFormatFirstRow
helpers, the ExcelTable/ExcelRange "Sort by" fluent builders, and the newer
conditional-formatting/pivot builders do NOT exist here). Everything documented
in this file was verified against the FreePPlus source.

Source repository: https://github.com/ellisnet/FreePPlus

================================================================================

INSTALLATION
============
PackageId: FreePPlus.LgplLicenseForever

    dotnet add package FreePPlus.LgplLicenseForever

Or as a project reference (NuGet resolves the version):

    <PackageReference Include="FreePPlus.LgplLicenseForever" />

IMPORTANT: the package id is "FreePPlus.LgplLicenseForever", NOT "FreePPlus".
The assembly is FreePPlus.OfficeOpenXml.dll and the namespace root is
OfficeOpenXml. Three different names, all correct in their own place.

NuGet dependencies (pulled in automatically, no versions pinned by you):
  -> CodeBrix.Imaging.ApacheLicenseForever   (images, colors, font metrics)
  -> Microsoft.Extensions.Configuration
  -> Microsoft.Extensions.Configuration.FileExtensions
  -> Microsoft.Extensions.Configuration.Json
  -> System.Security.Cryptography.Pkcs       (VBA digital signatures, agile
                                              workbook encryption)

License: LGPL-3.0-or-later. Referencing the unmodified NuGet package from
proprietary software is permitted; if you modify FreePPlus source you must make
those modifications available under the LGPL.

Requirements / limits:
  -> No native libraries and no OS-specific setup are required.
  -> Color, Image and Font types used by the public API come from
     CodeBrix.Imaging, not from System.Drawing. Add "using CodeBrix.Imaging;"
     whenever you touch a Color, an Image, or ExcelFont.SetFromFont.
  -> AutoFitColumns() and SetFromFont() measure text with real font metrics. On
     Linux, install a font family that ships bold and italic faces (for example
     "sudo apt install fonts-dejavu") or measurement falls back to whatever is
     present.

================================================================================

KEY NAMESPACES / USINGS
=======================

    using OfficeOpenXml;                        // ExcelPackage, ExcelWorkbook,
                                                // ExcelWorksheet, ExcelRange
    using OfficeOpenXml.Style;                  // fonts, fills, borders,
                                                // alignment, rich text
    using OfficeOpenXml.Style.Dxf;              // differential styles used by
                                                // conditional formatting
    using OfficeOpenXml.Table;                  // ExcelTable, TableStyles
    using OfficeOpenXml.Table.PivotTable;       // pivot tables
    using OfficeOpenXml.Drawing;                // pictures, shapes
    using OfficeOpenXml.Drawing.Chart;          // charts
    using OfficeOpenXml.ConditionalFormatting;  // conditional formatting
    using OfficeOpenXml.ConditionalFormatting.Contracts;  // the rule interfaces
    using OfficeOpenXml.DataValidation;         // validation rules
    using OfficeOpenXml.DataValidation.Contracts;
    using OfficeOpenXml.Sparkline;              // sparkline groups
    using OfficeOpenXml.VBA;                    // VBA project / modules
    using OfficeOpenXml.FormulaParsing;         // FormulaParserManager,
                                                // ExcelCalculationOption
    using OfficeOpenXml.FormulaParsing.Excel.Functions;
    using OfficeOpenXml.FormulaParsing.ExpressionGraph;   // CompileResult,
                                                          // DataType
    using CodeBrix.Imaging;                     // Color, Image, Font

Minimum for most tasks: "using OfficeOpenXml;" plus "using OfficeOpenXml.Style;"
for anything that touches .Style.

NOTE: Calculate() is an EXTENSION method on the static class
CalculationExtension, which lives in the ROOT namespace OfficeOpenXml (not in
OfficeOpenXml.FormulaParsing, despite its source file sitting in that folder).
"using OfficeOpenXml;" is all you need for ws.Calculate(). Add
"using OfficeOpenXml.FormulaParsing;" only for ExcelCalculationOption and the
FormulaParserManager types.

================================================================================

FEATURE AREAS AT A GLANCE
=========================
Every item below has a section in this file with real signatures:

  -> Package lifecycle: create / open / template / save / save async /
     byte array / streams / encryption
  -> Worksheets: add, copy, delete, move, hide, chartsheets
  -> Cells and ranges: A1 and row/column addressing, values, text, typed reads
  -> Range workhorse members: Copy, Clear, Offset, Sort, Merge, AutoFilter,
     AutoFitColumns, array formulas, R1C1
  -> Styling: font, fill, border, number format, alignment, named styles
  -> Rich text inside a single cell
  -> Formulas: 154 built-in functions, in-memory calculation, R1C1
  -> Named ranges, named values, named formulas
  -> Bulk loading: collections, DataTable, IDataReader, object[] arrays, CSV
  -> Tables with table styles and totals rows
  -> Conditional formatting: ~55 rule types in 8 families
  -> Pivot tables with row/column/page/data fields and grouping
  -> Charts: 70+ chart types, titles, legends, axes, series, secondary axes,
     combo charts, chart sheets
  -> Sparklines (line, column, stacked)
  -> Pictures, shapes and background images
  -> Cell comments
  -> Data validation (integer, decimal, list, text length, date, time, custom)
  -> Hyperlinks (external and in-workbook)
  -> Worksheet and workbook protection, protected ranges, file encryption
  -> VBA projects, modules, code modules, VBA protection and signatures
  -> Header/footer, print settings, freeze panes, workbook/worksheet views
  -> Document properties (core, extended and custom)
  -> Formula-parser extensibility: your own worksheet functions

================================================================================

CORE API REFERENCE
==================

1. PACKAGE LIFECYCLE (ExcelPackage)
-----------------------------------
ExcelPackage is IDisposable. ALWAYS dispose it.

Constructors (all public):

    new ExcelPackage()                                   // empty, in memory
    new ExcelPackage(FileInfo newFile)
    new ExcelPackage(FileInfo newFile, string password)
    new ExcelPackage(FileInfo newFile, FileInfo template)
    new ExcelPackage(FileInfo newFile, FileInfo template, string password)
    new ExcelPackage(FileInfo template, bool useStream)
    new ExcelPackage(FileInfo template, bool useStream, string password)
    new ExcelPackage(Stream newStream)
    new ExcelPackage(Stream newStream, string password)
    new ExcelPackage(Stream newStream, Stream templateStream)
    new ExcelPackage(Stream newStream, Stream templateStream, string password)

Create:

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Sheet1");
    ws.Cells["A1"].Value = "Hello";
    package.SaveAs(new FileInfo("output.xlsx"));

Open an existing file (the FileInfo constructor opens it if it exists):

    using var package = new ExcelPackage(new FileInfo("existing.xlsx"));
    var ws = package.Workbook.Worksheets["Sheet1"];      // by name
    var first = package.Workbook.Worksheets[0];          // by index (0-based
                                                         // by default)

Open from a stream, or an encrypted file:

    using var stream = File.OpenRead("existing.xlsx");
    using var package = new ExcelPackage(stream);

    using var enc = new ExcelPackage(new FileInfo("secret.xlsx"), "password");

Save:

    void Save()
    void Save(string password)
    void SaveAs(FileInfo file)
    void SaveAs(FileInfo file, string password)
    void SaveAs(Stream outputStream)
    void SaveAs(Stream outputStream, string password)
    Task SaveAsync()
    Task SaveAsync(string password)
    Task SaveAsAsync(FileInfo file)
    Task SaveAsAsync(FileInfo file, string password)
    Task SaveAsAsync(Stream outputStream)
    Task SaveAsAsync(Stream outputStream, string password)
    byte[] GetAsByteArray()
    byte[] GetAsByteArray(string password)
    void Load(Stream input)
    void Load(Stream input, string password)

Other ExcelPackage members:

    ExcelWorkbook Workbook { get; }
    ExcelEncryption Encryption { get; }
    CompatibilitySettings Compatibility { get; }
    CompressionLevel Compression { get; set; }   // Level0..Level9, None,
                                                 // BestSpeed, Default,
                                                 // BestCompression
    bool DoAdjustDrawings { get; set; }          // move/resize drawings when
                                                 // rows and columns change
    FileInfo File { get; set; }
    Stream Stream { get; }
    const int MaxRows = 1048576;
    const int MaxColumns = 16384;

In-memory round trip for web APIs:

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

2. WORKBOOK AND WORKSHEETS
--------------------------
ExcelWorkbook members you will use:

    ExcelWorksheets Worksheets { get; }
    ExcelNamedRangeCollection Names { get; }     // workbook-scope names
    ExcelStyles Styles { get; }                  // named styles, dxfs
    ExcelProtection Protection { get; }
    ExcelWorkbookView View { get; }
    OfficeProperties Properties { get; }
    ExcelVbaProject VbaProject { get; }
    void CreateVBAProject()
    FormulaParserManager FormulaParserManager { get; }
    ExcelCalcMode CalcMode { get; set; }
    bool FullCalcOnLoad { get; set; }
    bool Date1904 { get; set; }
    ExcelVBAModule CodeModule { get; }
    decimal MaxFontWidth { get; set; }

ExcelWorksheets (package.Workbook.Worksheets):

    ExcelWorksheet Add(string Name)
    ExcelWorksheet Add(string Name, ExcelWorksheet Copy)      // clone a sheet
    ExcelChartsheet AddChart(string Name, eChartType chartType)
    ExcelChartsheet AddChart(string Name, eChartType chartType,
                             ExcelPivotTable pivotTableSource)
    ExcelWorksheet Copy(string Name, string NewName)
    void Delete(int Index) / Delete(string name) / Delete(ExcelWorksheet ws)
    void MoveBefore(string sourceName, string targetName)
    void MoveBefore(int sourcePositionId, int targetPositionId)
    void MoveAfter(string sourceName, string targetName)
    void MoveAfter(int sourcePositionId, int targetPositionId)
    void MoveToStart(string sourceName) / MoveToStart(int sourcePositionId)
    void MoveToEnd(string sourceName)   / MoveToEnd(int sourcePositionId)

ExcelWorksheet members (grouped):

    string Name { get; set; }
    int Index { get; }
    eWorkSheetHidden Hidden { get; set; }        // Visible, Hidden, VeryHidden
    Color TabColor { get; set; }
    ExcelRange Cells { get; }
    ExcelRange SelectedRange { get; }
    ExcelAddressBase Dimension { get; }          // null on an empty sheet
    ExcelRow Row(int row)
    ExcelColumn Column(int col)
    double DefaultRowHeight { get; set; }
    double DefaultColWidth { get; set; }
    ExcelWorksheetView View { get; }
    ExcelHeaderFooter HeaderFooter { get; }
    ExcelPrinterSettings PrinterSettings { get; }
    ExcelSheetProtection Protection { get; }
    ExcelProtectedRangeCollection ProtectedRanges { get; }
    ExcelDrawings Drawings { get; }
    ExcelTableCollection Tables { get; }
    ExcelPivotTableCollection PivotTables { get; }
    ExcelConditionalFormattingCollection ConditionalFormatting { get; }
    ExcelDataValidationCollection DataValidations { get; }
    ExcelSparklineGroupCollection SparklineGroups { get; }
    ExcelCommentCollection Comments { get; }
    ExcelNamedRangeCollection Names { get; }     // worksheet-scope names
    MergeCellsCollection MergedCells { get; }
    ExcelAddressBase AutoFilterAddress { get; set; }
    ExcelBackgroundImage BackgroundImage { get; }
    ExcelVBAModule CodeModule { get; }
    ExcelWorkbook Workbook { get; }

    void InsertRow(int rowFrom, int rows)
    void InsertRow(int rowFrom, int rows, int copyStylesFromRow)
    void InsertColumn(int columnFrom, int columns)
    void InsertColumn(int columnFrom, int columns, int copyStylesFromColumn)
    void DeleteRow(int row) / DeleteRow(int rowFrom, int rows)
    void DeleteRow(int rowFrom, int rows, bool shiftOtherRowsUp)
    void DeleteColumn(int column) / DeleteColumn(int columnFrom, int columns)
    object GetValue(int Row, int Column)
    T GetValue<T>(int Row, int Column)
    void SetValue(int Row, int Column, object Value)
    void SetValue(string Address, object Value)
    void Select() / Select(string Address)
                  / Select(string Address, bool SelectSheet)
                  / Select(ExcelAddress Address)
                  / Select(ExcelAddress Address, bool SelectSheet)

There is NO public ws.SetFormula(...) method. Formulas are set through the
range: ws.Cells[row, col].Formula = "..." (see FORMULAS below).

ExcelRow: Height, CustomHeight, Hidden, Collapsed, OutlineLevel, PageBreak,
Merged, Style, StyleName, StyleID, Phonetic.
ExcelColumn: Width, BestFit, Hidden, Collapsed, OutlineLevel, PageBreak,
Merged, Style, StyleName, StyleID, ColumnMin, ColumnMax, plus
AutoFit(), AutoFit(double MinimumWidth),
AutoFit(double MinimumWidth, double MaximumWidth).

    ws.Row(1).Hidden = true;
    ws.Row(1).Height = 22;
    ws.Column(1).Width = 25;
    ws.Column(2).AutoFit();

Chart sheets are worksheets whose only content is a chart:

    var cs = package.Workbook.Worksheets.AddChart("Trend", eChartType.Line);
    cs.Chart.Title.Text = "Trend";               // ExcelChartsheet.Chart

3. CELLS, RANGES AND ADDRESSING
-------------------------------
ws.Cells is an ExcelRange with three indexers:

    ExcelRange this[string Address]                     // "A1", "A1:D10",
                                                        // "A1:A5,C1:C5"
    ExcelRange this[int Row, int Col]                   // 1-based
    ExcelRange this[int FromRow, int FromCol, int ToRow, int ToCol]

Reading:

    object v   = ws.Cells["A1"].Value;      // raw value
    string t   = ws.Cells["A1"].Text;       // value formatted by its number
                                            // format
    int i      = ws.Cells["A1"].GetValue<int>();
    int?  n    = ws.Cells["A1"].GetValue<int?>();   // blank -> null
    var  last  = ws.Dimension.End.Row;      // Dimension is null if empty

Writing:

    ws.Cells["A1"].Value = "Text";
    ws.Cells["B1"].Value = 42;
    ws.Cells["C1"].Value = 3.14m;
    ws.Cells["D1"].Value = DateTime.Now;
    ws.Cells["E1"].Value = true;
    ws.SetValue(1, 6, (short)1);            // row, column (1-based)
    ws.SetValue("G1", (decimal)5);

Address helpers (static, on ExcelCellBase, which every range derives from):

    string ExcelCellBase.GetAddress(int Row, int Column)
    string ExcelCellBase.GetAddress(int Row, int Column, bool Absolute)
    string ExcelCellBase.GetAddress(int FromRow, int FromCol,
                                    int ToRow, int ToColumn)
    string ExcelCellBase.GetAddress(int FromRow, int FromColumn,
                                    int ToRow, int ToColumn, bool Absolute)
    string ExcelCellBase.GetAddressRow(int Row, bool Absolute = false)
    string ExcelCellBase.GetAddressCol(int Col, bool Absolute = false)
    string ExcelCellBase.GetFullAddress(string worksheetName, string address)
    bool   ExcelCellBase.IsValidAddress(string address)
    bool   ExcelCellBase.IsValidCellAddress(string cellAddress)
    string ExcelCellBase.TranslateFromR1C1(string value, int row, int col)
    string ExcelCellBase.TranslateToR1C1(string value, int row, int col)
    string ExcelCellAddress.GetColumnLetter(int column)

Address objects:

    ExcelAddress  a = new ExcelAddress("B2:D10");
    ExcelAddress  b = new ExcelAddress(2, 2, 10, 4);      // same range
    a.Start.Row, a.Start.Column, a.End.Row, a.End.Column  // ExcelCellAddress
    a.Rows, a.Columns, a.Address, a.IsName, a.Table

ExcelRangeBase derives from ExcelAddress, so anywhere the API asks for an
ExcelAddress or ExcelAddressBase you may pass ws.Cells["A1:A5"] directly.

4. RANGE WORKHORSE MEMBERS (ExcelRangeBase)
-------------------------------------------
Properties:

    object Value { get; set; }
    string Text { get; }                     // read-only, formatted
    string Formula { get; set; }
    string FormulaR1C1 { get; set; }
    ExcelStyle Style { get; }
    string StyleName { get; set; }
    int StyleID { get; set; }
    bool Merge { get; set; }
    bool AutoFilter { get; set; }
    Uri Hyperlink { get; set; }
    bool IsRichText { get; }
    bool IsArrayFormula { get; }
    ExcelRichTextCollection RichText { get; }
    ExcelComment Comment { get; }
    ExcelWorksheet Worksheet { get; }
    string FullAddress { get; }              // 'Sheet1'!A1:D10
    string FullAddressAbsolute { get; }
    IRangeConditionalFormatting ConditionalFormatting { get; }
    IRangeDataValidation DataValidation { get; }

Methods:

    void Copy(ExcelRangeBase Destination)
    void Copy(ExcelRangeBase Destination,
              ExcelRangeCopyOptionFlags? excelRangeCopyOptionFlags)
              // ExcelRangeCopyOptionFlags.ExcludeFormulas
    void Clear()
    void CreateArrayFormula(string ArrayFormula)
    ExcelRangeBase Offset(int RowOffset, int ColumnOffset)
    ExcelRangeBase Offset(int RowOffset, int ColumnOffset,
                          int NumberOfRows, int NumberOfColumns)
    ExcelComment AddComment(string Text, string Author)
    T GetValue<T>()
    void AutoFitColumns()
    void AutoFitColumns(double minimumWidth)
    void AutoFitColumns(double minimumWidth, double maximumWidth)
    void Sort()                                   // by the range's first column
    void Sort(int column, bool descending = false)
    void Sort(int[] columns, bool[] descending = null,
              CultureInfo culture = null,
              CompareOptions compareOptions = CompareOptions.None)
    // Sort column indices are ZERO-BASED WITHIN THE RANGE: 0 is the range's
    // leftmost column. An index outside the range throws ArgumentException.

A range is also IEnumerable<ExcelRangeBase>, which makes LINQ over cells work:

    var nonEmpty = ws.Cells["A1:D100"]
                     .Where(c => c.Value != null)
                     .Select(c => c.Address)
                     .ToList();

Examples:

    ws.Cells["A1:D10"].Copy(ws.Cells["F1"]);
    ws.Cells["A1"].Copy(ws.Cells["B1"],
                        ExcelRangeCopyOptionFlags.ExcludeFormulas);
    ws.Cells["A1:D10"].Clear();
    ws.Cells["A1:D1"].Merge = true;
    ws.Cells["A1:D10"].AutoFilter = true;
    ws.Cells["A2:D100"].Sort(1, descending: true);   // by column B (index 1
                                                     // within A:D)
    ws.Cells["B1:B3"].CreateArrayFormula("A1:A3");
    ws.Cells.AutoFitColumns();

Freeze panes and view options are on the worksheet view, not the range:

    ws.View.FreezePanes(2, 1);      // freeze row 1 and nothing to the left of
                                    // column A (first unfrozen cell is A2)
    ws.View.UnFreezePanes();
    ws.View.ShowGridLines = false;
    ws.View.ZoomScale = 120;
    ws.View.TabSelected = true;
    ws.View.SetTabSelected(true, allowMultiple: false);
    ws.View.PageLayoutView = true;
    ws.View.RightToLeft = false;

RANGE-LEVEL Insert/Delete do NOT exist. Insert and delete whole rows and
columns on the worksheet: ws.InsertRow / ws.DeleteRow / ws.InsertColumn /
ws.DeleteColumn.

5. CELL STYLING
---------------
Everything hangs off range.Style (ExcelStyle):

    ExcelFont Font { get; }
    ExcelFill Fill { get; }
    Border Border { get; }
    ExcelNumberFormat Numberformat { get; }
    ExcelHorizontalAlignment HorizontalAlignment { get; set; }
    ExcelVerticalAlignment VerticalAlignment { get; set; }
    bool WrapText { get; set; }
    bool ShrinkToFit { get; set; }
    int Indent { get; set; }
    int TextRotation { get; set; }
    bool Locked { get; set; }
    bool Hidden { get; set; }
    bool QuotePrefix { get; set; }
    ExcelReadingOrder ReadingOrder { get; set; }

Font (ExcelFont): Name, Size, Family, Bold, Italic, Strike, UnderLine,
UnderLineType, VerticalAlign, Scheme, Color, and
void SetFromFont(Font font)  // CodeBrix.Imaging.Font

    ws.Cells["A1"].Style.Font.Bold = true;
    ws.Cells["A1"].Style.Font.Size = 14;
    ws.Cells["A1"].Style.Font.Name = "Calibri";
    ws.Cells["A1"].Style.Font.UnderLine = true;
    ws.Cells["A1"].Style.Font.Color.SetColor(255, 255, 0, 0);   // a,r,g,b
    ws.Cells["A1"].Style.Font.Color.SetColor(Color.DarkRed);    // Imaging Color

ExcelColor has exactly two setters: SetColor(Color color) and
SetColor(int alpha, int red, int green, int blue).

Fill (ExcelFill): PatternType (ExcelFillStyle), BackgroundColor, PatternColor,
Gradient.

    ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
    ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(255, 0, 51, 102);

ExcelFillStyle: None, Solid, DarkGray, MediumGray, LightGray, Gray125,
Gray0625, DarkVertical, DarkHorizontal, DarkDown, DarkUp, DarkGrid,
DarkTrellis, LightVertical, LightHorizontal, LightDown, LightUp, LightGrid,
LightTrellis.

Border: Left, Right, Top, Bottom, Diagonal (each an ExcelBorderItem with
Style and Color), DiagonalUp, DiagonalDown, plus
BorderAround(ExcelBorderStyle Style) and
BorderAround(ExcelBorderStyle Style, Color Color).

    ws.Cells["A1:D4"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
    ws.Cells["A1:D4"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

ExcelBorderStyle: None, Hair, Dotted, DashDot, Thin, DashDotDot, Dashed,
MediumDashDotDot, MediumDashed, MediumDashDot, Thick, Medium, Double.

Number format:

    ws.Cells["B2:B10"].Style.Numberformat.Format = "#,##0.00";
    ws.Cells["C2:C10"].Style.Numberformat.Format = "0.00%";
    ws.Cells["D2:D10"].Style.Numberformat.Format = "yyyy-mm-dd";

Alignment:

    ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    ws.Cells["A1"].Style.WrapText = true;

Named styles (define once, apply by name):

    var named = package.Workbook.Styles.CreateNamedStyle("Money");
    named.Style.Numberformat.Format = "#,##0.00";
    named.Style.Font.Bold = true;
    ws.Cells["C2:C100"].StyleName = "Money";

    // derive one named style from another
    var bold = package.Workbook.Styles.CreateNamedStyle("Bold", named.Style);

ExcelStyles also exposes the raw collections Fonts, Fills, Borders,
NumberFormats, CellXfs, CellStyleXfs, NamedStyles and Dxfs.

6. RICH TEXT
------------
A cell can hold several differently formatted runs. range.RichText is an
ExcelRichTextCollection:

    ExcelRichText Add(string Text)
    ExcelRichText Insert(int index, string text)
    void Clear() / void RemoveAt(int Index) / void Remove(ExcelRichText Item)
    int Count { get; }
    string Text { get; set; }        // the whole cell as plain text
    ExcelRichText this[int Index] { get; }

ExcelRichText: Text, Bold, Italic, Strike, UnderLine, Size, FontName, Color,
VerticalAlign, PreserveSpace.

    var cell = ws.Cells["A1"];
    cell.RichText.Add("Total: ");
    cell.RichText[0].Bold = true;
    cell.RichText[0].PreserveSpace = true;
    cell.RichText.Add("1,234.00");
    cell.RichText[1].Color = Color.Green;
    bool isRich = cell.IsRichText;      // true now

7. FORMULAS AND CALCULATION
---------------------------
Set formulas WITHOUT a leading "=":

    ws.Cells["D2"].Formula = "B2*C2";
    ws.Cells[10, 1].Formula = "SUM(A1:A8)";
    ws.Cells["A11"].Formula = "AVERAGE(A1:A8)";
    ws.Cells["A12"].Formula = "SUBTOTAL(9,A1:A8)";
    ws.Cells[1, 2].Formula = "ISBLANK(A1)";

Assigning a formula to a MULTI-CELL range fills the whole range and shifts the
relative references per row/column, exactly like filling down in Excel:

    ws.Cells["D2:D10"].Formula = "B2*C2";

R1C1 style:

    ws.Cells["D2"].FormulaR1C1 = "RC[-2]*RC[-1]";
    var r1c1 = ws.Cells["D2:D10"].FormulaR1C1;

Array formulas:

    ws.Cells["B1:B3"].CreateArrayFormula("A1:A3");
    bool isArray = ws.Cells["B1"].IsArrayFormula;

Calculation (extension methods on CalculationExtension, namespace
OfficeOpenXml):

    void Calculate(this ExcelWorkbook workbook)
    void Calculate(this ExcelWorkbook workbook, ExcelCalculationOption options)
    void Calculate(this ExcelWorksheet worksheet)
    void Calculate(this ExcelWorksheet worksheet, ExcelCalculationOption options)
    void Calculate(this ExcelRangeBase range)
    void Calculate(this ExcelRangeBase range, ExcelCalculationOption options)
    object Calculate(this ExcelWorksheet worksheet, string formula)
    object Calculate(this ExcelWorksheet worksheet, string formula,
                     ExcelCalculationOption options)

    ws.Calculate();                             // whole worksheet
    package.Workbook.Calculate();               // whole workbook
    ws.Cells["A4"].Calculate();                 // one cell / one range
    var r = ws.Calculate("2.5-A1+ABS(-3.0)-SIN(3)");   // ad-hoc expression

ExcelCalculationOption has one property: bool AllowCirculareReferences.

Workbook-level calculation switches:

    package.Workbook.CalcMode = ExcelCalcMode.Automatic;
    package.Workbook.FullCalcOnLoad = true;     // ask Excel to recalc on open

BUILT-IN FUNCTIONS — the complete registered set, 154 of them, all
case-insensitive in a formula:

  abs acos acosh address and asin asinh atan atan2 atanh average averagea
  averageif averageifs ceiling char choose column columns concatenate cos cosh
  count counta countblank countif countifs date datevalue daverage day days360
  dcount dcounta degrees dget dmax dmin dsum dvar dvarp edate eomonth
  error.type exact exp fact false find fixed floor hlookup hour hyperlink if
  iferror ifna index indirect int isblank iserr iserror iseven islogical isna
  isnontext isnumber isodd isoweeknum istext large left len ln log log10
  lookup lower match max maxa median mid min mina minute mod month n na
  networkdays networkdays.intl not now offset or pi pmt power product proper
  quotient rand randbetween rank rank.avg rank.eq replace rept right round
  rounddown roundup row rows search second sign sin sinh small sqrt sqrtpi
  stdev stdev.p stdevp stdev.s substitute subtotal sum sumif sumifs sumproduct
  sumsq t tan tanh text time timevalue today true trunc upper value var varp
  vlookup weekday weeknum workday year yearfrac

Anything outside that list is written to the file verbatim and evaluated by
Excel when the file is opened, but Calculate() cannot produce a value for it.
Add your own implementation if you need the value in code (see EXTENDING THE
FORMULA PARSER).

8. NAMED RANGES, VALUES AND FORMULAS
------------------------------------
Two scopes: package.Workbook.Names (workbook-wide) and ws.Names (sheet-local).
Both are an ExcelNamedRangeCollection:

    ExcelNamedRange Add(string Name, ExcelRangeBase Range)
    ExcelNamedRange AddValue(string Name, object value)
    ExcelNamedRange AddFormula(string Name, string Formula)
    ExcelNamedRange AddFormla(string Name, string Formula)   // legacy spelling,
                                                             // same behaviour
    void Remove(string Name)
    bool ContainsKey(string key)
    int Count { get; }
    ExcelNamedRange this[string Name] { get; }
    ExcelNamedRange this[int Index] { get; }

ExcelNamedRange derives from ExcelRangeBase and adds Name, LocalSheetId,
IsNameHidden and NameComment, so it carries Value, Formula and Style too.

    ws.Names.AddValue("PRICE", 10);
    ws.Names.AddValue("QUANTITY", 11);
    ws.Names.AddFormula("AMOUNT", "PRICE*QUANTITY");
    ws.Cells["A1"].Formula = "AMOUNT";

    ws.Names["PRICE"].Value = 30;
    ws.Names["QUANTITY"].Value = 10;
    ws.Calculate();
    var amount = ws.Names["AMOUNT"].Value;      // 300

    package.Workbook.Names.Add("SalesData", ws.Cells["A1:D10"]);
    ws.Names["SalesData"].Style.Font.Bold = true;

9. BULK LOADING DATA
--------------------
LoadFromCollection is the fastest and most compact way to fill a sheet:

    ExcelRangeBase LoadFromCollection<T>(IEnumerable<T> Collection)
    ExcelRangeBase LoadFromCollection<T>(IEnumerable<T> Collection,
                                         bool PrintHeaders)
    ExcelRangeBase LoadFromCollection<T>(IEnumerable<T> Collection,
                                         bool PrintHeaders,
                                         TableStyles TableStyle)
    ExcelRangeBase LoadFromCollection<T>(IEnumerable<T> Collection,
                                         bool PrintHeaders,
                                         TableStyles TableStyle,
                                         BindingFlags memberFlags,
                                         MemberInfo[] Members)

    var inventory = new[]
    {
        new { Sku = "A100", Name = "Hammer", Stock = 37,  Price = 12.10m },
        new { Sku = "A101", Name = "Nails",  Stock = 500, Price = 3.99m  },
        new { Sku = "A102", Name = "Saw",    Stock = 12,  Price = 15.37m },
    };
    var filled = ws.Cells["A1"].LoadFromCollection(inventory, true,
                                                   TableStyles.Medium6);

Header rules (verified in source):
  -> [DescriptionAttribute("...")] wins if present;
  -> otherwise [DisplayNameAttribute("...")];
  -> otherwise the member name with '_' replaced by ' '.
Derived-class properties are emitted BEFORE base-class properties, which
decides column order for inherited types.
Passing an EMPTY MemberInfo[] throws ArgumentException ("Parameter Members must
have at least one property"). Pass null to mean "all properties".
The return value is the filled range (useful for AutoFitColumns and for feeding
a pivot table); it can be null when the collection is empty and PrintHeaders is
false.

Selecting specific members:

    var members = typeof(Product)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.Name is "Sku" or "Price")
        .Cast<MemberInfo>()
        .ToArray();
    ws.Cells["A1"].LoadFromCollection(products, true, TableStyles.None,
        BindingFlags.Public | BindingFlags.Instance, members);

Simple element types work too (one column):

    ws.Cells["A1"].LoadFromCollection(new List<string> { "Alice", "Bob" });

Other loaders:

    ExcelRangeBase LoadFromDataTable(DataTable Table, bool PrintHeaders)
    ExcelRangeBase LoadFromDataTable(DataTable Table, bool PrintHeaders,
                                     TableStyles TableStyle)
    ExcelRangeBase LoadFromDataReader(IDataReader Reader, bool PrintHeaders)
    ExcelRangeBase LoadFromDataReader(IDataReader Reader, bool PrintHeaders,
                                      string TableName,
                                      TableStyles TableStyle =
                                          TableStyles.None)
    ExcelRangeBase LoadFromArrays(IEnumerable<object[]> Data)

    ws.Cells["A1"].LoadFromDataTable(table, true, TableStyles.Light9);
    ws.Cells["A1"].LoadFromArrays(new[]
    {
        new object[] { "Name", "Qty" },
        new object[] { "Bolt", 12 },
    });

CSV / delimited text:

    ExcelRangeBase LoadFromText(string Text)
    ExcelRangeBase LoadFromText(string Text, ExcelTextFormat Format)
    ExcelRangeBase LoadFromText(string Text, ExcelTextFormat Format,
                                TableStyles TableStyle, bool FirstRowIsHeader)
    ExcelRangeBase LoadFromText(FileInfo TextFile)
    ExcelRangeBase LoadFromText(FileInfo TextFile, ExcelTextFormat Format)
    ExcelRangeBase LoadFromText(FileInfo TextFile, ExcelTextFormat Format,
                                TableStyles TableStyle, bool FirstRowIsHeader)

CRITICAL: the string overloads take the CSV CONTENT, not a file name. Passing
"data.csv" writes the literal text "data.csv" into the cell. Use the FileInfo
overloads for files:

    var format = new ExcelTextFormat
    {
        Delimiter = ',',
        TextQualifier = '"',
        EOL = "\r\n",
        Culture = CultureInfo.InvariantCulture,
        Encoding = Encoding.UTF8,
        SkipLinesBeginning = 0,
        SkipLinesEnd = 0,
        DataTypes = new[] { eDataTypes.String, eDataTypes.Number,
                            eDataTypes.DateTime },
    };
    ws.Cells["A1"].LoadFromText(new FileInfo("data.csv"), format,
                                TableStyles.Medium9, true);

eDataTypes controls per-column parsing. The FileInfo overloads throw
ArgumentNullException for a null file and FileNotFoundException when the file
does not exist; LoadFromText(FileInfo) with no format reads the file as ASCII,
the format overloads read it with Format.Encoding.

10. TABLES
----------
    ExcelTable Add(ExcelAddressBase Range, string Name)
    void Delete(int Index, bool ClearRange = false)
    void Delete(string Name, bool ClearRange = false)
    void Delete(ExcelTable Table, bool ClearRange = false)
    ExcelTable GetFromRange(ExcelRangeBase Range)
    ExcelTable this[int Index] / this[string Name]

    var table = ws.Tables.Add(ws.Cells["A1:D10"], "SalesTable");
    table.TableStyle = TableStyles.Medium6;
    table.ShowFilter = true;
    table.ShowHeader = true;
    table.ShowFirstColumn = true;
    table.ShowRowStripes = true;
    table.Columns[0].Name = "Product";

ExcelTable: Name, Address, WorkSheet, Columns, TableStyle, StyleName,
ShowHeader, ShowFilter, ShowTotal, ShowFirstColumn, ShowLastColumn,
ShowRowStripes, ShowColumnStripes, HeaderRowCellStyle, DataCellStyleName,
TotalsRowCellStyle.

ExcelTableColumn: Id, Position, Name, TotalsRowLabel, TotalsRowFunction
(RowFunctions: Average, Count, CountNums, Custom, Max, Min, None, StdDev, Sum,
Var), TotalsRowFormula, CalculatedColumnFormula, DataCellStyleName.

TableStyles values: None, Custom, Light1..Light21, Medium1..Medium28,
Dark1..Dark11.

11. CONDITIONAL FORMATTING
--------------------------
Two entry points, same rule objects:

  -> ws.ConditionalFormatting  (ExcelConditionalFormattingCollection) — every
     Add* method takes an ExcelAddress as its first argument.
  -> range.ConditionalFormatting (IRangeConditionalFormatting) — the same Add*
     methods with NO address argument; the rule applies to that range.

Collection members:

    int Count { get; }
    IExcelConditionalFormattingRule this[int index] { get; }
    IExcelConditionalFormattingRule RulesByPriority(int priority)
    void Remove(IExcelConditionalFormattingRule item)
    void RemoveAt(int index)
    void RemoveByPriority(int priority)
    void RemoveAll()

Every rule implements IExcelConditionalFormattingRule:

    eExcelConditionalFormattingRuleType Type { get; }
    ExcelAddress Address { get; set; }
    int Priority { get; set; }
    bool StopIfTrue { get; set; }
    ExcelDxfStyleConditionalFormatting Style { get; }
    XmlNode Node { get; }

The Style is a DIFFERENTIAL style (OfficeOpenXml.Style.Dxf), not an ExcelStyle.
Its members are nullable and its colors are ExcelDxfColor:

    Style.Font    -> Bold, Italic, Strike, Underline, Color   (ExcelDxfFontBase)
    Style.Fill    -> PatternType, BackgroundColor, PatternColor (ExcelDxfFill)
    Style.Border  -> Left/Right/Top/Bottom (ExcelDxfBorderItem: Style, Color)
    Style.NumberFormat -> Format, NumFmtID
    ExcelDxfColor -> Color (a nullable CodeBrix.Imaging Color), Theme, Index,
                     Auto, Tint

THE RULE FAMILIES AND THEIR Add* METHODS

  (a) Average / standard deviation — return IExcelConditionalFormattingAverageGroup
      or IExcelConditionalFormattingStdDevGroup (the StdDev group adds
      "ushort StdDev"):
        AddAboveAverage(ExcelAddress)
        AddAboveOrEqualAverage(ExcelAddress)
        AddBelowAverage(ExcelAddress)
        AddBelowOrEqualAverage(ExcelAddress)
        AddAboveStdDev(ExcelAddress)
        AddBelowStdDev(ExcelAddress)

  (b) Top / bottom — return IExcelConditionalFormattingTopBottomGroup, which
      adds "ushort Rank":
        AddTop(ExcelAddress)      AddTopPercent(ExcelAddress)
        AddBottom(ExcelAddress)   AddBottomPercent(ExcelAddress)

  (c) Time period — return IExcelConditionalFormattingTimePeriodGroup:
        AddLast7Days  AddLastWeek   AddLastMonth
        AddThisWeek   AddThisMonth
        AddNextWeek   AddNextMonth
        AddYesterday  AddToday      AddTomorrow
      (each takes an ExcelAddress)

  (d) Cell value comparisons — the returned interfaces carry
      "string Formula" (IExcelConditionalFormattingWithFormula) and, for the
      two-sided ones, "string Formula2":
        AddEqual                AddNotEqual
        AddGreaterThan          AddGreaterThanOrEqual
        AddLessThan             AddLessThanOrEqual
        AddBetween              AddNotBetween
        AddExpression           (free-form formula rule)

  (e) Text and blank/error tests — the text ones carry "string Text"
      (IExcelConditionalFormattingWithText):
        AddContainsText         AddNotContainsText
        AddBeginsWith           AddEndsWith
        AddContainsBlanks       AddNotContainsBlanks
        AddContainsErrors       AddNotContainsErrors

  (f) Unique / duplicate:
        AddUniqueValues(ExcelAddress)
        AddDuplicateValues(ExcelAddress)

  (g) Color scales — IExcelConditionalFormattingTwoColorScale exposes LowValue
      and HighValue; the three-color version adds MiddleValue. Each value is an
      ExcelConditionalFormattingColorScaleValue with Type
      (eExcelConditionalFormattingValueObjectType: Formula, Max, Min, Num,
      Percent, Percentile), Value, Formula and Color:
        AddTwoColorScale(ExcelAddress)
        AddThreeColorScale(ExcelAddress)

  (h) Icon sets — Reverse, ShowValue, IconSet (the generic T) and the per-icon
      thresholds Icon1, Icon2, Icon3 (plus Icon4 / Icon5 on the four- and
      five-icon variants), each an ExcelConditionalFormattingIconDataBarValue
      with Type, Value, Formula and GreaterThanOrEqualTo:
        AddThreeIconSet(ExcelAddress, eExcelconditionalFormatting3IconsSetType)
            Arrows, ArrowsGray, Flags, Signs, Symbols, Symbols2,
            TrafficLights1, TrafficLights2
        AddFourIconSet(ExcelAddress, eExcelconditionalFormatting4IconsSetType)
            Arrows, ArrowsGray, Rating, RedToBlack, TrafficLights
        AddFiveIconSet(ExcelAddress, eExcelconditionalFormatting5IconsSetType)
            Arrows, ArrowsGray, Quarters, Rating

  (i) Data bars — IExcelConditionalFormattingDataBarGroup with Color,
      ShowValue, LowValue and HighValue (ExcelConditionalFormattingIconDataBarValue:
      Type, Value, Formula, GreaterThanOrEqualTo):
        AddDatabar(ExcelAddress address, Color color)

The concrete rule classes (ExcelConditionalFormattingAboveAverage,
...Between, ...ContainsText, ...DataBar, ...ThreeIconSet, ...TwoColorScale and
about fifty siblings under OfficeOpenXml.ConditionalFormatting) are created for
you by the Add* methods. Program against the interfaces; you never need to
construct a rule class directly.

Examples:

    // highlight values greater than 100
    var hot = ws.ConditionalFormatting.AddGreaterThan(new ExcelAddress("B2:B50"));
    hot.Formula = "100";
    hot.Style.Fill.PatternType = ExcelFillStyle.Solid;
    hot.Style.Fill.BackgroundColor.Color = Color.LightSalmon;
    hot.Style.Font.Bold = true;

    // three-color scale, tuned
    var scale = ws.ConditionalFormatting.AddThreeColorScale(
                    new ExcelAddress("C2:C50"));
    scale.LowValue.Type = eExcelConditionalFormattingValueObjectType.Num;
    scale.LowValue.Value = 0;
    scale.LowValue.Color = Color.FromArgb(0xFF, 0xF8, 0x69, 0x6B);
    scale.MiddleValue.Type =
        eExcelConditionalFormattingValueObjectType.Percentile;
    scale.MiddleValue.Value = 50;
    scale.HighValue.Type = eExcelConditionalFormattingValueObjectType.Max;

    // data bar and an icon set
    var bar = ws.ConditionalFormatting.AddDatabar(
                  new ExcelAddress("D2:D50"), Color.BlueViolet);
    bar.ShowValue = true;
    var icons = ws.ConditionalFormatting.AddThreeIconSet(
                    new ExcelAddress("E2:E50"),
                    eExcelconditionalFormatting3IconsSetType.TrafficLights1);
    icons.Reverse = true;

    // the range-based form needs no address
    var dup = ws.Cells["F2:F50"].ConditionalFormatting.AddDuplicateValues();
    dup.Style.Font.Color.Color = Color.Red;

    // ordering: lower Priority wins; StopIfTrue halts later rules
    dup.Priority = 1;
    dup.StopIfTrue = true;
    var byPriority = ws.ConditionalFormatting.RulesByPriority(1);

12. PIVOT TABLES
----------------
    ExcelPivotTable Add(ExcelAddressBase Range, ExcelRangeBase Source,
                        string Name)

Range is where the pivot table is PLACED (must be on the worksheet you call it
on); Source is the data range, INCLUDING its header row, usually on another
sheet. Names must be unique across the workbook, and the placement range must
not collide with an existing pivot table.

ExcelPivotTable field collections:

    ExcelPivotTableFieldCollection Fields { get; }             // all fields
    ExcelPivotTableRowColumnFieldCollection RowFields { get; }
    ExcelPivotTableRowColumnFieldCollection ColumnFields { get; }
    ExcelPivotTableRowColumnFieldCollection PageFields { get; }  // filters
    ExcelPivotTableDataFieldCollection DataFields { get; }       // values

    RowFields/ColumnFields/PageFields:
        ExcelPivotTableField Add(ExcelPivotTableField Field)
        void Remove(ExcelPivotTableField Field) / void RemoveAt(int Index)
    DataFields:
        ExcelPivotTableDataField Add(ExcelPivotTableField field)
        void Remove(ExcelPivotTableDataField dataField)
    Fields:
        ExcelPivotTableField this[int Index] / this[string name]
        ExcelPivotTableField GetDateGroupField(eDateGroupBy GroupBy)
        ExcelPivotTableField GetNumericGroupField()

ExcelPivotTableField: Index, Name, Axis (ePivotFieldAxis: None, Column, Page,
Row, Values), IsRowField, IsColumnField, IsPageField, IsDataField, Sort
(eSortType: None, Ascending, Descending), SubTotalFunctions
(eSubTotalFunctions, a FLAGS enum: None, Count, CountA, Avg, Default, Min, Max,
Product, StdDev, StdDevP, Sum, Var, VarP), SubtotalTop, Compact, Outline,
ShowAll, ShowDropDowns, ShowInFieldList, IncludeNewItemsInFilter,
MultipleItemSelectionAllowed, Items, Grouping, PageFieldSettings, plus:

    void AddNumericGrouping(double Start, double End, double Interval)
    void AddDateGrouping(eDateGroupBy groupBy)
    void AddDateGrouping(eDateGroupBy groupBy, DateTime startDate,
                         DateTime endDate)
    void AddDateGrouping(int days, DateTime startDate, DateTime endDate)

eDateGroupBy is a FLAGS enum: Years, Quarters, Months, Days, Hours, Minutes,
Seconds — combine with "|".

ExcelPivotTableDataField: Field, Index, Name, Format, BaseField, BaseItem and
Function (DataFieldFunctions: Average, Count, CountNums, Max, Min, Product,
None, StdDev, StdDevP, Sum, Var, VarP).

Pivot table layout and behaviour: DataOnRows, Compact, CompactData, Outline,
OutlineData, GridDropZones, RowGrandTotals, ColumnGrandTotals, PageWrap,
Indent, MultipleFieldFilters, UseAutoFormatting, EnableDrill, ShowDrill,
ShowHeaders, ShowDataTips, ShowError, ErrorCaption, MissingCaption,
DataCaption, GrandTotalCaption, RowHeaderCaption, ColumnHeaderCaption,
FirstHeaderRow, FirstDataRow, FirstDataCol, TableStyle, StyleName, Address,
WorkSheet, CacheDefinition.

ExcelPivotCacheDefinition exposes SourceRange (settable, to repoint the pivot
at a different range) and CacheSource (eSourceType: Consolidation, External,
Scenario, Worksheet).

Example:

    var wsData = package.Workbook.Worksheets.Add("SalesData");
    var dataRange = wsData.Cells["A1"].LoadFromCollection(sales, true,
                                                          TableStyles.Medium2);

    var wsPivot = package.Workbook.Worksheets.Add("Pivot");
    var pivot = wsPivot.PivotTables.Add(wsPivot.Cells["A3"], dataRange,
                                        "SalesByRegion");

    pivot.RowFields.Add(pivot.Fields["Region"]);
    pivot.ColumnFields.Add(pivot.Fields["Category"]);
    var page = pivot.PageFields.Add(pivot.Fields["Year"]);

    var amount = pivot.DataFields.Add(pivot.Fields["Amount"]);
    amount.Function = DataFieldFunctions.Sum;
    amount.Format = "#,##0.00";
    amount.Name = "Total sales";

    pivot.DataOnRows = false;
    pivot.RowGrandTotals = true;
    pivot.Fields["Region"].SubTotalFunctions =
        eSubTotalFunctions.Sum | eSubTotalFunctions.Count;

    // group a date column by year and quarter
    var dateField = pivot.RowFields.Add(pivot.Fields["OrderDate"]);
    dateField.AddDateGrouping(eDateGroupBy.Years | eDateGroupBy.Quarters);
    var quarters = pivot.Fields.GetDateGroupField(eDateGroupBy.Quarters);
    quarters.Items[1].Text = "Q1";

A chart can be driven by a pivot table:

    var chart = wsPivot.Drawings.AddChart("PivotChart", eChartType.ColumnClustered,
                                          pivot);

FreePPlus writes the pivot table definition and an EMPTY pivot cache marked
"refresh on load"; it does not compute the aggregated cells itself. Excel fills
them in when the file is opened, so do not expect to read pivot results back
through this library.

13. CHARTS
----------
Create a chart on a worksheet or as its own chart sheet:

    ExcelChart ExcelDrawings.AddChart(string Name, eChartType ChartType)
    ExcelChart ExcelDrawings.AddChart(string Name, eChartType ChartType,
                                      ExcelPivotTable PivotTableSource)
    ExcelChartsheet ExcelWorksheets.AddChart(string Name, eChartType chartType)
    ExcelChartsheet ExcelWorksheets.AddChart(string Name, eChartType chartType,
                                             ExcelPivotTable pivotTableSource)

Position and size come from ExcelDrawing (the base class of every chart,
picture and shape):

    void SetPosition(int Row, int RowOffsetPixels,
                     int Column, int ColumnOffsetPixels)
    void SetPosition(int PixelTop, int PixelLeft)
    void SetSize(int PixelWidth, int PixelHeight)
    void SetSize(int Percent)
    void AdjustPositionAndSize()
    string Name { get; set; }
    eEditAs EditAs { get; set; }
    bool Locked { get; set; }
    bool Print { get; set; }
    ExcelPosition From { get; }   // Row, Column, RowOff, ColumnOff
    ExcelPosition To { get; }
    const int EMU_PER_PIXEL = 9525;

ExcelChart members:

    eChartType ChartType { get; }
    ExcelChartTitle Title { get; }
    ExcelChartSeries Series { get; }
    ExcelChartAxis XAxis { get; }
    ExcelChartAxis YAxis { get; }
    ExcelChartAxis[] Axis { get; }
    ExcelChartLegend Legend { get; }
    ExcelChartPlotArea PlotArea { get; }
    ExcelDrawingBorder Border { get; }
    ExcelDrawingFill Fill { get; }
    ExcelView3D View3D { get; }
    eChartStyle Style { get; set; }             // None, Style1..Style48
    bool UseSecondaryAxis { get; set; }
    bool RoundedCorners { get; set; }
    bool VaryColors { get; set; }
    bool ShowHiddenData { get; set; }
    bool ShowDataLabelsOverMaximum { get; set; }
    eDisplayBlanksAs DisplayBlanksAs { get; set; }
    eGrouping Grouping { get; set; }
    ExcelWorksheet WorkSheet { get; }
    ExcelPivotTable PivotTableSource { get; }

Series:

    ExcelChartSeriesItem Add(ExcelRangeBase series, ExcelRangeBase xSeries)
    ExcelChartSeriesItem Add(string seriesAddress, string xSeriesAddress)
    void Delete(int positionId)
    int Count { get; }
    ExcelChartSeriesItem this[int positionId] { get; }

    // bubble charts add a third argument (ExcelBubbleChartSeries):
    Add(ExcelRangeBase series, ExcelRangeBase xSeries,
        ExcelRangeBase bubbleSize)
    Add(string seriesAddress, string xSeriesAddress, string bubbleSizeAddress)

ExcelChartSeriesItem: Header (the legend text), HeaderAddress (point the header
at a cell instead), Series, XSeries, Fill, Border, TrendLines.

Title (ExcelChartTitle): Text, Font, Fill, Border, RichText, Overlay, Rotation,
Anchor, AnchorCtr, TextVertical.

Legend (ExcelChartLegend): Position (eLegendPosition: Top, Left, Right, Bottom,
TopRight), Overlay, Font, Fill, Border, Add(), Remove().

Axis (ExcelChartAxis) — XAxis, YAxis and each element of Axis[]:
    Title, Font, Fill, Border, Format, SourceLinked, Deleted, Orientation
    (eAxisOrientation), AxisPosition (eAxisPosition), Crosses (eCrosses),
    CrossBetween (eCrossBetween), CrossesAt, MinValue, MaxValue, MajorUnit,
    MinorUnit, MajorTimeUnit, MinorTimeUnit, LogBase, DisplayUnit,
    MajorTickMark, MinorTickMark (eAxisTickMark), LabelPosition,
    TickLabelPosition (eTickLabelPosition), MajorGridlines, MinorGridlines,
    RemoveGridlines(), RemoveGridlines(bool removeMajor, bool removeMinor).

Combo charts and secondary axes go through PlotArea.ChartTypes:

    ExcelChartCollection ChartTypes { get; }   // Add(eChartType), indexer, Count
    ExcelChartDataTable CreateDataTable() / void RemoveDataTable()

    var chart = ws.Drawings.AddChart("combo", eChartType.LineMarkers);
    chart.Series.Add(ws.Cells["B2:B24"], ws.Cells["A2:A24"]);

    var bars = chart.PlotArea.ChartTypes.Add(eChartType.ColumnClustered);
    bars.Series.Add(ws.Cells["C2:C24"], ws.Cells["A2:A24"]);

    var secondary = chart.PlotArea.ChartTypes.Add(eChartType.Line);
    var s = secondary.Series.Add(ws.Cells["D2:D24"], ws.Cells["A2:A24"]);
    s.Header = "Margin %";
    secondary.UseSecondaryAxis = true;

Chart-type specific classes (the object returned by AddChart is one of these,
cast when you need the extra members):
    ExcelBarChart      -> Direction (eDirection), Shape (eShape), GapWidth,
                          DataLabel
    ExcelLineChart     -> Marker, Smooth, DataLabel
    ExcelPieChart      -> DataLabel
    ExcelScatterChart  -> ScatterStyle (eScatterStyle), Marker
    ExcelDoughnutChart, ExcelRadarChart, ExcelSurfaceChart, ExcelBubbleChart,
    ExcelOfPieChart

eChartType values (exact names — note the "XYScatter" spellings):
    Area, AreaStacked, AreaStacked100, Area3D, AreaStacked3D, AreaStacked1003D,
    BarClustered, BarStacked, BarStacked100, BarClustered3D, BarStacked3D,
    BarStacked1003D, BarOfPie,
    Column3D, ColumnClustered, ColumnStacked, ColumnStacked100,
    ColumnClustered3D, ColumnStacked3D, ColumnStacked1003D,
    ConeBarClustered, ConeBarStacked, ConeBarStacked100, ConeCol,
    ConeColClustered, ConeColStacked, ConeColStacked100,
    CylinderBarClustered, CylinderBarStacked, CylinderBarStacked100,
    CylinderCol, CylinderColClustered, CylinderColStacked,
    CylinderColStacked100,
    Doughnut, DoughnutExploded,
    Line, Line3D, LineMarkers, LineMarkersStacked, LineMarkersStacked100,
    LineStacked, LineStacked100,
    Pie, Pie3D, PieExploded, PieExploded3D, PieOfPie,
    PyramidBarClustered, PyramidBarStacked, PyramidBarStacked100, PyramidCol,
    PyramidColClustered, PyramidColStacked, PyramidColStacked100,
    Radar, RadarFilled, RadarMarkers,
    StockHLC, StockOHLC, StockVHLC, StockVOHLC,
    Surface, SurfaceWireframe, SurfaceTopView, SurfaceTopViewWireframe,
    Bubble, Bubble3DEffect,
    XYScatter, XYScatterLines, XYScatterLinesNoMarkers, XYScatterSmooth,
    XYScatterSmoothNoMarkers

There are no members named "Scatter", "ScatterLines" or "ScatterSmooth" — use
the XYScatter* names above.

14. SPARKLINES
--------------
    ExcelSparklineGroup ExcelSparklineGroupCollection.Add(
        eSparklineType type,
        ExcelAddressBase locationRange,
        ExcelAddressBase dataRange)
    void RemoveAt(int index) / void Remove(ExcelSparklineGroup group)
    int Count { get; } / ExcelSparklineGroup this[int index] { get; }

eSparklineType: Line, Column, Stacked.

ExcelSparklineGroup: Type, DataRange, LocationRange, DateAxisRange, Sparklines,
LineWidth, DisplayEmptyCellsAs (eDispBlanksAs: Span, Gap, Zero), Markers, High,
Low, First, Last, Negative, DisplayXAxis, DisplayHidden, RightToLeft,
ManualMin, ManualMax, MinAxisType / MaxAxisType (eSparklineAxisMinMax:
Individual, Group, Custom) and the color properties ColorSeries, ColorNegative,
ColorAxis, ColorMarkers, ColorFirst, ColorLast, ColorHigh, ColorLow (each an
ExcelSparklineColor with SetColor(Color), Rgb, Indexed, Theme, Tint).

The location range and the data range must line up: one sparkline is produced
per row (or per column) of the data range, mapped onto the cells of the
location range.

    var sg = ws.SparklineGroups.Add(eSparklineType.Line,
                                    ws.Cells["A1:A4"],     // where they go
                                    ws.Cells["B1:C4"]);    // the data
    sg.DisplayEmptyCellsAs = eDispBlanksAs.Gap;
    sg.Markers = true;
    sg.High = true;
    sg.Low = true;
    sg.ColorSeries.SetColor(Color.SteelBlue);
    sg.MinAxisType = eSparklineAxisMinMax.Custom;
    sg.ManualMin = 0;

15. PICTURES, SHAPES AND BACKGROUND IMAGES
------------------------------------------
    ExcelPicture AddPicture(string Name, Image image)
    ExcelPicture AddPicture(string Name, Image image, Uri Hyperlink)
    ExcelPicture AddPicture(string Name, FileInfo ImageFile)
    ExcelPicture AddPicture(string Name, FileInfo ImageFile, Uri Hyperlink)
    ExcelShape   AddShape(string Name, eShapeStyle Style)
    ExcelShape   AddShape(string Name, ExcelShape Source)
    void Remove(int Index) / Remove(ExcelDrawing) / Remove(string Name)
    void Clear()
    ExcelDrawing this[int PositionID] / this[string Name]

Image is CodeBrix.Imaging.Image:

    using CodeBrix.Imaging;

    var image = Image.Load("photo.jpg");
    var picture = ws.Drawings.AddPicture("Logo", image);
    picture.SetPosition(0, 0, 5, 0);
    picture.SetSize(200, 100);

    // or straight from a file
    ws.Drawings.AddPicture("Logo2", new FileInfo("photo.jpg"));

    // a blank image of a given size, handy for tests and placeholders
    using var blank = ExcelPicture.CreateImage(100, 50);   // width, height
    ws.Drawings.AddPicture("Placeholder", blank);

ExcelPicture: Image, ImageFormat, Hyperlink, Fill, Border, plus the inherited
SetPosition/SetSize. An image constructed in memory has no known encoded
format; FreePPlus falls back to PNG when it stores such an image.

ExcelShape: Style (eShapeStyle — about 180 preset shapes, e.g. Rect,
RoundRect, Ellipse, Triangle, Diamond, Arrow, Star5, FlowChartProcess), Text,
RichText, Font, Fill, Border, LineEnds, TextAnchoring, TextAnchoringControl,
TextAlignment (eTextAlignment), TextVertical, Indent, LockText.

    var shape = ws.Drawings.AddShape("callout", eShapeStyle.RoundRect);
    shape.SetPosition(100, 100);
    shape.SetSize(300, 120);
    shape.Text = "Reviewed";
    shape.Fill.Style = eFillStyle.SolidFill;
    shape.Fill.Color = Color.LightYellow;
    shape.Fill.Transparancy = 50;              // note the spelling
    shape.Border.LineStyle = eLineStyle.Solid; // Dash, DashDot, Dot, LongDash,
                                               // LongDashDot, LongDashDotDot,
                                               // Solid, SystemDash,
                                               // SystemDashDot,
                                               // SystemDashDotDot, SystemDot
    shape.Border.Width = 2;

ExcelDrawingFill (Fill on shapes, pictures, charts, series, axes, legends and
titles): Style (eFillStyle), Color, Transparancy. eFillStyle READS as NoFill,
SolidFill, GradientFill, PatternFill, BlipFill or GroupFill, but only NoFill
and SolidFill can be ASSIGNED — anything else throws NotImplementedException.
Setting Fill.Color when the style is already something other than SolidFill
throws; set Fill.Style = eFillStyle.SolidFill first, or set Color on a fill
that has not been configured yet.

ExcelDrawingBorder: Fill, LineStyle (eLineStyle), LineCap (eLineCap), Width.
ExcelDrawingLineEnd (shape.LineEnds): head/tail styles using eEndStyle and
eEndSize.

NAMESPACE QUIRK: five drawing enums — eShapeStyle, eTextAlignment, eFillStyle,
eEndStyle and eEndSize — are declared in the GLOBAL namespace, not under
OfficeOpenXml.Drawing. They resolve with no using directive at all, and adding
"using OfficeOpenXml.Drawing;" does not help if you cannot find them. Everything
else in this area is under OfficeOpenXml.Drawing as expected.

Worksheet background image (not printed by Excel, shown on screen):

    ws.BackgroundImage.SetFromFile(new FileInfo("watermark.png"));
    ws.BackgroundImage.Image = someImage;

Header/footer pictures:

    ws.HeaderFooter.OddHeader.InsertPicture(new FileInfo("logo.png"),
                                            PictureAlignment.Left);

16. COMMENTS
------------
    ExcelComment AddComment(string Text, string Author)      // on a range
    ExcelComment ExcelCommentCollection.Add(ExcelRangeBase cell, string Text,
                                            string author)
    void Remove(ExcelComment comment) / void RemoveAt(int Index)
    int Count { get; }
    ExcelComment this[int Index] / this[ExcelCellAddress cell]

ExcelComment: Author, Text, Font (an ExcelRichText), RichText.

    ws.Cells["A1"].AddComment("Check this figure", "Reviewer");
    var c = ws.Cells["A1"].Comment;         // null when there is none
    c.Font.Bold = true;
    ws.Comments.RemoveAt(0);

17. DATA VALIDATION
-------------------
Worksheet-level (ws.DataValidations, address as a string):

    IExcelDataValidationAny      AddAnyValidation(string address)
    IExcelDataValidationInt      AddIntegerValidation(string address)
    IExcelDataValidationDecimal  AddDecimalValidation(string address)
    IExcelDataValidationList     AddListValidation(string address)
    IExcelDataValidationInt      AddTextLengthValidation(string address)
    IExcelDataValidationDateTime AddDateTimeValidation(string address)
    IExcelDataValidationTime     AddTimeValidation(string address)
    IExcelDataValidationCustom   AddCustomValidation(string address)
    bool Remove(IExcelDataValidation item) / void Clear()
    void RemoveAll(Predicate<IExcelDataValidation> match)
    IExcelDataValidation Find(Predicate<IExcelDataValidation> match)
    IEnumerable<IExcelDataValidation> FindAll(Predicate<IExcelDataValidation>)
    int Count { get; }
    IExcelDataValidation this[int index] / this[string address]

Range-level (range.DataValidation, no address argument):
    AddAnyDataValidation, AddIntegerDataValidation, AddDecimalDataValidation,
    AddListDataValidation, AddTextLengthDataValidation,
    AddDateTimeDataValidation, AddTimeDataValidation, AddCustomDataValidation

Every validation carries (IExcelDataValidation):
    ExcelAddress Address { get; }
    ExcelDataValidationType ValidationType { get; }
    ExcelDataValidationWarningStyle ErrorStyle { get; set; }   // undefined,
                                                               // stop, warning,
                                                               // information
    bool? AllowBlank, ShowInputMessage, ShowErrorMessage
    string ErrorTitle, Error, PromptTitle, Prompt
    void Validate()

Typed validations add an Operator (ExcelDataValidationOperator: between,
notBetween, equal, notEqual, greaterThan, greaterThanOrEqual, lessThan,
lessThanOrEqual) and Formula / Formula2 objects. Every formula object has
"string ExcelFormula"; the typed ones also have a "Value" of the matching
nullable type:
    Integer / text-length -> Formula.Value is int?
    Decimal              -> double?
    DateTime             -> DateTime?
    Time                 -> ExcelTime
    List                 -> no Value; use Formula.Values (IList<string>)
    Custom / Any         -> ExcelFormula only

    var v = ws.DataValidations.AddIntegerValidation("B2:B100");
    v.Operator = ExcelDataValidationOperator.between;
    v.Formula.Value = 1;
    v.Formula2.Value = 100;
    v.ShowErrorMessage = true;
    v.ErrorStyle = ExcelDataValidationWarningStyle.stop;
    v.ErrorTitle = "Invalid Entry";
    v.Error = "Please enter a number between 1 and 100";
    v.PromptTitle = "Quantity";
    v.Prompt = "1 to 100";
    v.ShowInputMessage = true;

    var list = ws.DataValidations.AddListValidation("C2:C100");
    list.Formula.Values.Add("Option A");
    list.Formula.Values.Add("Option B");
    list.AllowBlank = true;

    var custom = ws.Cells["D2:D100"].DataValidation.AddCustomDataValidation();
    custom.Formula.ExcelFormula = "ISNUMBER(D2)";

18. HYPERLINKS
--------------
    ws.Cells["A1"].Value = "Click here";
    ws.Cells["A1"].Hyperlink = new Uri("https://example.com");
    ws.Cells["A1"].StyleName = "HyperLink";   // after creating a named style

In-workbook link, with display text and tooltip (ExcelHyperLink derives from
Uri):

    var link = new ExcelHyperLink("Sheet2!A1", "Go to Sheet2");
    link.ToolTip = "Jump to the summary";
    ws.Cells["A2"].Hyperlink = link;

ExcelHyperLink constructors: (string uriString), (string uriString,
UriKind uriKind), (string referenceAddress, string display). Members:
ReferenceAddress, Display, ToolTip, ColSpann, RowSpann, OriginalUri.

19. PROTECTION AND ENCRYPTION
-----------------------------
Worksheet protection (ws.Protection, ExcelSheetProtection):

    void SetPassword(string Password)
    bool IsProtected { get; set; }
    AllowSelectLockedCells, AllowSelectUnlockedCells, AllowFormatCells,
    AllowFormatColumns, AllowFormatRows, AllowInsertColumns, AllowInsertRows,
    AllowInsertHyperlinks, AllowDeleteColumns, AllowDeleteRows, AllowSort,
    AllowAutoFilter, AllowPivotTables, AllowEditObject, AllowEditScenarios

Cells are protected only when the sheet is protected AND the cell style is
locked (Style.Locked, true by default). Unlock the cells you want editable:

    ws.Cells["C2:D100"].Style.Locked = false;
    ws.Protection.SetPassword("sheetPass");
    ws.Protection.AllowSelectUnlockedCells = true;

Named protected ranges:

    ws.ProtectedRanges.Add("EditableBlock", new ExcelAddress("C2:D100"));

Workbook protection (package.Workbook.Protection, ExcelProtection):

    void SetPassword(string Password)
    bool LockStructure { get; set; }
    bool LockWindows { get; set; }
    bool LockRevision { get; set; }

FILE ENCRYPTION (a real, password-to-open encrypted .xlsx):

    package.Encryption.IsEncrypted = true;
    package.Encryption.Password = "openPassword";
    package.Encryption.Algorithm = EncryptionAlgorithm.AES256;  // AES128,
                                                                // AES192,
                                                                // AES256
    package.Encryption.Version = EncryptionVersion.Agile;       // Standard or
                                                                // Agile
    package.SaveAs(new FileInfo("secure.xlsx"));

Passing a password to SaveAs / GetAsByteArray / Save is the shorthand and sets
the same settings:

    package.SaveAs(new FileInfo("secure.xlsx"), "openPassword");
    byte[] bytes = package.GetAsByteArray("openPassword");

Reading it back requires the same password in the constructor or Load().

20. VBA MACROS
--------------
    package.Workbook.CreateVBAProject();          // creates an empty project
    ExcelVbaProject vba = package.Workbook.VbaProject;   // null if none

ExcelVbaProject: Name, Description, Modules, References, Protection, Signature,
SystemKind, Constants, HelpFile1, HelpFile2, HelpContextID, CodePage,
Remove().

Modules (ExcelVbaModuleCollection):

    ExcelVBAModule AddModule(string Name)
    ExcelVBAModule AddClass(string Name, bool Exposed)
    bool Exists(string Name)
    void Remove(ExcelVBAModule Item) / void RemoveAt(int index)
    ExcelVBAModule this[string Name] / this[int Index]
    int Count { get; }

ExcelVBAModule: Name, Code, Description, Type (eModuleType), Attributes,
HelpContext, ReadOnly, Private.

The workbook and every worksheet also expose a built-in code module:

    package.Workbook.CodeModule.Code =
        "Private Sub Workbook_Open()\r\n  MsgBox \"Hi\"\r\nEnd Sub";
    ws.CodeModule.Code = "' sheet-level code";

Protection and signing:

    package.Workbook.VbaProject.Protection.SetPassword("vbaPass");
    // Protection also exposes UserProtected, HostProtected, VbeProtected,
    // VisibilityState (all read-only).
    package.Workbook.VbaProject.Signature.Certificate =
        new X509Certificate2("codesign.pfx", "pfxPassword");
    // Signature.Verifier (EnvelopedCms) is populated when reading a signed file.

Complete example:

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Macros");
    package.Workbook.CreateVBAProject();
    var module = package.Workbook.VbaProject.Modules.AddModule("Helpers");
    module.Code = "Public Sub Hello()\r\n  MsgBox \"Hello\"\r\nEnd Sub";
    package.SaveAs(new FileInfo("macros.xlsm"));   // .xlsm, not .xlsx

IMPORTANT: save a workbook that has a VBA project with the .xlsm extension.
FreePPlus switches the internal content type to macro-enabled automatically,
but Excel decides how to open the file from its extension.

21. HEADER/FOOTER, PRINT SETTINGS AND VIEWS
-------------------------------------------
Header and footer (ws.HeaderFooter, ExcelHeaderFooter): OddHeader, OddFooter,
EvenHeader, EvenFooter, FirstHeader, FirstFooter (each an
ExcelHeaderFooterText with LeftAlignedText, CenteredText, RightAlignedText),
plus DifferentOddEven, DifferentFirst, AlignWithMargins, ScaleWithDocument and
Pictures.

Format codes are exposed as constants: ExcelHeaderFooter.PageNumber ("&P"),
NumberOfPages ("&N"), SheetName ("&A"), FileName ("&F"), FilePath ("&Z"),
CurrentDate ("&D"), CurrentTime ("&T"), FontColor ("&K"), Image ("&G"),
OutlineStyle ("&O"), ShadowStyle ("&H").

    ws.HeaderFooter.OddHeader.LeftAlignedText = "Quarterly Report";
    ws.HeaderFooter.OddFooter.CenteredText =
        $"Page {ExcelHeaderFooter.PageNumber} of "
        + ExcelHeaderFooter.NumberOfPages;

Print settings (ws.PrinterSettings, ExcelPrinterSettings): Orientation
(eOrientation: Portrait, Landscape), PaperSize (ePaperSize), Scale, FitToPage,
FitToWidth, FitToHeight, LeftMargin, RightMargin, TopMargin, BottomMargin,
HeaderMargin, FooterMargin, ShowGridLines, ShowHeaders, HorizontalCentered,
VerticalCentered, BlackAndWhite, Draft, PageOrder (ePageOrder), PrintArea,
RepeatRows, RepeatColumns.

    ws.PrinterSettings.Orientation = eOrientation.Landscape;
    ws.PrinterSettings.FitToPage = true;
    ws.PrinterSettings.FitToWidth = 1;
    ws.PrinterSettings.FitToHeight = 0;
    ws.PrinterSettings.TopMargin = 0.75m;      // decimals, inches
    ws.PrinterSettings.PrintArea = ws.Cells["A1:F60"];
    ws.PrinterSettings.RepeatRows = new ExcelAddress("1:1");

Page breaks: ws.Row(n).PageBreak = true; ws.Column(n).PageBreak = true.

Workbook view (package.Workbook.View, ExcelWorkbookView): ActiveTab,
ShowSheetTabs, ShowHorizontalScrollBar, ShowVerticalScrollBar, Minimized,
Left, Top, Width, Height, SetWindowSize(int left, int top, int width,
int height).

22. DOCUMENT PROPERTIES
-----------------------
package.Workbook.Properties (OfficeProperties):

    Core:     Title, Subject, Author, Comments, Keywords, Category, Status,
              LastModifiedBy, LastPrinted, Created, Modified
    Extended: Application, AppVersion, Company, Manager, HyperlinkBase,
              LinksUpToDate, HyperlinksChanged, ScaleCrop, SharedDoc
    Custom:   object GetCustomPropertyValue(string propertyName)
              void SetCustomPropertyValue(string propertyName, object value)
              string GetExtendedPropertyValue(string propertyName)
              void SetExtendedPropertyValue(string propertyName, string value)

    package.Workbook.Properties.Title = "Q3 Sales";
    package.Workbook.Properties.Author = "Reporting Service";
    package.Workbook.Properties.SetCustomPropertyValue("Environment", "prod");

23. WORKSHEET COLLECTION INDEXING
---------------------------------
Cell row/column indices are ALWAYS 1-based: ws.Cells[1, 1] is A1 and
ws.Cells[0, 0] throws. The WORKSHEET COLLECTION is 0-based by default and can
be switched to 1-based.

In code, per package:

    package.Compatibility.IsWorksheets1Based = true;
    // now Worksheets[1] is the first sheet and Worksheets[0] throws

Or for the whole application, in appsettings.json next to the executable:

    {
      "FreePPlus": {
        "ExcelPackage": {
          "Compatibility": {
            "IsWorksheets1Based": true
          }
        }
      }
    }

The equivalent legacy key is also honoured, so a settings file carried over
from the upstream library keeps working:

    {
      "EPPlus": {
        "ExcelPackage": {
          "Compatibility": {
            "IsWorksheets1Based": true
          }
        }
      }
    }

The file is optional; when it is absent the default (0-based) applies. Set the
property in code if you want to be independent of deployment files.

24. EXTENDING THE FORMULA PARSER
--------------------------------
package.Workbook.FormulaParserManager (FormulaParserManager):

    void LoadFunctionModule(IFunctionModule module)
    void AddOrReplaceFunction(string functionName, ExcelFunction functionImpl)
    void CopyFunctionsFrom(ExcelWorkbook otherWorkbook)
    IEnumerable<string> GetImplementedFunctionNames()
    IEnumerable<KeyValuePair<string, ExcelFunction>> GetImplementedFunctions()
    object Parse(string formula)
    void AttachLogger(IFormulaParserLogger logger)
    void AttachLogger(FileInfo logfile)
    void DetachLogger()

Write a function by deriving from ExcelFunction and overriding one method:

    public abstract CompileResult Execute(IEnumerable<FunctionArgument> arguments,
                                          ParsingContext context)

Useful protected helpers on ExcelFunction: ValidateArguments(arguments, min),
ArgToInt, ArgToDecimal, ArgToString, ArgToBool, ArgToAddress,
ArgsToDoubleEnumerable, ArgsToObjectEnumerable, CreateResult(object, DataType),
GetResultByObject, ThrowExcelErrorValueException(eErrorType),
ThrowArgumentExceptionIf(...). Also overridable: BeforeInvoke(context),
IsLookupFuction, IsErrorHandlingFunction.

DataType values for CreateResult: Integer, Decimal, String, Boolean, Date,
Time, Enumerable, LookupArray, ExcelAddress, ExcelError, Empty, Unknown.

Group several functions in a module by deriving from FunctionsModule, which
implements IFunctionModule (Functions and CustomCompilers dictionaries).

    class SumAddTwo : ExcelFunction
    {
        public override CompileResult Execute(
            IEnumerable<FunctionArgument> arguments, ParsingContext context)
        {
            ValidateArguments(arguments, 1);
            var numbers = ArgsToDoubleEnumerable(arguments, context);
            var result = 0d;
            foreach (var n in numbers) result += n + 2;
            return CreateResult(result, DataType.Decimal);
        }
    }

    class MyFunctions : FunctionsModule
    {
        public MyFunctions() { Functions.Add("sum.addtwo", new SumAddTwo()); }
    }

    package.Workbook.FormulaParserManager.LoadFunctionModule(new MyFunctions());
    // or a single function, which can also REPLACE a built-in:
    package.Workbook.FormulaParserManager
           .AddOrReplaceFunction("sum.addtwo", new SumAddTwo());

    ws.Cells["A4"].Formula = "SUM.ADDTWO(A1:A2)";
    ws.Calculate();

Register function names in lower case; formula lookup is case-insensitive.

================================================================================

COMPLETE EXAMPLES
=================

Example 1: Sales report with styling and formulas
-------------------------------------------------
    using System.IO;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Sales Report");

    ws.Cells["A1"].Value = "Product";
    ws.Cells["B1"].Value = "Quantity";
    ws.Cells["C1"].Value = "Unit Price";
    ws.Cells["D1"].Value = "Total";

    ws.Cells["A2"].Value = "Widget"; ws.Cells["B2"].Value = 25;
    ws.Cells["C2"].Value = 3.50;
    ws.Cells["A3"].Value = "Gadget"; ws.Cells["B3"].Value = 10;
    ws.Cells["C3"].Value = 12.99;
    ws.Cells["A4"].Value = "Gizmo";  ws.Cells["B4"].Value = 50;
    ws.Cells["C4"].Value = 1.75;

    ws.Cells["D2:D4"].Formula = "B2*C2";

    using (var header = ws.Cells["A1:D1"])
    {
        header.Style.Font.Bold = true;
        header.Style.Fill.PatternType = ExcelFillStyle.Solid;
        header.Style.Fill.BackgroundColor.SetColor(255, 0, 51, 102);
        header.Style.Font.Color.SetColor(255, 255, 255, 255);
    }

    ws.Cells["C2:D4"].Style.Numberformat.Format = "#,##0.00";
    ws.Cells["A1:D4"].AutoFitColumns();
    ws.View.FreezePanes(2, 1);

    package.SaveAs(new FileInfo("SalesReport.xlsx"));

Example 2: Load a collection into a styled table
------------------------------------------------
    using System.IO;
    using OfficeOpenXml;
    using OfficeOpenXml.Table;

    var data = new[]
    {
        new { Name = "Alice",   Department = "Engineering", Salary =  95000m },
        new { Name = "Bob",     Department = "Marketing",   Salary =  72000m },
        new { Name = "Charlie", Department = "Engineering", Salary = 105000m },
    };

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Employees");

    var filled = ws.Cells["A1"].LoadFromCollection(data, true,
                                                   TableStyles.Medium6);
    ws.Cells[2, 3, filled.End.Row, 3].Style.Numberformat.Format = "#,##0";
    ws.Cells.AutoFitColumns();

    package.SaveAs(new FileInfo("Employees.xlsx"));

Example 3: Read and modify an existing file
-------------------------------------------
    using System;
    using System.IO;
    using OfficeOpenXml;

    using var package = new ExcelPackage(new FileInfo("input.xlsx"));
    var ws = package.Workbook.Worksheets["Sheet1"];

    if (ws.Dimension != null)
    {
        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var name = ws.Cells[row, 1].Text;
            var value = ws.Cells[row, 2].GetValue<int>();
            Console.WriteLine($"{name}: {value}");
            ws.Cells[row, 3].Value = "Processed";
        }
        ws.Cells[1, 3].Value = "Status";
    }

    package.SaveAs(new FileInfo("output.xlsx"));

Example 4: In-memory workbook for a web API
-------------------------------------------
    using OfficeOpenXml;

    public byte[] GenerateExcelReport(IEnumerable<OrderDto> orders)
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Orders");
        ws.Cells["A1"].LoadFromCollection(orders, true);
        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

Example 5: Multi-sheet workbook with cross-sheet formulas
---------------------------------------------------------
    using System.IO;
    using OfficeOpenXml;

    using var package = new ExcelPackage();

    var dataSheet = package.Workbook.Worksheets.Add("Data");
    dataSheet.Cells["A1"].Value = 100;
    dataSheet.Cells["A2"].Value = 200;
    dataSheet.Cells["A3"].Value = 300;

    var summary = package.Workbook.Worksheets.Add("Summary");
    summary.Cells["A1"].Value = "Total";
    summary.Cells["B1"].Formula = "SUM(Data!A1:A3)";
    summary.Cells["A2"].Value = "Average";
    summary.Cells["B2"].Formula = "AVERAGE(Data!A1:A3)";

    package.Workbook.Calculate();          // B1 -> 600, B2 -> 200

    package.SaveAs(new FileInfo("MultiSheet.xlsx"));

Example 6: Conditional formatting dashboard
-------------------------------------------
    using System.IO;
    using CodeBrix.Imaging;
    using OfficeOpenXml;
    using OfficeOpenXml.ConditionalFormatting;
    using OfficeOpenXml.Style;

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Scores");

    ws.Cells["A1"].Value = "Team";
    ws.Cells["B1"].Value = "Score";
    ws.Cells["C1"].Value = "Trend";
    ws.Cells["D1"].Value = "Status";
    for (int row = 2; row <= 11; row++)
    {
        ws.Cells[row, 1].Value = $"Team {row - 1}";
        ws.Cells[row, 2].Value = (row * 7) % 100;
        ws.Cells[row, 3].Value = (row * 13) % 50;
        ws.Cells[row, 4].Value = row % 3 == 0 ? "AT RISK" : "OK";
    }

    // 1. cell-value rule with a differential style
    var risk = ws.ConditionalFormatting.AddContainsText(
                   new ExcelAddress("D2:D11"));
    risk.Text = "AT RISK";
    risk.Style.Fill.PatternType = ExcelFillStyle.Solid;
    risk.Style.Fill.BackgroundColor.Color = Color.MistyRose;
    risk.Style.Font.Color.Color = Color.DarkRed;
    risk.Style.Font.Bold = true;

    // 2. top 3 scores
    var top = ws.ConditionalFormatting.AddTop(new ExcelAddress("B2:B11"));
    top.Rank = 3;
    top.Style.Font.Bold = true;

    // 3. three-color scale across the same column
    var scale = ws.ConditionalFormatting.AddThreeColorScale(
                    new ExcelAddress("B2:B11"));
    scale.LowValue.Type = eExcelConditionalFormattingValueObjectType.Min;
    scale.MiddleValue.Type =
        eExcelConditionalFormattingValueObjectType.Percentile;
    scale.MiddleValue.Value = 50;
    scale.HighValue.Type = eExcelConditionalFormattingValueObjectType.Max;

    // 4. data bar
    var bar = ws.ConditionalFormatting.AddDatabar(
                  new ExcelAddress("C2:C11"), Color.SteelBlue);
    bar.ShowValue = true;

    // 5. icon set, applied through the range API
    var icons = ws.Cells["C2:C11"].ConditionalFormatting.AddThreeIconSet(
                    eExcelconditionalFormatting3IconsSetType.Arrows);
    icons.Reverse = false;

    // 6. duplicate detection, evaluated first and stopping the rest
    var dup = ws.ConditionalFormatting.AddDuplicateValues(
                  new ExcelAddress("A2:A11"));
    dup.Style.Font.Italic = true;
    dup.Priority = 1;
    dup.StopIfTrue = true;

    ws.Cells.AutoFitColumns();
    package.SaveAs(new FileInfo("Dashboard.xlsx"));

Example 7: Pivot table over a loaded collection
-----------------------------------------------
    using System.IO;
    using OfficeOpenXml;
    using OfficeOpenXml.Table;
    using OfficeOpenXml.Table.PivotTable;

    var sales = new[]
    {
        new { Region = "North", Category = "Tools", Amount = 1200m },
        new { Region = "North", Category = "Parts", Amount =  800m },
        new { Region = "South", Category = "Tools", Amount = 1500m },
        new { Region = "South", Category = "Parts", Amount =  650m },
        new { Region = "West",  Category = "Tools", Amount =  900m },
    };

    using var package = new ExcelPackage();

    var wsData = package.Workbook.Worksheets.Add("SalesData");
    var dataRange = wsData.Cells["A1"].LoadFromCollection(sales, true,
                                                          TableStyles.Medium2);
    wsData.Cells.AutoFitColumns();

    var wsPivot = package.Workbook.Worksheets.Add("Pivot");
    var pivot = wsPivot.PivotTables.Add(wsPivot.Cells["A1"], dataRange,
                                        "SalesByRegion");

    pivot.RowFields.Add(pivot.Fields["Region"]);
    pivot.ColumnFields.Add(pivot.Fields["Category"]);

    var amount = pivot.DataFields.Add(pivot.Fields["Amount"]);
    amount.Function = DataFieldFunctions.Sum;
    amount.Format = "#,##0.00";
    amount.Name = "Total";

    pivot.DataOnRows = false;
    pivot.RowGrandTotals = true;
    pivot.ColumnGrandTotals = true;
    pivot.TableStyle = TableStyles.Medium9;

    // a chart fed by the pivot table
    var chart = wsPivot.Drawings.AddChart("PivotChart",
                                          eChartType.ColumnClustered, pivot);
    chart.SetPosition(1, 0, 6, 0);
    chart.SetSize(600, 380);

    package.SaveAs(new FileInfo("Pivot.xlsx"));

Example 8: Fully customised combo chart
---------------------------------------
    using System.IO;
    using CodeBrix.Imaging;
    using OfficeOpenXml;
    using OfficeOpenXml.Drawing.Chart;

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Monthly");

    ws.Cells["A1"].Value = "Month";
    ws.Cells["B1"].Value = "Revenue";
    ws.Cells["C1"].Value = "Margin %";
    string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
    double[] revenue = { 1200, 1500, 1100, 1750, 1900, 2100 };
    double[] margin  = { 0.21, 0.24, 0.19, 0.27, 0.28, 0.31 };
    for (int i = 0; i < months.Length; i++)
    {
        ws.Cells[i + 2, 1].Value = months[i];
        ws.Cells[i + 2, 2].Value = revenue[i];
        ws.Cells[i + 2, 3].Value = margin[i];
    }
    ws.Cells["C2:C7"].Style.Numberformat.Format = "0%";

    var chart = ws.Drawings.AddChart("Performance",
                                     eChartType.ColumnClustered);
    chart.SetPosition(1, 0, 4, 0);
    chart.SetSize(700, 400);
    chart.Style = eChartStyle.Style25;

    var revenueSeries = chart.Series.Add(ws.Cells["B2:B7"],
                                         ws.Cells["A2:A7"]);
    revenueSeries.Header = "Revenue";
    revenueSeries.Fill.Style = eFillStyle.SolidFill;
    revenueSeries.Fill.Color = Color.SteelBlue;

    chart.Title.Text = "Revenue and margin";
    chart.Title.Font.Size = 14;
    chart.Title.Font.Bold = true;

    chart.Legend.Position = eLegendPosition.Bottom;
    chart.Legend.Font.Size = 9;

    chart.XAxis.Title.Text = "Month";
    chart.YAxis.Title.Text = "Revenue";
    chart.YAxis.MinValue = 0;
    chart.YAxis.MajorUnit = 500;
    chart.YAxis.Format = "#,##0";
    chart.YAxis.RemoveGridlines(removeMajor: false, removeMinor: true);

    // second chart type on a secondary axis
    var line = chart.PlotArea.ChartTypes.Add(eChartType.Line);
    var marginSeries = line.Series.Add(ws.Cells["C2:C7"], ws.Cells["A2:A7"]);
    marginSeries.Header = "Margin %";
    line.UseSecondaryAxis = true;
    line.YAxis.Format = "0%";
    line.YAxis.Title.Text = "Margin";

    package.SaveAs(new FileInfo("ComboChart.xlsx"));

Example 9: A custom worksheet function
--------------------------------------
    using System.Collections.Generic;
    using System.IO;
    using OfficeOpenXml;
    using OfficeOpenXml.FormulaParsing;
    using OfficeOpenXml.FormulaParsing.Excel.Functions;
    using OfficeOpenXml.FormulaParsing.ExpressionGraph;

    public sealed class SumOfSquares : ExcelFunction
    {
        public override CompileResult Execute(
            IEnumerable<FunctionArgument> arguments, ParsingContext context)
        {
            ValidateArguments(arguments, 1);
            var result = 0d;
            foreach (var n in ArgsToDoubleEnumerable(arguments, context))
                result += n * n;
            return CreateResult(result, DataType.Decimal);
        }
    }

    public sealed class MyFunctions : FunctionsModule
    {
        public MyFunctions()
        {
            Functions.Add("sumofsquares", new SumOfSquares());
        }
    }

    using var package = new ExcelPackage();
    package.Workbook.FormulaParserManager.LoadFunctionModule(new MyFunctions());

    var ws = package.Workbook.Worksheets.Add("Custom");
    ws.Cells["A1"].Value = 3;
    ws.Cells["A2"].Value = 4;
    ws.Cells["A3"].Formula = "SUMOFSQUARES(A1:A2)";
    ws.Calculate();
    // ws.Cells["A3"].Value is now 25

    package.SaveAs(new FileInfo("CustomFunction.xlsx"));

================================================================================

MINIMUM VIABLE PROJECT
======================
    dotnet new console -n MyExcelApp --framework net10.0
    cd MyExcelApp
    dotnet add package FreePPlus.LgplLicenseForever

MyExcelApp.csproj:

    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="FreePPlus.LgplLicenseForever" />
      </ItemGroup>
    </Project>

Program.cs:

    using System;
    using System.IO;
    using OfficeOpenXml;

    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Sheet1");
    ws.Cells["A1"].Value = "Hello, Excel!";
    ws.Cells["B1"].Value = 42;
    ws.Cells["C1"].Formula = "B1*2";
    ws.Calculate();
    ws.Cells.AutoFitColumns();
    package.SaveAs(new FileInfo("output.xlsx"));
    Console.WriteLine("Created output.xlsx");

    dotnet build
    dotnet run

================================================================================

PERFORMANCE TIPS
================
 1. Use LoadFromCollection / LoadFromDataTable / LoadFromArrays instead of a
    loop that assigns cells one by one. They write straight into the cell store
    and handle headers, types and table creation in one pass.

 2. Style RANGES, not cells. ws.Cells["A1:D1000"].Style.Font.Bold = true
    creates one style record; a thousand single-cell assignments create work
    for every one of them. Named styles (Workbook.Styles.CreateNamedStyle plus
    range.StyleName) are cheaper still when the same look repeats.

 3. Call AutoFitColumns AFTER the data is in place, and scope it to the used
    range (ws.Cells["A1:F500"].AutoFitColumns()) rather than the whole sheet.
    Auto-fit measures text with real font metrics and is the most expensive
    routine in a typical report.

 4. Use GetAsByteArray() or SaveAs(Stream) for web and API responses; never
    write a temporary file to disk just to read it back.

 5. Prefer the async saves (SaveAsync, SaveAsAsync) on server request paths so
    the compression and serialisation work does not block the thread.

 6. Package.Compression trades size against time: CompressionLevel.BestSpeed
    for large throwaway exports, Default for normal use.

 7. Only call Calculate() when your CODE needs the results. Excel recalculates
    on open anyway; set package.Workbook.FullCalcOnLoad = true if that is all
    you need. Calculate() on the whole workbook walks the dependency chain of
    every formula.

 8. Always dispose ExcelPackage ("using"). It holds an open zip package and,
    when constructed from a FileInfo, a file handle.

 9. In tests, build packages over a MemoryStream (or the parameterless
    constructor) to avoid disk I/O entirely.

10. Reading only? Read ws.Cells[row, col].Value rather than .Text when you do
    not need the number format applied — Text runs the formatter for every
    cell.

================================================================================

COMMON PITFALLS TO AVOID
========================
 1. Package id, assembly and namespace are three different names.
    Package: FreePPlus.LgplLicenseForever. Assembly: FreePPlus.OfficeOpenXml.
    Namespace: OfficeOpenXml. There is no "FreePPlus" namespace.

 2. There is NO ws.SetFormula(...) method. Set formulas through the range:
    ws.Cells[row, col].Formula = "..." or ws.Cells["D2"].Formula = "...".
    ws.SetValue(row, col, value) does exist and sets VALUES only.

 3. LoadFromText(string, ...) takes the CSV TEXT, not a path. Passing a file
    name silently writes that file name into the first cell. Use the
    LoadFromText(FileInfo, ...) overloads for files.

 4. Cell indices are ALWAYS 1-based: ws.Cells[1, 1] is A1; ws.Cells[0, 0]
    throws. The worksheet COLLECTION is 0-based by default and configurable
    (see WORKSHEET COLLECTION INDEXING).

 5. ws.Dimension is null on an empty worksheet. Check it before reading
    ws.Dimension.End.Row.

 6. Formulas are written WITHOUT a leading "=". Formula cells have a null
    Value until Calculate() runs or Excel opens the file.

 7. Setting Value on a cell that has a Formula clears the formula, and vice
    versa. Pick one per cell.

 8. Only the 154 functions listed under FORMULAS are computable in process.
    Any other function still round-trips into the file, but Calculate() cannot
    evaluate it.

 9. Do NOT set ExcelTable.ShowTotal = true on a table created through
    ws.Tables.Add(). Excel reports the resulting file as corrupted. Use
    ShowFilter, ShowHeader and TableStyle instead.

10. LoadFromCollection with an EMPTY MemberInfo[] throws ArgumentException.
    Pass null to mean "all public instance properties".

11. With inheritance, LoadFromCollection emits DERIVED class properties first,
    then base class properties. That is the column order you get.

12. Color, Image and Font come from CodeBrix.Imaging, not System.Drawing. If
    Color does not resolve, you are missing "using CodeBrix.Imaging;".

13. Conditional formatting styles are DIFFERENTIAL styles (Style.Fill.
    BackgroundColor.Color = ..., Style.Font.Bold = true). They are NOT the
    ExcelStyle used for cells, and there is no SetColor(a,r,g,b) on
    ExcelDxfColor — assign its Color property.

14. Rule order is Priority (lower runs first), not insertion order, and
    StopIfTrue halts evaluation of later rules for a matching cell.

15. A worksheet's cells are only protected when the sheet is protected AND the
    cell style is Locked. Style.Locked defaults to TRUE, so unlock the ranges
    you want editable before calling ws.Protection.SetPassword(...).

16. ws.Protection.SetPassword() protects a sheet from editing in Excel; it does
    NOT encrypt the file. Use package.Encryption / SaveAs(file, password) for
    real encryption.

17. Save a workbook that has a VBA project with the .xlsm extension. The
    content type is switched for you, but Excel keys off the file extension.

18. Pivot table cells are computed by Excel on open, not by FreePPlus. Reading
    a pivot table's values back through this library gives you nothing.

19. Chart type names are XYScatter / XYScatterLines / XYScatterSmooth, not
    Scatter / ScatterLines / ScatterSmooth.

20. On Linux, AutoFitColumns() and ExcelFont.SetFromFont() need real fonts,
    including bold and italic faces. Install a family such as DejaVu
    ("sudo apt install fonts-dejavu") on slim containers.

21. Only .xlsx / .xlsm (Office Open XML) is supported. Legacy .xls will not
    open.

22. ExcelPackage is IDisposable and holds a zip package (and a file handle when
    constructed from FileInfo). Not disposing it leaves files locked.

23. LGPL v3: consuming the unmodified NuGet package from closed-source software
    is fine; MODIFYING FreePPlus source obliges you to publish those
    modifications under the LGPL.

================================================================================

WHAT THIS PACKAGE DOES NOT DO
=============================
  -> Legacy .xls (BIFF / Excel 97-2003) reading or writing — .xlsx and .xlsm
     only.
  -> ODS (LibreOffice/OpenOffice), Google Sheets or any non-OOXML format.
  -> Exporting to PDF, HTML or images.
  -> Automating a running copy of Excel, Excel add-ins, Excel Services or
     SharePoint integration.
  -> Power Query, Power Pivot, the data model, or slicers.
  -> Computing pivot table results (Excel does that when it opens the file).
  -> Writing CSV. LoadFromText imports delimited text; there is no CSV export.
  -> Evaluating formula functions outside the built-in set (extend the parser
     yourself — see EXTENDING THE FORMULA PARSER).
  -> Range-level Insert/Delete, ToText/ToDataTable, fluent sort builders and
     other APIs added in later major versions of the upstream project. This
     package's surface is the 4.x generation.
  -> Rendering: it never draws a chart, an image or a page; it writes the OOXML
     that Excel renders.

================================================================================

WORKING EXAMPLES ON GITHUB
==========================
The test project is the most reliable source of working, compiling usage. Fetch
raw files from:

    https://raw.githubusercontent.com/ellisnet/FreePPlus/main/<path>

Browse them at:

    https://github.com/ellisnet/FreePPlus/tree/main/tests/FreePPlus.OfficeOpenXml.Tests

Feature-to-test-file map (paths relative to the repository root):

  Worksheets end to end — creation, naming, visibility, values, rows, columns,
  insert/delete, formulas, styling, merging, AutoFilter, freeze panes,
  comments, data validation, protection, hyperlinks, rich text, CSV loading,
  header/footer, print settings. The single most comprehensive file:
    tests/FreePPlus.OfficeOpenXml.Tests/WorksheetTests.cs

  LoadFromCollection — headers, anonymous types, inheritance, member selection,
  Description/DisplayName attributes, table styles, round trips, simple types:
    tests/FreePPlus.OfficeOpenXml.Tests/LoadFromCollectionTests.cs

  Formula calculation — SUM, AVERAGE, IF, LEFT/RIGHT/MID, INT, ISBLANK,
  ISTEXT, SUBTOTAL, named ranges, named values, date arithmetic:
    tests/FreePPlus.OfficeOpenXml.Tests/CalculationTests.cs

  Charts, pictures and shapes — every chart family, positioning, series
  headers, secondary axes, combo charts, chart styles, images, drawing fills:
    tests/FreePPlus.OfficeOpenXml.Tests/DrawingTests.cs

  Chart series colours and alpha handling:
    tests/FreePPlus.OfficeOpenXml.Tests/ChartSeriesColorTests.cs

  Conditional formatting — color scales, data bars, icon sets, equal rules:
    tests/FreePPlus.OfficeOpenXml.Tests/ConditionalFormatting/ConditionalFormattingTests.cs

  Sparklines — all three types, location/data range pairings, date axis,
  manual min/max:
    tests/FreePPlus.OfficeOpenXml.Tests/SparkLineTests.cs

  Cell styling — fonts, bold/italic, size, colour:
    tests/FreePPlus.OfficeOpenXml.Tests/Style/ExcelFontTests.cs
    tests/FreePPlus.OfficeOpenXml.Tests/ExcelStyleTests.cs

  Colour hex formatting (packed ARGB, low alpha values):
    tests/FreePPlus.OfficeOpenXml.Tests/ColorHexFormatTests.cs

  Images built in memory versus loaded from a file, and format fallback:
    tests/FreePPlus.OfficeOpenXml.Tests/ImageFormatFallbackTests.cs
    tests/FreePPlus.OfficeOpenXml.Tests/ImagingEfficiencyTests.cs

  Encryption and file passwords:
    tests/FreePPlus.OfficeOpenXml.Tests/EncryptTests.cs

  Protection and other security behaviour:
    tests/FreePPlus.OfficeOpenXml.Tests/SecurityBaselineTests.cs

  Addresses, ranges and range operations (Copy, Clear, Offset, R1C1):
    tests/FreePPlus.OfficeOpenXml.Tests/AddressTests.cs
    tests/FreePPlus.OfficeOpenXml.Tests/ExcelCellBaseTests.cs
    tests/FreePPlus.OfficeOpenXml.Tests/ExcelRangeBaseTests.cs
    tests/FreePPlus.OfficeOpenXml.Tests/CellStoreTests.cs

  Comments:
    tests/FreePPlus.OfficeOpenXml.Tests/CommentsTests.cs

  VBA — compression round trips and project handling:
    tests/FreePPlus.OfficeOpenXml.Tests/VBATests.cs

  Opening template workbooks and named styles:
    tests/FreePPlus.OfficeOpenXml.Tests/ReadTemplateTests.cs

  Regression tests reproducing specific reported issues (array formulas,
  R1C1 round trips, rich text, LoadFromDataTable, LoadFromArrays):
    tests/FreePPlus.OfficeOpenXml.Tests/IssueBasedTests.cs

Example, fetching one file:

    https://raw.githubusercontent.com/ellisnet/FreePPlus/main/tests/FreePPlus.OfficeOpenXml.Tests/LoadFromCollectionTests.cs

================================================================================

QUICK REFERENCE CARD
====================

Install:            dotnet add package FreePPlus.LgplLicenseForever
Namespace:          using OfficeOpenXml;
Create package:     new ExcelPackage()
Open file:          new ExcelPackage(new FileInfo("file.xlsx"))
Open encrypted:     new ExcelPackage(new FileInfo("f.xlsx"), "password")
From template:      new ExcelPackage(newFile, templateFile)
Add worksheet:      package.Workbook.Worksheets.Add("Name")
Copy worksheet:     package.Workbook.Worksheets.Add("Copy", sourceSheet)
Add chart sheet:    package.Workbook.Worksheets.AddChart("C", eChartType.Line)
Get worksheet:      package.Workbook.Worksheets["Name"] or [0]
Set value:          ws.Cells["A1"].Value = "text"
Get value:          ws.Cells["A1"].Value / .Text / .GetValue<T>()
Used range:         ws.Dimension (null when empty)
Set formula:        ws.Cells["A1"].Formula = "SUM(B1:B10)"
R1C1 formula:       ws.Cells["A1"].FormulaR1C1 = "SUM(RC[1]:RC[10])"
Array formula:      ws.Cells["B1:B3"].CreateArrayFormula("A1:A3")
Calculate:          ws.Calculate() / package.Workbook.Calculate()
Copy range:         ws.Cells["A1:D10"].Copy(ws.Cells["F1"])
Clear range:        ws.Cells["A1:D10"].Clear()
Sort range:         ws.Cells["A2:D50"].Sort(0)   // column index is zero-based
                                                 // WITHIN the range
Merge:              ws.Cells["A1:D1"].Merge = true
AutoFilter:         ws.Cells["A1:D10"].AutoFilter = true
Freeze panes:       ws.View.FreezePanes(2, 1)
Insert rows:        ws.InsertRow(3, 2)
Delete columns:     ws.DeleteColumn(3, 2)
Load collection:    ws.Cells["A1"].LoadFromCollection(items, true,
                                                      TableStyles.Medium6)
Load DataTable:     ws.Cells["A1"].LoadFromDataTable(table, true)
Load arrays:        ws.Cells["A1"].LoadFromArrays(rows)
Load CSV file:      ws.Cells["A1"].LoadFromText(new FileInfo("d.csv"), format)
Style font:         ws.Cells["A1"].Style.Font.Bold = true
Style fill:         ws.Cells["A1"].Style.Fill.PatternType =
                        ExcelFillStyle.Solid
Set colour:         ...BackgroundColor.SetColor(255, 0, 51, 102)
Number format:      ws.Cells["A1"].Style.Numberformat.Format = "#,##0.00"
Named style:        package.Workbook.Styles.CreateNamedStyle("Money")
Apply named style:  ws.Cells["C2:C99"].StyleName = "Money"
Auto-fit:           ws.Cells.AutoFitColumns()
Rich text:          ws.Cells["A1"].RichText.Add("run")
Add table:          ws.Tables.Add(ws.Cells["A1:D10"], "T1")
Named range:        package.Workbook.Names.Add("Data", ws.Cells["A1:D10"])
Named value:        ws.Names.AddValue("PRICE", 10)
Conditional fmt:    ws.ConditionalFormatting.AddGreaterThan(
                        new ExcelAddress("B2:B50"))
Data bar:           ws.ConditionalFormatting.AddDatabar(addr, Color.SteelBlue)
Icon set:           ws.ConditionalFormatting.AddThreeIconSet(addr, iconSet)
Pivot table:        ws.PivotTables.Add(ws.Cells["A1"], dataRange, "P1")
Pivot data field:   pivot.DataFields.Add(pivot.Fields["Amount"])
Add chart:          ws.Drawings.AddChart("C", eChartType.ColumnClustered)
Chart series:       chart.Series.Add(ws.Cells["B2:B10"], ws.Cells["A2:A10"])
Chart title:        chart.Title.Text = "Sales"
Secondary axis:     chart.PlotArea.ChartTypes.Add(...).UseSecondaryAxis = true
Sparklines:         ws.SparklineGroups.Add(eSparklineType.Line, loc, data)
Add picture:        ws.Drawings.AddPicture("P", new FileInfo("img.png"))
Add shape:          ws.Drawings.AddShape("S", eShapeStyle.RoundRect)
Add comment:        ws.Cells["A1"].AddComment("text", "author")
Data validation:    ws.DataValidations.AddIntegerValidation("B2:B100")
Hyperlink:          ws.Cells["A1"].Hyperlink = new Uri("https://example.com")
Protect sheet:      ws.Protection.SetPassword("pass")
Protect workbook:   package.Workbook.Protection.LockStructure = true
VBA project:        package.Workbook.CreateVBAProject()
Custom function:    package.Workbook.FormulaParserManager
                           .AddOrReplaceFunction("name", impl)
Print landscape:    ws.PrinterSettings.Orientation = eOrientation.Landscape
Header text:        ws.HeaderFooter.OddHeader.CenteredText = "Report"
Save file:          package.SaveAs(new FileInfo("out.xlsx"))
Save encrypted:     package.SaveAs(new FileInfo("out.xlsx"), "password")
Save async:         await package.SaveAsAsync(new FileInfo("out.xlsx"))
Save bytes:         package.GetAsByteArray()

File formats:       .xlsx and .xlsm (Office Open XML) only
Cell indices:       1-based (ws.Cells[1,1] is A1)
Worksheet index:    0-based by default (package.Compatibility
                                       .IsWorksheets1Based to change)
Max rows/columns:   ExcelPackage.MaxRows (1048576) /
                    ExcelPackage.MaxColumns (16384)
Target framework:   .NET 10 or later
License:            LGPL-3.0-or-later

================================================================================
