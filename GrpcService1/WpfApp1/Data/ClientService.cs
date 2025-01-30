namespace WpfApp1.Data;

using Grpc.Net.Client;
using GrpcWagonService;
using System.Threading.Tasks;

public class ClientService : IClientService
{
    private readonly WagonService.WagonServiceClient _client;

    public ClientService(GrpcChannel channel)
    {
        _client = new WagonService.WagonServiceClient(channel);
    }

    public async Task<WagonResponse> GetWagonsAsync(WagonRequest request)
    {
        return await _client.GetWagonsAsync(request);
    }
}

