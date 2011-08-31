using System;
using System.Configuration;

namespace HashLibQualityTest.Configurations
{
    public class LastOpenFiles : ConfigurationSection
    {
        private static String SECTION_NAME = "LastOpenFiles";
        private static LastOpenFiles s_lastOpenFiles;

        static LastOpenFiles()
        {
            s_lastOpenFiles = Config.Instance.GetSection(SECTION_NAME) as LastOpenFiles;
   
            if (s_lastOpenFiles == null)
            {
                s_lastOpenFiles = new LastOpenFiles();
                s_lastOpenFiles.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                Config.Instance.Sections.Add(SECTION_NAME, s_lastOpenFiles);
            }

            s_lastOpenFiles.Check();
        }

        public static LastOpenFiles Instance 
        {
            get
            {
                return s_lastOpenFiles;
            }
        }

        [ConfigurationProperty("HashFile", DefaultValue = "")]
        public string HashFile
        {
            get
            {
                return (string)base["HashFile"];
            }
            set
            {
                base["HashFile"] = value;
                try
                {
                    HashFileDir = new System.IO.FileInfo(value).DirectoryName;
                }
                catch (Exception)
                {
                    HashFileDir = "";
                }

                if (HashFile != CheckFileName(HashFile))
                    HashFile = CheckFileName(HashFile);

                Config.Instance.Save();
            }
        }

        [ConfigurationProperty("HashText", DefaultValue = "")]
        public string HashText
        {
            get
            {
                return (string)base["HashText"];
            }
            set
            {
                base["HashText"] = value;
            }
        }

        [ConfigurationProperty("HashFileDir", DefaultValue = "")]
        public string HashFileDir
        {
            get
            {
                return (string)base["HashFileDir"];
            }
            set
            {
                base["HashFileDir"] = value;

                if (HashFileDir != CheckDirectory(HashFileDir))
                    HashFileDir = CheckDirectory(HashFileDir);
            }
        }

        [ConfigurationProperty("HashFileCrypto", DefaultValue = "")]
        public string HashFileCrypto
        {
            get
            {
                return (string)base["HashFileCrypto"];
            }
            set
            {
                base["HashFileCrypto"] = value;
                try
                {
                    HashFileDirCrypto = new System.IO.FileInfo(value).DirectoryName;
                }
                catch (Exception)
                {
                    HashFileDirCrypto = "";
                }

                if (HashFileCrypto != CheckFileName(HashFileCrypto))
                    HashFileCrypto = CheckFileName(HashFileCrypto);

                Config.Instance.Save();
            }
        }

        [ConfigurationProperty("HashTextCrypto", DefaultValue = "")]
        public string HashTextCrypto
        {
            get
            {
                return (string)base["HashTextCrypto"];
            }
            set
            {
                base["HashTextCrypto"] = value;
            }
        }

        [ConfigurationProperty("HashFileDirCrypto", DefaultValue = "")]
        public string HashFileDirCrypto
        {
            get
            {
                return (string)base["HashFileDirCrypto"];
            }
            set
            {
                base["HashFileDirCrypto"] = value;

                if (HashFileDirCrypto != CheckDirectory(HashFileDirCrypto))
                    HashFileDirCrypto = CheckDirectory(HashFileDirCrypto);
            }
        }

        [ConfigurationProperty("HashFileHMAC", DefaultValue = "")]
        public string HashFileHMAC
        {
            get
            {
                return (string)base["HashFileHMAC"];
            }
            set
            {
                base["HashFileHMAC"] = value;
                try
                {
                    HashFileDirHMAC = new System.IO.FileInfo(value).DirectoryName;
                }
                catch (Exception)
                {
                    HashFileDirHMAC = "";
                }

                if (HashFileHMAC != CheckFileName(HashFileHMAC))
                    HashFileHMAC = CheckFileName(HashFileHMAC);

                Config.Instance.Save();
            }
        }

        [ConfigurationProperty("HashTextHMAC", DefaultValue = "")]
        public string HashTextHMAC
        {
            get
            {
                return (string)base["HashTextHMAC"];
            }
            set
            {
                base["HashTextHMAC"] = value;
            }
        }

        [ConfigurationProperty("HashFileDirHMAC", DefaultValue = "")]
        public string HashFileDirHMAC
        {
            get
            {
                return (string)base["HashFileDirHMAC"];
            }
            set
            {
                base["HashFileDirHMAC"] = value;

                if (HashFileDirHMAC != CheckDirectory(HashFileDirHMAC))
                    HashFileDirHMAC = CheckDirectory(HashFileDirHMAC);
            }
        }

        [ConfigurationProperty("HashKey", DefaultValue = "")]
        public string HashKey
        {
            get
            {
                return (string)base["HashKey"];
            }
            set
            {
                base["HashKey"] = value;
            }
        }

        private string CheckFileName(string a_fileName)
        {
            try
            {
                if (!new System.IO.FileInfo(a_fileName).Exists)
                    return string.Empty;

                return a_fileName;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        private string CheckDirectory(string a_fileName)
        {
            try
            {
               String str = new System.IO.DirectoryInfo(a_fileName).FindExistingDirectory();

               if (str.Equals(""))
               {
                   str = AppDomain.CurrentDomain.BaseDirectory + "Examples";
               }

               return str;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void Check()
        {
            HashFile = CheckFileName(HashFile);
            HashFileCrypto = CheckFileName(HashFileCrypto);
            HashFileHMAC = CheckFileName(HashFileHMAC);

            HashFileDir = CheckDirectory(HashFileDir);
            HashFileDirCrypto = CheckDirectory(HashFileDirCrypto);
            HashFileDirHMAC = CheckDirectory(HashFileDirHMAC);
        }
    }
}