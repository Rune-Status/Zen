using System;
using Org.BouncyCastle.Math;

namespace Zen.Util.Crypto
{
    public class RsaKeyGenerator
    {
        private const int Bits = 512;

        public static void Generate()
        {
            var random = new Random();

            BigInteger phi, modulus, publicKey, privateKey;

            do
            {
                var p = BigInteger.ProbablePrime(Bits / 2, random);
                var q = BigInteger.ProbablePrime(Bits / 2, random);
                phi = p.Subtract(BigInteger.One).Multiply(q.Subtract(BigInteger.One));

                modulus = p.Multiply(q);
                publicKey = new BigInteger("65537");
                privateKey = publicKey.ModInverse(phi);
            } while (modulus.BitLength != Bits || privateKey.BitLength != Bits ||
                     !phi.Gcd(publicKey).Equals(BigInteger.One));

            Console.WriteLine($"Modulus = {modulus}");
            Console.WriteLine($"Public Key = {publicKey}");
            Console.WriteLine($"Private Key = {privateKey}");
        }
    }
}