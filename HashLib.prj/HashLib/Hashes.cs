using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using HashLib.Checksum;
using HashLib.Crypto;
using HashLib.Crypto.BuildIn;
using HashLib.Crypto.SHA3;
using HashLib.Hash32;

namespace HashLib
{
	public static class Hashes
	{
		public static readonly ReadOnlyCollection<Type> All;
		public static readonly ReadOnlyCollection<Type> AllUnique;
		public static readonly ReadOnlyCollection<Type> Hash32;
		public static readonly ReadOnlyCollection<Type> Hash64;
		public static readonly ReadOnlyCollection<Type> CryptoAll;
		public static readonly ReadOnlyCollection<Type> Crypto;
		public static readonly ReadOnlyCollection<Type> CryptoBuildIn;
		public static readonly ReadOnlyCollection<Type> HMACCryptoBuildIn;

		public static readonly ReadOnlyCollection<Type> NonBlock;
		public static readonly ReadOnlyCollection<Type> FastComputes;
		public static readonly ReadOnlyCollection<Type> Checksums;

		static Hashes()
		{
			All = (from hf in Assembly.GetAssembly(typeof(IHash)).GetTypes()
			       where hf.IsClass
			       where !hf.IsAbstract
			       where hf != typeof(HMACNotBuildInAdapter)
			       where hf != typeof(HashCryptoBuildIn)
			       where hf != typeof(HMACBuildInAdapter)
			       where hf != typeof(CRC32)
			       where hf != typeof(CRC64)
			       where hf != typeof(HMACBuildInAdapter)
			       where hf != typeof(CubeHashCustom)
			       where hf.IsImplementingInterface(typeof(IHash))
			       where !hf.IsNested
			       select hf).ToList().AsReadOnly();

			All = (from hf in All
			       orderby hf.Name
			       select hf).ToList().AsReadOnly();

			var x2 = new[]
			         	{
			         		typeof(SHA1Cng),
			         		typeof(SHA1Managed),
			         		typeof(SHA256Cng),
			         		typeof(SHA256Managed),
			         		typeof(SHA384Cng),
			         		typeof(SHA384Managed),
			         		typeof(SHA512Cng),
			         		typeof(SHA512Managed),
			         		typeof(MD5),
			         		typeof(RIPEMD160),
			         		typeof(SHA1),
			         		typeof(SHA256),
			         		typeof(SHA384),
			         		typeof(SHA512),
			         	};

			AllUnique = (from hf in All
			             where !(hf.IsDerivedFrom(typeof(DotNet)))
			             where !x2.Contains(hf)
			             where !hf.IsNested
			             select hf).ToList().AsReadOnly();

			Hash32 = (from hf in All
			          where hf.IsImplementingInterface(typeof(IHash32))
			          select hf).ToList().AsReadOnly();

			Hash64 = (from hf in All
			          where hf.IsImplementingInterface(typeof(IHash64))
			          select hf).ToList().AsReadOnly();

			Checksums = (from hf in All
			             where hf.IsImplementingInterface(typeof(IChecksum))
			             select hf).ToList().AsReadOnly();

			FastComputes = (from hf in All
			                where hf.IsImplementingInterface(typeof(IFastHashCodes))
			                select hf).ToList().AsReadOnly();

			NonBlock = (from hf in All
			            where hf.IsImplementingInterface(typeof(INonBlockHash))
			            select hf).ToList().AsReadOnly();

			CryptoAll = (from hf in All
			             where hf.IsImplementingInterface(typeof(ICrypto))
			             select hf).ToList().AsReadOnly();

			Crypto = (from hf in CryptoAll
			          where hf.IsImplementingInterface(typeof(ICryptoNotBuildIn))
			          select hf).ToList().AsReadOnly();

			CryptoBuildIn = (from hf in CryptoAll
			                 where hf.IsImplementingInterface(typeof(ICryptoBuildIn))
			                 select hf).ToList().AsReadOnly();

			HMACCryptoBuildIn = (from hf in CryptoBuildIn
			                     where hf.IsImplementingInterface(typeof(IHasHMACBuildIn))
			                     select hf).ToList().AsReadOnly();
		}
	}
}