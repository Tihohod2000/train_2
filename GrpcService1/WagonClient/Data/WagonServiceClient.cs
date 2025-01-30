using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;
using GrpcWagonService;

namespace WagonClient.Data
{
    public class WagonServiceClient : IWagonServiceClient
    {
        private readonly WagonService.WagonServiceClient _client;

        public WagonServiceClient(WagonService.WagonServiceClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<WagonResponse> GetWagonsAsync(Timestamp startTime, Timestamp endTime)
        {
            var request = new WagonRequest { StartTime = startTime, EndTime = endTime };
            return await _client.GetWagonsAsync(request);
        }
    }
}