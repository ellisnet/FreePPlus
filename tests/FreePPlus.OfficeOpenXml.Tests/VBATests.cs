using OfficeOpenXml;
using OfficeOpenXml.Utils;
using OfficeOpenXml.VBA;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class VBATests
{
#if SAVE_TEMP_FILES
    public const string TempFolder = @"C:\Temp";
#endif

    #region VBA Compression

    [Fact]
    public void Compression_RoundTrip_PreservesContent()
    {
        // Inspired by original EPPlus VBATests.Compression
        var original = "#if 0 then\r\n#end if\r\n";
        var bytes = Encoding.GetEncoding(1252).GetBytes(original);

        var compressed = VBACompression.CompressPart(bytes);
        var decompressed = VBACompression.DecompressPart(compressed);
        var result = Encoding.GetEncoding(1252).GetString(decompressed);

        Assert.Equal(original, result);
    }

    [Fact]
    public void Compression_EmptyInput_RoundTrips()
    {
        var bytes = Encoding.GetEncoding(1252).GetBytes("");
        var compressed = VBACompression.CompressPart(bytes);
        var decompressed = VBACompression.DecompressPart(compressed);

        Assert.Equal("", Encoding.GetEncoding(1252).GetString(decompressed));
    }

    [Fact]
    public void Compression_SingleCharacter_RoundTrips()
    {
        var bytes = Encoding.GetEncoding(1252).GetBytes("A");
        var compressed = VBACompression.CompressPart(bytes);
        var decompressed = VBACompression.DecompressPart(compressed);

        Assert.Equal("A", Encoding.GetEncoding(1252).GetString(decompressed));
    }

    [Fact]
    public void Compression_RepeatedContent_RoundTrips()
    {
        // Content with lots of repetition should compress well
        var original = string.Concat(Enumerable.Repeat("ABCDEFGH", 100));
        var bytes = Encoding.GetEncoding(1252).GetBytes(original);

        var compressed = VBACompression.CompressPart(bytes);
        var decompressed = VBACompression.DecompressPart(compressed);
        var result = Encoding.GetEncoding(1252).GetString(decompressed);

        Assert.Equal(original, result);
        Assert.True(compressed.Length < bytes.Length, "Repeated content should compress smaller");
    }

    [Fact]
    public void Compression_VBACode_RoundTrips()
    {
        var vbaCode =
            "Attribute VB_Name = \"Module1\"\r\n" +
            "Public Sub HelloWorld()\r\n" +
            "    MsgBox \"Hello World!\"\r\n" +
            "End Sub\r\n" +
            "Public Function Add(a As Long, b As Long) As Long\r\n" +
            "    Add = a + b\r\n" +
            "End Function\r\n";

        var bytes = Encoding.GetEncoding(1252).GetBytes(vbaCode);
        var compressed = VBACompression.CompressPart(bytes);
        var decompressed = VBACompression.DecompressPart(compressed);
        var result = Encoding.GetEncoding(1252).GetString(decompressed);

        Assert.Equal(vbaCode, result);
    }

    [Fact]
    public void Compression_LargeContent_RoundTrips()
    {
        // Inspired by DecompressionChunkGreaterThan4k - tests chunks > 4096 bytes
        var sb = new StringBuilder();
        for (var i = 0; i < 200; i++)
            sb.AppendLine($"Public Sub Procedure{i}()\r\n    ' Some code line {i}\r\nEnd Sub");

        var original = sb.ToString();
        var bytes = Encoding.GetEncoding(1252).GetBytes(original);

        Assert.True(bytes.Length > 4096, "Test data should exceed 4k to test chunked compression");

        var compressed = VBACompression.CompressPart(bytes);
        var decompressed = VBACompression.DecompressPart(compressed);
        var result = Encoding.GetEncoding(1252).GetString(decompressed);

        Assert.Equal(original, result);
    }

    [Fact]
    public void Compression_WithOffset_DecompressesCorrectly()
    {
        var original = "Sub Test()\r\nEnd Sub\r\n";
        var bytes = Encoding.GetEncoding(1252).GetBytes(original);

        var compressed = VBACompression.CompressPart(bytes);

        // Prepend some extra bytes to simulate an offset scenario
        var withPrefix = new byte[10 + compressed.Length];
        Array.Copy(compressed, 0, withPrefix, 10, compressed.Length);
        var decompressed = VBACompression.DecompressPart(withPrefix, 10);
        var result = Encoding.GetEncoding(1252).GetString(decompressed);

        Assert.Equal(original, result);
    }

    #endregion

    #region Create VBA Project

    [Fact]
    public void CreateVBAProject_SetsDefaultProperties()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Sheet1");

        package.Workbook.CreateVBAProject();

        Assert.NotNull(package.Workbook.VbaProject);
        Assert.Equal("VBAProject", package.Workbook.VbaProject.Name);
    }

    [Fact]
    public void CreateVBAProject_IncludesThisWorkbookModule()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Sheet1");

        package.Workbook.CreateVBAProject();

        var modules = package.Workbook.VbaProject.Modules;
        Assert.True(modules.Exists("ThisWorkbook"));
        Assert.Equal(eModuleType.Document, modules["ThisWorkbook"].Type);
    }

    [Fact]
    public void CreateVBAProject_IncludesSheetModules()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.Worksheets.Add("Sheet2");

        package.Workbook.CreateVBAProject();

        var modules = package.Workbook.VbaProject.Modules;
        Assert.True(modules.Exists("Sheet1"));
        Assert.True(modules.Exists("Sheet2"));
        Assert.Equal(eModuleType.Document, modules["Sheet1"].Type);
        Assert.Equal(eModuleType.Document, modules["Sheet2"].Type);
    }

    [Fact]
    public void CreateVBAProject_ThrowsIfAlreadyExists()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");

        package.Workbook.CreateVBAProject();

        Assert.Throws<InvalidOperationException>(() => package.Workbook.CreateVBAProject());
    }

    [Fact]
    public void CreateVBAProject_ReferencesCollectionIsAccessible()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");

        package.Workbook.CreateVBAProject();

        // References collection should be initialized (empty by default from Create())
        Assert.NotNull(package.Workbook.VbaProject.References);
        Assert.True(package.Workbook.VbaProject.References.Count >= 0);
    }

    [Fact]
    public void CreateVBAProject_ProjectId_IsNotEmpty()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");

        package.Workbook.CreateVBAProject();

        Assert.False(string.IsNullOrEmpty(package.Workbook.VbaProject.ProjectID));
    }

    #endregion

    #region Module and Class Management

    [Fact]
    public void AddModule_CreatesModuleWithCorrectType()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var module = package.Workbook.VbaProject.Modules.AddModule("Module1");

        Assert.Equal("Module1", module.Name);
        Assert.Equal(eModuleType.Module, module.Type);
    }

    [Fact]
    public void AddModule_IncreasesModuleCount()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var initialCount = package.Workbook.VbaProject.Modules.Count;
        package.Workbook.VbaProject.Modules.AddModule("Module1");

        Assert.Equal(initialCount + 1, package.Workbook.VbaProject.Modules.Count);
    }

    [Fact]
    public void AddClass_Private_CreatesClassWithCorrectType()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var cls = package.Workbook.VbaProject.Modules.AddClass("Class1", false);

        Assert.Equal("Class1", cls.Name);
        Assert.Equal(eModuleType.Class, cls.Type);
        Assert.True(cls.Private);
    }

    [Fact]
    public void AddClass_Public_IsNotPrivate()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var cls = package.Workbook.VbaProject.Modules.AddClass("Class1", true);

        Assert.Equal(eModuleType.Class, cls.Type);
        Assert.False(cls.Private);
    }

    [Fact]
    public void AddClass_HasAttributes()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var cls = package.Workbook.VbaProject.Modules.AddClass("Class1", false);

        Assert.True(cls.Attributes.Count > 0);
    }

    [Fact]
    public void AddMultipleModulesAndClasses()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var initialCount = package.Workbook.VbaProject.Modules.Count;

        package.Workbook.VbaProject.Modules.AddModule("Module1");
        package.Workbook.VbaProject.Modules.AddModule("Module2");
        package.Workbook.VbaProject.Modules.AddClass("Class1", false);
        package.Workbook.VbaProject.Modules.AddClass("Class2", true);

        Assert.Equal(initialCount + 4, package.Workbook.VbaProject.Modules.Count);
        Assert.True(package.Workbook.VbaProject.Modules.Exists("Module1"));
        Assert.True(package.Workbook.VbaProject.Modules.Exists("Module2"));
        Assert.True(package.Workbook.VbaProject.Modules.Exists("Class1"));
        Assert.True(package.Workbook.VbaProject.Modules.Exists("Class2"));
    }

    [Fact]
    public void Module_IndexerByName_ReturnsCorrectModule()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var module = package.Workbook.VbaProject.Modules.AddModule("TestModule");
        module.Code += "Public Sub Hello()\r\nEnd Sub\r\n";

        var retrieved = package.Workbook.VbaProject.Modules["TestModule"];
        Assert.NotNull(retrieved);
        Assert.Contains("Hello", retrieved.Code);
    }

    [Fact]
    public void Module_IndexerByIndex_ReturnsModule()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var count = package.Workbook.VbaProject.Modules.Count;

        // Access the first module by index
        var module = package.Workbook.VbaProject.Modules[0];
        Assert.NotNull(module);
        Assert.False(string.IsNullOrEmpty(module.Name));
    }

    [Fact]
    public void Module_RemoveModule_DecreasesCount()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var module = package.Workbook.VbaProject.Modules.AddModule("ToRemove");
        var countAfterAdd = package.Workbook.VbaProject.Modules.Count;

        package.Workbook.VbaProject.Modules.Remove(module);

        Assert.Equal(countAfterAdd - 1, package.Workbook.VbaProject.Modules.Count);
        Assert.False(package.Workbook.VbaProject.Modules.Exists("ToRemove"));
    }

    [Fact]
    public void Module_Name_RejectsUnicodeCharacters()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var module = package.Workbook.VbaProject.Modules.AddModule("TestModule");

        // Characters > 255 (e.g., CJK characters) are rejected
        Assert.Throws<InvalidOperationException>(() => module.Name = "\u4F60\u597D");
    }

    [Fact]
    public void Module_Code_RejectsAttributePrefix()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var module = package.Workbook.VbaProject.Modules.AddModule("TestModule");

        Assert.Throws<InvalidOperationException>(() =>
            module.Code = "Attribute VB_Name = \"Module1\"\r\nSub Test()\r\nEnd Sub");
    }

    #endregion

    #region Code Assignment

    [Fact]
    public void WorkbookCodeModule_IsAccessible()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var codeModule = package.Workbook.CodeModule;
        Assert.NotNull(codeModule);
        Assert.Equal("ThisWorkbook", codeModule.Name);
    }

    [Fact]
    public void WorkbookCodeModule_CanSetCode()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var code = "Private Sub Workbook_Open()\r\n    MsgBox \"Hello\"\r\nEnd Sub\r\n";
        package.Workbook.CodeModule.Code += code;

        Assert.Contains("Workbook_Open", package.Workbook.CodeModule.Code);
    }

    [Fact]
    public void WorksheetCodeModule_IsAccessible()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var codeModule = ws.CodeModule;
        Assert.NotNull(codeModule);
    }

    [Fact]
    public void WorksheetCodeModule_CanSetCode()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var code = "Private Sub Worksheet_SelectionChange(ByVal Target As Range)\r\nEnd Sub\r\n";
        ws.CodeModule.Code += code;

        Assert.Contains("Worksheet_SelectionChange", ws.CodeModule.Code);
    }

    [Fact]
    public void Module_CanSetAndRetrieveCode()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var module = package.Workbook.VbaProject.Modules.AddModule("Module1");
        var code =
            "Public Sub TestSub()\r\n" +
            "    Dim x As Long\r\n" +
            "    x = 42\r\n" +
            "End Sub\r\n";
        module.Code += code;

        Assert.Equal(code, module.Code);
    }

    [Fact]
    public void Class_CanSetCode()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var cls = package.Workbook.VbaProject.Modules.AddClass("MyClass", false);
        var code =
            "Private _value As Long\r\n" +
            "Public Property Get Value() As Long\r\n" +
            "    Value = _value\r\n" +
            "End Property\r\n" +
            "Public Property Let Value(v As Long)\r\n" +
            "    _value = v\r\n" +
            "End Property\r\n";
        cls.Code += code;

        Assert.Contains("Property Get Value", cls.Code);
        Assert.Contains("Property Let Value", cls.Code);
    }

    #endregion

    #region Protection

    [Fact]
    public void SetPassword_SetsProtectionFlags()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        package.Workbook.VbaProject.Protection.SetPassword("TestPassword");

        Assert.True(package.Workbook.VbaProject.Protection.VbeProtected);
        Assert.False(package.Workbook.VbaProject.Protection.VisibilityState);
    }

    [Fact]
    public void SetPassword_EmptyString_ClearsProtection()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        package.Workbook.VbaProject.Protection.SetPassword("TestPassword");
        Assert.True(package.Workbook.VbaProject.Protection.VbeProtected);

        package.Workbook.VbaProject.Protection.SetPassword("");
        Assert.False(package.Workbook.VbaProject.Protection.VbeProtected);
    }

    [Fact]
    public void SetPassword_Null_ClearsProtection()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        package.Workbook.VbaProject.Protection.SetPassword("TestPassword");
        Assert.True(package.Workbook.VbaProject.Protection.VbeProtected);

        package.Workbook.VbaProject.Protection.SetPassword(null);
        Assert.False(package.Workbook.VbaProject.Protection.VbeProtected);
    }

    [Fact]
    public void Protection_InitialState_IsNotProtected()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        Assert.False(package.Workbook.VbaProject.Protection.VbeProtected);
    }

    #endregion

    #region References

    [Fact]
    public void AddReference_IncreasesCount()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var initialCount = package.Workbook.VbaProject.References.Count;

        package.Workbook.VbaProject.References.Add(new ExcelVbaReference
        {
            Name = "Scripting",
            Libid = "*\\G{420B2830-E718-11CF-893D-00A0C9054228}#1.0#0#C:\\Windows\\System32\\scrrun.dll#Microsoft Scripting Runtime"
        });

        Assert.Equal(initialCount + 1, package.Workbook.VbaProject.References.Count);
    }

    [Fact]
    public void AddReference_CanBeRetrievedByName()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        package.Workbook.VbaProject.References.Add(new ExcelVbaReference
        {
            Name = "Scripting",
            Libid = "*\\G{420B2830-E718-11CF-893D-00A0C9054228}#1.0#0#C:\\Windows\\System32\\scrrun.dll#Microsoft Scripting Runtime"
        });

        var reference = package.Workbook.VbaProject.References["Scripting"];
        Assert.NotNull(reference);
        Assert.Equal("Scripting", reference.Name);
    }

    #endregion

    #region Round-Trip Persistence

    [Fact]
    public void VbaProject_SurvivesRoundTrip()
    {
        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();
            package.Workbook.VbaProject.Modules.AddModule("Module1");

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            Assert.NotNull(package.Workbook.VbaProject);
            Assert.True(package.Workbook.VbaProject.Modules.Exists("Module1"));
        }
    }

    [Fact]
    public void VbaProject_CodeSurvivesRoundTrip()
    {
        var moduleCode =
            "Public Sub TestProcedure()\r\n" +
            "    Dim result As Long\r\n" +
            "    result = 42\r\n" +
            "End Sub\r\n";

        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();

            var module = package.Workbook.VbaProject.Modules.AddModule("Module1");
            module.Code += moduleCode;

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            var module = package.Workbook.VbaProject.Modules["Module1"];
            Assert.NotNull(module);
            Assert.Contains("TestProcedure", module.Code);
            Assert.Contains("result = 42", module.Code);
        }
    }

    [Fact]
    public void VbaProject_ClassSurvivesRoundTrip()
    {
        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();

            var cls = package.Workbook.VbaProject.Modules.AddClass("MyClass", false);
            cls.Code += "Public Value As Long\r\n";

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            var cls = package.Workbook.VbaProject.Modules["MyClass"];
            Assert.NotNull(cls);
            Assert.Equal(eModuleType.Class, cls.Type);
            Assert.Contains("Value", cls.Code);
        }
    }

    [Fact]
    public void VbaProject_ProtectionSurvivesRoundTrip()
    {
        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();
            package.Workbook.VbaProject.Protection.SetPassword("SecretPass");

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            Assert.True(package.Workbook.VbaProject.Protection.VbeProtected);
            Assert.False(package.Workbook.VbaProject.Protection.VisibilityState);
        }
    }

    [Fact]
    public void VbaProject_WorkbookCodeSurvivesRoundTrip()
    {
        var workbookCode = "Private Sub Workbook_Open()\r\n    MsgBox \"Started\"\r\nEnd Sub\r\n";

        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();
            package.Workbook.CodeModule.Code += workbookCode;

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            Assert.Contains("Workbook_Open", package.Workbook.CodeModule.Code);
        }
    }

    [Fact]
    public void VbaProject_ReferenceSurvivesRoundTrip()
    {
        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();

            package.Workbook.VbaProject.References.Add(new ExcelVbaReference
            {
                Name = "Scripting",
                Libid = "*\\G{420B2830-E718-11CF-893D-00A0C9054228}#1.0#0#C:\\Windows\\System32\\scrrun.dll#Microsoft Scripting Runtime"
            });

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            var reference = package.Workbook.VbaProject.References["Scripting"];
            Assert.NotNull(reference);
        }
    }

    [Fact]
    public void VbaProject_FullScenario_SurvivesRoundTrip()
    {
        // Inspired by original EPPlus WriteVBA — comprehensive project with modules, classes, password
        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            var ws1 = package.Workbook.Worksheets.Add("Sheet1");
            var ws2 = package.Workbook.Worksheets.Add("Sheet2");
            ws1.Cells["A1"].Value = "Test";

            package.Workbook.CreateVBAProject();

            // Add workbook code
            package.Workbook.CodeModule.Code += "Private Sub Workbook_Open()\r\nEnd Sub\r\n";

            // Add worksheet code
            ws1.CodeModule.Code += "Private Sub Worksheet_Change(ByVal Target As Range)\r\nEnd Sub\r\n";

            // Add standard module
            var mod = package.Workbook.VbaProject.Modules.AddModule("Module1");
            mod.Code += "Public Sub RunAll()\r\n    ' Do something\r\nEnd Sub\r\n";

            // Add class
            var cls = package.Workbook.VbaProject.Modules.AddClass("Calculator", false);
            cls.Code += "Public Function Add(a As Long, b As Long) As Long\r\n    Add = a + b\r\nEnd Function\r\n";

            // Set password
            package.Workbook.VbaProject.Protection.SetPassword("EPPlus");

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();

#if SAVE_TEMP_FILES
            if (Directory.Exists(TempFolder))
            {
                var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(VbaProject_FullScenario_SurvivesRoundTrip)}.xlsm");
                File.WriteAllBytes(filename, savedData);
            }
#endif
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            Assert.NotNull(package.Workbook.VbaProject);
            Assert.True(package.Workbook.VbaProject.Modules.Exists("Module1"));
            Assert.True(package.Workbook.VbaProject.Modules.Exists("Calculator"));
            Assert.Contains("RunAll", package.Workbook.VbaProject.Modules["Module1"].Code);
            Assert.Contains("Add", package.Workbook.VbaProject.Modules["Calculator"].Code);
            Assert.True(package.Workbook.VbaProject.Protection.VbeProtected);
            Assert.Contains("Workbook_Open", package.Workbook.CodeModule.Code);
        }
    }

    #endregion

    #region Unicode and Edge Cases

    [Fact]
    public void CreateUnicodeWorksheetName_WithVBA()
    {
        // Inspired by original EPPlus CreateUnicodeWsName
        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("Wörk Shéét");
            ws.Cells["A1"].Value = "Unicode test";

            package.Workbook.CreateVBAProject();

            var module = package.Workbook.VbaProject.Modules.AddModule("Module1");
            module.Code += "Public Sub Test()\r\nEnd Sub\r\n";

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();

#if SAVE_TEMP_FILES
            if (Directory.Exists(TempFolder))
            {
                var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(CreateUnicodeWorksheetName_WithVBA)}.xlsm");
                File.WriteAllBytes(filename, savedData);
            }
#endif
        }

        // Verify the package can be reopened
        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            Assert.NotNull(package.Workbook.VbaProject);
            Assert.Equal("Unicode test", package.Workbook.Worksheets["Wörk Shéét"].Cells["A1"].Value);
        }
    }

    [Fact]
    public void MultipleWorksheets_AllHaveCodeModules()
    {
        // Inspired by original EPPlus VbaError — multiple sheets with VBA
        using var package = new ExcelPackage();
        var ws1 = package.Workbook.Worksheets.Add("Sheet1");
        var ws2 = package.Workbook.Worksheets.Add("Sheet2");
        var ws3 = package.Workbook.Worksheets.Add("Sheet3");

        package.Workbook.CreateVBAProject();

        ws1.CodeModule.Code += "Private Sub Worksheet_Activate()\r\nEnd Sub\r\n";
        ws2.CodeModule.Code += "Private Sub Worksheet_Activate()\r\nEnd Sub\r\n";
        ws3.CodeModule.Code += "Private Sub Worksheet_Activate()\r\nEnd Sub\r\n";

        Assert.Contains("Worksheet_Activate", ws1.CodeModule.Code);
        Assert.Contains("Worksheet_Activate", ws2.CodeModule.Code);
        Assert.Contains("Worksheet_Activate", ws3.CodeModule.Code);
    }

    [Fact]
    public void MultipleWorksheets_VBA_SurvivesRoundTrip()
    {
        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            var ws1 = package.Workbook.Worksheets.Add("Sheet1");
            var ws2 = package.Workbook.Worksheets.Add("Sheet2");

            ws1.Cells["A1"].Value = "Hello";
            ws2.Cells["A1"].Value = "World";

            package.Workbook.CreateVBAProject();

            ws1.CodeModule.Code += "Private Sub Worksheet_Change(ByVal Target As Range)\r\n    ' Sheet1\r\nEnd Sub\r\n";
            ws2.CodeModule.Code += "Private Sub Worksheet_Change(ByVal Target As Range)\r\n    ' Sheet2\r\nEnd Sub\r\n";

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();

#if SAVE_TEMP_FILES
            if (Directory.Exists(TempFolder))
            {
                var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(MultipleWorksheets_VBA_SurvivesRoundTrip)}.xlsm");
                File.WriteAllBytes(filename, savedData);
            }
#endif
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            Assert.NotNull(package.Workbook.VbaProject);
            Assert.Equal("Hello", package.Workbook.Worksheets["Sheet1"].Cells["A1"].Value);
            Assert.Equal("World", package.Workbook.Worksheets["Sheet2"].Cells["A1"].Value);
        }
    }

    [Fact]
    public void VbaProject_ModuleDescription_CanBeSet()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var module = package.Workbook.VbaProject.Modules.AddModule("Module1");
        module.Description = "This is a test module";

        Assert.Equal("This is a test module", module.Description);
    }

    [Fact]
    public void VbaProject_ModuleReadOnly_CanBeSet()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        var module = package.Workbook.VbaProject.Modules.AddModule("Module1");
        module.ReadOnly = true;

        Assert.True(module.ReadOnly);
    }

    [Fact]
    public void VbaProject_ProjectName_CanBeChanged()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        package.Workbook.VbaProject.Name = "MyVBAProject";

        Assert.Equal("MyVBAProject", package.Workbook.VbaProject.Name);
    }

    [Fact]
    public void VbaProject_ProjectDescription_CanBeSet()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        package.Workbook.VbaProject.Description = "My test VBA project";

        Assert.Equal("My test VBA project", package.Workbook.VbaProject.Description);
    }

    #endregion

    #region Long VBA Code

    [Fact]
    public void WriteLongVBAModule_SurvivesRoundTrip()
    {
        // Inspired by original EPPlus WriteLongVBAModule
        var sb = new StringBuilder();
        for (var i = 0; i < 100; i++)
        {
            sb.AppendLine($"Public Sub Procedure{i}()");
            sb.AppendLine($"    Dim x As Long");
            sb.AppendLine($"    x = {i}");
            sb.AppendLine($"    ' This is procedure number {i}");
            sb.AppendLine($"End Sub");
        }

        var longCode = sb.ToString();

        byte[] savedData;
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();

            var module = package.Workbook.VbaProject.Modules.AddModule("LargeModule");
            module.Code += longCode;

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            savedData = ms.ToArray();

#if SAVE_TEMP_FILES
            if (Directory.Exists(TempFolder))
            {
                var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(WriteLongVBAModule_SurvivesRoundTrip)}.xlsm");
                File.WriteAllBytes(filename, savedData);
            }
#endif
        }

        using (var package = new ExcelPackage(new MemoryStream(savedData)))
        {
            var module = package.Workbook.VbaProject.Modules["LargeModule"];
            Assert.NotNull(module);
            Assert.Contains("Procedure0", module.Code);
            Assert.Contains("Procedure99", module.Code);
        }
    }

    #endregion

    #region Enumeration and Iteration

    [Fact]
    public void Modules_CanBeEnumerated()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        package.Workbook.VbaProject.Modules.AddModule("Module1");
        package.Workbook.VbaProject.Modules.AddModule("Module2");

        var names = package.Workbook.VbaProject.Modules.Select(m => m.Name).ToList();
        Assert.Contains("Module1", names);
        Assert.Contains("Module2", names);
        Assert.Contains("ThisWorkbook", names);
    }

    [Fact]
    public void References_CanBeEnumerated()
    {
        using var package = new ExcelPackage();
        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.CreateVBAProject();

        // Add a reference so we have something to enumerate
        package.Workbook.VbaProject.References.Add(new ExcelVbaReference
        {
            Name = "Scripting",
            Libid = "*\\G{420B2830-E718-11CF-893D-00A0C9054228}#1.0#0#scrrun.dll#Microsoft Scripting Runtime"
        });

        var references = package.Workbook.VbaProject.References.ToList();
        Assert.True(references.Count > 0);
        Assert.All(references, r => Assert.False(string.IsNullOrEmpty(r.Name)));
    }

    #endregion

    #region Project Removal

    [Fact]
    public void RemoveVbaProject_ClearsModulesAndReferences()
    {
        using var ms = new MemoryStream();
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();

            package.Workbook.VbaProject.Modules.AddModule("Module1");
            package.Workbook.VbaProject.References.Add(new ExcelVbaReference
            {
                Name = "TestRef",
                Libid = "TestLibId"
            });

            package.SaveAs(ms);
        }

        ms.Position = 0;
        using (var package = new ExcelPackage(ms))
        {
            Assert.True(package.Workbook.VbaProject.Modules.Count > 0);

            package.Workbook.VbaProject.Remove();

            Assert.Equal(0, package.Workbook.VbaProject.Modules.Count);
            Assert.Equal(0, package.Workbook.VbaProject.References.Count);
        }
    }

    [Fact]
    public void RemoveVbaProject_ClearsProperties()
    {
        using var ms = new MemoryStream();
        using (var package = new ExcelPackage())
        {
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();

            package.SaveAs(ms);
        }

        ms.Position = 0;
        using (var package = new ExcelPackage(ms))
        {
            Assert.True(package.Workbook.VbaProject.Lcid != 0);

            package.Workbook.VbaProject.Remove();

            Assert.Equal(0, package.Workbook.VbaProject.Lcid);
            Assert.Equal(0, package.Workbook.VbaProject.LcidInvoke);
            Assert.Equal(0, package.Workbook.VbaProject.CodePage);
        }
    }

    #endregion
}
