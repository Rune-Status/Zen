using Zen.Builder;

namespace Zen.Game.Msg
{
    public abstract class MessageDecoder<T> where T : Message
    {
        protected MessageDecoder(int opcode)
        {
            Opcode = opcode;
        }

        public int Opcode { get; }

        public abstract T Decode(GameFrame frame);
    }
}