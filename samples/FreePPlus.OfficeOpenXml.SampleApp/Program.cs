using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FreePPlus.OfficeOpenXml.SampleApp;

internal class Program
{
    //These hard-coded constants may not work for everyone; but can be modified as needed
    public const string TempFolder = @"C:\Temp";
    public const string AdvWorksConnectString = "Server=localhost;Database=AdventureWorksLT2022;Integrated Security=true;TrustServerCertificate=true";

    public static int[] ValidSamples = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];

    private static void RunSamples(IList<int> samples)
    {
        ArgumentNullException.ThrowIfNull(samples, nameof(samples));

        if (samples.Count < 1)
        {
            Console.WriteLine("\nNo samples to run!");
        }
        else
        {
            Console.WriteLine($"\nYou have selected to run samples: {string.Join(", ", samples)}");

            var tempFolder = Path.Combine(TempFolder, $"FreePPlus_Samples_{DateTime.Now.Ticks}");
            Utils.OutputDir = Directory.CreateDirectory(tempFolder);

            Console.WriteLine($"\nFiles that are created during the running of these samples will be stored in this folder: {tempFolder}");

            var executed = 0;

            foreach (var sample in samples)
            {
                try
                {
                    Console.WriteLine($"\nAttempting to run Sample # {sample}...");
                    switch (sample)
                    {
                        case 1:
                            Sample1.RunSample1();
                            executed++;
                            break;

                        case 2:
                            var s2FilePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Sample2.xlsx");
                            Sample2.RunSample2(s2FilePath);
                            executed++;
                            break;

                        case 3:
                            Sample3.RunSample3(AdvWorksConnectString);
                            executed++;
                            break;

                        case 4:
                            var s4FilePath = Path.Combine(AppContext.BaseDirectory, "Resources", "GraphTemplate.xlsx");
                            Sample4.RunSample4(AdvWorksConnectString, new FileInfo(s4FilePath));
                            executed++;
                            break;

                        case 5:
                            Sample5.RunSample5();
                            executed++;
                            break;

                        case 6:
                            Sample6.RunSample6(new DirectoryInfo(AppContext.BaseDirectory), depth: 2, skipIcons: false);
                            executed++;
                            break;

                        case 7:
                            Sample7.RunSample7(rows: 200); //Needs to be a number greater than 100
                            executed++;
                            break;

                        case 8:
                            LinqSample.RunLinqSample();
                            executed++;
                            break;

                        case 9:
                            Sample9.RunSample9();
                            executed++;
                            break;

                        case 10:
                            Sample10.RunSample10();
                            executed++;
                            break;

                        case 11:
                            Sample11.RunSample11();
                            executed++;
                            break;

                        case 12:
                            Sample12.RunSample12(AdvWorksConnectString);
                            executed++;
                            break;

                        case 13:
                            Sample13.RunSample13();
                            executed++;
                            break;

                        case 14:
                            Sample14.RunSample14();
                            executed++;
                            break;

                        case 15:
                            Sample15.VBASample();
                            executed++;
                            break;

                        case 16:
                            Sample16.RunSample16();
                            executed++;
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nSample # {sample} caused an exception to be thrown - {e.Message}");
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine("\nDone running samples.");
            if (executed > 0)
            {
                Console.WriteLine($"It seems like {executed} sample{(executed > 1 ? "s" : "")} might have executed without problems.");
            }
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine($"This application is hard-coded to try to use this directory as its output folder: {TempFolder}");

        if (!Directory.Exists(TempFolder))
        {
            throw new DirectoryNotFoundException($"Cannot find the folder: {TempFolder}");
        }

        Console.WriteLine("Looking for samples to run...");

        if (args?.Any(a => (!string.IsNullOrWhiteSpace(a)) && a.Trim().StartsWith("--run:", StringComparison.InvariantCultureIgnoreCase)) ?? false)
        {
            var samplesToRun = new HashSet<int>();

            var option = args.First(f => 
                (!string.IsNullOrWhiteSpace(f)) 
                && f.Trim().StartsWith("--run:", StringComparison.InvariantCultureIgnoreCase));

            var parts = option
                .Trim()
                .ToLowerInvariant()
                .Split(':')
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Select(s => s.Trim())
                .ToArray();

            if (parts.Length > 1)
            {
                foreach (var selected in parts[1].Split(',').Where(w => !string.IsNullOrWhiteSpace(w)))
                {
                    if (int.TryParse(selected.Trim(), out var parsed))
                    {
                        if (ValidSamples.Contains(parsed))
                        {
                            samplesToRun.Add(parsed);
                        }
                        else
                        {
                            Console.WriteLine($"The value '{parsed}' does not match a known sample.");
                        }
                    }
                }
            }

            RunSamples(samplesToRun.ToArray());
        }
        else
        {
            Console.WriteLine("Use a command line option like this to run samples: '--run:1,4,7' - where 1, 4, and 7 are the samples you want to run.");
        }
    }
}
