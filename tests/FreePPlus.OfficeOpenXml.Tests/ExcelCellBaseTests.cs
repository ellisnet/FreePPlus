using OfficeOpenXml;
using System;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class ExcelCellBaseTests
{
    #region UpdateFormulaReferences Tests

    [Fact]
    public void UpdateFormulaReferencesOnTheSameSheet()
    {
        var result = ExcelCellBase.UpdateFormulaReferences("C3", 3, 3, 2, 2, "sheet", "sheet");
        Assert.Equal("F6", result);
    }

    [Fact]
    public void UpdateFormulaReferencesIgnoresIncorrectSheet()
    {
        var result = ExcelCellBase.UpdateFormulaReferences("C3", 3, 3, 2, 2, "sheet", "other sheet");
        Assert.Equal("C3", result);
    }

    [Fact]
    public void UpdateFormulaReferencesFullyQualifiedReferenceOnTheSameSheet()
    {
        var result = ExcelCellBase.UpdateFormulaReferences("'sheet name here'!C3", 3, 3, 2, 2, "sheet name here", "sheet name here");
        Assert.Equal("'sheet name here'!F6", result);
    }

    [Fact]
    public void UpdateFormulaReferencesFullyQualifiedCrossSheetReferenceArray()
    {
        var result = ExcelCellBase.UpdateFormulaReferences("SUM('sheet name here'!B2:D4)", 3, 3, 3, 3, "cross sheet", "sheet name here");
        Assert.Equal("SUM('sheet name here'!B2:G7)", result);
    }

    [Fact]
    public void UpdateFormulaReferencesFullyQualifiedReferenceOnADifferentSheet()
    {
        var result = ExcelCellBase.UpdateFormulaReferences("'updated sheet'!C3", 3, 3, 2, 2, "boring sheet", "updated sheet");
        Assert.Equal("'updated sheet'!F6", result);
    }

    [Fact]
    public void UpdateFormulaReferencesReferencingADifferentSheetIsNotUpdated()
    {
        var result = ExcelCellBase.UpdateFormulaReferences("'boring sheet'!C3", 3, 3, 2, 2, "boring sheet", "updated sheet");
        Assert.Equal("'boring sheet'!C3", result);
    }
    #endregion

    #region UpdateCrossSheetReferenceNames Tests
    [Fact]
    public void UpdateFormulaSheetReferences()
    {
        var result = ExcelCellBase.UpdateFormulaSheetReferences("5+'OldSheet'!$G3+'Some Other Sheet'!C3+SUM(1,2,3)", "OldSheet", "NewSheet");
        Assert.Equal("5+'NewSheet'!$G3+'Some Other Sheet'!C3+SUM(1,2,3)", result);
    }

    [Fact]
    public void UpdateFormulaSheetReferencesNullOldSheetThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            ExcelCellBase.UpdateFormulaSheetReferences("formula", null, "sheet2"));
    }

    [Fact]
    public void UpdateFormulaSheetReferencesEmptyOldSheetThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ExcelCellBase.UpdateFormulaSheetReferences("formula", string.Empty, "sheet2"));
        ;
    }

    [Fact]
    public void UpdateFormulaSheetReferencesNullNewSheetThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            ExcelCellBase.UpdateFormulaSheetReferences("formula", "sheet1", null));
        ;
    }

    [Fact]
    public void UpdateFormulaSheetReferencesEmptyNewSheetThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            ExcelCellBase.UpdateFormulaSheetReferences("formula", "sheet1", string.Empty));
        ;
    }

    #endregion
}
