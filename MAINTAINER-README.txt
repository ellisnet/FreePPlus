================================================================================
MAINTAINER-README: FreePPlus
Notes for people and agents MAINTAINING this repository — not for package
consumers
================================================================================

If you are CONSUMING the NuGet package, read AGENT-README.txt instead. This file
covers building, testing, packaging and the internal conventions of the
repository itself.

PURPOSE AND SCOPE
=================
The repository produces exactly one NuGet package:

    PackageId   FreePPlus.LgplLicenseForever
    Project     src/FreePPlus.OfficeOpenXml/FreePPlus.OfficeOpenXml.csproj
    Assembly    FreePPlus.OfficeOpenXml.dll
    RootNamespace  OfficeOpenXml
    License     LGPL-3.0-or-later (PackageRequireLicenseAcceptance is true)
    Consumer documentation  AGENT-README.txt (repo root)

FreePPlus reads and writes Office Open XML spreadsheets without Excel or COM
interop. The library targets net10.0 only.

REPOSITORY LAYOUT
=================
    AGENT-README.txt        Consumer documentation for the package; also packed
                            into the .nupkg.
    MAINTAINER-README.txt   This file.
    EXTRAS-README.txt       The sample application and non-package content.
    README-INDEX.txt        Map of the README files.
    README.md               Human-facing overview; shown on GitHub and
                            nuget.org.
    LICENSE                 LGPL v3.
    THIRD-PARTY-NOTICES.txt Notices for EPPlus, DotNetZip, jzlib and zlib.
    icon-codebrix-128.png   Package icon.
    FreePPlus.sln           Solution: library, tests, sample app, plus a
                            "Solution Items" folder carrying the root files.
    nuspec/                 A hand-written .nuspec kept from an earlier
                            packaging approach, with placeholder docs/ and
                            lib/ folders. It is NOT used by the current build,
                            which packs from the csproj
                            (GeneratePackageOnBuild). Treat it as historical.
    src/FreePPlus.OfficeOpenXml/    The library.
    tests/FreePPlus.OfficeOpenXml.Tests/    xUnit v3 test project.
    samples/FreePPlus.OfficeOpenXml.SampleApp/   Console sample app (see
                                                 EXTRAS-README.txt).

Source folders inside src/FreePPlus.OfficeOpenXml, each matching a namespace
under OfficeOpenXml:

    (root)              ExcelPackage, ExcelWorkbook, ExcelWorksheet(s),
                        ExcelRange/ExcelRangeBase, ExcelAddress, ExcelStyles,
                        ExcelTextFormat, ExcelHyperLink, ExcelEncryption,
                        ExcelPrinterSettings, ExcelHeaderFooter, protection,
                        comments, named ranges and the cell store.
    Compatibility/      CompatibilitySettings.
    ConditionalFormatting/  Collection, enums, helpers; Contracts/ holds the
                        public rule interfaces and Rules/ the ~55 concrete rule
                        classes.
    DataValidation/     Validation types; Contracts/ and Formulas/ mirror the
                        same split.
    Drawing/            Drawings, pictures, shapes, borders/fills; Chart/ holds
                        every chart class; Vml/ the legacy VML drawings used by
                        comments and header images.
    Encryption/         Standard and agile encryption handlers.
    FormulaParsing/     The whole calculation engine: lexer, expression graph,
                        dependency chain, ~200 function classes under
                        Excel/Functions, logging.
    Packaging/          The OOXML zip package abstraction, plus a vendored
                        DotNetZip under Packaging/DotNetZip.
    Sparkline/          Sparkline groups and colors.
    Style/              Cell styles; Dxf/ differential styles;
                        XmlAccess/ the XML-backed style records.
    Table/              Tables and table columns; PivotTable/ pivot tables.
    Utils/              Address helpers, argument validation;
                        CompundDocument/ (sic) the OLE compound-file reader
                        used for VBA and legacy encryption.
    VBA/                VBA project, modules, references, protection,
                        signatures.

NAMESPACE NOTE: five drawing enums (eShapeStyle, eTextAlignment, eFillStyle in
Drawing/ExcelShape.cs and eEndStyle, eEndSize in Drawing/ExcelDrawingLineEnd.cs)
are declared BEFORE the namespace statement in their files and therefore live in
the global namespace. Moving them into OfficeOpenXml.Drawing would be a breaking
change for consumers, so leave them where they are and keep the quirk documented
in AGENT-README.txt.

BUILDING
========
    dotnet restore FreePPlus.sln
    dotnet build FreePPlus.sln -c Release

The library targets net10.0 exclusively. GeneratePackageOnBuild is true, so
every build of the library project also produces a .nupkg.

Package assets copied into the .nupkg by the csproj:
    icon-codebrix-128.png, README.md, AGENT-README.txt, THIRD-PARTY-NOTICES.txt

If you rename or move AGENT-README.txt you must update the <None Include=...>
item in src/FreePPlus.OfficeOpenXml/FreePPlus.OfficeOpenXml.csproj or the pack
will fail.

TESTING
=======
    dotnet test tests/FreePPlus.OfficeOpenXml.Tests/FreePPlus.OfficeOpenXml.Tests.csproj

Framework: xUnit v3 with xunit.runner.visualstudio and Microsoft.NET.Test.Sdk;
assertions use SilverAssertions in some files and plain Assert in others.

Test data: three images (test-image-01.bmp/.jpg/.png) under
tests/FreePPlus.OfficeOpenXml.Tests/SampleFiles are compiled in as
EmbeddedResource; the csproj also removes them from the None item group so they
are not copied twice.

Optional artefacts: many tests are wrapped in "#if SAVE_TEMP_FILES" and write
the workbook they build to a temp folder for manual inspection. The constant is
NOT defined anywhere in the repository; define it yourself
(-p:DefineConstants=SAVE_TEMP_FILES) when you want the .xlsx files on disk. The
guarded blocks target a hard-coded Windows path (C:\Temp), so on Linux they are
skipped by the Directory.Exists check even when the constant is on.

A few tests in ConditionalFormattingTests are commented out because they need
sample workbooks (cf.xlsx, CofCTemplate.xlsx) that are not in the repository.
Leave them commented; do not "fix" them by inventing data files.

PACKAGING AND PUBLISHING
========================
Packing is driven by the library csproj, not by nuspec/. Versioning is the
date-stamped, auto-incrementing scheme shared across the CodeBrix family:

    1.<years since _VersionBaseYear>.<UTC day of year>.<UTC minute of day>

Every field is derived from DateTime.UtcNow, so the version strictly increases
over time and every build produces a new version. Two builds inside the same
UTC minute produce the SAME version — never publish twice within one minute. To
re-baseline the minor number, change the _VersionBaseYear property. The full
rationale is written out in a comment block at the top of the csproj; keep that
comment in sync with any change.

Because the version moves with the clock, no version number belongs in
AGENT-README.txt, README.md or any other documentation file -- not even the
upstream release this project forked from. That belongs in
THIRD-PARTY-NOTICES.txt, and the provenance statements elsewhere point there.

The nuspec/ folder is stale: it pins an old version, targets net8.0 and
references a FreePPlus.Imaging.dll that no longer exists. Do not resurrect it
without reconciling it with the csproj metadata.

PROVENANCE AND VENDORED SOURCES
===============================
FreePPlus is a fork of EPPlus 4.5.3.3 — the last release published under the
LGPL, before that project moved to a commercial Polyform Noncommercial license
at version 5. The original repository (JanKallman/EPPlus) was archived in March
2020. FreePPlus keeps the upstream OfficeOpenXml namespace so that consumer code
ports with only a package swap.

Consequences for maintenance:
  -> Upstream file headers ("You may amend and distribute as you like, but
     don't remove this header!", Copyright (C) 2011 Jan Källman, and the
     per-file "Code change notes" tables) MUST stay in the files that carry
     them.
  -> Newer upstream releases are NOT a valid source of code or of API
     documentation for this repository; their license does not permit it, and
     their API has diverged. Verify every documented member against the source
     in this repository.
  -> System.Drawing was replaced by CodeBrix.Imaging throughout; Color, Image
     and Font in the public API are CodeBrix.Imaging types. Keep it that way —
     it is what makes the library work identically on Linux and macOS.

Vendored third-party code, all recorded in THIRD-PARTY-NOTICES.txt:
  -> DotNetZip (Ionic.Zip / Ionic.Zlib), Ms-PL, under
     src/FreePPlus.OfficeOpenXml/Packaging/DotNetZip. Note that a few of these
     files declare "namespace Ionic.Zip" rather than
     OfficeOpenXml.Packaging.Ionic.Zip.
  -> jzlib (BSD 3-Clause) and zlib (zlib License), incorporated through the
     DotNetZip inflate/deflate code.
Edit vendored files in place when a fix is needed, and note the change in the
file's existing change-notes header.

CODING CONVENTIONS
==================
  -> net10.0 only. Do not add other target frameworks.
  -> Nullable reference types are OFF; do not add "?" annotations to reference
     types.
  -> Public API changes are consumer-visible. Adding a member is fine; renaming
     or removing one breaks the "swap the package and recompile" promise that
     is the whole point of the fork.
  -> Keep the upstream member names, including the historical misspellings that
     are part of the public surface (ExcelNamedRangeCollection.AddFormla,
     ExcelDrawingFill.Transparancy, ExcelPivotTable.ColumGrandTotals, the
     Utils/CompundDocument folder). They are documented, not defects.
  -> Several files are UTF-8 with a BOM, inherited from upstream. Preserve the
     encoding of a file you edit.
  -> Some enums (eSubTotalFunctions, eDateGroupBy) are flags enums with
     explicit values; do not renumber them.

NOTES
=====
  -> ExcelWorksheet.SetFormula(int, int, object) is INTERNAL and is used by the
     table totals-row code. It is not, and should not become, part of the
     consumer API — consumers set formulas through ExcelRangeBase.Formula. An
     earlier revision of AGENT-README.txt documented it as public; that has
     been corrected.
  -> ExcelPackage reads an optional appsettings.json at construction time
     (through Microsoft.Extensions.Configuration) and honours both
     "FreePPlus:ExcelPackage:Compatibility:IsWorksheets1Based" and the legacy
     "EPPlus:..." key. That is why the three Microsoft.Extensions.Configuration
     packages are dependencies.
  -> Pivot tables are written with an EMPTY pivot cache marked refreshOnLoad;
     Excel computes the values. Do not file that as a bug.
  -> ExcelTable.ShowTotal = true on a table created through ws.Tables.Add()
     produces a workbook Excel reports as corrupt. The behaviour is documented
     as a pitfall for consumers; if it is ever fixed, remove the pitfall from
     AGENT-README.txt.
  -> AutoFitColumns and ExcelFont.SetFromFont depend on real font metrics. On
     Linux build agents, install a font family with bold and italic faces
     (fonts-dejavu) before running the font-sensitive tests.
