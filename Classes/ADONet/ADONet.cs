using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using static DTO.DTO;
using Classes.Customers;
using Microsoft.Data.SqlClient;

namespace C_Sharp_ADONet
{
    public class ADONet
    {
        private readonly IConfiguration _configuration;

        /* connection - PostgreSQL */
        // public NpgsqlConnection connectionPostgreSQL;

        /* connection - Acess 97/2000 */
        // public OLEDBConnection connectionAccess;

        /* connection - SQL Server */
        public SqlConnection connectionSQLServer;

        /* connection - MySQL */
        // public MySqlConnection connectionMySQL;

        public ADONet(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection connectionOpen(SqlConnection pSQLServerConnection, int piconnectionType)
        {
            //connection - open
            switch (piconnectionType)
            {
                case 2:
                    {
                        /* connection - SQL Server - open */
                        pSQLServerConnection = new SqlConnection(_configuration.GetConnectionString("connectionStringSQLServer") /* connectionStringSQLServer */);
                        pSQLServerConnection.Open();
                        break;
                    }
            }
            
            return (pSQLServerConnection);
        }
        //public MySqlConnection connectionOpen(MySqlConnection pConnection, int piconnectionType)
        //{
        //    //connection - open
        //    switch (piconnectionType)
        //    {
        //        case 1:
        //            {
        //                /* connection - MYSQL - open */
        //                pConnection = new MySqlConnection(_configuration.GetConnectionString("connectionStringMySQL"));
        //                pConnection.Open();
        //                break;
        //            }
        //    }

        //    return (pConnection);
        //}

        //public OleDbConnection connectionOpen(SqlConnection pConnection, int piconnectionType)
        //{
        //    //connection - open
        //    switch (piconnectionType)
        //    {

        //        case 3:
        //            {
        //                /* connection - Access 97/2000 - open */
        //                pConnection = new OleDbConnection((_configuration.GetConnectionString("connectionStringAccess");                        
        //                pConnection.Open();
        //                break;
        //            }
        //    }

        //    return (pConnection);
        //}

        //public NpgsqlConnection connectionOpen(NpgsqlConnection pConnection, int piconnectionType)
        //{
        //    //connection - open
        //    switch (piconnectionType)
        //    {
        //        case 4:
        //            {
        //                /* connection - Postgre SQL - open */
        //                pConnection = new NpgsqlConnection(_configuration.GetConnectionString("connectionStringPostgreSQL"));
        //                pConnection.Open();
        //                break;
        //            }
        //    }

        //    return (pConnection);
        //}
    }
}
