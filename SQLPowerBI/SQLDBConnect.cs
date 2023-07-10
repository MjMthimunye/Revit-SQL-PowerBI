using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SQLPowerBI
{
    public class SQLDBConnect
    {
        public static SqlConnection connect;

        /// <summary>
        /// Fuction to connect to SQL database
        /// </summary>
        public void ConnectDB()
        {
            //Connect to SQL database
            connect = new SqlConnection("Your SQL Connection String");
            connect.Open();
        }

        /// <summary>
        /// Function to handle SQL command queries
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public SqlCommand Query(string sqlQuery)
        {
            SqlCommand command = new SqlCommand(sqlQuery, connect);
            return command;
        }
    }
}
