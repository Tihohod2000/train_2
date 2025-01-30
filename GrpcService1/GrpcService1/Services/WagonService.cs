using GrpcService1.Data;

namespace GrpcService1.Services;

using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class WagonService : GrpcService1.WagonService.WagonServiceBase
{
    private readonly IWagonRepository _repository;

    public WagonService(IWagonRepository repository)
    {
        _repository = repository;
    }

    public override async Task<WagonResponse> GetWagons(WagonRequest request, ServerCallContext context)
    {
        var wagons = await _repository.GetWagonsAsync(request.StartTime, request.EndTime);

        var response = new WagonResponse();
        response.Wagons.AddRange(
            wagons // Отбрасываем null-элементы
                .Select(w => new Wagon
                {
                    InventoryNumber = w.InventoryNumber, // Подстрахуемся
                    ArrivalTime = w.ArrivalTime != null ? Timestamp.FromDateTime(w.ArrivalTime.ToDateTime()) : null,
                    DepartureTime = w.DepartureTime != null ? Timestamp.FromDateTime(w.DepartureTime.ToDateTime()) : null
                }));

        return response;
    }
}
