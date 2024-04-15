using Azure.Messaging.ServiceBus;
using ServiceBus.Common;
using System.Text.Json;

// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

// number of messages to be sent to the queue
const int numOfMessages = 1;

// The Service Bus client types are safe to cache and use as a singleton for the lifetime
// of the application, which is best practice when messages are being published or read
// regularly.

// set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
// If you use the default AmqpTcp, you will need to make sure that the ports 5671 and 5672 are open
var clientOptions = new ServiceBusClientOptions()
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};

// TODO: Replace the <NAMESPACE-CONNECTION-STRING> and <QUEUE-NAME> placeholders
client = new ServiceBusClient("<NAMESPACE-CONNECTION-STRING>", clientOptions);
sender = client.CreateSender("<QUEUE-NAME>");

// create a batch 
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

for (int i = 1; i <= numOfMessages; i++)
{
    // need to add data to the queue with this information
    var packet = new DataPacket
    {
        FromDate = DateTime.UtcNow.AddDays(-1),
        ToDate = DateTime.UtcNow,
        RadarList = new List<string> { "v001", "v002" }
    };

    var serializedData = JsonSerializer.Serialize(packet);

    // try adding a message to the batch
    //if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Sending {i} test message(s) to queue")))
    if (!messageBatch.TryAddMessage(new ServiceBusMessage(serializedData)))
    {
        // if it is too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus queue
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();