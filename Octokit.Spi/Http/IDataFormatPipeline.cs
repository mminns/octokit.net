using Octokit.Internal;

namespace Octokit.Http
{
    public interface IDataFormatPipeline
    {
        void SerializeRequest(IRequest request);
        IApiResponse<T>DeserializeResponse<T>(IResponse response);
        IDataFormatSerializer Serializer { get; }
    }
}