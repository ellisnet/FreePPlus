using OfficeOpenXml;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

/// <summary>
///     Saves a package and reads the raw XML parts back out of the resulting .xlsx.
/// </summary>
/// <remarks>
///     Several colour bugs round-tripped correctly against FreePPlus itself while writing values Excel would
///     reject, so tests covering them have to assert on the XML that actually lands in the package rather than
///     on a property round trip.
/// </remarks>
internal static class PackageXmlInspector
{
    /// <summary>
    ///     Saves the package and returns the named part as an XML document.
    /// </summary>
    /// <remarks>The package is closed by the save, so it must not be used afterwards.</remarks>
    internal static XmlDocument GetPart(ExcelPackage package, string partPath)
    {
        var text = GetPartText(package, partPath);
        var doc = new XmlDocument();
        doc.LoadXml(text);
        return doc;
    }

    /// <summary>
    ///     Saves the package and returns the text of the named part.
    /// </summary>
    /// <remarks>The package is closed by the save, so it must not be used afterwards.</remarks>
    internal static string GetPartText(ExcelPackage package, string partPath)
    {
        var bytes = package.GetAsByteArray();

        using var ms = new MemoryStream(bytes);
        using var zip = new ZipArchive(ms, ZipArchiveMode.Read);

        var entry = zip.GetEntry(partPath)
                    ?? zip.Entries.FirstOrDefault(e =>
                        e.FullName.Equals(partPath, System.StringComparison.OrdinalIgnoreCase));

        Assert.True(entry != null,
            $"Part '{partPath}' is not in the saved package. Parts present: {string.Join(", ", zip.Entries.Select(e => e.FullName))}");

        using var stream = entry.Open();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    ///     Builds a namespace manager carrying the prefixes the OOXML parts use.
    /// </summary>
    internal static XmlNamespaceManager NamespaceManager(XmlDocument doc)
    {
        var nsm = new XmlNamespaceManager(doc.NameTable);
        nsm.AddNamespace("c", "http://schemas.openxmlformats.org/drawingml/2006/chart");
        nsm.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
        nsm.AddNamespace("d", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        return nsm;
    }
}
