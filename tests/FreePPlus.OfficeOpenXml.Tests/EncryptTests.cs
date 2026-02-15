using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using Xunit;

#if SAVE_TEMP_FILES
using System;
using System.Threading;
#endif

namespace FreePPlus.OfficeOpenXml.Tests;

public class EncryptTests
{
#if SAVE_TEMP_FILES
    public const string TempFolder = @"C:\Temp";
#endif

    public const string TestPassword = "test";
    public const string SecretPassword = "secret123";
    public const string SimplePassword = "pass";
    public const string AlgorithmPassword = "algo";
    public const string WorkbookPassword = "workbook";
    public const string FilePassword = "file";
    public const string ProtectionPassword = "protect";
    public const string AgilePassword = "agile";
    public const string StandardPassword = "standard";
    public const string StreamPassword = "streampass";
    public const string ByteArrayPassword = "bytepass";
    public const string MultiSheetPassword = "multi";

    #region Encryption Property Tests

    [Fact]
    public void EncryptionDefaultAlgorithmIsAes256()
    {
        using var pck = new ExcelPackage();

        Assert.Equal(EncryptionAlgorithm.AES256, pck.Encryption.Algorithm);
    }

    [Fact]
    public void EncryptionDefaultVersionIsAgile()
    {
        using var pck = new ExcelPackage();

        Assert.Equal(EncryptionVersion.Agile, pck.Encryption.Version);
    }

    [Fact]
    public void EncryptionIsNotEncryptedByDefault()
    {
        using var pck = new ExcelPackage();

        Assert.False(pck.Encryption.IsEncrypted);
        Assert.Null(pck.Encryption.Password);
    }

    [Fact]
    public void SettingPasswordEnablesEncryption()
    {
        using var pck = new ExcelPackage();

        pck.Encryption.Password = TestPassword;

        Assert.True(pck.Encryption.IsEncrypted);
        Assert.Equal(TestPassword, pck.Encryption.Password);
    }

    [Fact]
    public void SettingPasswordToNullDisablesEncryption()
    {
        using var pck = new ExcelPackage();
        pck.Encryption.Password = TestPassword;

        pck.Encryption.Password = null;

        Assert.False(pck.Encryption.IsEncrypted);
        Assert.Null(pck.Encryption.Password);
    }

    [Fact]
    public void SettingIsEncryptedTrueCreatesEmptyPassword()
    {
        using var pck = new ExcelPackage();

        pck.Encryption.IsEncrypted = true;

        Assert.True(pck.Encryption.IsEncrypted);
        Assert.Equal("", pck.Encryption.Password);
    }

    [Fact]
    public void SettingIsEncryptedFalseClearsPassword()
    {
        using var pck = new ExcelPackage();
        pck.Encryption.Password = TestPassword;

        pck.Encryption.IsEncrypted = false;

        Assert.False(pck.Encryption.IsEncrypted);
        Assert.Null(pck.Encryption.Password);
    }

    [Fact]
    public void SettingVersionToStandardChangesAlgorithmToAes128()
    {
        using var pck = new ExcelPackage();

        pck.Encryption.Version = EncryptionVersion.Standard;

        Assert.Equal(EncryptionVersion.Standard, pck.Encryption.Version);
        Assert.Equal(EncryptionAlgorithm.AES128, pck.Encryption.Algorithm);
    }

    [Fact]
    public void SettingVersionToAgileChangesAlgorithmToAes256()
    {
        using var pck = new ExcelPackage();
        pck.Encryption.Version = EncryptionVersion.Standard;

        pck.Encryption.Version = EncryptionVersion.Agile;

        Assert.Equal(EncryptionVersion.Agile, pck.Encryption.Version);
        Assert.Equal(EncryptionAlgorithm.AES256, pck.Encryption.Algorithm);
    }

    [Fact]
    public void AlgorithmCanBeSetExplicitly()
    {
        using var pck = new ExcelPackage();

        pck.Encryption.Algorithm = EncryptionAlgorithm.AES192;

        Assert.Equal(EncryptionAlgorithm.AES192, pck.Encryption.Algorithm);
    }

    #endregion

    #region Workbook Protection Tests

    [Fact]
    public void WorkbookProtectionLockStructureCanBeSet()
    {
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Protection.LockStructure = true;

        Assert.True(pck.Workbook.Protection.LockStructure);
    }

    [Fact]
    public void WorkbookProtectionLockWindowsCanBeSet()
    {
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Protection.LockWindows = true;

        Assert.True(pck.Workbook.Protection.LockWindows);
    }

    [Fact]
    public void WorkbookProtectionLockRevisionCanBeSet()
    {
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Protection.LockRevision = true;

        Assert.True(pck.Workbook.Protection.LockRevision);
    }

    [Fact]
    public void WorkbookProtectionPasswordCanBeSet()
    {
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Protection.SetPassword(TestPassword);
        pck.Workbook.Protection.LockStructure = true;

        Assert.True(pck.Workbook.Protection.LockStructure);
    }

    [Fact]
    public void WorkbookProtectionPasswordCanBeCleared()
    {
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");

        pck.Workbook.Protection.SetPassword(TestPassword);
        pck.Workbook.Protection.SetPassword("");

        // Clearing the password should not throw
        Assert.False(pck.Workbook.Protection.LockStructure);
    }

    #endregion

    #region Encrypt and Save Tests

    [Fact]
    public async Task EncryptedWorkbookCanBeSavedToStream()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Encrypted");
        ws.Cells[1, 1].Value = "1; 1";
        ws.Cells[2, 1].Value = "2; 1";
        ws.Cells[1, 2].Value = "1; 2";
        ws.Cells[2, 2].Value = "2; 2";
        ws.Row(1).Style.Font.Bold = true;
        ws.Column(1).Style.Font.Bold = true;

        pck.Encryption.Password = TestPassword;

        var bytes = pck.GetAsByteArray();

        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(EncryptedWorkbookCanBeSavedToStream)}_{nameof(TestPassword)}.xlsx");
            await File.WriteAllBytesAsync(filename, bytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task EncryptedWorkbookCanBeOpenedWithPassword()
    {
        // Create and encrypt
        byte[] encryptedBytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Encrypted");
            ws.Cells[1, 1].Value = "Hello";
            ws.Cells[1, 2].Value = "World";

            pck.Encryption.Password = SecretPassword;
            encryptedBytes = pck.GetAsByteArray();
        }

        // Open with password
        using var ms = new MemoryStream(encryptedBytes);
        using var readMs = new MemoryStream();
        ms.CopyTo(readMs);
        readMs.Position = 0;
        using var pck2 = new ExcelPackage(readMs, SecretPassword);

        Assert.Equal(1, pck2.Workbook.Worksheets.Count);
        Assert.Equal("Hello", pck2.Workbook.Worksheets[0].Cells[1, 1].Value);
        Assert.Equal("World", pck2.Workbook.Worksheets[0].Cells[1, 2].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(EncryptedWorkbookCanBeOpenedWithPassword)}_{nameof(SecretPassword)}.xlsx");
            await File.WriteAllBytesAsync(filename, encryptedBytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task EncryptedWorkbookCanBeDecrypted()
    {
        // Create and encrypt
        byte[] encryptedBytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Encrypted");
            ws.Cells[1, 1].Value = "Decrypt me";
            pck.Encryption.Password = SimplePassword;
            encryptedBytes = pck.GetAsByteArray();
        }

        // Open encrypted, remove encryption, save again
        byte[] decryptedBytes;
        using (var ms = new MemoryStream(encryptedBytes))
        using (var readMs = new MemoryStream())
        {
            ms.CopyTo(readMs);
            readMs.Position = 0;
            using var pck2 = new ExcelPackage(readMs, SimplePassword);
            pck2.Encryption.IsEncrypted = false;
            decryptedBytes = pck2.GetAsByteArray();
        }

        // Open without password — should succeed since it's no longer encrypted
        using var ms2 = new MemoryStream(decryptedBytes);
        using var pck3 = new ExcelPackage(ms2);

        Assert.Equal("Decrypt me", pck3.Workbook.Worksheets[0].Cells[1, 1].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(EncryptedWorkbookCanBeDecrypted)}.xlsx");
            await File.WriteAllBytesAsync(filename, decryptedBytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Theory]
    [InlineData(EncryptionAlgorithm.AES128)]
    [InlineData(EncryptionAlgorithm.AES192)]
    [InlineData(EncryptionAlgorithm.AES256)]
    public async Task EncryptionAlgorithmsProduceValidPackages(EncryptionAlgorithm algorithm)
    {
        byte[] encryptedBytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("AlgoTest");
            ws.Cells[1, 1].Value = algorithm.ToString();
            pck.Encryption.Algorithm = algorithm;
            pck.Encryption.Password = AlgorithmPassword;
            encryptedBytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(encryptedBytes);
        using var readMs = new MemoryStream();
        ms.CopyTo(readMs);
        readMs.Position = 0;
        using var pck2 = new ExcelPackage(readMs, AlgorithmPassword);

        Assert.Equal(algorithm.ToString(), pck2.Workbook.Worksheets[0].Cells[1, 1].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(EncryptionAlgorithmsProduceValidPackages)}_{algorithm}_{nameof(AlgorithmPassword)}.xlsx");
            await File.WriteAllBytesAsync(filename, encryptedBytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Encrypt with Protection Tests

    [Fact]
    public async Task EncryptedWorkbookWithProtectionCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Protected");

        for (var r = 1; r <= 100; r++)
        {
            ws.Cells[r, 1].Value = r;
        }

        pck.Workbook.Protection.SetPassword(WorkbookPassword);
        pck.Workbook.Protection.LockStructure = true;
        pck.Workbook.Protection.LockWindows = true;
        pck.Encryption.Password = FilePassword;
        pck.Encryption.Algorithm = EncryptionAlgorithm.AES192;

        var bytes = pck.GetAsByteArray();

        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);

        // Verify we can open with the file password
        using var ms = new MemoryStream(bytes);
        using var readMs = new MemoryStream();
        ms.CopyTo(readMs);
        readMs.Position = 0;
        using var pck2 = new ExcelPackage(readMs, FilePassword);

        Assert.True(pck2.Workbook.Protection.LockStructure);
        Assert.True(pck2.Workbook.Protection.LockWindows);
        Assert.Equal(100, pck2.Workbook.Worksheets[0].Cells[100, 1].GetValue<int>());

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(EncryptedWorkbookWithProtectionCanBeCreated)}_{nameof(FilePassword)}.xlsx");
            await File.WriteAllBytesAsync(filename, bytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task WorkbookProtectionWithoutEncryptionCanBeSaved()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("ProtectedOnly");
        ws.Cells[1, 1].Value = "Protected";

        pck.Workbook.Protection.SetPassword(ProtectionPassword);
        pck.Workbook.Protection.LockStructure = true;

        var bytes = pck.GetAsByteArray();

        // Open without any password (file is not encrypted, only workbook-protected)
        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);

        Assert.True(pck2.Workbook.Protection.LockStructure);
        Assert.Equal("Protected", pck2.Workbook.Worksheets[0].Cells[1, 1].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(WorkbookProtectionWithoutEncryptionCanBeSaved)}.xlsx");
            await File.WriteAllBytesAsync(filename, bytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Encryption Version Tests

    [Fact]
    public async Task AgileEncryptionCanBeRoundTripped()
    {
        byte[] encryptedBytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Agile");
            ws.Cells[1, 1].Value = "Agile Encryption";
            pck.Encryption.Version = EncryptionVersion.Agile;
            pck.Encryption.Password = AgilePassword;
            encryptedBytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(encryptedBytes);
        using var readMs = new MemoryStream();
        ms.CopyTo(readMs);
        readMs.Position = 0;
        using var pck2 = new ExcelPackage(readMs, AgilePassword);

        Assert.Equal("Agile Encryption", pck2.Workbook.Worksheets[0].Cells[1, 1].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(AgileEncryptionCanBeRoundTripped)}_{nameof(AgilePassword)}.xlsx");
            await File.WriteAllBytesAsync(filename, encryptedBytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task StandardEncryptionCanBeRoundTripped()
    {
        byte[] encryptedBytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Standard");
            ws.Cells[1, 1].Value = "Standard Encryption";
            pck.Encryption.Version = EncryptionVersion.Standard;
            pck.Encryption.Password = StandardPassword;
            encryptedBytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(encryptedBytes);
        using var readMs = new MemoryStream();
        ms.CopyTo(readMs);
        readMs.Position = 0;
        using var pck2 = new ExcelPackage(readMs, StandardPassword);

        Assert.Equal("Standard Encryption", pck2.Workbook.Worksheets[0].Cells[1, 1].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(StandardEncryptionCanBeRoundTripped)}_{nameof(StandardPassword)}.xlsx");
            await File.WriteAllBytesAsync(filename, encryptedBytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region SaveAs with Password Tests

    [Fact]
    public async Task SaveAsStreamWithPasswordEncryptsPackage()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("StreamPassword");
        ws.Cells[1, 1].Value = "Stream encrypted";

        using var outputStream = new MemoryStream();
        pck.SaveAs(outputStream, StreamPassword);

        Assert.True(outputStream.Length > 0);

        // Verify we can read it back with the password
        outputStream.Position = 0;
        using var pck2 = new ExcelPackage(outputStream, StreamPassword);

        Assert.Equal("Stream encrypted", pck2.Workbook.Worksheets[0].Cells[1, 1].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(SaveAsStreamWithPasswordEncryptsPackage)}_{nameof(StreamPassword)}.xlsx");
            outputStream.Position = 0;
            await using var fs = new FileStream(filename, FileMode.Create);
            await outputStream.CopyToAsync(fs, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task GetAsByteArrayWithPasswordEncryptsPackage()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("ByteArrayPassword");
        ws.Cells[1, 1].Value = "Byte array encrypted";

        var bytes = pck.GetAsByteArray(ByteArrayPassword);

        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);

        // Verify we can read it back with the password
        using var ms = new MemoryStream(bytes);
        using var readMs = new MemoryStream();
        ms.CopyTo(readMs);
        readMs.Position = 0;
        using var pck2 = new ExcelPackage(readMs, ByteArrayPassword);

        Assert.Equal("Byte array encrypted", pck2.Workbook.Worksheets[0].Cells[1, 1].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(GetAsByteArrayWithPasswordEncryptsPackage)}_{nameof(ByteArrayPassword)}.xlsx");
            await File.WriteAllBytesAsync(filename, bytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Multi-Worksheet Encrypted Tests

    [Fact]
    public async Task EncryptedWorkbookWithMultipleSheetsRoundTrips()
    {
        byte[] encryptedBytes;
        using (var pck = new ExcelPackage())
        {
            for (var i = 1; i <= 3; i++)
            {
                var ws = pck.Workbook.Worksheets.Add($"Sheet{i}");
                for (var r = 1; r < 50; r++)
                {
                    ws.Cells[r, 1].Value = r * i;
                }
            }

            pck.Encryption.Password = MultiSheetPassword;
            encryptedBytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(encryptedBytes);
        using var readMs = new MemoryStream();
        ms.CopyTo(readMs);
        readMs.Position = 0;
        using var pck2 = new ExcelPackage(readMs, MultiSheetPassword);

        Assert.Equal(3, pck2.Workbook.Worksheets.Count);
        Assert.Equal("Sheet1", pck2.Workbook.Worksheets[0].Name);
        Assert.Equal("Sheet2", pck2.Workbook.Worksheets[1].Name);
        Assert.Equal("Sheet3", pck2.Workbook.Worksheets[2].Name);
        Assert.Equal(1, pck2.Workbook.Worksheets[0].Cells[1, 1].GetValue<int>());
        Assert.Equal(98, pck2.Workbook.Worksheets[1].Cells[49, 1].GetValue<int>());

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(EncryptedWorkbookWithMultipleSheetsRoundTrips)}_{nameof(MultiSheetPassword)}.xlsx");
            await File.WriteAllBytesAsync(filename, encryptedBytes, CancellationToken.None);
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion
}
