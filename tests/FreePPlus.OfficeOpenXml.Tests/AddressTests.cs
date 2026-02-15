using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.ExcelUtilities;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class AddressTests
{
    #region InsertDelete Tests

    [Fact]
    public void InsertDeleteTest()
    {
        var addr = new ExcelAddressBase("A1:B3");

        Assert.Equal("A1:B7", addr.AddRow(2, 4).Address);
        Assert.Equal("A1:F3", addr.AddColumn(2, 4).Address);
        Assert.Equal("A1:A3", addr.DeleteColumn(2, 1).Address);
        Assert.Equal("A1:B1", addr.DeleteRow(2, 2).Address);
        Assert.Null(addr.DeleteRow(1, 3));
        Assert.Null(addr.DeleteColumn(1, 2));
    }

    #endregion

    #region Address Parsing Tests

    [Fact]
    public void AddressWithWorksheetReference()
    {
        var a1 = new ExcelAddress("SalesData!$K$445");
        Assert.Equal("SalesData", a1.WorkSheet);
    }

    [Fact]
    public void AddressWithMultipleRanges()
    {
        var a2 = new ExcelAddress("SalesData!$K$445:$M$449,SalesData!$N$448:$Q$454,SalesData!$L$458:$O$464");
        Assert.Equal("SalesData", a2.WorkSheet);
    }

    [Fact]
    public void AddressWithSimpleRange()
    {
        var a3 = new ExcelAddress("SalesData!$K$445:$L$448");
        Assert.Equal("SalesData", a3.WorkSheet);
    }

    [Fact]
    public void AddressWithTableReferenceAll()
    {
        var a5 = new ExcelAddress("Table1[[#All],[Title]]");
        Assert.NotNull(a5);
    }

    [Fact]
    public void AddressWithTableReferenceAllOnly()
    {
        var a6 = new ExcelAddress("Table1[#All]");
        Assert.NotNull(a6);
    }

    [Fact]
    public void AddressWithTableReferenceHeadersAndColumns()
    {
        var a7 = new ExcelAddress("Table1[[#Headers],[FirstName]:[LastName]]");
        Assert.NotNull(a7);
    }

    [Fact]
    public void AddressWithTableReferenceHeaders()
    {
        var a8 = new ExcelAddress("Table1[#Headers]");
        Assert.NotNull(a8);
    }

    [Fact]
    public void AddressWithTableReferenceSubTotal()
    {
        var a9 = new ExcelAddress("Table2[[#All],[SubTotal]]");
        Assert.NotNull(a9);
    }

    [Fact]
    public void AddressWithExternalWorkbookTableReference()
    {
        var a12 = new ExcelAddress("[1]!Table1[[LastName]:[Name]]");
        Assert.NotNull(a12);
    }

    [Fact]
    public void AddressWithSheetReferenceAndFormula()
    {
        var a14 = new ExcelAddress("SalesData!$N$5+'test''1'!$J$33");
        Assert.NotNull(a14);
    }

    #endregion

    #region IsValidCellAddress Tests

    [Theory]
    [InlineData("A1", true)]
    [InlineData("A1048576", true)]
    [InlineData("XFD1", true)]
    [InlineData("XFD1048576", true)]
    [InlineData("Table1!A1", true)]
    [InlineData("Table1!A1048576", true)]
    [InlineData("Table1!XFD1", true)]
    [InlineData("Table1!XFD1048576", true)]
    [InlineData("A", false)]
    [InlineData("XFD", false)]
    [InlineData("1", false)]
    [InlineData("1048576", false)]
    [InlineData("A1:A1048576", false)]
    [InlineData("A1:XFD1", false)]
    [InlineData("A1048576:XFD1048576", false)]
    [InlineData("XFD1:XFD1048576", false)]
    [InlineData("Table1!A1:A1048576", false)]
    [InlineData("Table1!A1:XFD1", false)]
    [InlineData("Table1!A1048576:XFD1048576", false)]
    [InlineData("Table1!XFD1:XFD1048576", false)]
    public void IsValidCellAddress(string address, bool expected)
    {
        Assert.Equal(expected, ExcelCellBase.IsValidCellAddress(address));
    }

    #endregion

    #region IsValidName Tests

    [Theory]
    [InlineData("123sa", false)]
    [InlineData("*d", false)]
    [InlineData("\t", false)]
    [InlineData("\\t", false)]
    [InlineData("A+1", false)]
    [InlineData("A%we", false)]
    [InlineData("BB73", false)]
    [InlineData("BBBB75", true)]
    [InlineData("BB1500005", true)]
    public void IsValidName(string name, bool expected)
    {
        Assert.Equal(expected, ExcelAddressUtil.IsValidName(name));
    }

    #endregion

    #region NamedRange Insert/Delete Tests

    [Fact]
    public void NamedRangeMovesDownIfRowInsertedAbove()
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("NEW");
        var range = sheet.Cells[2, 1, 3, 3];
        var namedRange = sheet.Names.Add("NewNamedRange", range);

        sheet.InsertRow(1, 1);

        Assert.Equal("'NEW'!A3:C4", namedRange.Address);
    }

    [Fact]
    public void NamedRangeDoesNotChangeIfRowInsertedBelow()
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("NEW");
        var range = sheet.Cells[2, 1, 3, 3];
        var namedRange = sheet.Names.Add("NewNamedRange", range);

        sheet.InsertRow(4, 1);

        Assert.Equal("A2:C3", namedRange.Address);
    }

    [Fact]
    public void NamedRangeExpandsDownIfRowInsertedWithin()
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("NEW");
        var range = sheet.Cells[2, 1, 3, 3];
        var namedRange = sheet.Names.Add("NewNamedRange", range);

        sheet.InsertRow(3, 1);

        Assert.Equal("'NEW'!A2:C4", namedRange.Address);
    }

    [Fact]
    public void NamedRangeMovesRightIfColumnInsertedBefore()
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("NEW");
        var range = sheet.Cells[2, 2, 3, 4];
        var namedRange = sheet.Names.Add("NewNamedRange", range);

        sheet.InsertColumn(1, 1);

        Assert.Equal("'NEW'!C2:E3", namedRange.Address);
    }

    [Fact]
    public void NamedRangeUnchangedIfColumnInsertedAfter()
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("NEW");
        var range = sheet.Cells[2, 2, 3, 4];
        var namedRange = sheet.Names.Add("NewNamedRange", range);

        sheet.InsertColumn(5, 1);

        Assert.Equal("B2:D3", namedRange.Address);
    }

    [Fact]
    public void NamedRangeWithWorkbookScopeIsMovedDownIfRowInsertedAbove()
    {
        using var package = new ExcelPackage();
        var workbook = package.Workbook;
        var sheet = package.Workbook.Worksheets.Add("NEW");
        var range = sheet.Cells[2, 1, 3, 3];
        var namedRange = workbook.Names.Add("NewNamedRange", range);

        sheet.InsertRow(1, 1);

        Assert.Equal("'NEW'!A3:C4", namedRange.Address);
    }

    [Fact]
    public void NamedRangeWithWorkbookScopeIsMovedRightIfColumnInsertedBefore()
    {
        using var package = new ExcelPackage();
        var workbook = package.Workbook;
        var sheet = package.Workbook.Worksheets.Add("NEW");
        var range = sheet.Cells[2, 2, 3, 3];
        var namedRange = workbook.Names.Add("NewNamedRange", range);

        sheet.InsertColumn(1, 1);

        Assert.Equal("'NEW'!C2:D3", namedRange.Address);
    }

    [Fact]
    public void NamedRangeIsUnchangedForOutOfScopeSheet()
    {
        using var package = new ExcelPackage();
        var workbook = package.Workbook;
        var sheet1 = package.Workbook.Worksheets.Add("NEW");
        var sheet2 = package.Workbook.Worksheets.Add("NEW2");
        var range = sheet2.Cells[2, 2, 3, 3];
        var namedRange = workbook.Names.Add("NewNamedRange", range);

        sheet1.InsertColumn(1, 1);

        Assert.Equal("B2:C3", namedRange.Address);
    }

    #endregion

    #region WorksheetSpec Tests

    [Fact]
    public void ShouldHandleWorksheetSpec()
    {
        var address = "Sheet1!A1:Sheet1!A2";
        var excelAddress = new ExcelAddress(address);

        Assert.Equal("Sheet1", excelAddress.WorkSheet);
        Assert.Equal(1, excelAddress._fromRow);
        Assert.Equal(2, excelAddress._toRow);
    }

    #endregion

    #region IsValidAddress Tests

    [Theory]
    [InlineData("$A12:XY1:3", false)]
    [InlineData("A1$2:XY$13", false)]
    [InlineData("A12$:X$Y$13", false)]
    [InlineData("A12:X$Y$13", false)]
    [InlineData("$A$12:$XY$13,$A12:XY1:3", false)]
    [InlineData("$A$12:", false)]
    [InlineData("$XFD$1048576", true)]
    [InlineData("$XFE$1048576", false)]
    [InlineData("$XFD$1048577", false)]
    [InlineData("A12", true)]
    [InlineData("A$12", true)]
    [InlineData("$A$12", true)]
    [InlineData("$A$12:$XY$13", true)]
    [InlineData("$A$12:$XY$13,$A12:XY$14", true)]
    [InlineData("$A$12:$XY$13,$A12:XY$14$", false)]
    public void IsValidAddress(string address, bool expected)
    {
        Assert.Equal(expected, ExcelCellBase.IsValidAddress(address));
    }

    #endregion
}
