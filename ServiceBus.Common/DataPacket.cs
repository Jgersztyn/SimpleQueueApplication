namespace ServiceBus.Common;

// represents the data to send to the queue
public class DataPacket
{
    public DateTime FromDate { get; set; } = DateTime.Now;

    public DateTime ToDate { get; set; } = DateTime.Now;

    public List<string> RadarList { get; set; } = new();
}