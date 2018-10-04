using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTJBot.Models
{
    internal class User
    {
        public int UserId { get; set; }
        public int UserDiscordId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public DateTime DateJoined { get; set; }

        internal async static Task<User> GetUserByNickAsync(string nick)
        {
            string query = "SELECT * FROM [User] WHERE Nickname='" + nick + "'";
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
                {
                    //connection.Open();
                    var result = connection.Query<User>(query).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

         // New methods, checking if this works out of my lack of understanding of dapper


    }
}

