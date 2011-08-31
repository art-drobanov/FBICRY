using System;
using System.Configuration;
using HashLibQualityTest.DataSourceRows;

namespace HashLibQualityTest.Configurations
{
    public class SpeedTestElement : ConfigurationElement
    {
        public SpeedTestElement()
        {
        }

        public SpeedTestElement(SpeedTestDataSourceRow a_row)
        {
            Algorithm = a_row.Algorithm;

            StringSpeed = a_row.StringSpeed;
            BytesSpeed = a_row.BytesSpeed;
            CharsSpeed = a_row.CharsSpeed;
            ShortsSpeed = a_row.ShortsSpeed;
            UShortsSpeed = a_row.UShortsSpeed;
            IntsSpeed = a_row.IntsSpeed;
            UIntsSpeed = a_row.UIntsSpeed;
            LongsSpeed = a_row.LongsSpeed;
            ULongsSpeed = a_row.ULongsSpeed;
            FloatsSpeed = a_row.FloatsSpeed;
            DoublesSpeed = a_row.DoublesSpeed;
            ByteSpeed = a_row.ByteSpeed;
            CharSpeed = a_row.CharSpeed;
            ShortSpeed = a_row.ShortSpeed;
            UShortSpeed = a_row.UShortSpeed;
            IntSpeed = a_row.IntSpeed;
            UIntSpeed = a_row.UIntSpeed;
            LongSpeed = a_row.LongSpeed;
            ULongSpeed = a_row.ULongSpeed;
            FloatSpeed = a_row.FloatSpeed;
            DoubleSpeed = a_row.DoubleSpeed;
        }

        public void CopyTo(SpeedTestDataSourceRow a_row)
        {
            a_row.StringSpeed = StringSpeed;
            a_row.BytesSpeed = BytesSpeed;
            a_row.CharsSpeed = CharsSpeed;
            a_row.ShortsSpeed = ShortsSpeed;
            a_row.UShortsSpeed = UShortsSpeed;
            a_row.IntsSpeed = IntsSpeed;
            a_row.UIntsSpeed = UIntsSpeed;
            a_row.LongsSpeed = LongsSpeed;
            a_row.ULongsSpeed = ULongsSpeed;
            a_row.FloatsSpeed = FloatsSpeed;
            a_row.DoublesSpeed = DoublesSpeed;
            a_row.ByteSpeed = ByteSpeed;
            a_row.CharSpeed = CharSpeed;
            a_row.ShortSpeed = ShortSpeed;
            a_row.UShortSpeed = UShortSpeed;
            a_row.IntSpeed = IntSpeed;
            a_row.UIntSpeed = UIntSpeed;
            a_row.LongSpeed = LongSpeed;
            a_row.ULongSpeed = ULongSpeed;
            a_row.FloatSpeed = FloatSpeed;
            a_row.DoubleSpeed = DoubleSpeed;
        }

        [ConfigurationProperty("Algorithm", IsKey = true)]
        public string Algorithm
        {
            get
            {
                return (string)this["Algorithm"];
            }
            set
            {
                this["Algorithm"] = value;
            }
        }

        [ConfigurationProperty("StringSpeed")]
        public double StringSpeed
        {
            get
            {
                return (double)this["StringSpeed"];
            }
            set
            {
                this["StringSpeed"] = value;
            }
        }

        [ConfigurationProperty("BytesSpeed")]
        public double BytesSpeed
        {
            get
            {
                return (double)this["BytesSpeed"];
            }
            set
            {
                this["BytesSpeed"] = value;
            }
        }

        [ConfigurationProperty("CharsSpeed")]
        public double CharsSpeed
        {
            get
            {
                return (double)this["CharsSpeed"];
            }
            set
            {
                this["CharsSpeed"] = value;
            }
        }

        [ConfigurationProperty("ShortsSpeed")]
        public double ShortsSpeed
        {
            get
            {
                return (double)this["ShortsSpeed"];
            }
            set
            {
                this["ShortsSpeed"] = value;
            }
        }

        [ConfigurationProperty("UShortsSpeed")]
        public double UShortsSpeed
        {
            get
            {
                return (double)this["UShortsSpeed"];
            }
            set
            {
                this["UShortsSpeed"] = value;
            }
        }

        [ConfigurationProperty("IntsSpeed")]
        public double IntsSpeed
        {
            get
            {
                return (double)this["IntsSpeed"];
            }
            set
            {
                this["IntsSpeed"] = value;
            }
        }

        [ConfigurationProperty("UIntsSpeed")]
        public double UIntsSpeed
        {
            get
            {
                return (double)this["UIntsSpeed"];
            }
            set
            {
                this["UIntsSpeed"] = value;
            }
        }

        [ConfigurationProperty("LongsSpeed")]
        public double LongsSpeed
        {
            get
            {
                return (double)this["LongsSpeed"];
            }
            set
            {
                this["LongsSpeed"] = value;
            }
        }

        [ConfigurationProperty("ULongsSpeed")]
        public double ULongsSpeed
        {
            get
            {
                return (double)this["ULongsSpeed"];
            }
            set
            {
                this["ULongsSpeed"] = value;
            }
        }

        [ConfigurationProperty("FloatsSpeed")]
        public double FloatsSpeed
        {
            get
            {
                return (double)this["FloatsSpeed"];
            }
            set
            {
                this["FloatsSpeed"] = value;
            }
        }

        [ConfigurationProperty("DoublesSpeed")]
        public double DoublesSpeed
        {
            get
            {
                return (double)this["DoublesSpeed"];
            }
            set
            {
                this["DoublesSpeed"] = value;
            }
        }

        [ConfigurationProperty("ByteSpeed")]
        public double ByteSpeed
        {
            get
            {
                return (double)this["ByteSpeed"];
            }
            set
            {
                this["ByteSpeed"] = value;
            }
        }

        [ConfigurationProperty("CharSpeed")]
        public double CharSpeed
        {
            get
            {
                return (double)this["CharSpeed"];
            }
            set
            {
                this["CharSpeed"] = value;
            }
        }

        [ConfigurationProperty("ShortSpeed")]
        public double ShortSpeed
        {
            get
            {
                return (double)this["ShortSpeed"];
            }
            set
            {
                this["ShortSpeed"] = value;
            }
        }

        [ConfigurationProperty("UShortSpeed")]
        public double UShortSpeed
        {
            get
            {
                return (double)this["UShortSpeed"];
            }
            set
            {
                this["UShortSpeed"] = value;
            }
        }

        [ConfigurationProperty("IntSpeed")]
        public double IntSpeed
        {
            get
            {
                return (double)this["IntSpeed"];
            }
            set
            {
                this["IntSpeed"] = value;
            }
        }

        [ConfigurationProperty("UIntSpeed")]
        public double UIntSpeed
        {
            get
            {
                return (double)this["UIntSpeed"];
            }
            set
            {
                this["UIntSpeed"] = value;
            }
        }

        [ConfigurationProperty("LongSpeed")]
        public double LongSpeed
        {
            get
            {
                return (double)this["LongSpeed"];
            }
            set
            {
                this["LongSpeed"] = value;
            }
        }

        [ConfigurationProperty("ULongSpeed")]
        public double ULongSpeed
        {
            get
            {
                return (double)this["ULongSpeed"];
            }
            set
            {
                this["ULongSpeed"] = value;
            }
        }

        [ConfigurationProperty("FloatSpeed")]
        public double FloatSpeed
        {
            get
            {
                return (double)this["FloatSpeed"];
            }
            set
            {
                this["FloatSpeed"] = value;
            }
        }

        [ConfigurationProperty("DoubleSpeed")]
        public double DoubleSpeed
        {
            get
            {
                return (double)this["DoubleSpeed"];
            }
            set
            {
                this["DoubleSpeed"] = value;
            }
        }
    }

    public class SpeedTestCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SpeedTestElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as SpeedTestElement).Algorithm;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public SpeedTestElement this[int a_index]
        {
            get
            {
                return (SpeedTestElement)BaseGet(a_index);
            }
        }

        public SpeedTestElement this[object a_key]
        {
            get
            {
                return (SpeedTestElement)BaseGet(a_key);
            }
        }

        public void Add(SpeedTestDataSourceRow a_row)
        {
            BaseRemove(a_row.Algorithm);
            base.BaseAdd(new SpeedTestElement(a_row));
        }

        public void Clear()
        {
            base.BaseClear();
        }
    }

    public class SpeedTests : ConfigurationSection
    {
        private static String SECTION_NAME = "SpeedTests";
        private static SpeedTests s_speedTests;

        static SpeedTests()
        {
            s_speedTests = Config.Instance.GetSection(SECTION_NAME) as SpeedTests;

            if (s_speedTests == null)
            {
                s_speedTests = new SpeedTests();
                s_speedTests.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                Config.Instance.Sections.Add(SECTION_NAME, s_speedTests);
            }
        }

        public static SpeedTests Instance
        {
            get
            {
                return s_speedTests;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public SpeedTestCollection List
        {
            get
            {
                return base[""] as SpeedTestCollection;
            }
        }
    }
}
