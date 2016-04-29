namespace NLiteEmitCompare.FlatteningClass
{
    [Mapper(Category = "Flattening.Class", Name = "Manual")]
    public class ManualMapper : MapperBase
    {
        public override void Map()
        {
            var destination = new ModelDto
            {
                BaseDate = _source.BaseDate,
                Sub2ProperName = _source.Sub2.ProperName,
                SubProperName = _source.Sub.ProperName,
                SubSubSubIAmACoolProperty = _source.Sub.SubSub.IAmACoolProperty,
                SubWithExtraNameProperName = _source.SubWithExtraName.ProperName
            };
        }
    }
}