using System;
using PerformanceTool;

namespace MappersPerformance
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
    }
}
