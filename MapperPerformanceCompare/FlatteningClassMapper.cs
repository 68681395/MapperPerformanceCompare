using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite;

namespace NLiteEmitCompare
{
    
    namespace FlatteningClass
    {
        public abstract class MapperBase : ITestRunner
        {
            protected ModelObject _source;
            protected ModelDto _target;

            protected virtual void OnInitialize() { }
            public void Initialize()
            {
                OnInitialize();

                _source = new ModelObject
                {
                    BaseDate = new DateTime(2007, 4, 5),
                    Sub = new ModelSubObject
                    {
                        ProperName = "Some name",
                        SubSub = new ModelSubSubObject
                        {
                            IAmACoolProperty = "Cool daddy-o"
                        }
                    },
                    Sub2 = new ModelSubObject
                    {
                        ProperName = "Sub 2 name"
                    },
                    SubWithExtraName = new ModelSubObject
                    {
                        ProperName = "Some other name"
                    },
                };
            }

            public abstract void Run();
        }


        public class ModelObject
        {
            public DateTime BaseDate { get; set; }
            public ModelSubObject Sub { get; set; }
            public ModelSubObject Sub2 { get; set; }
            public ModelSubObject SubWithExtraName { get; set; }
        }

        public class ModelSubObject
        {
            public string ProperName { get; set; }
            public ModelSubSubObject SubSub { get; set; }
        }

        public class ModelSubSubObject
        {
            public string IAmACoolProperty { get; set; }
        }

        public class ModelDto
        {
            public DateTime BaseDate { get; set; }
            public string SubProperName { get; set; }
            public string Sub2ProperName { get; set; }
            public string SubWithExtraNameProperName { get; set; }
            public string SubSubSubIAmACoolProperty { get; set; }
        }
    }
}
