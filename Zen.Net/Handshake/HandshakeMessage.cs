namespace Zen.Net.Handshake
{
    public class HandshakeMessage
    {
        public const int ServiceUpdate = 15;
        public const int ServiceLogin = 14;
        public const int ServiceWorldList = 255;

        public HandshakeMessage(int opcode)
        {
            Opcode = opcode;
        }

        public int Opcode { get; }
    }
}