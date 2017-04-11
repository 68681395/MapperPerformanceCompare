using MappersPerformance;
using MappersPerformance.FlatteningClass;
using PerformanceTool;
using PerformanceTool.Osgi;

[assembly: RegLazyLoading(typeof(ITestRunner),typeof(AutoMapperWrapper))]

namespace MappersPerformance
{
    [TestMetadata(Category = "Flattening.Class", Name = "AutoMapper")]
    public class AutoMapperWrapper : MapperBase
    {
        protected override  void OnInitialize()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ModelObject, ModelDto>();
            });
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public override void Run()
        {
            _target =AutoMapper.Mapper.Map<ModelObject, ModelDto>(_source);
        }
    }
}