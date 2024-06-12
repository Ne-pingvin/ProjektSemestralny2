using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ProjektSemestralny2
{
    internal class DataBase
    {
        static string connectionString = @"Data Source=LAPTOP-472C2EDF;Initial Catalog=VotesOnline;Integrated Security=True";
        SqlConnection sqlConnection = new SqlConnection(connectionString);

        public void OpenConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();
        }

        public void CloseConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
                sqlConnection.Close();
        }
        public SqlConnection GetConnection()
        {
            return sqlConnection;
        }
    }
}
