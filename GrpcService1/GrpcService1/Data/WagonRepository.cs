namespace GrpcService1.Data;

using Dapper;
using System.Collections.Generic;
using System.Data;
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
        ""Epc"".""Number"" AS ""inventory_number"",
        MIN(""EventArrival"".""Time"") AS ""arrival_time"",
        MAX(""EventDeparture"".""Time"") AS ""departure_time""
    FROM
        ""Epc""
        LEFT JOIN ""EpcEvent"" ON ""Epc"".""Id"" = ""EpcEvent"".""IdEpc""
        JOIN ""EventArrival"" ON ""EpcEvent"".""IdPath"" = ""EventArrival"".""IdPath""
        JOIN ""EventDeparture"" ON ""EpcEvent"".""IdPath"" = ""EventDeparture"".""IdPath""
        LEFT JOIN ""Path"" ON ""EpcEvent"".""IdPath"" = ""Path"".""Id""
    WHERE
        ""Epc"".""Type"" = 1
        AND ""Epc"".""Number"" != '00000000'
        AND ""EventDeparture"".""Time"" BETWEEN @StartTime AND @EndTime
        AND ""EventArrival"".""IdPath"" = ""EventDeparture"".""IdPath""
        AND ""EventDeparture"".""Time"" > ""EventArrival"".""Time""
    GROUP BY
        ""Epc"".""Number"", ""Path"".""Id"";";

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
