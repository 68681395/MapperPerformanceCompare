using MappersPerformance;
using MappersPerformance.FlatteningClass;
using PerformanceTool;
using PerformanceTool.Osgi;

[assembly: RegLazyLoading(typeof(ITestRunner), typeof(BLToolkitMapper))]
namespace MappersPerformance
{
    [TestMetadata(Category = "Flattening.Class", Name = "BLToolkitMapper")]
    public class BLToolkitMapper : MapperBase
    {
        //public override string Name
        //{
        //    get { return "BLToolkitMapper"; }
        //}

        protected override void OnInitialize()
        {
            base.OnInitialize();

        }

        public override void Run()
        {
            _target = BLToolkit.Mapping.Map.ObjectToObject<ModelDto>(_source);
        }
    }
}