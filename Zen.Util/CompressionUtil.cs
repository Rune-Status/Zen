using System;
using System.IO;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;

namespace Zen.Util
{
    public class CompressionUtil
    {
        public static byte[] Bzip2(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var bout = new MemoryStream();
                using (var os = new BZip2OutputStream(bout, 1))
                {
                    var buf = new byte[4096];
                    int len;
                    while ((len = stream.Read(buf, 0, buf.Length)) != 0)
                        os.Write(buf, 0, len);
                }

                bytes = bout.ToArray();
                var bzip2 = new byte[bytes.Length - 2];
                Array.Copy(bytes, 4, bzip2, 0, bzip2.Length);
                return bzip2;
            }
        }

        public static byte[] Bunzip2(byte[] bytes)
        {
            var bzip2 = new byte[bytes.Length + 4];
            bzip2[0] = (byte) 'B';
            bzip2[1] = (byte) 'Z';
            bzip2[2] = (byte) 'h';
            bzip2[3] = (byte) '1';
            Array.Copy(bytes, 0, bzip2, 4, bytes.Length);

            using (var stream = new BZip2InputStream(new MemoryStream(bzip2)))
            {
                using (var os = new MemoryStream())
                {
                    var buf = new byte[4096];
                    int len;
                    while ((len = stream.Read(buf, 0, buf.Length)) != 0)
                        os.Write(buf, 0, len);

                    return os.ToArray();
                }
            }
        }

        public static byte[] Gzip(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var bout = new MemoryStream();
                using (var os = new GZipOutputStream(bout))
                {
                    var buf = new byte[4096];
                    int len;
                    while ((len = stream.Read(buf, 0, buf.Length)) != 0)
                        os.Write(buf, 0, len);
                }

                return bout.ToArray();
            }
        }

        public static byte[] Gunzip(byte[] bytes)
        {
            using (var stream = new GZipInputStream(new MemoryStream(bytes)))
            {
                using (var os = new MemoryStream())
                {
                    var buf = new byte[4096];
                    int len;
                    while ((len = stream.Read(buf, 0, buf.Length)) != 0)
                        os.Write(buf, 0, len);

                    return os.ToArray();
                }
            }
        }
    }
}