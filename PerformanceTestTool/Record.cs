using System.Collections.Generic;

namespace PerformanceTool
{
    public class Record
    {
        public ITestMetadata Mapper { get; set; }

        public List<IterationRecord> Iterations { get; set; }
    }
}