using Azure.Messaging.ServiceBus;
using ServiceBus.Sender.Utilties;
using System.Text.Json;

ServiceBusClient client;
ServiceBusSender sender;

// The number of messages to be sent to the queue
const int numOfMessages = 1;

// Set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
// If you use the default AmqpTcp, you will need to make sure that the ports 5671 and 5672 are open.
var clientOptions = new ServiceBusClientOptions()
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};

// TODO: Replace the <NAMESPACE-CONNECTION-STRING> and <QUEUE-NAME> with your connection info
client = new ServiceBusClient("<NAMESPACE-CONNECTION-STRING>", clientOptions);
sender = client.CreateSender("<QUEUE-NAME>");

// create a batch 
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

for (int i = 1; i <= numOfMessages; i++)
{
    // Generate information that will be added to queue
    var packet = Helpers.CreateDataPacket();

    var serializedData = JsonSerializer.Serialize(packet);

    if (!messageBatch.TryAddMessage(new ServiceBusMessage(serializedData)))
    {
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
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();