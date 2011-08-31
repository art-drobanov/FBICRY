using System;
using System.Diagnostics;
using System.IO;
using HashLib;
using System.Linq;

namespace Examples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Prepare temp file.
            string file_name = Path.GetTempFileName();
            using (var fs = new FileStream(file_name, FileMode.Open))
            {
                byte[] v = new byte[256];
                new Random().NextBytes(v);
                fs.Write(v, 0, v.Length);
            }

            // Prepare stream.
            MemoryStream ms = new MemoryStream(new byte[] { 2, 3, 4, 5, 6, 7 });

            // Choose algorithm. Explore HashFacory for more algorithms.
            IHash hash = HashFactory.Crypto.CreateSHA256();

            // Hash data immediate.
            HashResult r = hash.ComputeString("test", System.Text.Encoding.ASCII);

            // Hash data.
            hash.Initialize();
            hash.TransformULong(6);
            hash.TransformString("test");
            r = hash.TransformFinal();

            // Calculate 32-bits hash.
            hash = HashFactory.Checksum.CreateCRC32a();
            uint crc32 = hash.ComputeString("test").GetUInt();

            // For Haval, Tiger, CRC32, CRC64 you may specify parameters.
            hash = HashFactory.Checksum.CreateCRC32(HashLib.Checksum.CRC32Polynomials.IEEE_802_3, uint.MaxValue, uint.MaxValue);
            hash = HashFactory.Checksum.CreateCRC32(0xF0F0F0F0, uint.MaxValue, uint.MaxValue);

            // Calculate 64-bits hash.
            hash = HashFactory.Hash64.CreateMurmur2();
            ulong crc64 = hash.ComputeString("test").GetULong();

            // Get some information about algorithm.
            System.Console.WriteLine("{0}, {1}, {2}", hash.BlockSize, hash.HashSize, hash.Name);

            // Here you can find algorithms grouped by its properties.
            foreach (var h in Hashes.Crypto)
                System.Console.WriteLine(((IHash)Activator.CreateInstance(h)).Name);

            // Hash stream.
            r = hash.ComputeStream(ms);
            ms.Position = 2;
            r = hash.ComputeStream(ms); // Compute all bytes starting from 2
            r = hash.ComputeStream(ms, 2); // Compute 2 bytes starting from 2
            hash.TransformStream(ms);

            // Hash file
            r = hash.ComputeFile(file_name);
            r = hash.ComputeFile(file_name, 10); // Compute all bytes starting from 2
            r = hash.ComputeFile(file_name, 10, 10); // Compute 10 bytes starting from 10.
            hash.TransformFile(file_name);

            // Calculate HMAC.
            IHMAC hmac = HashFactory.HMAC.CreateHMAC(HashFactory.Crypto.CreateSHA256());
            hmac.Key = Converters.ConvertStringToBytes("secret");
            r = hmac.ComputeString("test");

            // Get System.Security.Cryptography.HashAlgorithm wrapper for algorithms from this library.
            System.Security.Cryptography.HashAlgorithm hash2 = HashFactory.Wrappers.HashToHashAlgorithm(hash);

            // And back.
            hash = HashFactory.Wrappers.HashAlgorithmToHash(hash2, -1); // Second parameter specify BlockSize, it has only informative meaning.

            // Some allgorithms has fast specialized methods for calculating hashes for all data types. There are designed for calculating good-behave hash codes for hash-tables.
            hash = HashFactory.Hash32.CreateMurmur2();
            Debug.Assert(hash is IFastHashCodes);

            // Some algorithms can calculated hashes only when they had all needed data, they accumulated data to the very end.
            hash = HashFactory.Hash32.CreateMurmur2();
            Debug.Assert(hash is INonBlockHash);

            // Use build-in cryptography hash algorithms.
            hash = HashFactory.Crypto.BuildIn.CreateSHA256Cng();

            // Delete temp file.
            new FileInfo(file_name).Delete();
        }
    }
}
