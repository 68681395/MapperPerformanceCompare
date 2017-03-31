using System;

namespace MappersPerformance
{
    public class ModelObject
    {
        public DateTime BaseDate { get; set; }
        public ModelSubObject Sub { get; set; }
        public ModelSubObject Sub2 { get; set; }
        public ModelSubObject SubWithExtraName { get; set; }
    }
}