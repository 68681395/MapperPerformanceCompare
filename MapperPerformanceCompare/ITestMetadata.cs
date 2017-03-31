namespace PerformanceTool
{
    public interface ITestMetadata
    {
        //目录
        string Category { get; }
        //名称
        string Name { get; }
        string Descrption { get; }
    }
}