using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace My_telegram_bot
{
    internal class Database
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=DESKTOP-6ELFR;initial Catalog=OwnDatabase;Integrated Security=True");

        public void OpenConnection()
        {
            if(sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();
        }
        public void CloseConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
                sqlConnection.Close();
        }

        private void Query(string query)
        {
            SqlCommand sqlCommand = new SqlCommand(query,sqlConnection);
            if(sqlCommand.ExecuteNonQuery() == 0)
            {
                Console.WriteLine("Error with access to database");
            }
            CloseConnection();
        }

    }
}
