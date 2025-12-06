using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace C_Sharp_ADONet
{
    public class Settings
    {
        /*  settings */
        public string command { get; set; } = string.Empty;
        public int maxRows { get; set; } = 0;
        public string sorting { get; set; } = string.Empty;
        public string searchField { get; set; } = string.Empty;
        public string searchValue { get; set; } = string.Empty;
        public bool writeLog { get; set; } = true;

        /* end settings */
        private readonly IConfiguration _configuration;

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;

            /* settings */
            command = _configuration["command"] ?? string.Empty;
            maxRows = int.Parse(_configuration["maxRows"] ?? "0");
            sorting = _configuration["sorting"] ?? string.Empty;
            searchField = _configuration["searchField"] ?? string.Empty;
            searchValue = _configuration["searchValue"] ?? string.Empty;
            /* end settings */

            /* variables - initialization */
            //List<PrinterConfiguration> test = new List<PrinterConfiguration>()
            //    { new PrinterConfiguration { IP = String.Empty },
            //        new PrinterConfiguration { IP = String.Empty}
            //};
            //PrinterConfiguration x = new PrinterConfiguration() { Name = "Canon MF240 Serices PCL6 V4", IP = "172.22.1.242", Port = 9100, web_interface_url = "portal_top.html", health_url = "", health_regex = "" };
            //List<string> y = new List<string>() { };
            //string[] z = new string[] { };
        }
    }
}