using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTJBot.Models
{
    class RolesToUsers
    {
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public async static Task<List<RolesToUsers>> GetUserRoles(int userId)
        {
            string query = "SELECT RoleName, Roles.RoleId, UserId FROM Roles INNER JOIN RolesToUsers ON Roles.RoleId = RolesToUsers.RoleId WHERE RolesToUsers.UserId=" + userId;
            using (var sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
            {
                try
                {
                    using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
                    {
                        var result = connection.Query<RolesToUsers>(query).ToList();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return null;
        }
    }
}
