using System.Threading.Tasks;
using GrpcWagonService;

namespace WpfApp1.Data;

public interface IClientService
{
    Task<WagonResponse> GetWagonsAsync(WagonRequest request);
}