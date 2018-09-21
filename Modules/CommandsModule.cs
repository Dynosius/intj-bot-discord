using Discord;
using Discord.Commands;
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
            var user = Context.User as IGuildUser;
            var roles = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == role.ToLower());
            // Iterate through the list of all roles on the server and remove each one from the user
            foreach (var roleOne in Context.Guild.Roles)
            {
                // Skip admin and @everyone because obvious reasons
                if (!(roleOne.Name.Equals("Admin") || roleOne.Name.Equals("@everyone"))) { await user.RemoveRoleAsync(roleOne); }
            }
            await user.AddRoleAsync(roles);
        }
    }
}
