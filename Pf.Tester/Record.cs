using System.Collections.Generic;
using PerformanceTool;

namespace Pf.Tester
{
    public class Record
    {
        public ITestMetadata Mapper { get; set; }

        public List<IterationRecord> Iterations { get; set; }
    }
}