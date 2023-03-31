using System.Collections.Generic;
using System.Net;
using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Functions.ServiceBus;

public static class TestTrigger
{
    [Function("TestTrigger")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("TestTrigger");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var content = req.Body;


        ReadOnlyMemory<byte> bytes;
        if (content.Length == 0)
        {
            bytes = new ReadOnlyMemory<byte>("Hello, World!"u8.ToArray());
        }
        else
        {
            using var ms = new MemoryStream();
            await content.CopyToAsync(ms);
            bytes = new ReadOnlyMemory<byte>(ms.ToArray());
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        await DoTheStuff(bytes);

        await response.WriteStringAsync("Welcome to Azure Functions!");

        return response;
    }

    private static async Task DoTheStuff(ReadOnlyMemory<byte> bytes)
    {
        await using var client =
            new ServiceBusClient(Environment.GetEnvironmentVariable("EmulatorConnectionString"));
        await using var sender = client.CreateSender("mytopic");

        ServiceBusMessage message = new ServiceBusMessage(bytes);

        await sender.SendMessageAsync(message);
        Console.WriteLine("Message sent");
    }
}