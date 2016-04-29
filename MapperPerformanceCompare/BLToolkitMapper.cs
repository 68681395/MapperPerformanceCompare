namespace NLiteEmitCompare.FlatteningClass
{
    [Mapper(Category = "Flattening.Class", Name = "BLToolkitMapper")]
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

        public override void Map()
        {
            _target = BLToolkit.Mapping.Map.ObjectToObject<ModelDto>(_source);
        }
    }
}