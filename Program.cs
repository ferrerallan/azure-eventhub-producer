using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

string connectionString = "Endpoint=sb://eh-allan.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=zOmvRiUoxoDgKXh5L2IAET/f2zaSjSqZ+kS2t4U8W3E=";

// name of the event hub
string eventHubName = "allan-test";

// number of events to be sent to the event hub
int numOfEvents = 3;

EventHubProducerClient producerClient;

int currentPos = 10;
void MoveRobot(string direction)
{
    if (direction == "left")
    {
        currentPos--;
    }
    else
    {
        currentPos++;
    }
    var blank="";
    for (int i = 0; i < currentPos; i++)
    {
        blank+=" ";
    }
    Console.WriteLine($"{blank}▲");
}

async Task Main()
{
    // Create a producer client that you can use to send events to an event hub
    producerClient = new EventHubProducerClient(connectionString, eventHubName);

    // Create a batch of events 
   
    while (true) {

        var ch = Console.ReadKey(false).Key;
        var command = "";
        switch(ch)
        {
            case ConsoleKey.LeftArrow:
                command="left";
                break;
            case ConsoleKey.RightArrow:
                command="right";
                break;
            case ConsoleKey.Spacebar:
                await producerClient.DisposeAsync();
                break;            
        }
        
        MoveRobot(command);
        
        EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
        
        if (! eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"{command}"))))
        {
            Console.WriteLine("Failed to add event to batch.");
            throw new Exception($"{command} event is too large to add to the batch");
        }


        try
        {
            // Use the producer client to send the batch of events to the event hub
            //Console.WriteLine("Sending batch of events...");
            await producerClient.SendAsync(eventBatch);            
        }
        finally
        {
            ;
        }
    }
}

await Main();