namespace NLiteEmitCompare.FlatteningClass
{
    [Mapper(Category = "Flattening.Class", Name = "NLiteMapper")]
    public class NLiteMaperWrapper : MapperBase
    {
        private NLite.Mapping.IMapper<ModelObject, ModelDto> mapper;
        protected override void OnInitialize()
        {
            base.OnInitialize();

            mapper = NLite.Mapper.CreateMapper<ModelObject, ModelDto>();
        }

        public override void Map()
        {
            _target = mapper.Map(_source);
        }
    }
}