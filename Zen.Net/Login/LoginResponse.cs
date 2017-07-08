using DotNetty.Buffers;

namespace Zen.Net.Login
{
    public class LoginResponse
    {
        public LoginResponse(int status, IByteBuffer payload = null)
        {
            Status = status;
            Payload = payload ?? Unpooled.Empty;
        }

        public IByteBuffer Payload { get; }
        public int Status { get; }
    }
}