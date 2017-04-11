using EmitMapper;
using MappersPerformance;
using MappersPerformance.FlatteningClass;
using PerformanceTool;
using PerformanceTool.Osgi;

[assembly: RegLazyLoading(typeof(ITestRunner), typeof(EmitMapperWrapper))]
namespace MappersPerformance
{
    [TestMetadata(Category = "Flattening.Class", Name = "EmitMapper")]
    public class EmitMapperWrapper : MapperBase
    {
        ObjectsMapper<ModelObject, ModelDto> mapper;
        protected override void OnInitialize()
        {
            mapper = ObjectMapperManager.DefaultInstance.GetMapper<ModelObject, ModelDto>();
        }

        public override void Run()
        {
            _target = mapper.Map(_source);
        }
    }
}