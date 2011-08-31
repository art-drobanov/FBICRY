using System;
using System.Collections.Generic;
using HashLibTest;
using HashLib;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HashLibOutputDataGenerator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var list = from hf in Hashes.AllUnique
                       where hf.Name != "DotNet"
                       select (IHash)Activator.CreateInstance(hf);

            Console.WriteLine("Uncomment me if you know what you're doing.\n");
            Console.ReadKey();

            //Console.WriteLine("Generating...");
            //foreach (IHash hash in list)
            //    new TestData(hash).Save();
        }
    }
}