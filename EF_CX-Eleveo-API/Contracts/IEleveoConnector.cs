using Eleveo_EFCX_Connector_API.Dto;
using System.Runtime.CompilerServices;

namespace Eleveo_EFCX_Connector_API.Contracts
{
    public interface IEleveoConnector
    {
        Task<List<ConversationDataDto>> GetAllConversationDataAsync();
        Task<List<ConversationDataDto>> GetConversationDataAsync(string? JtapiId);
        Task<string> GetJSessionIdAsync();
        string GetDownloadToken(string jsessionid, string sid);
        Task<bool> RecordingExists(string JtapiId);
        Stream GetRecordingWavByDToken(string jsessionId, string downloadtoken);
        Task<RecordingFile> DecodeRecordingAsync(RecordingFile recording);
        Stream GetRecordingWavByJTapi(string jsessionId, string JtapiId);
        Stream MergeAudioParts(List<string> filemp3parts);
        Stream GetStream4RecordingList(List<ConversationDataDto> Sids, string Jsessionid);
        void DeleteUsedFiles(string filepath);
        List<string> ConvertStreamList(List<Stream> streamlist);
        Stream GetMP3Stream(string filepath);
    }
}
