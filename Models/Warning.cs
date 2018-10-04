using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace INTJBot.Models
{
    class Warning
    {
        public int UserId { get; set; }
        public int WarningId { get; set; }
        public string WarningText { get; set; }

        internal async static Task<List<Warning>> GetUserWarnings(int userId)
        {
            string query = "SELECT WarningText FROM Warning WHERE WarnedUserId =" + userId;
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
                {
                    //connection.Open();
                    var result = connection.Query<Warning>(query).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        internal async static Task<int> InsertWarningInDb(int userId, string warningMessage)
        {
            string query = "INSERT INTO Warning (WarnedUserId, WarningText) VALUES ("+ userId + ",'" +  warningMessage + "')";
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
                {
                    connection.Open();
                    SqlCommand sqlCmd = new SqlCommand(query, connection);
                    return sqlCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }
    }
}
