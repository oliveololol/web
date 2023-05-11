using Microsoft.AspNetCore.Http;
using System;
using System.Data.SqlClient;
using web.Controllers;

namespace web.function
{
    public class parols
    {
        private void Check(HttpContext httpContext, string login, string parol)
        {
            // Create a SqlConnection object with the appropriate connection string
            SqlConnection connection = new SqlConnection("userDB");

            // Open the connection
            connection.Open();

            // Create the SQL query with a parameter for the login
            string query = "SELECT parol FROM users WHERE log=@login";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@login", login);

            // Execute the query and read the result
            SqlDataReader reader = command.ExecuteReader();
            string password = null;
            if (reader.Read())
            {
                password = reader.GetString(0);
            }
            reader.Close();

            // Check if the password matches
            if (string.IsNullOrEmpty(password))
            {
                // If user with entered login doesn't exist
                Console.WriteLine("login or pass is incorect");
            }
            else if (password == parol)
            {
                // If passwords match, user is authenticated
                Console.WriteLine("");

                // Set session variables
                httpContext.Session.SetString("login", login);

            }
            else
            {
                // If passwords don't match
                Console.WriteLine("login or pass is incorect");
            }

            // Clean up
            connection.Close();
        }
    }
}
