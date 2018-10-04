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
                    roles += Role.RoleName + " ";
                }
                builder.AddField("Role: ", roles);
                foreach(Warning warn in listOfWarnings)
                {
                    builder.AddField("Warning: ", warn.WarningText);
                }
            }
            
            await Context.Channel.SendMessageAsync("", false, builder);
        }

        [Command("warning"), Summary("Warns the user with a specific message and puts a warning in the database to the current user")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task WarnUser([Remainder]string message)
        {
            string[] messageArray = message.Split(' ');
            string warningReason = "";
            User player = await User.GetUserByNickAsync(messageArray[0]);

            if(player != null)
            {
                for(int i = 1; i < messageArray.Length; i++)
                {
                    warningReason = warningReason + messageArray[i] + " ";
                }
                int result = await Warning.InsertWarningInDb(player.UserId, warningReason);
                if(result > 0)
                {
                    await Context.Channel.SendMessageAsync("Updated warnings for user " + player.Username +" successfully");
                }
            }
        }
    }
}
