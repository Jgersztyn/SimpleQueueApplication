using ServiceBus.Common.Enums;

namespace ServiceBus.Common.Models;

public class HistoricDataPacketRequest
{
    public JobType JobType { get; set; } = JobType.CreatePacket;

    public Dictionary<string, string> Parameters { get; set; } = new();
}