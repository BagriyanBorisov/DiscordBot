using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotTemplate.Commands
{
    public class Basic : BaseCommandModule
    {
        [Command("test")]
        public async Task TestCommand(CommandContext ctx) 
        {
            await ctx.Channel.SendMessageAsync("Test Message");
        }

        [Command("play")]
        public async Task PlayCommand(CommandContext ctx, [RemainingText]string query)
        {
            var userVc = ctx.Member.VoiceState?.Channel;
            var lava = ctx.Client.GetLavalink();

            if (userVc == null || ctx.Member.VoiceState == null)
            {
                await ctx.Channel.SendMessageAsync("Vlezni da lafish i togawa puskai leshnik :P");
                return;
            }

            if(!lava.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Neam vryzka sys servera nesh brat :(");
                return;
            }

            if(userVc.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Vlezni v glasow kanal leshnik :(");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVc);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Neam vryzka sys servera nesh bachi :(");
                return;
            }
            
            var search = await node.Rest.GetTracksAsync(query, LavalinkSearchType.Youtube);
            if(search.LoadResultType == LavalinkLoadResultType.LoadFailed || search.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.Channel.SendMessageAsync("Ne namiram taq pesen bratle :pensive:");
                return;
            }

            var track = search.Tracks.First();

            await conn.PlayAsync(track);

            string musicDescription = $"Now Playing: {track.Title}\n" +
                                      $"Author: {track.Author}\n" +
                                      $"URL: {track.Uri}";

            var nowPlayingEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Blurple,
                Title = $"Joined channel {userVc.Name} and playing music",
                Description = musicDescription
            };

            await ctx.Channel.SendMessageAsync(embed: nowPlayingEmbed);

        }
    }
}
