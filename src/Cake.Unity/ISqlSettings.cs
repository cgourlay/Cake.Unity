namespace Cake.Mix
{
    public interface ISqlSettings
    {
        string ConnectionString { get; }
        string Delimiter { get; }
        bool IsTransaction { get; }
        string SqlScript { get; }
    }
}