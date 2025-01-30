namespace GrpcService1.Data;

using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Google.Protobuf.WellKnownTypes;

public class WagonRepository : IWagonRepository
{
    private readonly string _connectionString;

    public WagonRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Wagon>> GetWagonsAsync(Timestamp startTime, Timestamp endTime)
{
    using var connection = new NpgsqlConnection(_connectionString);
    
    string query = @"SELECT
    epc.""Number"" AS ""inventory_number"",
    arr.""Time"" AS ""arrival_time"",
    dep.""Time"" AS ""departure_time""
 --   dep.""IdPath"",
  --  dep.""TrainNumber""
FROM
    ""EventDeparture"" dep
LEFT JOIN LATERAL (
    SELECT *
    FROM ""EventArrival""
    WHERE
        ""EventArrival"".""IdPath"" = dep.""IdPath""
        AND ""EventArrival"".""Time"" < dep.""Time""
    ORDER BY ""EventArrival"".""Time"" DESC
    LIMIT 1
) arr ON true
LEFT JOIN ""EpcEvent"" epc_event
    ON epc_event.""Time"" = dep.""Time""
    AND epc_event.""IdPath"" = dep.""IdPath""
LEFT JOIN ""Epc"" epc
    ON epc.""Id"" = epc_event.""IdEpc""
WHERE
    epc.""Type"" = 1
  AND dep.""Time"" BETWEEN @StartTime AND @EndTime
    AND epc.""Number"" != '00000000';";

    var dbResult = await connection.QueryAsync(query, new
    {
        StartTime = startTime.ToDateTime(),
        EndTime = endTime.ToDateTime()
    });

    // Конвертируем результат в протобуф-объекты
    var wagons = dbResult.Select(row => new Wagon
    {
        InventoryNumber = Convert.ToInt64(row.inventory_number), // Конвертируем к int64
        ArrivalTime = row.arrival_time == null ? null : Timestamp.FromDateTime((DateTime)row.arrival_time),
        DepartureTime = row.departure_time == null ? null : Timestamp.FromDateTime((DateTime)row.departure_time)
    });

    return wagons;
}
}
