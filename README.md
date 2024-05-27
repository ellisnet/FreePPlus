# FreePPlus
Create advanced Excel spreadsheets using .NET, without the need of interop.

FreePPlus is a .NET library that reads and writes Excel files using the Office Open XML format (xlsx). 
FreePPlus has no dependencies other than .NET.

FreePPlus is provided as a .NET 8 library and associated `FreePPlus.LgplLicenseForever` NuGet package.
FreePPlus supports applications and assemblies that target Microsoft .NET version 8.0 and later.

Microsoft .NET version 8.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 14, 
2023; and will be actively supported by Microsoft until Nov 10, 2026. Please update your C#/.NET code 
and projects to a current LTS version of Microsoft .NET.

FreePPlus is a fork of the code of the popular EPPlus library version 4.5.3.3 - see below for licensing
details. This project also incorporates code from the SixLabors.ImageSharp project (v2.1.3) and the 
SixLabors.Fonts project (v1.0.0) - licensing details about these code inclusions are provided below.
 
## FreePPlus supports:
* Cell Ranges 
* Cell styling (Border, Color, Fill, Font, Number, Alignments) 
* Data validation 
* Conditional formatting 
* Charts 
* Pictures 
* Shapes 
* Comments 
* Tables 
* Pivot tables 
* Protection 
* Encryption 
* VBA 
* Formula calculation 
* Many more... 

## License
The project is licensed under the GNU Lesser General Public License (LGPL) version 3.
see: https://en.wikipedia.org/wiki/GNU_Lesser_General_Public_License

All code from EPPlus version 4.5.3.3 was licensed under the GNU Lesser General Public License (LGPL) 
version 3 - as of Jan 30, 2020.  This project (FreePPlus) complies with all provisions of the open 
source license of EPPlus version 4.5.3.3 (code) - including making all modified, adapted and derived 
code freely available as open source, under the same license as the EPPlus code license.

All code originating from SixLabors.ImageSharp was included as allowed by the Apache License 2.0 
permissible open source software license - as of Jun 19, 2022. This project (FreePPlus) complies with 
all provisions of the source code license of SixLabors.ImageSharp v2.1.3 (Apache License 2.0).

All code originating from SixLabors.Fonts was included as allowed by the Apache License 2.0 
permissible open source software license - as of Jul 22, 2022. This project (FreePPlus) complies with 
all provisions of the source code license of SixLabors.Fonts v1.0.0 (Apache License 2.0).