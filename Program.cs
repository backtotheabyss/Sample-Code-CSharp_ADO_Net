using C_Sharp_ADONet;
using Classes.Customers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using static DTO.DTO;

namespace CSharp_Net8_RESTful_Query
{
    public class Program
    {
        const int CONSOLEMAXLINES = 5000;

        static void Main(string[] args)
        {
            bool commandLineError = false;
            int iRet = 0;
            string tErrorMessage = "";

            ConsoleKeyInfo keyInfo;
            Console.BufferHeight = CONSOLEMAXLINES;

            /* dev */
            // args = new[] { "--customersByCountry", "--maxrows:500", "--sorting:ASC", "--searchfield:Country", "--searchvalue:USA"};
            // args = new[] { "--customersDump", "--sorting:ASC", "--maxrows:500" };
            // args = new[] { "--customersByCountry", "--maxrows:50", "--searchfield:Country", "--searchvalue:United States", "--sorting:DESC" };
            // args = new[] { "--customersByCompanyName", "--maxrows:1000", "--searchfield:CompanyName", "--searchvalue:Market" };
            
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());  // Ruta actual
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();                     // opcional
                })
                .ConfigureServices((ctx, services) =>
                {
                    // services.AddSingleton<Settings>();
                    services.AddSingleton<Settings>(sp => new Settings(sp.GetRequiredService<IConfiguration>()));
                    services.AddTransient<Customers>();
                })
                .Build();

            /* logs - folder - creation */
            /* dev */
            //string appDataFolder = Path.Combine(
            //    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            //    "Diego Sendra",
            //    "code",
            //    "C#",
            //    "Digital_Forces",
            //    "C#_Time_Sheet_Export",
            //    "Logs"
            //);

            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "C#_ADO.Net",
                "Logs"
            );

            /* settings */
            var settings = host.Services.GetRequiredService<Settings>();

            /* command line arguments - read */
            if (args.Length > 0)
            {
                // Primer argumento → comando (obligatorio)
                settings.command = args[0].Replace("-", "").Trim();

                // Resto de argumentos → opcionales, key:value
                for (int i = 1; i < args.Length; i++)
                {
                    var arg = args[i];

                    if (!arg.StartsWith("--") || !arg.Contains(":"))
                    {
                        commandLineError = true;
                        break;
                    }

                    var parts = arg.Substring(2).Split(":", 2); // divide en key y value
                    if (parts.Length != 2)
                    {
                        commandLineError = true;
                        break;
                    }

                    string key = parts[0].ToLower().Trim();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "maxrows":
                            if (int.TryParse(value, out int maxRows) && maxRows > 0)
                                settings.maxRows = maxRows;
                            else
                            {
                                commandLineError = true;
                            }
                            break;

                        case "sorting":
                            value = value.ToUpper();
                            if (value == "ASC" || value == "DESC")
                                settings.sorting = value;
                            else
                            {
                                commandLineError = true;
                            }
                            break;

                        case "searchvalue":
                            settings.searchValue = value;
                            break;

                        case "writelog":
                            if (bool.TryParse(value, out bool writeLog))
                                settings.writeLog = writeLog;
                            else
                            {
                                commandLineError = true;
                            }
                            break;

                        default:
                            commandLineError = true;
                            break;
                    }
                }
            }
            else
            {
                commandLineError = true;
            }

            if (!commandLineError && (!(string.IsNullOrEmpty(settings.command))))
            {
                /* customers */
                Customers customersObj = host.Services.GetRequiredService<Customers>();

                switch (settings.command)
                {
                    case "customersDump": { iRet = customersObj.customersDump(ref tErrorMessage, 1); break; }
                    case "customersByCountry": { iRet = customersObj.customersDump(ref tErrorMessage, 2); break; }
                    case "customersByCompanyName": { iRet = customersObj.customersDump(ref tErrorMessage, 3); break; }
                }   
            }
            else
                CommandLineUsage();

            Console.WriteLine();
            if (iRet > 0)
            {
                /* customers - error log - write */
                Console.WriteLine($"{settings.command} returned an error. Check log in \\logs subfolder.");
           }
            else
                Console.WriteLine($"{settings.command} completed succesfully!");

            /* console output - exit */
            Console.WriteLine("-------------------------");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();

            keyInfo = Console.ReadKey();
        }

        static void CommandLineUsage()
        {
            Console.WriteLine("Invalid command-line parameters. Usage:");
            Console.WriteLine();

            Console.WriteLine("REQUIRED PARAMETERS:");
            Console.WriteLine("  command (choose one of the following):");
            Console.WriteLine("    --customersDump\t\t\tRetrieves all customers from database");
            Console.WriteLine("    --customersByCountry\t\tRetrieves customers filtered by country");
            Console.WriteLine("    --customersByCompanyName\tRetrieves customers filtered by company Name");
            Console.WriteLine();

            Console.WriteLine("OPTIONAL PARAMETERS:");
            Console.WriteLine("  maxRows");
            Console.WriteLine("    --maxRows:number\tMaximum number of rows to retrieve (default from appsettings.json)");
            Console.WriteLine();

            Console.WriteLine("  sorting");
            Console.WriteLine("    --sorting:ASC or DESC\tSort criteria (default from appsettings.json)");
            Console.WriteLine();

            Console.WriteLine("  searchField");
            Console.WriteLine("    --searchField:FieldName\tField to filter by (optional, default from appsettings.json)");
            Console.WriteLine();

            Console.WriteLine("  searchValue");
            Console.WriteLine("    --searchValue:Value\tValue to filter by (optional, default from appsettings.json)");
            Console.WriteLine();

            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("  dotnet run --customersDump");
            Console.WriteLine("  dotnet run --customersByCountry --maxRows:100");
            Console.WriteLine("  dotnet run --customersByCountry --maxRows:50 --searchField:Country --searchValue:France --sorting:ASC");
            Console.WriteLine("  dotnet run --customersByCompanyName --maxRows:100 --searchField:CompanyName --searchValue:Corp");
            Console.WriteLine();
        }
    }
}
