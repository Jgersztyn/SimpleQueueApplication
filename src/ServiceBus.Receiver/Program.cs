using Azure.Messaging.ServiceBus;;

ServiceBusClient client;
ServiceBusProcessor processor;

var clientOptions = new ServiceBusClientOptions()
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};

// Replace the <NAMESPACE-CONNECTION-STRING> with your connection string
client = new ServiceBusClient("<NAMESPACE-CONNECTION-STRING>", clientOptions);

// Replace the <QUEUE-NAME> with your queue name
processor = client.CreateProcessor("<QUEUE-NAME>", new ServiceBusProcessorOptions());

try
{
    // Handler to process messages
    processor.ProcessMessageAsync += MessageHandler;

    // Handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();

    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();
}

///
/// Handler for received messages 
///
async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");

    // Complete the message and delete from the queue
    await args.CompleteMessageAsync(args.Message);
}

///
/// Handler for any errors when receiving messages
///
Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}