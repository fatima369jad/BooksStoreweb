using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
// AD0.net stands for Active data object
//sql connection, sql command, sql datareader.Classes of ADO.net

namespace BooksStore.Models
{
    public class DBACCESS
    {
        static string constring = @"Data Source=(localdb)\MSSQLLocalDB;initial catalog=BookStore;integrated security=true";
        public SqlConnection con = new SqlConnection(constring);
        public SqlCommand cmd = null;
        public SqlDataReader sdr = null;
        public void OpenConnection()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
        }
        public void CloseConnection()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        public void IUD(String query)
        {
            int rows = 0;
            cmd = new SqlCommand(query, con);
            //cmd shows [server konsa, database konse or query konse] 
            //kise server se connect ker k database select ker k apne query likh de hai
            rows = cmd.ExecuteNonQuery();
        }
    }
}