using System;
using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;
using Zen.Util;

namespace Zen.Game.Update
{
    public class ChatPlayerBlock : PlayerBlock
    {
        private readonly ChatMessage _chatMessage;
        private readonly int _rights;

        public ChatPlayerBlock(Player player) : base(0x80)
        {
            _chatMessage = player.ChatMessage;
            _rights = player.Rights;
        }

        public override void Encode(PlayerUpdateMessage message, GameFrameBuilder builder)
        {
            var bytes = new byte[256];
            var size = _chatMessage.Text.Pack(bytes);

            builder.Put(DataType.Short, DataOrder.Little, (_chatMessage.Color << 8) | _chatMessage.Effects);
            builder.Put(DataType.Byte, _rights);
            builder.Put(DataType.Byte, size);

            var copy = new byte[size];
            Array.Copy(bytes, 0, copy, 0, Math.Min(bytes.Length, size));
            builder.PutBytesReverse(copy);
        }
    }
}