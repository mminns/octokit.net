namespace Octokit.Internal
{
    public interface IDataFormatSerializer
    {
        string Serialize(object item);
        T Deserialize<T>(string json);
    }
}
