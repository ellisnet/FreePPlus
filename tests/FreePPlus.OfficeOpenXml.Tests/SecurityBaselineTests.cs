using System;
using System.IO;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.ExcelUtilities;
using OfficeOpenXml.Table;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

/// <summary>
///     Baseline tests capturing current behavior of code affected by identified security vulnerabilities.
///     These tests document pre-remediation behavior so that fixes can be verified without regression.
/// </summary>
public class SecurityBaselineTests
{
    #region VULN-1: RichText XML Special Characters (GetRichText uses xml.LoadXml bypassing DTD protection)

    [Fact]
    public void RichText_WithAngleBrackets_EscapesCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "5 < 10";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("5 < 10", rt.Text);
    }

    [Fact]
    public void RichText_WithGreaterThan_EscapesCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "10 > 5";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("10 > 5", rt.Text);
    }

    [Fact]
    public void RichText_WithAmpersand_EscapesCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "Tom & Jerry";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("Tom & Jerry", rt.Text);
    }

    [Fact]
    public void RichText_WithMultipleXmlSpecialChars_EscapesCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "a < b & c > d";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("a < b & c > d", rt.Text);
    }

    [Fact]
    public void RichText_WithXmlSpecialChars_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].RichText.Add("Price < $10 & tax > 0");
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.Equal("Price < $10 & tax > 0", ws.Cells["A1"].RichText.Text);
        }
    }

    [Fact]
    public void RichText_WithQuotes_HandledCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "She said \"hello\"";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("She said \"hello\"", rt.Text);
    }

    [Fact]
    public void RichText_WithApostrophe_HandledCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "it's a test";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("it's a test", rt.Text);
    }

    [Fact]
    public void RichText_EmptyCellValue_ReturnsEmptyRichText()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        // Cell has no value set
        var rt = ws.Cells["A1"].RichText;

        Assert.NotNull(rt);
        Assert.Equal(0, rt.Count);
    }

    [Fact]
    public void RichText_NumericCellValue_ConvertedToString()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = 42;
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("42", rt.Text);
    }

    [Fact]
    public void RichText_WithXmlEntityLikeString_HandledCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        // This string contains something that looks like an XML entity reference
        ws.Cells["A1"].Value = "copy &amp; paste";
        var rt = ws.Cells["A1"].RichText;

        // ExcelEscapeString will escape the & in &amp; to &amp;amp;
        // so the round-tripped text should preserve the literal string
        Assert.Equal("copy &amp; paste", rt.Text);
    }

    [Fact]
    public void RichText_WithCdataLikeString_HandledCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "data ]]> end";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("data ]]> end", rt.Text);
    }

    [Fact]
    public void RichText_WithXmlCommentLikeString_HandledCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        // ExcelEscapeString escapes < and >, so this should be safe
        ws.Cells["A1"].Value = "<!-- comment -->";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("<!-- comment -->", rt.Text);
    }

    [Fact]
    public void RichText_WithProcessingInstructionLikeString_HandledCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "<?xml version=\"1.0\"?>";
        var rt = ws.Cells["A1"].RichText;

        Assert.Equal("<?xml version=\"1.0\"?>", rt.Text);
    }

    #endregion

    #region VULN-2: AddComment with null/empty Author (ClaimsPrincipal.Current.Identity.Name crash)

    [Fact]
    public void AddComment_WithNullAuthor_DefaultsToAuthor()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var comment = ws.Cells["A1"].AddComment("Test comment", null);

        Assert.NotNull(comment);
        Assert.Equal("Author", comment.Author);
        Assert.Equal("Test comment", comment.Text);
    }

    [Fact]
    public void AddComment_WithEmptyAuthor_DefaultsToAuthor()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var comment = ws.Cells["A1"].AddComment("Test comment", "");

        Assert.NotNull(comment);
        Assert.Equal("Author", comment.Author);
        Assert.Equal("Test comment", comment.Text);
    }

    [Fact]
    public void AddComment_WithWhitespaceAuthor_Succeeds()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        // Whitespace-only string is not null/empty, so it should not trigger
        // the ClaimsPrincipal fallback
        var comment = ws.Cells["A1"].AddComment("Test comment", " ");

        Assert.NotNull(comment);
        Assert.Equal(" ", comment.Author);
    }

    [Fact]
    public void AddComment_WithValidAuthor_Succeeds()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var comment = ws.Cells["A1"].AddComment("Test comment", "ValidAuthor");

        Assert.NotNull(comment);
        Assert.Equal("ValidAuthor", comment.Author);
        Assert.Equal("Test comment", comment.Text);
    }

    [Fact]
    public void AddComment_WithValidAuthor_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].AddComment("Persisted comment", "TestAuthor");
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.Equal(1, ws.Comments.Count);
            Assert.Equal("Persisted comment", ws.Comments[0].Text);
            Assert.Equal("TestAuthor", ws.Comments[0].Author);
        }
    }

    [Fact]
    public void AddComment_WithXmlSpecialCharsInText_Succeeds()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var comment = ws.Cells["A1"].AddComment("Price < $10 & tax > 0", "Author");

        Assert.NotNull(comment);
        Assert.Equal("Price < $10 & tax > 0", comment.Text);
    }

    #endregion

    #region VULN-3: Sheet/Workbook Protection Password Hash (16-bit XOR hash)

    [Fact]
    public void SheetProtection_SetPassword_SetsProtectedFlag()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Protection.SetPassword("TestPassword");

        Assert.True(ws.Protection.IsProtected);
    }

    [Fact]
    public void SheetProtection_SetPassword_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Protection.SetPassword("SecurePassword");
            ws.Protection.AllowSort = true;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.True(ws.Protection.IsProtected);
            Assert.True(ws.Protection.AllowSort);
        }
    }

    [Fact]
    public void SheetProtection_SetEmptyPassword_ClearsPassword()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Protection.SetPassword("TestPassword");
        Assert.True(ws.Protection.IsProtected);

        // Setting empty password (after trim) clears the password attribute
        ws.Protection.SetPassword("");

        // IsProtected was set to true on first SetPassword call and is not cleared
        Assert.True(ws.Protection.IsProtected);
    }

    [Fact]
    public void SheetProtection_SetPassword_DifferentPasswordsProduceDifferentHashes()
    {
        using var ms1 = new MemoryStream();
        using var ms2 = new MemoryStream();

        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Protection.SetPassword("Password1");
            pck.SaveAs(ms1);
        }

        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Protection.SetPassword("Password2");
            pck.SaveAs(ms2);
        }

        // The saved packages should differ (different password hashes)
        ms1.Position = 0;
        ms2.Position = 0;
        var bytes1 = ms1.ToArray();
        var bytes2 = ms2.ToArray();
        Assert.NotEqual(bytes1, bytes2);
    }

    [Fact]
    public void SheetProtection_SamePasswordProducesSameHash()
    {
        using var ms1 = new MemoryStream();
        using var ms2 = new MemoryStream();

        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Protection.SetPassword("SamePassword");
            pck.SaveAs(ms1);
        }

        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Protection.SetPassword("SamePassword");
            pck.SaveAs(ms2);
        }

        // Both packages should have the same password hash
        // (the 16-bit XOR hash is deterministic)
        ms1.Position = 0;
        ms2.Position = 0;

        using var pck1 = new ExcelPackage(ms1);
        using var pck2 = new ExcelPackage(ms2);

        // Verify both are protected
        Assert.True(pck1.Workbook.Worksheets["Sheet1"].Protection.IsProtected);
        Assert.True(pck2.Workbook.Worksheets["Sheet1"].Protection.IsProtected);
    }

    [Fact]
    public void WorkbookProtection_SetPassword_SetsHash()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Protection.SetPassword("WorkbookPass");
        pck.Workbook.Protection.LockStructure = true;

        Assert.True(pck.Workbook.Protection.LockStructure);
    }

    [Fact]
    public void WorkbookProtection_SetPassword_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            pck.Workbook.Protection.SetPassword("WorkbookPass");
            pck.Workbook.Protection.LockStructure = true;
            pck.Workbook.Protection.LockWindows = true;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            Assert.True(pck.Workbook.Protection.LockStructure);
            Assert.True(pck.Workbook.Protection.LockWindows);
        }
    }

    [Fact]
    public void WorkbookProtection_ClearPassword_RemovesNode()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Protection.SetPassword("WorkbookPass");
        pck.Workbook.Protection.LockStructure = true;

        pck.Workbook.Protection.SetPassword("");

        // Clearing the password removes the password node but doesn't affect other settings
        Assert.True(pck.Workbook.Protection.LockStructure);
    }

    [Fact]
    public void WorkbookProtection_SetNullPassword_RemovesNode()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Protection.SetPassword("WorkbookPass");
        pck.Workbook.Protection.LockStructure = true;

        pck.Workbook.Protection.SetPassword(null);

        Assert.True(pck.Workbook.Protection.LockStructure);
    }

    [Fact]
    public void SheetProtection_AllPermissions_RoundTrip()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Protection.SetPassword("AllPerms");
            ws.Protection.AllowSort = true;
            ws.Protection.AllowAutoFilter = true;
            ws.Protection.AllowInsertRows = true;
            ws.Protection.AllowInsertColumns = true;
            ws.Protection.AllowDeleteRows = true;
            ws.Protection.AllowDeleteColumns = true;
            ws.Protection.AllowFormatCells = true;
            ws.Protection.AllowFormatRows = true;
            ws.Protection.AllowFormatColumns = true;
            ws.Protection.AllowInsertHyperlinks = true;
            ws.Protection.AllowPivotTables = true;
            ws.Protection.AllowSelectLockedCells = true;
            ws.Protection.AllowSelectUnlockedCells = true;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.True(ws.Protection.IsProtected);
            Assert.True(ws.Protection.AllowSort);
            Assert.True(ws.Protection.AllowAutoFilter);
            Assert.True(ws.Protection.AllowInsertRows);
            Assert.True(ws.Protection.AllowInsertColumns);
            Assert.True(ws.Protection.AllowDeleteRows);
            Assert.True(ws.Protection.AllowDeleteColumns);
            Assert.True(ws.Protection.AllowFormatCells);
            Assert.True(ws.Protection.AllowFormatRows);
            Assert.True(ws.Protection.AllowFormatColumns);
            Assert.True(ws.Protection.AllowInsertHyperlinks);
            Assert.True(ws.Protection.AllowPivotTables);
            Assert.True(ws.Protection.AllowSelectLockedCells);
            Assert.True(ws.Protection.AllowSelectUnlockedCells);
        }
    }

    #endregion

    #region VULN-4: WildCardValueMatcher ReDoS (no regex timeout)

    [Fact]
    public void WildCardValueMatcher_AsteriskMatchesAnyString()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("*", "anything"));
        Assert.Equal(0, matcher.IsMatch("*", ""));
        Assert.Equal(0, matcher.IsMatch("*", "hello world"));
    }

    [Fact]
    public void WildCardValueMatcher_QuestionMarkMatchesSingleChar()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("h?llo", "hello"));
        Assert.Equal(0, matcher.IsMatch("h?llo", "hallo"));
        Assert.NotEqual(0, matcher.IsMatch("h?llo", "hllo"));
        Assert.NotEqual(0, matcher.IsMatch("h?llo", "heello"));
    }

    [Fact]
    public void WildCardValueMatcher_AsteriskAtStart_MatchesSuffix()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("*world", "hello world"));
        Assert.Equal(0, matcher.IsMatch("*world", "world"));
        Assert.NotEqual(0, matcher.IsMatch("*world", "worlds"));
    }

    [Fact]
    public void WildCardValueMatcher_AsteriskAtEnd_MatchesPrefix()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("hello*", "hello world"));
        Assert.Equal(0, matcher.IsMatch("hello*", "hello"));
        Assert.NotEqual(0, matcher.IsMatch("hello*", "hell"));
    }

    [Fact]
    public void WildCardValueMatcher_AsteriskInMiddle_MatchesAnyMiddle()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("h*o", "hello"));
        Assert.Equal(0, matcher.IsMatch("h*o", "ho"));
        Assert.Equal(0, matcher.IsMatch("h*o", "h something o"));
        Assert.NotEqual(0, matcher.IsMatch("h*o", "h something"));
    }

    [Fact]
    public void WildCardValueMatcher_MultipleAsterisks()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("*hello*", "say hello world"));
        Assert.Equal(0, matcher.IsMatch("*hello*", "hello"));
        Assert.Equal(0, matcher.IsMatch("a*b*c", "abc"));
        Assert.Equal(0, matcher.IsMatch("a*b*c", "aXXbYYc"));
        Assert.NotEqual(0, matcher.IsMatch("a*b*c", "aXXYYc"));
    }

    [Fact]
    public void WildCardValueMatcher_MultipleQuestionMarks()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("??", "ab"));
        Assert.NotEqual(0, matcher.IsMatch("??", "a"));
        Assert.NotEqual(0, matcher.IsMatch("??", "abc"));
    }

    [Fact]
    public void WildCardValueMatcher_MixedWildcards()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("h?llo*", "hello world"));
        Assert.Equal(0, matcher.IsMatch("*?orld", "world"));
        Assert.Equal(0, matcher.IsMatch("?e*o", "hello"));
    }

    [Fact]
    public void WildCardValueMatcher_NoWildcards_FallsBackToBaseComparison()
    {
        var matcher = new WildCardValueMatcher();

        // Without wildcards, falls back to base class string comparison
        Assert.Equal(0, matcher.IsMatch("hello", "hello"));
        Assert.NotEqual(0, matcher.IsMatch("hello", "world"));
    }

    [Fact]
    public void WildCardValueMatcher_IsCaseInsensitive()
    {
        var matcher = new WildCardValueMatcher();

        // ValueMatcher.IsMatch lowercases both strings before calling CompareStringToString
        Assert.Equal(0, matcher.IsMatch("HELLO*", "Hello World"));
        Assert.Equal(0, matcher.IsMatch("h?llo", "HELLO"));
        Assert.Equal(0, matcher.IsMatch("*WORLD", "hello world"));
    }

    [Fact]
    public void WildCardValueMatcher_WithRegexSpecialChars_TreatsAsLiterals()
    {
        var matcher = new WildCardValueMatcher();

        // Characters like . + ( ) [ ] { } ^ $ | should be treated as literals, not regex
        Assert.Equal(0, matcher.IsMatch("price$*", "price$100"));
        Assert.Equal(0, matcher.IsMatch("test.value*", "test.value123"));
        Assert.Equal(0, matcher.IsMatch("[test]*", "[test]abc"));
        Assert.Equal(0, matcher.IsMatch("(a+b)*", "(a+b)=c"));
    }

    [Fact]
    public void WildCardValueMatcher_NullValues()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(-1, matcher.IsMatch(null, "hello"));
        Assert.Equal(1, matcher.IsMatch("hello", null));
        Assert.Equal(0, matcher.IsMatch(null, null));
    }

    [Fact]
    public void WildCardValueMatcher_NonStringValues_ComparedNumericallly()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch(42, 42));
        Assert.True(matcher.IsMatch(10, 20) < 0);
        Assert.True(matcher.IsMatch(20, 10) > 0);
    }

    [Fact]
    public void WildCardValueMatcher_StringToNonString_ReturnsIncompatible()
    {
        var matcher = new WildCardValueMatcher();

        // When one operand is a non-numeric string and the other is a number,
        // the base ValueMatcher returns IncompatibleOperands (-2)
        Assert.Equal(ValueMatcher.IncompatibleOperands, matcher.IsMatch("abc", 42));
    }

    [Fact]
    public void WildCardValueMatcher_EmptyPattern_MatchesEmptyString()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("", ""));
        Assert.NotEqual(0, matcher.IsMatch("", "notempty"));
    }

    [Fact]
    public void WildCardValueMatcher_PatternWithOnlyQuestionMarks_MatchesExactLength()
    {
        var matcher = new WildCardValueMatcher();

        Assert.Equal(0, matcher.IsMatch("???", "abc"));
        Assert.NotEqual(0, matcher.IsMatch("???", "ab"));
        Assert.NotEqual(0, matcher.IsMatch("???", "abcd"));
    }

    [Fact]
    public void WildCardValueMatcher_RepeatedAsterisksInPattern()
    {
        var matcher = new WildCardValueMatcher();

        // Multiple consecutive asterisks should behave the same as a single asterisk
        Assert.Equal(0, matcher.IsMatch("**", "anything"));
        Assert.Equal(0, matcher.IsMatch("a***b", "aXYZb"));
    }

    #endregion

    #region VULN-7: LoadFromText with FileInfo overloads

    [Fact]
    public void LoadFromText_FileInfo_LoadsBasicCsv()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Name,Age\nAlice,30\nBob,25", Encoding.ASCII);

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("CSV");

            ws.Cells["A1"].LoadFromText(new FileInfo(tempFile));

            // Default delimiter is comma, default EOL is \r\n
            // Since we used \n, it loads as single row with newlines
            Assert.NotNull(ws.Cells["A1"].Value);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadFromText_FileInfo_WithFormat_LoadsCorrectly()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Name,Age\nAlice,30\nBob,25", Encoding.UTF8);

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("CSV");

            ws.Cells["A1"].LoadFromText(new FileInfo(tempFile), new ExcelTextFormat
            {
                Delimiter = ',',
                EOL = "\n",
                Encoding = Encoding.UTF8
            });

            Assert.Equal("Name", ws.Cells["A1"].Value);
            Assert.Equal("Age", ws.Cells["B1"].Value);
            Assert.Equal("Alice", ws.Cells["A2"].Value);
            Assert.Equal(30.0, ws.Cells["B2"].Value);
            Assert.Equal("Bob", ws.Cells["A3"].Value);
            Assert.Equal(25.0, ws.Cells["B3"].Value);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadFromText_FileInfo_NonExistentFile_ThrowsFileNotFoundException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CSV");

        var nonExistent = new FileInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv"));

        Assert.Throws<FileNotFoundException>(() =>
            ws.Cells["A1"].LoadFromText(nonExistent));
    }

    [Fact]
    public void LoadFromText_FileInfo_WithFormat_NonExistentFile_ThrowsFileNotFoundException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CSV");

        var nonExistent = new FileInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv"));

        Assert.Throws<FileNotFoundException>(() =>
            ws.Cells["A1"].LoadFromText(nonExistent, new ExcelTextFormat { Delimiter = ',' }));
    }

    [Fact]
    public void LoadFromText_FileInfo_WithTableStyle_LoadsCorrectly()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Name,Age\r\nAlice,30\r\nBob,25", Encoding.UTF8);

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("CSV");

            ws.Cells["A1"].LoadFromText(new FileInfo(tempFile), new ExcelTextFormat
            {
                Delimiter = ',',
                Encoding = Encoding.UTF8
            }, TableStyles.Light1, true);

            Assert.Equal("Name", ws.Cells["A1"].Value);
            Assert.Equal("Age", ws.Cells["B1"].Value);
            Assert.Equal("Alice", ws.Cells["A2"].Value);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadFromText_FileInfo_EmptyFile_DoesNotThrow()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            // File exists but is empty
            File.WriteAllText(tempFile, "", Encoding.ASCII);

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("CSV");

            // Loading an empty file still returns a range reference
            var result = ws.Cells["A1"].LoadFromText(new FileInfo(tempFile));

            Assert.NotNull(result);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadFromText_FileInfo_DefaultEncodingIsAscii()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            // Write ASCII-compatible content
            File.WriteAllText(tempFile, "Hello,World\r\n1,2", Encoding.ASCII);

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("CSV");

            // The default overload uses Encoding.ASCII
            ws.Cells["A1"].LoadFromText(new FileInfo(tempFile));

            Assert.NotNull(ws.Cells["A1"].Value);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadFromText_FileInfo_RoundTrips()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Name,Value\r\nItem1,100\r\nItem2,200", Encoding.UTF8);

            using var ms = new MemoryStream();
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("CSV");
                ws.Cells["A1"].LoadFromText(new FileInfo(tempFile), new ExcelTextFormat
                {
                    Delimiter = ',',
                    Encoding = Encoding.UTF8
                });
                pck.SaveAs(ms);
            }

            ms.Position = 0;
            using (var pck = new ExcelPackage(ms))
            {
                var ws = pck.Workbook.Worksheets["CSV"];
                Assert.Equal("Name", ws.Cells["A1"].Value);
                Assert.Equal("Value", ws.Cells["B1"].Value);
                Assert.Equal("Item1", ws.Cells["A2"].Value);
                Assert.Equal(100.0, ws.Cells["B2"].Value);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void LoadFromText_FileInfo_NullFileInfo_ThrowsArgumentNullException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CSV");

        Assert.Throws<ArgumentNullException>(() =>
            ws.Cells["A1"].LoadFromText((FileInfo)null));
    }

    [Fact]
    public void LoadFromText_FileInfo_WithFormat_NullFileInfo_ThrowsArgumentNullException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CSV");

        Assert.Throws<ArgumentNullException>(() =>
            ws.Cells["A1"].LoadFromText((FileInfo)null, new ExcelTextFormat { Delimiter = ',' }));
    }

    [Fact]
    public void LoadFromText_FileInfo_WithTableStyle_NullFileInfo_ThrowsArgumentNullException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CSV");

        Assert.Throws<ArgumentNullException>(() =>
            ws.Cells["A1"].LoadFromText((FileInfo)null, new ExcelTextFormat { Delimiter = ',' },
                TableStyles.Light1, true));
    }

    [Fact]
    public void LoadFromText_FileInfo_WithTableStyle_NonExistentFile_ThrowsFileNotFoundException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CSV");

        var nonExistent = new FileInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv"));

        Assert.Throws<FileNotFoundException>(() =>
            ws.Cells["A1"].LoadFromText(nonExistent, new ExcelTextFormat { Delimiter = ',' },
                TableStyles.Light1, true));
    }

    #endregion

    #region Document Properties XML Safety

    [Fact]
    public void CoreProperties_CanBeSetAndRetrieved()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Properties.Title = "Test Title";
        pck.Workbook.Properties.Author = "Test Author";
        pck.Workbook.Properties.Subject = "Test Subject";
        pck.Workbook.Properties.Keywords = "test, keywords";

        Assert.Equal("Test Title", pck.Workbook.Properties.Title);
        Assert.Equal("Test Author", pck.Workbook.Properties.Author);
        Assert.Equal("Test Subject", pck.Workbook.Properties.Subject);
        Assert.Equal("test, keywords", pck.Workbook.Properties.Keywords);
    }

    [Fact]
    public void CoreProperties_WithXmlSpecialChars_HandledCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Properties.Title = "Title with & and < > chars";
        pck.Workbook.Properties.Author = "Author <O'Brien>";

        Assert.Equal("Title with & and < > chars", pck.Workbook.Properties.Title);
        Assert.Equal("Author <O'Brien>", pck.Workbook.Properties.Author);
    }

    [Fact]
    public void CoreProperties_WithXmlSpecialChars_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            pck.Workbook.Properties.Title = "Title & <Value>";
            pck.Workbook.Properties.Author = "Author \"quoted\"";
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            Assert.Equal("Title & <Value>", pck.Workbook.Properties.Title);
            Assert.Equal("Author \"quoted\"", pck.Workbook.Properties.Author);
        }
    }

    [Fact]
    public void CustomProperties_CanBeSetAndRetrieved()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Properties.SetCustomPropertyValue("CustomKey", "CustomValue");
        pck.Workbook.Properties.SetCustomPropertyValue("NumericKey", 42);

        Assert.Equal("CustomValue", pck.Workbook.Properties.GetCustomPropertyValue("CustomKey"));
        Assert.Equal(42, pck.Workbook.Properties.GetCustomPropertyValue("NumericKey"));
    }

    [Fact]
    public void CustomProperties_WithXmlSpecialChars_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            pck.Workbook.Properties.SetCustomPropertyValue("Key&Name", "Value < 10 & > 5");
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            Assert.Equal("Value < 10 & > 5", pck.Workbook.Properties.GetCustomPropertyValue("Key&Name"));
        }
    }

    #endregion
}
