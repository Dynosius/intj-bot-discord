using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INTJBot.Modules
{
    public class UserCommands : ModuleBase
    {
        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync() => ReplyAsync("pong");


        [Command("assignme"), Summary("Assigns a desired role to the user")]
        [RequireUserPermission(GuildPermission.SendMessages)]
        [Alias("Assignme", "AssignMe", "assignMe")]
        public async Task AssignDesiredRole(string role)
        {
            // TODO: Make an interchangeable list of roles that are forbidden for users to assing to themselves (e.g. moderator)
            var user = Context.User as IGuildUser;
            var roles = Context.Guild.Roles;
            // Using this loop to collect the info on all roles of the server and remove all of them at once from the user
            List<IRole> roleList = new List<IRole>();
            foreach (var roleTemp in roles)
            {
                // This can be overridden by permissions given to the bot POSSIBLY
                if (!(roleTemp.Name.Equals("Admin") || roleTemp.Name.Equals("@everyone") || roleTemp.Name.Equals("Moderator") 
                    || roleTemp.Name.Equals("Penalty Box")))
                {
                    roleList.Add(roleTemp);
                }
            }
            await user.RemoveRolesAsync(roleList);
            // After that, add the specific role that was passed on through the command
            role = role.ToLower();
            if(!(role.Equals("admin") || role.Equals("moderator")))
            {
                var rolesIterate = roles.FirstOrDefault(x => String.Equals(x.Name, role, StringComparison.InvariantCultureIgnoreCase));
                if (rolesIterate != null) { await user.AddRoleAsync(rolesIterate); }
            }
        }
    }
}
