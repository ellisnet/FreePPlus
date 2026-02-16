using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class LoadFromCollectionTests
{
    #region Test Helper Types

    internal abstract class BaseClass
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    internal class Implementation : BaseClass
    {
        public int Number { get; set; }
    }

    internal class Aclass
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }

    internal class ClassWithDescription
    {
        [Description("The Identifier")]
        public string Id { get; set; }

        [Description("Display Name")]
        public string Name { get; set; }
    }

    internal class ClassWithDisplayName
    {
        [DisplayName("Identifier")]
        public string Id { get; set; }

        [DisplayName("Full Name")]
        public string Name { get; set; }
    }

    internal class ClassWithUnderscore
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
    }

    #endregion

    #region Header Tests

    [Fact]
    public void LoadFromCollectionUsesConcreteClassPropertyNamesAsHeaders()
    {
        var items = new List<Aclass>
        {
            new() { Id = "123", Name = "Item 1", Number = 3 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["C1"].LoadFromCollection(items, true, TableStyles.Dark1);

        Assert.Equal("Id", sheet.Cells["C1"].Value);
        Assert.Equal("Name", sheet.Cells["D1"].Value);
        Assert.Equal("Number", sheet.Cells["E1"].Value);
    }

    [Fact]
    public void LoadFromCollectionUsesBaseClassPropertyNamesAsHeaders()
    {
        var items = new List<Implementation>
        {
            new() { Id = "123", Name = "Item 1", Number = 3 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["C1"].LoadFromCollection(items, true, TableStyles.Dark1);

        // Derived class properties appear first, then base class properties
        Assert.Equal("Number", sheet.Cells["C1"].Value);
        Assert.Equal("Id", sheet.Cells["D1"].Value);
        Assert.Equal("Name", sheet.Cells["E1"].Value);
    }

    [Fact]
    public void LoadFromCollectionUsesAnonymousTypePropertyNamesAsHeaders()
    {
        var objs = new List<Implementation>
        {
            new() { Id = "123", Name = "Item 1", Number = 3 }
        };
        var items = objs.Select(x => new { x.Id, x.Name }).ToList();

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["C1"].LoadFromCollection(items, true, TableStyles.Dark1);

        Assert.Equal("Id", sheet.Cells["C1"].Value);
        Assert.Equal("Name", sheet.Cells["D1"].Value);
    }

    [Fact]
    public void LoadFromCollectionUsesDescriptionAttributeForHeaders()
    {
        var items = new List<ClassWithDescription>
        {
            new() { Id = "1", Name = "Test" }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true);

        Assert.Equal("The Identifier", sheet.Cells["A1"].Value);
        Assert.Equal("Display Name", sheet.Cells["B1"].Value);
    }

    [Fact]
    public void LoadFromCollectionUsesDisplayNameAttributeForHeaders()
    {
        var items = new List<ClassWithDisplayName>
        {
            new() { Id = "1", Name = "Test" }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true);

        Assert.Equal("Identifier", sheet.Cells["A1"].Value);
        Assert.Equal("Full Name", sheet.Cells["B1"].Value);
    }

    [Fact]
    public void LoadFromCollectionReplacesUnderscoresInPropertyNamesWithSpaces()
    {
        var items = new List<ClassWithUnderscore>
        {
            new() { First_Name = "John", Last_Name = "Doe" }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true);

        Assert.Equal("First Name", sheet.Cells["A1"].Value);
        Assert.Equal("Last Name", sheet.Cells["B1"].Value);
    }

    [Fact]
    public void LoadFromCollectionWithoutHeadersDoesNotWritePropertyNames()
    {
        var items = new List<Aclass>
        {
            new() { Id = "123", Name = "Item 1", Number = 3 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, false);

        Assert.Equal("123", sheet.Cells["A1"].Value);
        Assert.Equal("Item 1", sheet.Cells["B1"].Value);
        Assert.Equal(3, sheet.Cells["C1"].Value);
    }

    #endregion

    #region Data Loading Tests

    [Fact]
    public void LoadFromCollectionLoadsDataValues()
    {
        var items = new List<Aclass>
        {
            new() { Id = "123", Name = "Item 1", Number = 3 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true);

        Assert.Equal("123", sheet.Cells["A2"].Value);
        Assert.Equal("Item 1", sheet.Cells["B2"].Value);
        Assert.Equal(3, sheet.Cells["C2"].Value);
    }

    [Fact]
    public void LoadFromCollectionLoadsMultipleRows()
    {
        var items = new List<Aclass>
        {
            new() { Id = "1", Name = "First", Number = 10 },
            new() { Id = "2", Name = "Second", Number = 20 },
            new() { Id = "3", Name = "Third", Number = 30 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true);

        Assert.Equal("1", sheet.Cells["A2"].Value);
        Assert.Equal("2", sheet.Cells["A3"].Value);
        Assert.Equal("3", sheet.Cells["A4"].Value);
        Assert.Equal(10, sheet.Cells["C2"].Value);
        Assert.Equal(20, sheet.Cells["C3"].Value);
        Assert.Equal(30, sheet.Cells["C4"].Value);
    }

    [Fact]
    public void LoadFromCollectionStartsAtSpecifiedCell()
    {
        var items = new List<Aclass>
        {
            new() { Id = "1", Name = "Test", Number = 5 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["D5"].LoadFromCollection(items, true);

        Assert.Equal("Id", sheet.Cells["D5"].Value);
        Assert.Equal("1", sheet.Cells["D6"].Value);
        Assert.Equal("Test", sheet.Cells["E6"].Value);
        Assert.Equal(5, sheet.Cells["F6"].Value);
    }

    [Fact]
    public void LoadFromCollectionWithInheritedClassLoadsAllProperties()
    {
        var items = new List<Implementation>
        {
            new() { Id = "A", Name = "Inherited", Number = 42 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true);

        // Derived class properties appear first, then base class properties
        Assert.Equal("Number", sheet.Cells["A1"].Value);
        Assert.Equal("Id", sheet.Cells["B1"].Value);
        Assert.Equal("Name", sheet.Cells["C1"].Value);
        Assert.Equal(42, sheet.Cells["A2"].Value);
        Assert.Equal("A", sheet.Cells["B2"].Value);
        Assert.Equal("Inherited", sheet.Cells["C2"].Value);
    }

    [Fact]
    public void LoadFromCollectionWithAnonymousTypeLoadsValues()
    {
        var items = new[] { new { Name = "Test", Value = 100 } }.ToList();

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true);

        Assert.Equal("Test", sheet.Cells["A2"].Value);
        Assert.Equal(100, sheet.Cells["B2"].Value);
    }

    #endregion

    #region Primitive and Simple Type Tests

    [Fact]
    public void LoadFromCollectionWithStringsLoadsValues()
    {
        var items = new List<string> { "Alpha", "Beta", "Gamma" };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items);

        Assert.Equal("Alpha", sheet.Cells["A1"].Value);
        Assert.Equal("Beta", sheet.Cells["A2"].Value);
        Assert.Equal("Gamma", sheet.Cells["A3"].Value);
    }

    [Fact]
    public void LoadFromCollectionWithDecimalsLoadsValues()
    {
        var items = new List<decimal> { 1.5m, 2.75m, 3.125m };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items);

        Assert.Equal(1.5m, sheet.Cells["A1"].Value);
        Assert.Equal(2.75m, sheet.Cells["A2"].Value);
        Assert.Equal(3.125m, sheet.Cells["A3"].Value);
    }

    [Fact]
    public void LoadFromCollectionWithDateTimesLoadsValues()
    {
        var date = new DateTime(2024, 6, 15);
        var items = new List<DateTime> { date };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items);

        Assert.Equal(date, sheet.Cells["A1"].Value);
    }

    #endregion

    #region Table Style Tests

    [Fact]
    public void LoadFromCollectionCreatesTableWhenStyleIsNotNone()
    {
        var items = new List<Aclass>
        {
            new() { Id = "1", Name = "Test", Number = 1 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true, TableStyles.Dark1);

        Assert.Equal(1, sheet.Tables.Count);
    }

    [Fact]
    public void LoadFromCollectionDoesNotCreateTableWhenStyleIsNone()
    {
        var items = new List<Aclass>
        {
            new() { Id = "1", Name = "Test", Number = 1 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true, TableStyles.None);

        Assert.Equal(0, sheet.Tables.Count);
    }

    [Fact]
    public void LoadFromCollectionTableHasCorrectStyle()
    {
        var items = new List<Aclass>
        {
            new() { Id = "1", Name = "Test", Number = 1 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true, TableStyles.Medium9);

        Assert.Equal(TableStyles.Medium9, sheet.Tables[0].TableStyle);
    }

    #endregion

    #region Return Value Tests

    [Fact]
    public void LoadFromCollectionReturnsCorrectRange()
    {
        var items = new List<Aclass>
        {
            new() { Id = "1", Name = "First", Number = 1 },
            new() { Id = "2", Name = "Second", Number = 2 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        var range = sheet.Cells["B3"].LoadFromCollection(items, true);

        Assert.Equal("B3", range.Start.Address);
        Assert.Equal(3, range.Start.Row);
        Assert.Equal(2, range.Start.Column);
        Assert.Equal(5, range.End.Row);
        Assert.Equal(4, range.End.Column);
    }

    [Fact]
    public void LoadFromCollectionReturnsNullForEmptyCollectionWithoutHeaders()
    {
        var items = new List<Aclass>();

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        var range = sheet.Cells["A1"].LoadFromCollection(items, false);

        Assert.Null(range);
    }

    [Fact]
    public void LoadFromCollectionReturnsRangeForEmptyCollectionWithHeaders()
    {
        var items = new List<Aclass>();

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        var range = sheet.Cells["A1"].LoadFromCollection(items, true);

        Assert.NotNull(range);
        Assert.Equal("Id", sheet.Cells["A1"].Value);
    }

    #endregion

    #region Member Selection Tests

    [Fact]
    public void LoadFromCollectionWithSpecificMembersOnlyLoadsThoseMembers()
    {
        var items = new List<Aclass>
        {
            new() { Id = "1", Name = "Test", Number = 42 }
        };
        var members = typeof(Aclass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name == "Id" || p.Name == "Number")
            .Cast<MemberInfo>()
            .ToArray();

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        sheet.Cells["A1"].LoadFromCollection(items, true, TableStyles.None,
            BindingFlags.Public | BindingFlags.Instance, members);

        Assert.Equal("Id", sheet.Cells["A1"].Value);
        Assert.Equal("Number", sheet.Cells["B1"].Value);
        Assert.Equal("1", sheet.Cells["A2"].Value);
        Assert.Equal(42, sheet.Cells["B2"].Value);
    }

    [Fact]
    public void LoadFromCollectionThrowsWhenMembersArrayIsEmpty()
    {
        var items = new List<Aclass>
        {
            new() { Id = "1", Name = "Test", Number = 1 }
        };

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        Assert.Throws<ArgumentException>(() =>
            sheet.Cells["A1"].LoadFromCollection(items, true, TableStyles.None,
                BindingFlags.Public | BindingFlags.Instance, Array.Empty<MemberInfo>()));
    }

    [Fact]
    public void LoadFromCollectionThrowsInvalidCastExceptionForWrongMemberTypes()
    {
        var objs = new List<Implementation>
        {
            new() { Id = "123", Name = "Item 1", Number = 3 }
        };
        var items = objs.Select(x => new { x.Id, x.Name }).ToList();

        using var pck = new ExcelPackage(new MemoryStream());
        var sheet = pck.Workbook.Worksheets.Add("sheet");

        Assert.Throws<InvalidCastException>(() =>
            sheet.Cells["C1"].LoadFromCollection(items, true, TableStyles.Dark1,
                BindingFlags.Public | BindingFlags.Instance, typeof(string).GetMembers()));
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void LoadFromCollectionDataSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var sheet = pck.Workbook.Worksheets.Add("sheet");
            var items = new List<Aclass>
            {
                new() { Id = "1", Name = "First", Number = 10 },
                new() { Id = "2", Name = "Second", Number = 20 }
            };
            sheet.Cells["A1"].LoadFromCollection(items, true);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws = pck2.Workbook.Worksheets[0];

        Assert.Equal("Id", ws.Cells["A1"].Value);
        Assert.Equal("Name", ws.Cells["B1"].Value);
        Assert.Equal("Number", ws.Cells["C1"].Value);
        Assert.Equal("1", ws.Cells["A2"].Value);
        Assert.Equal("First", ws.Cells["B2"].Value);
        Assert.Equal(10d, ws.Cells["C2"].Value);
        Assert.Equal("2", ws.Cells["A3"].Value);
        Assert.Equal("Second", ws.Cells["B3"].Value);
        Assert.Equal(20d, ws.Cells["C3"].Value);
    }

    [Fact]
    public void LoadFromCollectionWithTableSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var sheet = pck.Workbook.Worksheets.Add("sheet");
            var items = new List<Aclass>
            {
                new() { Id = "1", Name = "Test", Number = 5 }
            };
            sheet.Cells["A1"].LoadFromCollection(items, true, TableStyles.Medium6);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws = pck2.Workbook.Worksheets[0];

        Assert.Equal(1, ws.Tables.Count);
        Assert.Equal(TableStyles.Medium6, ws.Tables[0].TableStyle);
    }

    #endregion
}
