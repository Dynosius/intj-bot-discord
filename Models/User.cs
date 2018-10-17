using Dapper;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace INTJBot.Models
{
    internal class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public DateTime DateJoined { get; set; }

        internal async static Task<User> GetUserByNickAsync(string nick)
        {
            string query = "SELECT * FROM [User] WHERE Nickname='" + nick + "' OR Username='" + nick + "'";
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
                {
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

        public async static Task CheckAndAddUser(SocketGuildUser user)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
            {
                string query = "SELECT Count(UserId) FROM [User] WHERE Username ='" + user.Username + "'";
                connection.Open();
                SqlCommand sqlCmd = new SqlCommand(query, connection);
                var result = (Int32)sqlCmd.ExecuteScalar();
                Console.WriteLine();
                if (result == 0)
                {
                    await InsertUserIntoDb(user);
                }
                else
                {
                    var channel = user.Guild.Channels.FirstOrDefault(x => x.Name == "general") as SocketTextChannel;
                    await channel.SendMessageAsync("User exists");
                    await ReassignRole(user);
                }
            }
        }

        private async static Task ReassignRole(SocketGuildUser user)
        {
            // This method is a bit confusing since I'm merging objects pulled from the database with the 
            // actual discord server objects, so I'm using similar names throughout

            User userObject = await GetUserByNickAsync(user.Username);
            List<RolesToUsers> dbRoles = await RolesToUsers.GetUserRoles(userObject.UserId);
            List<IRole> realRoles = new List<IRole>();
            // fill the list of roles with the user roles from the db
            foreach (RolesToUsers roles in dbRoles)
            {
                string rolename = roles.RoleName.Replace(" ", string.Empty);
                var tempRoles = user.Guild.Roles.FirstOrDefault(x => String.Equals(x.Name, rolename, StringComparison.InvariantCultureIgnoreCase));
                realRoles.Add(tempRoles);
            }
            // Then fill the IRole
            await user.AddRolesAsync(realRoles);
        }

        private async static Task InsertUserIntoDb(SocketGuildUser user)
        {
            string query = $"INSERT INTO [User] (Username, Nickname, DateJoined) VALUES ('{ user.Username }', '{ user.Nickname }', '{ user.JoinedAt }')";
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
                {
                    connection.Open();
                    SqlCommand sqlCmd = new SqlCommand(query, connection);
                    var result = sqlCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        // Updates the role of a given user inside the database. Invoked by a gateway event that is 'GuildMemberUpdated' which contains the role change information
        public async static Task UpdateRolesInDB(SocketGuildUser user)
        {
            User dbUser = await GetUserByNickAsync(user.Username);
            string query = "DELETE FROM RolesToUsers WHERE UserID=" + dbUser.UserId;        /// First remove all roles in DB
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["INTJBotDbConnection"].ConnectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand sqlCmd = new SqlCommand(query, connection, transaction);
                    var result = sqlCmd.ExecuteNonQuery();

                    var roles = user.Roles;
                    foreach (var iteratorRole in roles)
                    {
                        if (iteratorRole.Name.Length < 5)   // For the purposes of the INTJ Pub server, the only roles we're interested in are the MBTI types that only have 4 letters
                        {
                            int roleId = await RolesToUsers.GetRoleIdByName(iteratorRole.Name);
                            if (roleId > 0)
                            {
                                query = $"INSERT INTO RolesToUsers (UserId, RoleId) VALUES ({dbUser.UserId}, {roleId})";     /// Then add the sequence of roles contained in user object
                                sqlCmd = new SqlCommand(query, connection, transaction);
                                sqlCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();
                }

            }
        }
    }
}

