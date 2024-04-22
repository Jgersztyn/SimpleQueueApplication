using ServiceBus.Common.Enums;
using ServiceBus.Common.Models;

namespace ServiceBus.Sender.Utilties;


public static class Helpers
{
    public static HistoricDataPacketRequest CreateDataPacket()
    {
        var jobData = new HistoricDataPacketRequest
        {
            JobType = JobType.CreatePacket,
            Parameters = new Dictionary<string, string>
        {
            { "radarList", "t001,t002,t003" },
            { "from", "2024-03-29T12:00:00" },
            { "to", "2024-03-30T18:00:00" }
        }
        };

        return jobData;
    }
}
