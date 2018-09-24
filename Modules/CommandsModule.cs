﻿using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INTJBot.Modules
{
    public class CommandsModule : ModuleBase
    {

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync() => ReplyAsync("pong");


        [Command("assignme"), Summary("Assigns a desired role to the user")]
        public async Task AssignDesiredRole(string role)
        {
            // TODO: Make an interchangeable list of roles that are forbidden for users to assing to themselves (e.g. moderator)
            var user = Context.User as IGuildUser;
            // Using this loop to collect the info on all roles of the server and remove all of them at once from the user
            List<IRole> roleList = new List<IRole>();
            foreach (var roleTemp in Context.Guild.Roles)
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
            if(!(role.Equals("Admin") || role.Equals("Moderator")))
            {
                var roles = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == role.ToLower());
                if (roles != null) { await user.AddRoleAsync(roles); }
            }
        }
    }
}