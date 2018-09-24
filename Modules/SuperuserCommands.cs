using Discord;
using Discord.Commands;
using INTJBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTJBot.Modules
{
    //[Name("Superuser"), Summary("Admin or moderator module. Used for managing the server.")]
    public class SuperuserCommands : ModuleBase
    {
        [Command("user"), Summary("Lists information about the requested user")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task ListUserInfo(string nick)
        {
            User player = await User.GetUserByNickAsync(nick);
            List<RolesToUsers> roleToUser = await RolesToUsers.GetUserRoles(player.UserId);
            List<Warning> listOfWarnings = await Warning.GetUserWarnings(player.UserId);
            var builder = new EmbedBuilder();
            builder.WithTitle("User information");
            string roles = "";
            if (player != null)
            {
                builder.Color = Color.Red;
                builder.AddInlineField("Name", player.Nickname);
                builder.AddInlineField("Date joined:", player.DateJoined);
                builder.AddInlineField("Username", player.Username);
                foreach(RolesToUsers Role in roleToUser)
                {
                    roles += Role.RoleName + ", ";
                }
                builder.AddField("Role: ", roles);
                foreach(Warning warn in listOfWarnings)
                {
                    builder.AddField("Warning: ", warn.WarningText);
                }
            }
            
            await Context.Channel.SendMessageAsync("", false, builder);
        }
    }
}
