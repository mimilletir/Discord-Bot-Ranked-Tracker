using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscordBotTFT.Attributes
{
    public enum ChannelCheckMode
    {
        Any = 0,
        None = 1,
        MineOrParentAny = 2,
    }
}
