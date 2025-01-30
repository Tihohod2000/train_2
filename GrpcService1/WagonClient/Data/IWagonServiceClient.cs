using Google.Protobuf.WellKnownTypes;
using GrpcWagonService;

namespace WagonClient.Data
{
    public interface IWagonServiceClient
    {
        Task<WagonResponse> GetWagonsAsync(Timestamp startTime, Timestamp endTime);
    }
}