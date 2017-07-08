using DotNetty.Buffers;
using Org.BouncyCastle.Math;

namespace Zen.Util.Crypto
{
    public class Rsa
    {
        public static IByteBuffer Crypt(IByteBuffer buffer, BigInteger modulus, BigInteger key)
        {
            var bytes = new byte[buffer.Capacity];
            buffer.ReadBytes(bytes);

            var input = new BigInteger(bytes);
            var output = input.ModPow(key, modulus);

            return Unpooled.WrappedBuffer(output.ToByteArray());
        }
    }
}