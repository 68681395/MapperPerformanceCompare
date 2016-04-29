using EmitMapper;

namespace NLiteEmitCompare.FlatteningClass
{
    [Mapper(Category = "Flattening.Class", Name = "EmitMapper")]
    public class EmitMapperWrapper : MapperBase
    {
        ObjectsMapper<ModelObject, ModelDto> mapper;
        protected override void OnInitialize()
        {
            mapper = ObjectMapperManager.DefaultInstance.GetMapper<ModelObject, ModelDto>();
        }

        public override void Map()
        {
            _target = mapper.Map(_source);
        }
    }
}