namespace GrpcService1.Data;

using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IWagonRepository
{
    Task<IEnumerable<Wagon>> GetWagonsAsync(Timestamp startTime, Timestamp endTime);
}