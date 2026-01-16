using C_Sharp_ADONet;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static DTO.DTO;

namespace Classes.Customers
{
    public class Customers
    {        
        private readonly Settings _settings;
        private readonly IConfiguration _configuration;
        public List<string> lstCustomersLog = new List<string>();

        public Customers(Settings settings, IConfiguration configuration)
        {
            _configuration = configuration;
            _settings = settings;
        }
        public int customersDump(ref string tErrorMessage, int command)
        {
            int iret = 0;

            Response<Customer> customers = customersRetrieve();

            if (!customers.Success)
            {
                tErrorMessage = customers.ErrorMessage;
                return 1;
            }
            else if (customers.Success && (customers.Results == null || customers.Results.Count == 0))
            {
                tErrorMessage = customers.ErrorMessage;
                return 2;
            }
            else
            {
                Console.WriteLine("Connection to SCRAPING stablished ...");
                Console.WriteLine();

                switch (command)
                {
                    case 2:
                        {
                            /* customers - filter - by country */
                            var filtered = customers.Results
                            .Where(c => !string.IsNullOrEmpty(c.Country) &&
                                        string.Equals(c.Country, _settings.searchValue, StringComparison.OrdinalIgnoreCase))                                
                            .ToList();

                            if (filtered.Count > 0)
                            {
                                Console.WriteLine(_settings.searchValue);
                                Console.WriteLine("-------------");
                            }

                            customers.Results = filtered;

                            break;
                        }

                    case 3:
                        {
                            /* customers - filter - by company name */
                            var filtered = customers.Results
                            .Where(c => !string.IsNullOrEmpty(c.CompanyName) &&
                                c.CompanyName.IndexOf(_settings.searchValue, StringComparison.OrdinalIgnoreCase) >= 0)

                            .ToList();

                            if (filtered.Count > 0)
                            {
                                Console.WriteLine(_settings.searchValue);
                                Console.WriteLine("-------------");
                            }

                            customers.Results = filtered;
                            
                            break;
                        }
                }

                /* customers - sorting */
                if (!string.IsNullOrEmpty(_settings.sorting))
                {
                    switch (_settings.sorting.ToLower())
                    {
                        case "asc":
                            {
                                switch (command)
                                {
                                    case 2: { customers.Results = customers.Results.OrderBy(c => c.Country).ToList(); break; }                                    
                                    default: { customers.Results = customers.Results.OrderBy(c => c.CompanyName).ToList(); break; } 
                                }
                                
                                break;
                            }

                        case "desc":
                            {
                                switch (command)
                                {
                                    case 2: { customers.Results = customers.Results.OrderByDescending(c => c.Country).ToList(); break; }
                                    default: { customers.Results = customers.Results.OrderByDescending(c => c.CompanyName).ToList(); break; }
                                }
                                break;
                            }
                    }
                }

                List<string> lstCustomersLog = new List<string>();
                
                /* customers - log - headers */
                string header = "CustomerId,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax";

                lstCustomersLog.Add(header);                

                /* customers - log */
                foreach (var customer in customers.Results)
                {
                    /* customer - item */
                    string customerItem =
                        $"{customer.CustomerId}," +
                        $"{customer.CompanyName?.Replace(",", " ")}," +
                        $"{customer.ContactName?.Replace(",", " ")}," +
                        $"{customer.ContactTitle?.Replace(",", " ")}," +
                        $"{customer.Address?.Replace(",", " ")}," +
                        $"{customer.City?.Replace(",", " ")}," +
                        $"{customer.Region?.Replace(",", " ")}," +
                        $"{customer.PostalCode?.Replace(",", " ")}," +
                        $"{customer.Country?.Replace(",", " ")}," +
                        $"{customer.Phone?.Replace(",", " ")}," +
                        $"{customer.Fax?.Replace(",", " ")}";

                    /* customers - log - item */
                    lstCustomersLog.Add(customerItem);
                }

                /* customers - log - output */
                foreach (var line in lstCustomersLog)
                {
                    string lineWithTabs = line.Replace(",", "\t");
                    Console.WriteLine(lineWithTabs);
                }

                /* customers - log - output - CSV */
                if (_settings.writeLog)
                {
                    /* dev */
                    //string appDataFolder = Path.Combine(
                    //    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
- Program.cs:                     //    "Diego Sendra",
                    //    "code",
                    //    "C#",
                    //    "ALTOUR",
                    //    "C#_ADO.Net",
                    //    "Logs"
                    //);

                    string appDataFolder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "C#_ADO_Net",
                        "Logs"
                    );

                    string filePath = Path.Combine(appDataFolder, $"{_settings.command}-{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}-log.csv");
                    File.WriteAllLines(filePath, lstCustomersLog, Encoding.UTF8);
                }

                return iret;
            }
        }
        public Response<Customer> customersRetrieve()
        {            
            ADONet objADONet = new ADONet(_configuration);

            /* connection - open */
            objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);            
            SqlCommand commandSQL = new SqlCommand("select TOP " + _settings.maxRows + " CustomerID, CompanyName, ContactName, ContactTitle, Country from Customers", objADONet.connectionSQLServer);
            SqlDataReader commandSQLReader = commandSQL.ExecuteReader();
            List<Customer> customersList = new List<Customer>();

            int icustomers = 0;
            if (commandSQLReader.HasRows)
            {
                while (commandSQLReader.Read()) // && icustomers < _settings.maxRows)
                {                    
                    var Customer = new Customer
                    {
                        CustomerId = commandSQLReader["CustomerID"].ToString() ?? "",
                        CompanyName = commandSQLReader["CompanyName"].ToString() ?? "",
                        ContactName = commandSQLReader["ContactName"].ToString() ?? "",
                        ContactTitle = commandSQLReader["ContactTitle"].ToString() ?? "",
                        Country = commandSQLReader["Country"].ToString() ?? ""
                    };

                    customersList.Add(Customer);

                    icustomers++;
                }

                commandSQLReader.Close();
                commandSQLReader = null;

                return new Response<Customer>
                {
                    Success = true,
                    Results = customersList
                };
            }
            else
            {
                commandSQLReader.Close();
                commandSQLReader = null;

                return new Response<Customer>
                {
                    Success = false,
                    ErrorMessage = "No customers have been found."
                };
            }
        }        
    }
}
