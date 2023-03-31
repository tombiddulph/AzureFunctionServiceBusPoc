using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions.ServiceBus;

public  static class TopicTrigger
{
    [Function("TopicTrigger")]
    public static void Run([ServiceBusTrigger("mytopic", "mysubscription", Connection = "EmulatorConnectionString")] string mySbMsg,
        FunctionContext context)
    {
        var logger = context.GetLogger("TopicTrigger");
        logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        
    }
    
}