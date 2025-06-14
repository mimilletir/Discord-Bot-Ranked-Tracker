using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DiscordBotTFT.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequireCategoriesAttribute : CheckBaseAttribute
    {
        public IReadOnlyList<string> CategoriesNames { get; }
        public ChannelCheckMode CheckMode { get; }

        public RequireCategoriesAttribute(ChannelCheckMode checkMode, params string[] channelNames)
        {
            CheckMode = checkMode;
            CategoriesNames = new ReadOnlyCollection<string>(channelNames);
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if(ctx.Guild == null || ctx.Member == null)
                return Task.FromResult(false);

            bool contains = CategoriesNames.Contains(ctx.Channel.Parent.Name, StringComparer.OrdinalIgnoreCase);

            return CheckMode switch
            {
                ChannelCheckMode.Any => Task.FromResult(contains),

                ChannelCheckMode.None => Task.FromResult(!contains),

                _ => Task.FromResult(false),
            };
        }
    }
}
