================================================================================
EXTRAS-README: FreePPlus
Samples, tools and other content in this repository that is not part of a NuGet
package
================================================================================

The repository ships one NuGet package (FreePPlus.LgplLicenseForever, built from
src/FreePPlus.OfficeOpenXml). Everything described below is repository content
only — none of it is packaged or published.

SAMPLE APPLICATION
==================
Path: samples/FreePPlus.OfficeOpenXml.SampleApp

A .NET 10 console application that references the library by project reference
and demonstrates sixteen scenarios, plus two extra samples that are not wired
into the menu. It also carries an appsettings.json showing the
FreePPlus:ExcelPackage:Compatibility:IsWorksheets1Based setting.

HOW TO RUN

The app expects an output folder to exist and refuses to start without it. The
folder is a hard-coded constant, Program.TempFolder, currently C:\Temp — edit
that constant (and Program.AdvWorksConnectString, if you want the database
samples) before running on Linux or macOS. Each run creates a timestamped
sub-folder under it for the workbooks it produces.

    cd samples/FreePPlus.OfficeOpenXml.SampleApp
    dotnet run -- --run:1,5,9

With no --run argument the app prints usage and exits. Valid sample numbers are
1 to 16; unknown numbers are reported and skipped. Failures in one sample are
caught and reported, and the remaining samples still run.

WHAT EACH SAMPLE DEMONSTRATES

     1  Creates a workbook from scratch: an inventory list on a single
        worksheet.
     2  Opens an existing workbook (Resources/Sample2.xlsx) and reads values
        and document properties.
     3  Populates a workbook from the AdventureWorks LT SQL Server database,
        including a named "HyperLink" style. Requires the database.
     4  Fills a template workbook that already contains a chart
        (Resources/GraphTemplate.xlsx) with exchange rates and points three
        series at the new data. Requires the database.
     5  Reopens the sample 1 output, inserts rows and adds a pie chart.
     6  Walks the file system and builds a report with pictures, freeze panes
        and hyperlinks (uses Resources/file_icon.png and folder_icon.png).
     7  Loads many rows, styles them, inserts a header row, freezes panes and
        protects the sheet so only two columns stay editable.
     8  LINQ over the Cells collection (class LinqSample).
     9  Loads two CSV files (csv/Sample9-1.txt, csv/Sample9-2.txt) with
        LoadFromText, turns them into tables and adds charts that combine two
        chart types and a secondary axis.
    10  Workbook and worksheet protection plus file encryption, including a
        second workbook written with EncryptionAlgorithm.AES192.
    11  Data validation: integer, list and other validation types.
    12  Pivot tables: a simple pivot with one row field and one data field, and
        a second pivot with date grouping by year and quarter, a page field and
        several formatted data fields, plus a pivot-driven chart. Requires the
        database.
    13  The loading APIs side by side: LoadFromDataTable, LoadFromCollection
        with an anonymous type, and LoadFromCollection with a List<T>.
    14  Conditional formatting end to end: two- and three-color scales,
        above/below average, above/below standard deviation, top/bottom and
        top/bottom percent, time-period rules, text rules, icon sets and data
        bars, with rule priorities and StopIfTrue.
    15  VBA: creates a VBA project, writes workbook and worksheet code modules,
        adds standard modules and a class module from the .txt files in
        VBA-Code/, sets a VBA project password and saves .xlsm workbooks.
    16  Sparklines: loads a semicolon-delimited currency file with a Swedish
        culture, then adds column, line and stacked sparkline groups.

NOT IN THE MENU (call them directly if you want to try them):

    Sample_FormulaCalc.cs
        RunSampleFormulaCalc() — worksheet, workbook and range Calculate(),
        and evaluating a formula string without calculating dependent cells.
    Sample_AddFormulaFunction.cs
        RunSample_AddFormulaFunction() — registering custom worksheet functions
        through FormulaParserManager, both as a FunctionsModule and one at a
        time with AddOrReplaceFunction, including overriding the built-in TEXT
        function.

SAMPLE DATA FILES
    csv/Sample9-1.txt, csv/Sample9-2.txt      delimited text for sample 9
    Resources/Sample2.xlsx                    input workbook for sample 2
    Resources/GraphTemplate.xlsx              chart template for sample 4
    Resources/file_icon.png, folder_icon.png  images for sample 6
    VBA-Code/*.txt                            VBA source for sample 15

The two database samples (3, 4 and 12) need a local SQL Server with the
AdventureWorksLT2022 database; they fail with a connection error otherwise,
which the sample runner catches and reports.

TEST PROJECT
============
Path: tests/FreePPlus.OfficeOpenXml.Tests

The xUnit v3 test project is not packaged either, but it is the best worked
set of usage examples in the repository — AGENT-README.txt maps features to
test files under "WORKING EXAMPLES ON GITHUB". Running it is covered in
MAINTAINER-README.txt.

Optional test data: tests/FreePPlus.OfficeOpenXml.Tests/SampleFiles holds
test-image-01.bmp, .jpg and .png, embedded as resources and used by the drawing
and imaging tests.

OTHER NON-PACKAGE CONTENT
=========================
    nuspec/     A hand-written .nuspec with placeholder docs/ and lib/ folders,
                left over from an earlier packaging approach. The current build
                packs from the csproj and does not use it.
