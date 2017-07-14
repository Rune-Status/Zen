using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class InterfaceClosedMessageDecoder : MessageDecoder<InterfaceClosedMessage>
    {
        private static readonly InterfaceClosedMessage Closed = new InterfaceClosedMessage();

        public InterfaceClosedMessageDecoder() : base(184)
        {
            /* empty */
        }

        public override InterfaceClosedMessage Decode(GameFrame frame) => Closed;
    }
}