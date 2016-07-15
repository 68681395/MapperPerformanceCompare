namespace NLiteEmitCompare.FlatteningClass
{
    [TestItem(Category = "Flattening.Class", Name = "AutoMapper")]
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