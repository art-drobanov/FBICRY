using System;
using HashLib;
using System.IO;
using System.Text;

namespace HashLibQualityTest.DataSourceRows
{
    public class CalculatorDataSourceRow : TestDataSourceRow
    {
        public string Hash { get; set; }

        public void CalculateHashFromFile(string a_file)
        {
            if (HashFunction is INonBlockHash)
            {
                if (new FileInfo(a_file).Length > 200*1024*1024)
                {
                    Hash = "Hashing skipped. File is too big and algorithm does not provide multiple transforms capabilities";
                    return;
                }
            }

            Hash = HashFunction.ComputeFile(a_file).ToString();
        }

        public void CalculateHashFromText(string a_text)
        {
            Hash = HashFunction.ComputeBytes(Converters.ConvertStringToBytes(a_text, Encoding.ASCII)).ToString();
        }

        public void CalculateHMACFromFile(string a_file, string a_key)
        {
            if (!(HashFunction is IHMAC) && (HashFunction is INonBlockHash))
            {
                if (new FileInfo(a_file).Length > 200 * 1024 * 1024)
                {
                    Hash = "Hashing skipped. File is too big and algorithm does not provide multiple transforms capabilities";
                    return;
                }
            }

            IHMAC hmac = HashFactory.HMAC.CreateHMAC(HashFunction);
            hmac.Key = Converters.ConvertStringToBytes(a_key, Encoding.ASCII);
            Hash = hmac.ComputeFile(a_file).ToString();
        }

        public void CalculateHMACFromText(string a_text, string a_key)
        {
            IHMAC hmac = HashFactory.HMAC.CreateHMAC(HashFunction);
            hmac.Key = Converters.ConvertStringToBytes(a_key, Encoding.ASCII);
            Hash = hmac.ComputeBytes(Converters.ConvertStringToBytes(a_text)).ToString(); 
        }
    }
}
