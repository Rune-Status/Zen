using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Zen.Util;

namespace Zen.Net.World
{
    public class WorldListEncoder : MessageToByteEncoder<WorldListMessage>
    {
        protected override void Encode(IChannelHandlerContext context, WorldListMessage message, IByteBuffer output)
        {
            var buf = context.Allocator.Buffer();
            buf.WriteByte(1);
            buf.WriteByte(1);

            var countries = message.Countries;
            buf.WriteSmart(countries.Length);
            foreach (var country in countries)
            {
                buf.WriteSmart(country.Flag);
                buf.WriteWorldListString(country.Name);
            }

            var worlds = message.WorldsEntry;
            var minId = worlds[0].Id;
            var maxId = worlds[worlds.Length - 1].Id;
            for (var i = 1; i < worlds.Length; i++)
            {
                var world = worlds[i];
                var id = world.Id;

                if (id > maxId)
                    maxId = id;
                if (id < minId)
                    minId = id;
            }

            buf.WriteSmart(minId);
            buf.WriteSmart(maxId);
            buf.WriteSmart(worlds.Length);

            foreach (var world in worlds)
            {
                buf.WriteSmart(world.Id - minId);
                buf.WriteByte(world.Country);
                buf.WriteInt(world.Flags);
                buf.WriteWorldListString(world.Activity);
                buf.WriteWorldListString(world.Ip);
            }

            buf.WriteInt(message.SessionId);

            var players = message.Players;
            for (var i = 0; i < worlds.Length; i++)
            {
                var world = worlds[i];
                buf.WriteSmart(world.Id - minId);
                buf.WriteShort(players[i]);
            }

            output.WriteByte(0);
            output.WriteShort(buf.ReadableBytes);
            output.WriteBytes(buf);
        }
    }
}