﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using INTJBot.Models;

namespace INTJBot
{
    class Bot
    {
        private string authToken;
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;
        //Add database to keep roles in there?
        private List<string> forbiddenRoles = new List<string>();
        public Bot(string auth)
        {
            authToken = auth;
        }
        public async Task Start()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            client.Log += LogAsync;
            client.UserJoined += UserJoinedAsync;
            client.UserLeft += UserLeftAsync;
            client.GuildMemberUpdated += UserUpdatedAsync;
            services = new ServiceCollection()
                    .BuildServiceProvider();
            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, authToken);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            // Save info on user to the database
            await User.CheckAndAddUser(user);
            // Announce user joined
            var channel = user.Guild.Channels.FirstOrDefault(x => x.Name == "general") as SocketTextChannel;
            await channel.SendMessageAsync($"Welcome to { channel.Guild.Name }, { user.Username}");
        }

        private async Task UserLeftAsync(SocketGuildUser user)
        {
            var channel = user.Guild.Channels.FirstOrDefault(x => x.Name == "general") as SocketTextChannel;
            await channel.SendMessageAsync($"{user.Username} has left");
        }
        // Fired off after user roles have been updated
        private async Task UserUpdatedAsync(SocketGuildUser beforeChange, SocketGuildUser afterChange)
        {
            if(beforeChange.Nickname != null)
            {
                if (!beforeChange.Nickname.Equals(afterChange.Nickname))
                {
                    string nick = afterChange.Nickname;
                    if(nick == null)
                    {
                        nick = afterChange.Username;
                    }
                    Console.WriteLine($"Nickname changed from { beforeChange.Nickname } to { nick } for user {afterChange.Username}");
                    await User.ChangeNickname(afterChange);
                }
            }
            else if(afterChange.Nickname != null)
            {
                Console.WriteLine($"User {afterChange.Username} set up new nickname: {afterChange.Nickname}");
                await User.ChangeNickname(afterChange);
            }
            
            await User.UpdateRolesInDB(afterChange);
        }
    }
}