using System.Xml;

namespace OfficeOpenXml.Sparkline;

/// <summary>
///     Represents a single sparkline within the sparkline group
/// </summary>
public class ExcelSparkline : XmlHelper
{
    private const string _fPath = "xm:f";
    private const string _sqrefPath = "xm:sqref";

    internal ExcelSparkline(XmlNamespaceManager nsm, XmlNode topNode) : base(nsm, topNode)
    {
        SchemaNodeOrder = new[] { "f", "sqref" };
    }

    /// <summary>
    ///     The datarange
    /// </summary>
    public ExcelAddressBase RangeAddress
    {
        get => new(GetXmlNodeString(_fPath));
        internal set => SetXmlNodeString(_fPath, value.FullAddress);
    }

    /// <summary>
    ///     Location of the sparkline
    /// </summary>
    public ExcelCellAddress Cell
    {
        get => new(GetXmlNodeString(_sqrefPath));
        internal set => SetXmlNodeString("xm:sqref", value.Address);
    }

    public override string ToString()
    {
        return Cell.Address + ", " + RangeAddress.Address;
    }
}