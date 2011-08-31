using System;
using HashLib;

namespace HashLibQualityTest.DataSourceRows
{
    public class TestDataSourceRow
    {
        public bool Calculate { get; set; }
        public string Algorithm { get; set; }

        public IHash HashFunction { get; set; }

        public override string ToString()
        {
            return String.Format("Algorithm: {0}, Calculate: {1}", Algorithm, Calculate);
        }

    }
}
