using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class CommentsTests
{
    #region Add Comment Tests

    [Fact]
    public void AddCommentSetsTextAndAuthor()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");
        var a1 = ws.Cells["A1"];
        a1.Value = "Test Value";

        a1.AddComment("This is a comment", "TestAuthor");

        Assert.NotNull(a1.Comment);
        Assert.Equal("This is a comment", a1.Comment.Text);
        Assert.Equal("TestAuthor", a1.Comment.Author);
    }

    [Fact]
    public void AddCommentIncreasesCommentCount()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");

        Assert.Equal(0, ws.Comments.Count);

        ws.Cells["A1"].Value = "Value1";
        ws.Cells["A1"].AddComment("Comment 1", "Author1");
        Assert.Equal(1, ws.Comments.Count);

        ws.Cells["B2"].Value = "Value2";
        ws.Cells["B2"].AddComment("Comment 2", "Author2");
        Assert.Equal(2, ws.Comments.Count);
    }

    [Fact]
    public void AddMultipleCommentsWithDifferentAuthors()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");

        ws.Cells["A1"].AddComment("Comment by Alice", "Alice");
        ws.Cells["B1"].AddComment("Comment by Bob", "Bob");

        Assert.Equal("Alice", ws.Comments[0].Author);
        Assert.Equal("Bob", ws.Comments[1].Author);
    }

    #endregion

    #region Comment Visibility Tests

    [Fact]
    public void CommentIsHiddenByDefault()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");
        var a1 = ws.Cells["A1"];
        a1.Value = "Justin Dearing";

        a1.AddComment("I am A1s comment", "JD");

        Assert.False(a1.Comment.Visible);
    }

    [Fact]
    public void CommentVisibilityCanBeToggled()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");
        var a1 = ws.Cells["A1"];
        a1.Value = "Justin Dearing";
        a1.AddComment("I am A1s comment", "JD");

        a1.Comment.Visible = true;
        Assert.True(a1.Comment.Visible);

        a1.Comment.Visible = false;
        Assert.False(a1.Comment.Visible);
    }

    [Fact]
    public void CommentVisibilityStyleSetToHiddenWhenNotVisible()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");
        var a1 = ws.Cells["A1"];
        a1.Value = "Justin Dearing";
        a1.AddComment("I am A1s comment", "JD");

        a1.Comment.Visible = true;
        a1.Comment.Visible = false;

        Assert.NotNull(a1.Comment);

        // Parse the style attribute and check visibility
        var stylesDict = new Dictionary<string, string>();
        var styles = a1.Comment.Style.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in styles)
        {
            var split = s.Split(':');
            if (split.Length == 2)
            {
                var k = (split[0] ?? "").Trim().ToLower();
                var v = (split[1] ?? "").Trim().ToLower();
                stylesDict[k] = v;
            }
        }

        Assert.True(stylesDict.ContainsKey("visibility"));
        Assert.Equal("hidden", stylesDict["visibility"]);
        Assert.False(a1.Comment.Visible);
    }

    [Fact]
    public void CommentVisibilityStyleSetToVisibleWhenVisible()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");
        var a1 = ws.Cells["A1"];
        a1.Value = "Test";
        a1.AddComment("Visible comment", "Author");

        a1.Comment.Visible = true;

        var stylesDict = new Dictionary<string, string>();
        var styles = a1.Comment.Style.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in styles)
        {
            var split = s.Split(':');
            if (split.Length == 2)
            {
                var k = (split[0] ?? "").Trim().ToLower();
                var v = (split[1] ?? "").Trim().ToLower();
                stylesDict[k] = v;
            }
        }

        Assert.True(stylesDict.ContainsKey("visibility"));
        Assert.Equal("visible", stylesDict["visibility"]);
        Assert.True(a1.Comment.Visible);
    }

    #endregion

    #region Remove Comment Tests

    [Fact]
    public void RemoveCommentDecreasesCount()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");

        ws.Cells["A1"].AddComment("Comment 1", "Author1");
        ws.Cells["B2"].AddComment("Comment 2", "Author2");
        Assert.Equal(2, ws.Comments.Count);

        ws.Comments.Remove(ws.Comments[0]);
        Assert.Equal(1, ws.Comments.Count);
    }

    [Fact]
    public void RemoveAtRemovesCorrectComment()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");

        ws.Cells["A1"].AddComment("First", "Author");
        ws.Cells["B1"].AddComment("Second", "Author");

        ws.Comments.RemoveAt(0);

        Assert.Equal(1, ws.Comments.Count);
        Assert.Equal("Second", ws.Comments[0].Text);
    }

    #endregion

    #region Comment Text Tests

    [Fact]
    public void CommentTextCanBeModified()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");
        ws.Cells["A1"].AddComment("Original text", "Author");

        ws.Comments[0].Text = "Modified text";

        Assert.Equal("Modified text", ws.Comments[0].Text);
    }

    [Fact]
    public void CommentAuthorCanBeChanged()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");
        ws.Cells["A1"].AddComment("Comment", "OriginalAuthor");

        ws.Comments[0].Author = "NewAuthor";

        Assert.Equal("NewAuthor", ws.Comments[0].Author);
    }

    #endregion

    #region Comment Indexer Tests

    [Fact]
    public void CommentsCanBeAccessedByIndex()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");

        ws.Cells["A1"].AddComment("Comment A1", "Author");
        ws.Cells["C3"].AddComment("Comment C3", "Author");

        Assert.Equal("Comment A1", ws.Comments[0].Text);
        Assert.Equal("Comment C3", ws.Comments[1].Text);
    }

    [Fact]
    public void CommentsCanBeAccessedByCellAddress()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");

        ws.Cells["A1"].AddComment("Comment A1", "Author");
        ws.Cells["C3"].AddComment("Comment C3", "Author");

        var comment = ws.Comments[new ExcelCellAddress("C3")];
        Assert.NotNull(comment);
        Assert.Equal("Comment C3", comment.Text);
    }

    [Fact]
    public void CommentIndexerReturnsNullForNonExistentCell()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Comment");

        ws.Cells["A1"].AddComment("Comment A1", "Author");

        var comment = ws.Comments[new ExcelCellAddress("B2")];
        Assert.Null(comment);
    }

    #endregion

    #region Comment Persistence Tests

    [Fact]
    public void CommentsSurviveSaveAndReload()
    {
        using var stream = new System.IO.MemoryStream();

        // Create and save
        using (var pck = new ExcelPackage(stream))
        {
            var ws = pck.Workbook.Worksheets.Add("Comment");
            ws.Cells["A1"].Value = "Test";
            ws.Cells["A1"].AddComment("Persisted comment", "Author");
            pck.Save();
        }

        // Reload and verify
        stream.Position = 0;
        using (var pck = new ExcelPackage(stream))
        {
            var ws = pck.Workbook.Worksheets.First();
            Assert.Equal(1, ws.Comments.Count);
            Assert.Equal("Persisted comment", ws.Comments[0].Text);
            Assert.Equal("Author", ws.Comments[0].Author);
        }
    }

    [Fact]
    public void CommentVisibilitySurvivesSaveAndReload()
    {
        using var stream = new System.IO.MemoryStream();

        using (var pck = new ExcelPackage(stream))
        {
            var ws = pck.Workbook.Worksheets.Add("Comment");
            ws.Cells["A1"].Value = "Test";
            ws.Cells["A1"].AddComment("Comment", "Author");
            ws.Cells["A1"].Comment.Visible = false;
            pck.Save();
        }

        stream.Position = 0;
        using (var pck = new ExcelPackage(stream))
        {
            var ws = pck.Workbook.Worksheets.First();
            Assert.False(ws.Comments[0].Visible);
        }
    }

    #endregion
}